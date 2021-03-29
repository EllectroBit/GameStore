using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Models.ViewModels
{
    public class GameViewModel
    {
        public int? ID { get; set; }
        [Required]
        //[Remote(action: "CheckName", controller: "Store", ErrorMessage = "Already exist")]
        public string Name { get; set; }
        [Range(1970, 2021)]
        [Required]
        public int Year { get; set; }
        [Required]
        public int Positive { get; set; }
        [Required]
        public int Negative { get; set; }
        public IFormFile Img { get; set; }
        [StringLength(5000)]
        public string Description { get; set; }
        [Required]
        public Genre Genre { get; set; }
    }
}
