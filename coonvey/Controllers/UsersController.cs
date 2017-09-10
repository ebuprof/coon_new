using coonvey.Models;
using coonvey.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace coonvey.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Users
        public ActionResult Index()
        {
            List<UsersViewModel> usersList = new List<UsersViewModel>();
            var users = db.Users.ToList();
            if (users != null)
            {
                foreach (var u in users)
                {
                    UsersViewModel usersViewModel = new UsersViewModel();
                    usersViewModel.Email = u.Email;
                    usersViewModel.EmailConfirmed = u.EmailConfirmed;
                    usersViewModel.PhoneNumber = u.PhoneNumber;
                    usersViewModel.PhoneNumberConfirmed = u.PhoneNumberConfirmed;
                    usersViewModel.Username = u.UserName;
                    usersViewModel.LockedOut = u.LockoutEnabled;
                    usersViewModel.Id = u.Id;
                    usersList.Add(usersViewModel);
                }
            }
            return View(usersList);
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            List<Profiles> profile = db.Profiles.Where(p => p.UserId.ToString() == id).ToList();
            if (profile.Count == 0)
            {
                return HttpNotFound();
            }
            //continue to load view
            ProfileViewModel pv = new ProfileViewModel();
            foreach (var p in profile)
            {
                pv.Id = p.Id;
                pv.UserId = p.UserId;
                pv.LastName = p.LastName;
                pv.MiddleName = p.MiddleName;
                pv.FirstName = p.FirstName;
                pv.City = p.City;
                pv.Address = p.Address;
            }

            return View(pv);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Lock/5
        public ActionResult Lock(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            UsersViewModel usersViewModel = new UsersViewModel();
            usersViewModel.Email = user.Email;
            usersViewModel.EmailConfirmed = user.EmailConfirmed;
            usersViewModel.PhoneNumber = user.PhoneNumber;
            usersViewModel.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            usersViewModel.Username = user.UserName;
            usersViewModel.LockedOut = user.LockoutEnabled;
            usersViewModel.Id = user.Id;
            return View(usersViewModel);
        }

        // POST: Users/Lock/5
        [HttpPost, ActionName("Lock")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LockConfirmed(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.LockoutEnabled = true;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Users/UnLock/5
        public ActionResult UnLock(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            UsersViewModel usersViewModel = new UsersViewModel();
            usersViewModel.Email = user.Email;
            usersViewModel.EmailConfirmed = user.EmailConfirmed;
            usersViewModel.PhoneNumber = user.PhoneNumber;
            usersViewModel.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            usersViewModel.Username = user.UserName;
            usersViewModel.LockedOut = user.LockoutEnabled;
            usersViewModel.Id = user.Id;
            return View(usersViewModel);
        }

        // POST: Users/UnLock/5
        [HttpPost, ActionName("UnLock")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnLockConfirmed(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.LockoutEnabled = false;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            UsersViewModel usersViewModel = new UsersViewModel();
            usersViewModel.Email = user.Email;
            usersViewModel.EmailConfirmed = user.EmailConfirmed;
            usersViewModel.PhoneNumber = user.PhoneNumber;
            usersViewModel.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            usersViewModel.Username = user.UserName;
            usersViewModel.LockedOut = user.LockoutEnabled;
            usersViewModel.Id = user.Id;
            return View(usersViewModel);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}