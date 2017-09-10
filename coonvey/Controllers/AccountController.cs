using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using coonvey.Models;
using coonvey.ViewModels;
using coonvey.Services;
using coonvey.Repositories;
using coonvey.Helpers;
using System.Web.Security;
using coonvey.Enums;
using Microsoft.AspNet.Identity.EntityFramework;

namespace coonvey.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private _UserRoleRepository _userRoleRepository = new _UserRoleRepository();
        private _LoginCountRepository _loginCountRepo = new _LoginCountRepository();
        private _LoginAuditRepository _loginAuditRepository = new _LoginAuditRepository();
        private _RoleRepository _roleRepository = new _RoleRepository();
        private _ProfileRepository _profileRepository = new _ProfileRepository();
        private EmailSender sendEmail = new EmailSender();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var user = await UserManager.FindByNameAsync(model.Email);
            var password = await UserManager.CheckPasswordAsync(user, model.Password);
            if (user != null && password == true)
            {
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Please confirm your email address to process.");
                    return View(model);
                }
                else if (user.TwoFactorEnabled)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                else if (user.LockoutEnabled)
                {
                    ModelState.AddModelError("", "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            //return RedirectToLocal(returnUrl);
                            FormsAuthentication.SetAuthCookie(user.UserName, true);
                            _loginCountRepo.CountUserLogin(user.Id.ToString());
                            //audit
                            var ip = GenericHelpers.GetUserIPAddress();
                            if (!string.IsNullOrEmpty(ip))
                            {
                                LoginAudits auditRecord = LoginAudits.CreateAuditEvent(Guid.NewGuid().ToString(), user.Id, en_LoginAuditEventType.Login, ip);
                                if (auditRecord != null)
                                {
                                    int auditRecording = _loginAuditRepository.Insert(auditRecord);
                                }
                            }

                            return RedirectToLocal(returnUrl, user.Id.ToString());
                        case SignInStatus.LockedOut:
                            if (user != null)
                            {
                                //audit
                                var ip3 = GenericHelpers.GetUserIPAddress();
                                if (!string.IsNullOrEmpty(ip3))
                                {
                                    LoginAudits auditRecord3 = LoginAudits.CreateAuditEvent(Guid.NewGuid().ToString(), user.Id, en_LoginAuditEventType.AccountLockedOut, ip3);
                                    if (auditRecord3 != null)
                                    {
                                        int auditRecording = _loginAuditRepository.Insert(auditRecord3);
                                    }
                                }

                            }
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            if (user != null)
                            {
                                //audit
                                var ip2 = GenericHelpers.GetUserIPAddress();
                                if (!string.IsNullOrEmpty(ip2))
                                {
                                    LoginAudits auditRecord2 = LoginAudits.CreateAuditEvent(Guid.NewGuid().ToString(), user.Id, en_LoginAuditEventType.FailedLogin, ip2);
                                    if (auditRecord2 != null)
                                    {
                                        int auditRecording = _loginAuditRepository.Insert(auditRecord2);
                                    }
                                }

                            }
                            return View(model);
                    }

                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email or Username.");
                if (user != null)
                {
                    //audit
                    var ip = GenericHelpers.GetUserIPAddress();
                    LoginAudits auditRecord = LoginAudits.CreateAuditEvent(Guid.NewGuid().ToString(), user.Id, en_LoginAuditEventType.FailedLogin, ip);
                    if (auditRecord != null)
                    {
                        int auditRecording = _loginAuditRepository.Insert(auditRecord);
                    }
                }
                return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string roleName = en_Roles.User.ToString();
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.SetLockoutEnabledAsync(user.Id, false);
                    ///send confirmation email
                    ///now send confirmation email
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    await sendEmail.SendEmailAsync(model.Email, "Confirm your account", "Please confirm your account by clicking this <a href=\"" + callbackUrl + "\">link</a>");

                    try
                    {
                        //check if role exists
                        if (_roleRepository.GetRoleByName(roleName) == null)
                            _roleRepository.Insert(new IdentityRole(roleName));
                        result = UserManager.AddToRole(user.Id, roleName); //add user role
                        //insert into Profile
                        Profiles profile = new Profiles();
                        profile.Id = Guid.NewGuid();
                        profile.UserId = Guid.Parse(user.Id);
                        profile.FirstName = model.FirstName;
                        profile.LastName = model.LastName;
                        profile.Activated = true;
                        _profileRepository.Insert(profile);

                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(ex.Message);
                    }
                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/RegisterSuperAdmin
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegisterSuperAdmin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/RegisterSuperAdmin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterSuperAdmin(RegisterViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                string roleName = en_Roles.SuperAdmin.ToString();
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.SetLockoutEnabledAsync(user.Id, false);
                    ///send confirmation email
                    ///now send confirmation email
                    //MailEngine.SendMail(user.Email, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    await sendEmail.SendEmailAsync(model.Email, "Confirm your account", "Please confirm your account by clicking this <a href=\"" + callbackUrl + "\">link</a>");

                    try
                    {
                        //check if role exists
                        if (_roleRepository.GetRoleByName(roleName) == null)
                            _roleRepository.Insert(new IdentityRole(roleName));
                        result = UserManager.AddToRole(user.Id, roleName); //add user role

                        //insert into Profile
                        Profiles profile = new Profiles();
                        profile.Id = Guid.NewGuid();
                        profile.UserId = Guid.Parse(user.Id);
                        profile.FirstName = model.FirstName;
                        profile.LastName = model.LastName;
                        profile.Activated = true;
                        _profileRepository.Insert(profile);

                    }
                    catch (Exception ex)
                    {
                        await UserManager.DeleteAsync(user); //delete user if system throw error
                        throw new InvalidOperationException(ex.Message);
                    }
                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await sendEmail.SendEmailAsync(model.Email, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        private ActionResult RedirectToLocal(string returnUrl, string userid)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                if (UserManager.IsInRole(userid, en_Roles.SystemAdmin.ToString()) || UserManager.IsInRole(userid, en_Roles.SuperAdmin.ToString()))
                {
                    return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
                }
                else if (UserManager.IsInRole(userid, en_Roles.MerchantAdmin.ToString()))
                {
                    ViewBag.Role = "3";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //login audit
            string userid = HttpContext.GetOwinContext().Request.User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(userid))
            {
                var ip = GenericHelpers.GetUserIPAddress();
                if (!string.IsNullOrEmpty(ip))
                {
                    LoginAudits auditRecord = LoginAudits.CreateAuditEvent(Guid.NewGuid().ToString(), userid, en_LoginAuditEventType.LogOut, ip);
                    if (auditRecord != null)
                    {
                        int auditRecording = _loginAuditRepository.Insert(auditRecord);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}