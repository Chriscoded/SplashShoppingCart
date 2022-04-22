using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "That Page does not exist";
                    break;
                default:
                    ViewBag.ErrorMessage = "Problem occured";
                    break;
            }
            return View("NotFound");
        }
    }
}
