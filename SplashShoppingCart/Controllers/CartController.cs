using Microsoft.AspNetCore.Mvc;
using SplashShoppingCart.Infrastructure;
using SplashShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PayStack.Net;
using Microsoft.AspNetCore.Identity;

namespace SplashShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly SplashShoppingCartContext context;
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly string token;
        private PayStackApi Paystack;

        public CartController(SplashShoppingCartContext Context, IConfiguration configuration, UserManager<AppUser> userManager)
        {
            context = Context;
            this.configuration = configuration;
            this.userManager = userManager;
            token = configuration["Payment:PaystackSK"];
            Paystack = new PayStackApi(token);

        }
        //GET //cart
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartViewModel cartViewModel = new CartViewModel
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Price * x.Quantity)
            };
            return View(cartViewModel);
        }

        //GET //cart/add/id
        public async Task<IActionResult> Add(int id)
        {
            Product product = await context.Products.FindAsync(id);
            //getting object of cartitem 
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            //lets check if the product we are  adding to cart is already in cart
            //if it is already in cart then we'll have to increase the quantity
            //else we add 1 to the quantity as per request
            CartItem cartItem = cart.Where(x => x.ProductId == product.Id).FirstOrDefault(); 

            if(cartItem == null)
            {
                //Add new products to cart if non is existing already in the cart
                cart.Add(new CartItem(product));
            }
            else
            {
                //add to quantity if it's an existing products
                cartItem.Quantity += 1;
            }
            //lets set session for the cart
            HttpContext.Session.SetJson("Cart", cart);
            //lets check if the request to add to cart was not an ajax request
            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                return RedirectToAction("Index");

            return ViewComponent("SmallCart");
        }

        //GET //cart/Decrease/id
        public IActionResult Decrease(int id)
        {
            //getting object of cartitem 
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            CartItem cartItem = cart.Where(x => x.ProductId == id).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                //Decrease quantity if it's more than one
                --cartItem.Quantity;
            }
            else
            {
                //remove the product totally from cart if its only one item in cart
                cart.RemoveAll(x => x.ProductId == id);
            }

            // if what we removed finished every content in cart
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                //lets set session for the cart if some items is still remaining on session
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
            
        }

        //GET //cart/Remove/id
        public IActionResult Remove(int id)
        {
            //getting object of cartitem 
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

                //remove the product totally from cart
                cart.RemoveAll(x => x.ProductId == id);

            // Confirm that after remove of items that the session is empty
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                //lets set session for the cart if some items is still remaining on session
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");

        }
        //GET //cart/Clear/id
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");

            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                return Redirect(Request.Headers["Referer"].ToString());

            return Ok();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task <IActionResult> InitializePayment(CartViewModel model)
        {
            AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
            TransactionInitializeRequest request = new TransactionInitializeRequest()
            {
                AmountInKobo = (int)(model.GrandTotal * 100),
                Email = user.Email,
                Reference = Generate().ToString(),
                Currency = "NGN",
                CallbackUrl = "http://localhost:56032/cart/verify"
            };

            TransactionInitializeResponse response = Paystack.Transactions.Initialize(request);
            if (response.Status)
            {
                var transaction = new Transaction
                {
                    Amount = model.GrandTotal,
                    Email = user.Email,
                    Name = User.Identity.Name,
                    TrxRef = request.Reference
                };

                await context.Transactions.AddAsync(transaction);
                await context.SaveChangesAsync();
                //redirect to url provided by paystack
                return Redirect(response.Data.AuthorizationUrl);
            }
            ViewData["PaystackError"] = response.Message;
            return RedirectToAction("Index");
        }

        public static int Generate()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            //min val max = 100000000 max = 999999999
            return rand.Next(100000000, 999999999);
        }

        public async Task<IActionResult> Verify(string reference)
        {
            TransactionVerifyResponse response = Paystack.Transactions.Verify(reference);
            //if the transaction is successfull
            if(response.Data.Status == "success")
            {
                //if the reference in the database is the same as the ref passed in then updat transaction.status = true
                var transaction = context.Transactions.Where(x => x.TrxRef == reference).FirstOrDefault();
                if(transaction != null)
                {
                    transaction.Status = true;
                    context.Transactions.Update(transaction);
                    await context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            ViewData["Paystack-error"] = response.Data.GatewayResponse;
            return RedirectToAction("Index");
        }
    }
}
