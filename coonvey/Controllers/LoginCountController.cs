using coonvey.Models;
using coonvey.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace coonvey.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class LoginCountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public LoginCountController()
        {
        }
        public LoginCountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        // GET: LoginCount
        public async Task<ActionResult> Index()
        {
            List<LoginCountViewModel> modelList = new List<LoginCountViewModel>();
            List<LoginCounts> countList = db.LoginCounts.ToList();
            if (countList.Count >= 1)
            {
                foreach (var l in countList)
                {
                    LoginCountViewModel model = new LoginCountViewModel();
                    model.LastLoggedInDate = l.LastLoggedInDate;
                    model.NumberOfTimes = l.NumberOfTimes;
                    model.UserId = l.UserId;
                    ApplicationUser user = await UserManager.FindByIdAsync(l.UserId);
                    if (user != null)
                        model.Username = user.UserName;

                    modelList.Add(model);
                }
            }
            return View(modelList);
        }

        public async Task<ActionResult> AuditUserLogin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<LoginAuditViewModel> modelList = new List<LoginAuditViewModel>();
            List<LoginAudits> loginEventList = db.LoginAudits.Where(l => l.UserId == id).ToList();
            if (loginEventList != null)
            {
                foreach (var l in loginEventList)
                {
                    LoginAuditViewModel model = new LoginAuditViewModel();
                    model.AuditEvent = l.AuditEvent;
                    model.AuditId = l.AuditId;
                    model.IpAddress = l.IpAddress;
                    model.Timestamp = l.Timestamp;
                    model.UserId = l.UserId;
                    ApplicationUser user = await UserManager.FindByIdAsync(l.UserId);
                    if (user != null)
                        model.UserName = user.UserName;

                    modelList.Add(model);
                }
            }

            ViewBag.LoginCount = loginEventList.Count();

            return View(modelList);
        }
    }

}