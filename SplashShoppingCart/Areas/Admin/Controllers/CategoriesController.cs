using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplashShoppingCart.Infrastructure;
using SplashShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly SplashShoppingCartContext context;

        public CategoriesController(SplashShoppingCartContext context)
        {
            this.context = context;
        }
        //GET /admin/categories
        public async Task<IActionResult> Index()
        {
            return View(await context.Categories.OrderBy(x => x.Sorting).ToListAsync());
        }

        //GET /admin/categories/create
        public async Task<IActionResult> Create() => View();

        //Post /admin/categories/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");
                category.Sorting = 100;

                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The Category already exists.");
                    return View(category);
                }

                context.Add(category);
                await context.SaveChangesAsync();

                TempData["Success"] = "The category has been added!";
                return RedirectToAction("index");
            }

            return View(category);
        }

        //Get /admin/categories/edit/id
        public async Task<IActionResult> Edit(int id)
        {
            Category category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //Post /admin/categories/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                //if id is 1 whether its edited or not the page.slug should be equal to home
                //else if its not id one then convert page.Title to lower case
                //replace white spaces with dash and assign it to slug
                category.Slug = category.Name.ToLower().Replace("-", " ").Replace(" ", "-");

                //where id its not the particular id we're editing
                //find if the slug is in the database already
                var slug = await context.Categories.Where(x => x.Id != category.Id).FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The Category already exists.");
                    return View(category);
                }

                context.Update(category);
                await context.SaveChangesAsync();

                TempData["Success"] = "The Category has been edited!";
                return RedirectToAction("Edit", new { id = category.Id });
            }

            return View(category);
        }
        //Get /admin/categories/delete/id
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["Error"] = "The Category does not exist";
            }
            else
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                TempData["Success"] = "The Category has been deleted!";
            }
            return RedirectToAction("index");
        }

        //Post /admin/categories/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;
            foreach (var categoryId in id)
            {
                Category category = await context.Categories.FindAsync(categoryId);
                category.Sorting = count;
                context.Update(category);
                await context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }

    }
}
