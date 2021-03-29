using GameStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStore.Models.ViewModels;

namespace GameStore.Controllers
{
    public class StoreController : Controller
    {
        private StoreContext db;
        public StoreController(StoreContext context)
        {
            db = context;
        }

        public IActionResult Index(int page = 1)
        {
            ViewData["action"] = "Index";
            return View(Page(page, db.Games.ToList()));
        }

        [Authorize]
        public IActionResult About(int id)
        {
            return View(db.Games.FirstOrDefault(g => g.ID == id));
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(GameViewModel model)
        {
            if(!ModelState.IsValid || model.Img == null)
                return RedirectToAction("Add");

            byte[] img = null;
            using(var br = new BinaryReader(model.Img.OpenReadStream()))
                img = br.ReadBytes((int)model.Img.Length);

            Game g = new Game
            {
                Name = model.Name,
                Year = model.Year,
                Img = img,
                Genre = model.Genre,
                Positive = model.Positive,
                Negative = model.Negative,
                Description = model.Description
            };

            db.Games.Add(g);
            db.SaveChanges();

            return RedirectToAction("Index", "Store");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            Game game = db.Games.Find(id);
            if(game != null)
            {
                GameViewModel model = new GameViewModel()
                {
                    Name = game.Name,
                    Description = game.Description,
                    Genre = game.Genre,
                    Negative = game.Negative,
                    Positive = game.Positive,
                    Year = game.Year,
                    ID = game.ID
                };
                return View(model);
            }

            return RedirectToAction("Index", "Store");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(GameViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Game g = new Game
            {
                Name = model.Name,
                Year = model.Year,
                Genre = model.Genre,
                Positive = model.Positive,
                Negative = model.Negative,
                Description = model.Description
            };

            Game game = db.Games.Find((int)model.ID);

            if (model.Img != null)
            {
                byte[] img = null;
                using (var br = new BinaryReader(model.Img.OpenReadStream()))
                    img = br.ReadBytes((int)model.Img.Length);

                g.Img = img;
            }
            else
            {
                g.Img = game.Img;
            }

            db.Games.Remove(game);
            db.Games.Add(g);
            db.SaveChanges();

            return RedirectToAction("Index", "Store");
        }

        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public IActionResult Buy(int id)
        {
            User user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            user.Games.Add(db.Games.Find(id));
            db.SaveChanges();
            return RedirectToActionPermanent("Index");
        }

        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            db.Games.Remove(db.Games.Find(id));
            db.SaveChanges();
            return RedirectToActionPermanent("Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public IActionResult MyGames(int page = 1)
        {
            ViewData["action"] = "MyGames";
            return View("Index", Page(page, db.Games.Include(g => g.Users).Where(g => g.Users.Any(u => u.Email == User.Identity.Name)).ToList()));
        }

        [HttpPost]
        public IActionResult Search(string str, int page = 1)
        {
            ViewData["action"] = "Search";
            ViewData["str"] = str;
            return View("Index", Page(page, db.Games.Where(g => g.Name.Contains(str)).ToList()));
        }

        public IActionResult Filter(Genre str, int page = 1)
        {
            ViewData["action"] = "Filter";
            ViewData["str"] = str;
            return View("Index", Page(page, db.Games.Where(g => g.Genre == str).ToList()));
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckName(string name)
        {
            return Json(!db.Games.Any(g => g.Name == name));
        }

        public List<Game> Page(int page, List<Game> g)
        {
            List<Game> games = new List<Game>();
            int start = (page - 1) * 9;
            ViewData["Page"] = page;
            ViewData["IsNext"] = true;
            ViewData["IsBack"] = start - 1 > 0;

            for (int i = start; i < start + 9; i++)
            {
                try
                {
                    if (g[i] != null)
                        games.Add(g[i]);
                }
                catch
                {
                    ViewData["IsNext"] = false;
                }
            }

            return games;
        }
    }
}
