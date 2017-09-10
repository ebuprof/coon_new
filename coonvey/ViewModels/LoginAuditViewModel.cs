using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace coonvey.ViewModels
{
    public class LoginAuditViewModel
    {
        [Display(Name = "Id")]
        public string AuditId { get; set; }

        [Display(Name = "User Id")]
        public string UserId { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Date/Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:MM:sss}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;

        [Display(Name = "Action")]
        public string AuditEvent { get; set; }

        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
    }

}