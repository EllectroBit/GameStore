using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Models.ViewModels
{
    public class RegisterViewModels
    {
        [Required]
        [RegularExpression(@"[A-Za-z]{2,30}", ErrorMessage = "Wrong Name")]
        public string Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage ="Wrong Email")]
        [Remote(action: "CheckEMail", controller: "Account", ErrorMessage = "Already exist")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password mismatch")]
        public string PasswordConfirm { get; set; }
    }
}
