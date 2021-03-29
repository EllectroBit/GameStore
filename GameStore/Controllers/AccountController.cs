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

namespace GameStore.Controllers
{
    public class AccountController : Controller
    {
        private StoreContext db;
        public AccountController(StoreContext context)
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
                User user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == login._Email && u.Password == login._Password);
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
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    user = new User { Email = model.Email, Password = model.Password, Name = model.Name, Role = userRole};
                    db.Users.Add(user);
                    await db.SaveChangesAsync();

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
            return Json(!db.Users.Any(u => u.Email == email));
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