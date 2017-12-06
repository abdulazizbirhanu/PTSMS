using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Web.Routing;
using PTSMS.Models;

namespace PTSMS
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            //return Task.FromResult(0);
            // Credentials:
            var credentialUserName = "fishat@ethiopianairlines.com";// ConfigurationManager.AppSettings["emailFrom"];
            var sentFrom = "fishat@ethiopianairlines.com";//ConfigurationManager.AppSettings["emailFrom"];
            var pwd = "HAVElongsgodwin1234";// ConfigurationManager.AppSettings["emailPassword"];

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("svhqsgw02.ethiopianairlines.com", 25);

            //// Configure the client:
            //System.Net.Mail.SmtpClient client =
            //    new System.Net.Mail.SmtpClient("smtp-mail.outlook.com");

            //client.Port = 587;
            //client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;

            // Create the credentials:
            System.Net.NetworkCredential credentials =
                 new System.Net.NetworkCredential(credentialUserName, pwd);

            //client.EnableSsl = true;
            //client.Credentials = credentials;

            // Create the message:
            var mail =
            new System.Net.Mail.MailMessage(sentFrom, message.Destination);

            mail.IsBodyHtml = true;
            mail.Subject = message.Subject;
            mail.Body = message.Body;

            // Send:
            return client.SendMailAsync(mail);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
    // Configure the application role manager used in this application.
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        { }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationRoleManager(
                new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    /// <summary>
    /// System user to be authorized
    /// </summary>
    public class PTSUser
    {
        public string Username { get; set; }

        private List<ApplicationUserRole> Roles = new List<ApplicationUserRole>();

        public PTSUser(string username)
        {
            this.Username = username;
            GetUserRolesPrivileges();
        }

        /// <summary>
        /// Gets user privileges using its role
        /// </summary>
        private void GetUserRolesPrivileges()
        {
            using (ApplicationDbContext _db = new ApplicationDbContext())
            {
                //get user
                ApplicationUser user = _db.Users.Where(u => u.UserName == this.Username).FirstOrDefault();
                if (user != null)
                {
                    foreach (var role in user.Roles)
                    {
                        this.Roles.Add(new ApplicationUserRole { RoleId = role.RoleId });
                    }
                }
            }
        }
                
        /// <summary>
        /// Checks whether a user has a given privilege
        /// </summary>
        /// <param name="requiredPrivilege">Privilege to be checked</param>
        /// <returns></returns>
        public bool HasPrivilege(string requiredPrivilege)
        {
            bool found = false;
            foreach (ApplicationUserRole userRole in this.Roles)
            {
                using (ApplicationDbContext _db = new ApplicationDbContext())
                {
                    List<ApplicationRolePrivilege> rolePrivilege = _db.ApplicationRolePrivileges.Where(r => r.RoleId == userRole.RoleId).ToList();
                    foreach (var privilege in rolePrivilege)
                    {
                        found = _db.ApplicationPrivileges.Where(p => p.Action == requiredPrivilege && privilege.PrivilegeId == p.Id).ToList().Count > 0;
                        if (found)
                            break;
                    }
                }
            }
            return found;
        }
    }

    /// <summary>
    /// System authorization attribute (inherits Authorize Attribute)
    /// </summary>
    public class PTSAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            /*Create permission string based on the requested controller 
              name and action name in the format 'controllername-action'*/
            string requiredPrivilege = String.Format("{0}-{1}",
                   filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                   filterContext.ActionDescriptor.ActionName);

            /*Create an instance of our custom user authorisation object passing requesting 
              user's 'Windows Username' into constructor*/
            PTSUser requestingUser = new PTSUser(filterContext.RequestContext
                                                   .HttpContext.User.Identity.Name);

            //Check if the requesting user has the permission to run the controller's action
            if (!requestingUser.HasPrivilege(requiredPrivilege))
            {
                /*User doesn't have the required permission and is not a SysAdmin, return our 
                  custom '401 Unauthorized' access error. Since we are setting 
                  filterContext.Result to contain an ActionResult page, the controller's 
                  action will not be run.

                  The custom '401 Unauthorized' access error will be returned to the 
                  browser in response to the initial request.*/
                filterContext.Result = new RedirectToRouteResult(
                                               new RouteValueDictionary {
                                                { "action", "Unauthorized" },
                                                { "controller", "Account" } });
            }
            /*If the user has the permission to run the controller's action, then 
              filterContext.Result will be uninitialized and executing the controller's 
              action is dependant on whether filterContext.Result is uninitialized.*/
        }
    }
}
