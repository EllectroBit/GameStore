using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int? RoleID { get; set; }
        public Role Role { get; set; }

        public List<Game> Games { get; set; }

        public User()
        {
            Games = new List<Game>();
        }
    }
}
