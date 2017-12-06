using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PTSMS.Models;
using PTSMSBAL.Others;
using PTSMSDAL.Access.Others;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Others.School;

namespace PTSMS.Controllers
{

    //[AATMSMethodFilter]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db = new ApplicationDbContext();
        private static SchoolAccess schoolAccess = new SchoolAccess();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            SchoolLogic schoolLogic = new SchoolLogic();
            List<School> schoolList = schoolLogic.List();
            schoolList.Add(new School { SchoolId = 0, SchoolCode = "", SchoolName = "-- Select School Name --" });

            ViewBag.SchoolCode = new SelectList(schoolList.OrderBy(x => x.SchoolId).Select(item => new
            {
                SchoolCode = item.SchoolCode,
                SchoolName = item.SchoolName
            }), "SchoolCode", "SchoolName");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //Append missed ZEROS
            string userName = model.Username.Trim();

            string appendableDigit = "";
            for (int i = 0; i < (8 - userName.Length); i++)
                appendableDigit += "0";

            model.Username = appendableDigit + model.Username.Trim();
            School school = schoolAccess.SchoolDetails(model.SchoolCode);
            if (!string.IsNullOrEmpty(school.SchoolCode))
            {
                System.Web.HttpContext.Current.Session["SchoolName"] = school.SchoolName;
                System.Web.HttpContext.Current.Session["SchoolCode"] = school.SchoolCode;
                PTSCurrentUserData.SchoolCode = school.SchoolCode;
            }



            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            SignInStatus result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = _db.Users.Where(u => u.UserName == model.Username).ToList().FirstOrDefault();
                    Session["Name"] = user.UserName;

                    //UserManager.AddClaim(user.UserName, new Claim(CustomClaimTypes.SchoolCode, schoolCode));
                    //.AddClaim(new Claim(CustomClaimTypes.SchoolCode, manager.Users.First));
                    //UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    //ClaimsIdentity identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    //identity.AddClaim(new Claim(ClaimTypes.UserData, schoolCode));
                    //AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);

                    //var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username), }, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);

                    //identity.AddClaim(new Claim(ClaimTypes.UserData, schoolCode));

                    if (user.FirstLogin)
                        return RedirectToAction("ChangePassword");
                    else
                        return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    Session["SchoolCode"] = null;

                    SchoolLogic schoolLogic = new SchoolLogic();
                    List<School> schoolList = schoolLogic.List();
                    schoolList.Add(new School { SchoolId = 0, SchoolCode = "", SchoolName = "-- Select School Name --" });

                    ViewBag.SchoolCode = new SelectList(schoolList.OrderBy(x => x.SchoolId).Select(item => new
                    {
                        SchoolCode = item.SchoolCode,
                        SchoolName = item.SchoolName
                    }), "SchoolCode", "SchoolName");

