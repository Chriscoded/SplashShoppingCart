using Microsoft.AspNetCore.Mvc;
using SplashShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Infrastructure
{
    public class SmallCartViewComponent : ViewComponent
    {
        private readonly SplashShoppingCartContext context;

        public SmallCartViewComponent(SplashShoppingCartContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            SmallCartViewModel smallCartViewModel;

            if(cart == null)
            {
                smallCartViewModel = null;
            }
            else
            {
                smallCartViewModel = new SmallCartViewModel
                {
                    NumberOfItems = cart.Sum(x => x.Quantity),
                    TotalAmount = cart.Sum(x => x.Quantity * x.Price)
                
                };
            }

            return View(smallCartViewModel);
        }
    }
}
