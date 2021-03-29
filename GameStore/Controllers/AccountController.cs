using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Models;
using GameStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GameStore.Interfaces;

namespace GameStore.Controllers
{
    public class AccountController : Controller
    {
        private IStore db;
        public AccountController(IStore context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                User user = await db.GetUserAsync(login);
                if(user != null)
                {
                    await Authenticate(user);
                    return RedirectToAction("Index", "Store");
                }
                else
                    ModelState.AddModelError("", "Wrong Email or Password");
            }
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModels model)
        {
            if (ModelState.IsValid)
            {
                List<User> users = await db.GetUsersAsync();
                User user = users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    Role userRole = await db.GetUserRoleAsync();
                    user = new User { Email = model.Email, Password = model.Password, Name = model.Name, Role = userRole};
                    await db.SetUserAsync(user);

                    await Authenticate(user); 

                    return RedirectToAction("Index", "Store");
                }
                else
                    ModelState.AddModelError("", "Wrong Email or Password");
            }
            return View("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckEMail(string email)
        {
            return Json(db.ChackEmail(email));
        }

        private async Task Authenticate(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),
                new Claim("Name", user.Name)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}