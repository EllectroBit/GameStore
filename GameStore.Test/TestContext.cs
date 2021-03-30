using GameStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Test
{
    public class TestContext
    {
        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<Game> Games { get; set; }

        public TestContext()
        {
            Role user_role = new Role() { ID = 1, Name = "user" };
            Role admin_role = new Role() { ID = 2, Name = "admin" };

            User Admin = new User() { ID = 1, Name = "Admin", Email = "admin@gmail.com", Password = "123", Role = admin_role, RoleID = 2 };
            User Tom = new User() { ID = 2, Name = "Tom", Email = "tom@gmail.com", Password = "1111", Role = user_role, RoleID = 1 };
            User John = new User() { ID = 3, Name = "John", Email = "john@gmail.com", Password = "1111", Role = user_role, RoleID = 1 };

            Game game1 = new Game() { ID = 1, Name = "Game1", Year = 2020, Genre = Genre.Action, Description = "desc", Negative = 2, Positive = 3, Img = null, Users = { Tom } };
            Game game2 = new Game() { ID = 2, Name = "Game2", Year = 2019, Genre = Genre.Simulator, Description = "desc", Negative = 1, Positive = 5, Img = null, Users = { Tom, John } };

            Roles = new List<Role>() { user_role, admin_role };
            Users = new List<User>() { Admin, Tom, John };
            Games = new List<Game>() { game1, game2 };
        }
    }
}