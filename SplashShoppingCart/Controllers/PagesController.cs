using Microsoft.AspNetCore.Mvc;
using SplashShoppingCart.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Controllers
{
    public class PagesController : Controller
    {
        private readonly SplashShoppingCartContext context;

        public PagesController(SplashShoppingCartContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
