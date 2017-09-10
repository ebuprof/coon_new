using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace coonvey.ViewModels
{
    public class UsersViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Phone Number Confirmed")]
        public bool PhoneNumberConfirmed { get; set; }
        [Display(Name = "Locked Out")]
        public bool LockedOut { get; set; }
    }
}