using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string _Email { get; set; }
        [Required]
        public string _Password { get; set; }
    }
}
