using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SplashShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IPasswordHasher<AppUser> passwordHasher;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPasswordHasher<AppUser> passwordHasher)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
        }
        //GET /account/register
        [AllowAnonymous]
        public IActionResult Register() => View();

        //POST /account/register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user) {
            if (ModelState.IsValid)
            {
                AppUser appuser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PasswordHash = user.Password
                };

                IdentityResult result = await userManager.CreateAsync(appuser, user.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach(IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(user);
        }

        //GET /account/login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login
            {
                ReturnUrl = returnUrl
            };
            return View(login);
        }

        //POST /account/login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                AppUser appuser = await userManager.FindByEmailAsync(login.Email);

                if (appuser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result =
                        await signInManager.PasswordSignInAsync(appuser, login.Password, false, false);
                    if (result.Succeeded)
                    {
                        //when successful login go to return url but if empty then go to homepage
                        return Redirect(login.ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(string.Empty, "Login failed, wrong credentials.");
            }
            return View(login);
        }

        //GET /account/logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }

        //GET /account/edit
        public async Task<IActionResult> Edit()
        {
            //lets find the loggedin user for our edit 
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserEdit user = new UserEdit(appUser);

            return View(user);
        }

        //POST /account/edit
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEdit user)
        {
            if (ModelState.IsValid)
            {
                AppUser appuser = await userManager.FindByNameAsync(User.Identity.Name);

                if (appuser != null)
                {
                    appuser.Email = user.Email;
                    if(user.Password != null)
                    {
                        appuser.PasswordHash = passwordHasher.HashPassword(appuser, user.Password);
                    }
                    IdentityResult result = await userManager.UpdateAsync(appuser);

                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Your information has been edited successfully!";
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
