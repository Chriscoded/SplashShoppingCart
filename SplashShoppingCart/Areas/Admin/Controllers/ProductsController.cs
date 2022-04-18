using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SplashShoppingCart.Infrastructure;
using SplashShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly SplashShoppingCartContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductsController(SplashShoppingCartContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        //GET /admin/products
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 6;
            var products = context.Products.OrderByDescending(x => x.Id)
                                            .Include(x => x.Category)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Count() / pageSize);

            return View(await products.ToListAsync());
        }

        //GET /admin/products/details/id
        public async Task<IActionResult> Details(int id)
        {
            Product product = await context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //GET /admin/products/create
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            return View();
        }

        //POST /admin/product/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.CategoryId = new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await context.Products.FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists.");
                    return View(product);
                }

                //Upload image
                string imageName = "noimage.png";

                if(product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");
                    imageName = Guid.NewGuid().ToString()+"_"+ product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                }

                product.Image = imageName;
                context.Add(product);
                await context.SaveChangesAsync();

                TempData["Success"] = "The product has been added!";
                return RedirectToAction("index");
            }

            return View(product);
        }

        //GET /admin/products/edit/id
        public async Task<IActionResult> Edit(int id)
        {
          
            Product product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);
            return View(product);
        }

        //POST /admin/product/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            ViewBag.CategoryId = new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await context.Products.Where(x => x.Id != product.Id).FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists.");
                    return View(product);
                }

                //Upload image
               
                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");
                    //if the image of the product is not default noimage
                    //then remove image
                    //this is to avoid removing noimage which is always needed(default) incase 
                    //of product without image
                    if (!string.Equals(product.Image, "noimage"))
                    {
                        string oldImagePath = Path.Combine(uploadsDir, product.Image);
                        //if the old image is in our image path delete
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                context.Update(product);
                await context.SaveChangesAsync();

                TempData["Success"] = "The product has been edited!";
                return RedirectToAction("index");
            }

            return View(product);
        }

        //GET /admin/products/delete/id
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "The product does not exist";
            }
            else
            {
                //if the image of the product is not default noimage
                //then remove image
                //this is to avoid removing noimage which is always needed(default) incase 
                //of product without image
                if (!string.Equals(product.Image, "noimage"))
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");
                    string oldImagePath = Path.Combine(uploadsDir, product.Image);
                    //if the old image is in our image path delete
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                TempData["Success"] = "The product has been deleted!";
            }
            return RedirectToAction("index");
        }

        //GET /admin/products/transactions
        public IActionResult Transactions()
        {

            var transactions = context.Transactions.Where(x => x.Status == true).ToList();
            ViewData["transactions"] = transactions;
            return View();
           
        }
    }
}
