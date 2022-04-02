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
    public class PagesController : Controller
    {
        private readonly SplashShoppingCartContext context;

        public PagesController(SplashShoppingCartContext context)
        {
            this.context = context;
        }
        //Get /admin/pages
        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pages = from p in context.Pages orderby p.Sorting select p;
            IList<Page> pagesList = await pages.ToListAsync();
            return View(pagesList);
        }       
        //Get /admin/pages/details/id
        public async Task<IActionResult> Details(int id)
        {
            Page page = await context.Pages.FirstOrDefaultAsync(x => x.Id == id);
            if(page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        //Get /admin/pages/create
        public async Task<IActionResult> Create() => View();

        //Post /admin/pages/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if(slug != null)
                {
                    ModelState.AddModelError("", "The Page already exists.");
                    return View(page);
                }

                context.Add(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";          
                return RedirectToAction("index");
            }

            return View(page);
        }

        //Get /admin/pages/edit/id
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        //Post /admin/pages/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                //if id is 1 whether its edited or not the page.slug should be equal to home
                //else if its not id one then convert page.Title to lower case
                //replace white spaces with dash and assign it to slug
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace("-", " ").Replace(" ", "-");

                //where id its not the particular id we're editing
                //find if the slug is in the database already
                var slug = await context.Pages.Where(x => x.Id != page.Id).FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The Page already exists.");
                    return View(page);
                }

                context.Update(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been edited!";
                return RedirectToAction("Edit", new { id = page.Id});
            }

            return View(page);
        }

        //Get /admin/pages/delete/id
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await context.Pages.FindAsync(id);
            if (page == null)
            {
                TempData["Error"] = "The page does not exist";
            }
            else
            {
                context.Pages.Remove(page);
                await context.SaveChangesAsync();
                TempData["Success"] = "The page has been deleted!";
            }
            return RedirectToAction("index");
        }
        //Post /admin/pages/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;
            foreach (var pageId in id)
            {
                Page page = await context.Pages.FindAsync(pageId);
                page.Sorting = count;
                context.Update(page);
                await context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }
    }
}
