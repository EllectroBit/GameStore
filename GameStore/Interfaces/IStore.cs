using GameStore.Models;
using GameStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Interfaces
{
    public interface IStore
    {
        Task<User> GetUserAsync(LoginViewModel login);
        User GetUser(string Email);

        Task<Role> GetUserRoleAsync();
        Game GetGame(int id);

        Task<List<User>> GetUsersAsync();
        List<Game> GetGames();

        Task SetUserAsync(User user);
        void SetGame(Game game);

        void AddGameToUser(string userEMail, int gameID);

        void DeleteGame(int id);

        bool ChackEmail(string mail);
    }
}
