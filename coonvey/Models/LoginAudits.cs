using coonvey.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace coonvey.Models
{
    public class LoginAudits
    {
        [Key]
        public string AuditId { get; private set; }

        [Required]
        public string UserId { get; private set; }

        [Required]
        public DateTimeOffset Timestamp { get; private set; } = DateTime.UtcNow;

        [Required]
        public string AuditEvent { get; set; }
        //public UserAuditEventType AuditEvent { get; set; }

        public string IpAddress { get; private set; }

        public static LoginAudits CreateAuditEvent(string auditId, string userId, en_LoginAuditEventType auditEventType, string ipAddress)
        {
            return new LoginAudits { AuditId = auditId, UserId = userId, AuditEvent = auditEventType.ToString(), IpAddress = ipAddress };
        }
    }
}