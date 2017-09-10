using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace coonvey.ViewModels
{
    public class ChangeUserRoleViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required, Display(Name = "Selected Role In Use")]
        public string RoleInUse { get; set; }
        //public string FirstName { get; set; }
        [Required, Display(Name = "Roles")]
        public string RoleId { get; set; }
    }
}