                    return View(model);
            }
        }

        public async Task<bool> IsUserValid(string userName, string password)
        {
            var result = await SignInManager.PasswordSignInAsync(userName, password, false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return true;
                default:
                    return false;
            }
        }

        //
        // GET: /Account/Index
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            //ApplicationDbContext _db = new ApplicationDbContext();
            var accounts = new List<RegisterViewModel>();
            foreach (var acct in _db.Users)
            {
                var account = new RegisterViewModel();
                account.Username = acct.UserName;
                accounts.Add(account);
            }

            if (TempData["AccountMessage"] != null)
            {
                ViewBag.AccountMessage = TempData["AccountMessage"];
                TempData["AccountMessage"] = null;
            }
            return View(accounts);
        }

        //
        // GET: /Account/Unauthorized
        [HttpGet]
        public ActionResult Unauthorized()
        {
            Session.Abandon();
            return View();
        }

        //
        // GET: /Account/Update
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Update(string userName)
        {
            //ApplicationDbContext _db = new ApplicationDbContext();
            var user = _db.Users.FirstOrDefault(x => x.UserName == userName);
            string selected = "";
            foreach (var item in user.Roles)
                selected += item.RoleId + ",";
            ViewBag.Selected = selected;
            ViewBag.Roles = _db.Roles.ToList();

            RegisterViewModel model = new RegisterViewModel();
            model.Username = user.UserName;
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [PTSAuthorizeAttribute]
        [ValidateAntiForgeryToken]
        public ActionResult Update(RegisterViewModel model)
        {
            //ApplicationDbContext _db = new ApplicationDbContext();
            string role = Request.Form["role"];
            if (role != null)
            {
                //Append missed ZEROS
                string userName = model.Username.Trim();

                string appendableDigit = "";
                for (int i = 0; i < (8 - userName.Length); i++)
                    appendableDigit += "0";

                model.Username = appendableDigit + model.Username.Trim();
                //
                var user = _db.Users.FirstOrDefault(x => x.UserName == model.Username);
                if (user != null)
                {
                    _db.ClearUserRoles(UserManager, user.Id);
                    string[] roles = role.Split(',');
                    foreach (var item in roles)
                    {
                        string name = _db.Roles.FirstOrDefault(x => x.Id == item).Name;
                        _db.AddUserToRole(UserManager, user.Id, name);
                    }
                    TempData["AccountMessage"] = "Account updated successfully.";
                    return RedirectToAction("Index", "Account");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        // GET: /Account/Register
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Register()
        {
            //ApplicationDbContext _db = new ApplicationDbContext();
            ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [PTSAuthorizeAttribute]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, FormCollection formCollection)
        {
            //ApplicationDbContext _db = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                //Append missed ZEROS               
                string userName = model.Username.Trim();
                string xx = Request.Form["Role"];
                string appendableDigit = "";
                for (int i = 0; i < (8 - userName.Length); i++)
                    appendableDigit += "0";

                model.Username = appendableDigit + model.Username.Trim();
                //
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var appContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _db.AddUserToRole(UserManager, user.Id, model.Role);

                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    TempData["AccountMessage"] = "User successfully registered.";
                    return RedirectToAction("Index", "Account");
                }
                //AddErrors(result);
                TempData["AccountMessage"] = GetErrors(result);
                return RedirectToAction("Index", "Account");
            }
            // If we got this far, something failed, redisplay form
            ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
            return View(model);
        }


        //
        [PTSAuthorizeAttribute]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Person person, ApplicationDbContext appContext)
        {
            try
            {
                //Append missed ZEROS               
                string userName = person.CompanyId;
                string appendableDigit = "";
                for (int i = 0; i < (8 - userName.Length); i++)
                    appendableDigit += "0";

                userName = appendableDigit + userName;
                //
                var user = new ApplicationUser { UserName = userName, Email = person.Email };

                var result = await UserManager.CreateAsync(user, "Abcd@1234");
                if (result.Succeeded)
                {
                    _db.AddUserToRole(UserManager, user.Id, "Trainee");

                
                }
                return View();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    user.FirstLogin = false;
                    await UserManager.UpdateAsync(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                TempData["AccountMessage"] = "Password has changed successfully.";
                return RedirectToAction("Welcome", "Home");
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
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
                //Append missed ZEROS
                string userName = model.Username.Trim();

                string appendableDigit = "";
                for (int i = 0; i < (8 - userName.Length); i++)
                    appendableDigit += "0";

                model.Username = appendableDigit + model.Username.Trim();
                //
                var user = await UserManager.FindByNameAsync(model.Username);
                if (user == null)// || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "<p>Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a></p>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [HttpGet]
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
            //Append missed ZEROS
            string userName = model.Username.Trim();

            string appendableDigit = "";
            for (int i = 0; i < (8 - userName.Length); i++)
                appendableDigit += "0";

            model.Username = appendableDigit + model.Username.Trim();
            //
            var user = await UserManager.FindByNameAsync(model.Username);
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
        [HttpGet]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/LogOff
        [HttpGet]
        [Authorize]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
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

        private string GetErrors(IdentityResult result)
        {
            string message = "";
            foreach (var error in result.Errors)
            {
                message = message + error + " ";
            }
            return message;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Welcome", "Home");
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
    public static class PTSCurrentUserData
    {
        public static string SchoolCode { get; set; }
    }
}