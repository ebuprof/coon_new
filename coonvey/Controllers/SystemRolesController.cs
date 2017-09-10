using coonvey.Models;
using coonvey.Repositories;
using coonvey.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
    public class SystemRolesController : Controller
    {

        private _UserRoleRepository _userRoleRepository = new _UserRoleRepository();
        private _RoleRepository _roleRepository = new _RoleRepository();

        public SystemRolesController()
        {
        }
        public SystemRolesController(ApplicationUserManager userManager)
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


        // GET: SystemRoles
        public ActionResult Index()
        {
            var r = _roleRepository.GetRoles();
            return View(r);
        }

        // GET: SystemRoles/Create
        public ActionResult Create()
        {
            return View(new RoleViewModel());
        }

        // POST: SystemRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //check if role exists
                    if (_roleRepository.GetRoleByName(model.Name) == null)
                    {
                        _roleRepository.Insert(new IdentityRole(model.Name));
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "This role already exists.");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }

            return View(model);
        }

        // GET: SystemRoles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = _roleRepository.GetRoleById(id);
            if (role == null)
            {
                return HttpNotFound();
            }

            return View(role);
        }

        // POST: SystemRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole role)
        {
            RoleViewModel roleView = new RoleViewModel();
            if (ModelState.IsValid)
            {
                //IdentityRole role = _roleRepository.GetRoleById(model.Id);
                //role.Name = model.Name;
                int k = _roleRepository.Update(role);
                if (k >= 1)
                {
                    return RedirectToAction("Index");
                }

                roleView.Id = role.Id;
                roleView.Name = role.Name;
            }
            return View(roleView);
        }

        // Get all the users making use of a selected role
        // GET: /SystemRoles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = _roleRepository.GetRoleById(id);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        // You should be able to add another role to user
        // GET: /SystemRoles/AddNewRole/5
        public ActionResult AddNewRoleToUser(string id, string role)
        {
            ChangeUserRoleViewModel model = new ChangeUserRoleViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (role == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserManager.FindByIdAsync(id);
            if (user.Result == null)
            {
                ModelState.AddModelError(string.Empty, "System was unable to find user.");
                return View();
            }
            //IList<string> userroles = await UserManager.GetRolesAsync(id);
            //if (userroles.Count < 1)
            //{
            //    ModelState.AddModelError(string.Empty, "No role found for this user.");
            //    return View();
            //}
            //string rolename = string.Empty;
            //foreach (string role in userroles)
            //{
            //    rolename = role;
            //}
            model.Email = user.Result.Email;
            model.UserName = user.Result.UserName;
            model.UserId = user.Result.Id;
            model.RoleInUse = role;

            //get list of role 
            ViewBag.RoleId = new SelectList(_roleRepository.GetRoles(), "Id", "Name", model.RoleId);
            return View(model);
        }

        // POST: SystemRoles/AddNewRole/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewRoleToUser(ChangeUserRoleViewModel model)
        {
            //get list of role 
            ViewBag.RoleId = new SelectList(_roleRepository.GetRoles(), "Id", "Name", model.RoleId);

            if (ModelState.IsValid)
            {
                if (model.RoleId == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                IdentityRole role = _roleRepository.GetRoleById(model.RoleId);
                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, "System was unable to find to chosen role.");
                    return View(model);
                }
                //check if user already exist in such selected role
                if (await UserManager.IsInRoleAsync(model.UserId, role.Name))
                {
                    ModelState.AddModelError(string.Empty, "User already exist in role.");
                    return View(model);
                }
                IdentityResult result = UserManager.AddToRole(model.UserId, role.Name); //add user role
                await UserManager.UpdateSecurityStampAsync(model.UserId);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // You should be able to change user's role from here
        // GET: /SystemRoles/Change/5
        public ActionResult Change(string id, string role)
        {
            ChangeUserRoleViewModel model = new ChangeUserRoleViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (role == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserManager.FindByIdAsync(id);
            if (user.Result == null)
            {
                ModelState.AddModelError(string.Empty, "System was unable to find user.");
                return View();
            }
            model.Email = user.Result.Email;
            model.UserName = user.Result.UserName;
            model.UserId = user.Result.Id;
            model.RoleInUse = role;

            //get list of role 
            ViewBag.RoleId = new SelectList(_roleRepository.GetRoles(), "Id", "Name", model.RoleId);
            return View(model);
        }

        // POST: SystemRoles/Change/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Change(ChangeUserRoleViewModel model)
        {
            //get list of role 
            ViewBag.RoleId = new SelectList(_roleRepository.GetRoles(), "Id", "Name", model.RoleId);

            if (ModelState.IsValid)
            {
                if (model.RoleId == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                IdentityRole role = _roleRepository.GetRoleById(model.RoleId);
                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, "System was unable to find to chosen role.");
                    return View(model);
                }
                //check if user already exist in such selected role
                if (await UserManager.IsInRoleAsync(model.UserId, role.Name))
                {
                    ModelState.AddModelError(string.Empty, "User already exist in role.");
                    return View(model);
                }
                UserManager.RemoveFromRole(model.UserId, model.RoleInUse); //remove user from RoleInUse
                IdentityResult result = UserManager.AddToRole(model.UserId, role.Name); //add user role
                await UserManager.UpdateSecurityStampAsync(model.UserId);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // You should be able to remove user from role here
        // GET: /SystemRoles/RemoveFromRole/5
        public ActionResult RemoveFromRole(string id, string role)
        {
            ChangeUserRoleViewModel model = new ChangeUserRoleViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (role == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserManager.FindByIdAsync(id);
            if (user.Result == null)
            {
                ModelState.AddModelError(string.Empty, "System was unable to find user.");
                return View();
            }

            model.Email = user.Result.Email;
            model.UserName = user.Result.UserName;
            model.UserId = user.Result.Id;
            model.RoleInUse = role;

            return View(model);
        }

        // POST: SystemRoles/RemoveFromRole/5
        [HttpPost, ActionName("RemoveFromRole")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveFromRoleConfirmed(string id, string role)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (role == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserManager.RemoveFromRole(id, role); //remove user from RoleInUse
            await UserManager.UpdateSecurityStampAsync(id);

            return RedirectToAction("Index");
        }

        public void ClearUserRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();

            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                UserManager.RemoveFromRole(userId, role.RoleId);
            }
        }

        // GET: SystemRoles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = _roleRepository.GetRoleById(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleView = new RoleViewModel();
            roleView.Id = role.Id;
            roleView.Name = role.Name;
            return View(roleView);
        }

        // POST: SystemRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            IdentityRole role = _roleRepository.GetRoleById(id);
            if (role != null)
                _roleRepository.Delete(id);

            return RedirectToAction("Index");
        }

    }
}