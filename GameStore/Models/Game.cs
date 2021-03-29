using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Models
{
    public class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Positive { get; set; }
        public int Negative { get; set; }
        public byte[] Img { get; set; }
        public string Description { get; set; }
        public Genre Genre { get; set; }

        public List<User> Users { get; set; }

        public Game()
        {
            Users = new List<User>();
        }
    }
}

public enum Genre
{
    Action,
    Strategy,
    RPG, 
    Simulator,
    Sport, 
    Fighting,
    Horror,
    Race
}