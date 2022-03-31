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
        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pages = from p in context.Pages orderby p.Sorting select p;
            IList<Page> pagesList = await pages.ToListAsync();
            return View(pagesList);
        }
    }
}
