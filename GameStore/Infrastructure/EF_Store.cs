using GameStore.Interfaces;
using GameStore.Models;
using GameStore.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Infrastructure
{
    public class EF_Store : IStore
    {
        private readonly StoreContext db;

        public EF_Store(StoreContext context)
        {
            db = context;
        }

        public Task<User> GetUserAsync(LoginViewModel login) => db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == login._Email && u.Password == login._Password);
        public User GetUser(string Email) => db.Users.Include(u => u.Games).FirstOrDefault(u => u.Email == Email);

        public Task<Role> GetUserRoleAsync() => db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
        public Task<List<User>> GetUsersAsync() => db.Users.Include(u => u.Role).ToListAsync();

        public List<Game> GetGames() => db.Games.Include(g => g.Users).ToList();
        public Game GetGame(int id) => db.Games.Find(id);

        public bool ChackEmail(string mail) => !db.Users.Any(u => u.Email == mail);

        public Task SetUserAsync(User user)
        {
            db.Add(user);
            return db.SaveChangesAsync();
        }

        public void SetGame(Game game)
        {
            db.Games.Add(game);
            db.SaveChanges();
        }

        public void AddGameToUser(string userEMail, int gameID)
        {
            User user = db.Users.FirstOrDefault(u => u.Email == userEMail);
            user.Games.Add(db.Games.Find(gameID));
            db.SaveChanges();
        }

        public void DeleteGame(int id)
        {
            db.Games.Remove(db.Games.Find(id));
            db.SaveChanges();
        }
    }
}
