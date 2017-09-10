using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace coonvey.ViewModels
{
    public class LoginCountViewModel
    {
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Display(Name = "Number of Times")]
        public long? NumberOfTimes { get; set; }
        [Display(Name = "Last Logged In Date")]
        public DateTime LastLoggedInDate { get; set; }
    }
}