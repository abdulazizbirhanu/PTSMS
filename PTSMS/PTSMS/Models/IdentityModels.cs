using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.Data.Entity;
using System.Linq;
using PTSMSDAL.Models.Others.School;
using PTSMSDAL.Access.Others;
using System.Threading;
using System.Web;
using System.Data.Entity.Infrastructure;
using Microsoft.Owin;
using Owin;

namespace PTSMS.Models
{

    /// <summary>
    /// Privilege base model
    /// </summary>
    public class ApplicationPrivilege
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Role privilege base model
    /// </summary>
    public class ApplicationRolePrivilege
    {
        public string RoleId { get; set; }
        public string PrivilegeId { get; set; }

        public ApplicationPrivilege Privilage { get; set; }
    }

    /// <summary>
    /// Role base model (inherits Identity Role model
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string name, string description)
            : base(name)
        {
            this.Description = description;
        }

        public virtual string Description { get; set; }

        public ICollection<ApplicationRolePrivilege> RolePrivileges { get; set; }
    }

    /// <summary>
    /// User role base model (inherits Identity User Role model)
    /// </summary>
    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationUserRole()
            : base()
        { }

        public ApplicationRole Role { get; set; }
    }

    /// <summary>
    /// User base model (inherits Identity User model)
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            FirstLogin = true;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            //Using the UserMaster data, set our custom claim types
            //userIdentity.AddClaim(new Claim(ClaimTypes.UserData, "Fisha Tariku"));
            //userIdentity.AddClaim(new Claim(CustomClaimTypes.SchoolCode, manager.Users.First));
            return userIdentity;
        }

        public bool FirstLogin { get; set; }
        //public string SchoolCode { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }

    public static class CustomClaimTypes
    {
        public static string SchoolCode { get; set; }
    }

    /// <summary>
    /// Identity database context containing all identity methods
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private static SchoolAccess schoolAccess = new SchoolAccess();
        public ApplicationDbContext()
            : base(GetConnectionString())
        {
            //Database.SetInitializer<ApplicationDbContext>(new CreateIfNotExistsInitializer());
        }
        public static string GetConnectionString()
        {
            string enteredSchoolCode = string.Empty;
            if (HttpContext.Current.Request.Form["SchoolCode"] != null)
                enteredSchoolCode = HttpContext.Current.Request.Form["SchoolCode"];

            var context = HttpContext.Current.GetOwinContext(); 

            //Get the current claims principal
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var claims = identity.Claims;



            if (System.Web.HttpContext.Current.Session != null)
            {
                if (System.Web.HttpContext.Current.Session["SchoolCode"] != null)
                {
                    string schoolCode = System.Web.HttpContext.Current.Session["SchoolCode"].ToString();
                    School school = schoolAccess.SchoolDetails(schoolCode);
                    if (school != null)
                    {
                        System.Web.HttpContext.Current.Session["SchoolName"] = school.SchoolName;
                        System.Web.HttpContext.Current.Session["SchoolCode"] = school.SchoolCode;
                        return "Data Source=" + school.Server + ";Initial Catalog=" + school.DatabaseName + ";User ID=" + school.Username + ";Password=" + school.Password + "";
                    }
                }
            }
            else
            {
                // HttpContext.Current.GetOwinContext().Environment.
                //Thread.CurrentPrincipal.Identity.Name.
            }

            if (!string.IsNullOrEmpty(enteredSchoolCode))
            {
                School school = schoolAccess.SchoolDetails(enteredSchoolCode);
                if (school != null)
                {
                    //System.Web.HttpContext.Current.Session["SchoolCode"] = enteredSchoolCode;
                    return "Data Source=" + school.Server + ";Initial Catalog=" + school.DatabaseName + ";User ID=" + school.Username + ";Password=" + school.Password + "";
                }
            }

            var SchoolCode = Controllers.PTSCurrentUserData.SchoolCode;
             
            if (SchoolCode != null)
            {
                School school = schoolAccess.SchoolDetails(SchoolCode);
                if (school != null)
                {
                    return "Data Source=" + school.Server + ";Initial Catalog=" + school.DatabaseName + ";User ID=" + school.Username + ";Password=" + school.Password + "";
                }
            }
            //return defualt connection string oe redirect to user login page 
            return "name=PTSContext"; 
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        /// <summary>
        /// Overridden On Model Creating method
        /// </summary>
        /// <param name="modelBuilder">Database Model Builder</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("ModelBuilder is NULL");
            }

            base.OnModelCreating(modelBuilder);

            //Rename the default table names
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");

            //Name the new tables; define keys and relations
            modelBuilder.Entity<ApplicationRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<ApplicationUser>().ToTable("Users").HasMany<ApplicationUserRole>((ApplicationUser u) => u.UserRoles);
            modelBuilder.Entity<ApplicationUserRole>().HasKey(r => new { UserId = r.UserId, RoleId = r.RoleId });
            modelBuilder.Entity<ApplicationPrivilege>().ToTable("Privileges").HasKey<string>(p => p.Id);
            modelBuilder.Entity<ApplicationRole>().HasMany<ApplicationRolePrivilege>((ApplicationRole r) => r.RolePrivileges);
            modelBuilder.Entity<ApplicationRolePrivilege>().ToTable("RolePrivileges").HasKey(p => new { RoleId = p.RoleId, PrivilegeId = p.PrivilegeId });
        }

        /// <summary>
        /// Seed method for initial system run
        /// </summary>
        /// <param name="context"> Identity database context</param>
        /// <returns></returns>
        public bool Seed(ApplicationDbContext context)
        {
            //#if DEBUG
            // Create my debug (testing) objects here

            bool success = false;

            ApplicationDbContext _db = new ApplicationDbContext();

            List<ApplicationPrivilege> privileges = new List<ApplicationPrivilege>()
            {
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Roles-Create", Description="Create Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Roles-Edit", Description="Edit Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Roles-Index", Description="List Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Roles-Delete", Description="Delete Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Roles-Create", Description="Create Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Roles-Index", Description="List Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Roles-Edit", Description="Edit Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Roles-Delete", Description="Delete Roles"},


                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Account-Index", Description="List Accounts"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Account-Index", Description="List Accounts"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Account-Update", Description="Update Account"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Account-Update", Description="Update Account"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Account-Register", Description="Create Account"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Account-Register", Description="Create Account"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Privileges-Create", Description="Create privileges"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Privileges-Create", Description="Create privileges"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Privileges-Edit", Description="Edit privileges"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Privileges-Edit", Description="Edit privileges"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Privileges-Delete", Description="Delete privileges"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Privileges-Index", Description="List privileges"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Privileges-Delete", Description="Delete privileges"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Privileges-Index", Description="List privileges"},
                //Begin, Curriculum
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Categories-Index", Description="List Categories"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Categories-Details", Description="Find Categories"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Categories-Create", Description="Create Categories"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Categories-Edit", Description="Edit Categories"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Categories-Delete", Description="Delete Categories"},

                 new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Curriculum-Index", Description="List Curriculum"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Courses-Index", Description="List Courses"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Courses-Details", Description="Find Courses"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Courses-Create", Description="Create Courses"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Courses-Edit", Description="Delete Courses"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Courses-Delete", Description="Delete Courses"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationCategories-Index", Description="List EvaluationCategories"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationCategories-Details", Description="Find EvaluationCategories"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationCategories-Create", Description="Create EvaluationCategories"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationCategories-Edit", Description="Edit EvaluationCategories"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationCategories-Delete", Description="Delete EvaluationCategories"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationItems-Index", Description="List EvaluationItems"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationItems-Details", Description="Find EvaluationItems"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationItems-Create", Description="Create EvaluationItems"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationItems-Edit", Description="Edit EvaluationItems"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationItems-Delete", Description="Delete EvaluationItems"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationTemplates-Index", Description="List EvaluationTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationTemplates-Details", Description="Find EvaluationTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationTemplates-Create", Description="Create EvaluationTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationTemplates-Edit", Description="Edit EvaluationTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="EvaluationTemplates-Delete", Description="Delete EvaluationTemplates"},


                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Exams-Index", Description="List Exams"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Exams-Details", Description="Find Exams"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Exams-Create", Description="Create Exams"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Exams-Edit", Description="Edit Exams"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Exams-Delete", Description="Delete Exams"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="GroundLessons-Index", Description="List GroundLessons"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="GroundLessons-Details", Description="Find GroundLessons"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="GroundLessons-Create", Description="Create GroundLessons"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="GroundLessons-Edit", Description="Edit GroundLessons"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="GroundLessons-Delete", Description="Delete GroundLessons"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Lessons-Index", Description="List Lessons"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Lessons-Details", Description="Find Lessons"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Lessons-Create", Description="Create Lessons"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Lessons-Edit", Description="Edit Lessons"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Lessons-Delete", Description="Delete Lessons"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Modules-Index", Description="List Modules"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Modules-Details", Description="Find Modules"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Modules-Create", Description="Create Modules"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Modules-Edit", Description="Edit Modules"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Modules-Delete", Description="Delete Modules"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Prerequisites-Index", Description="List Prerequisites"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Prerequisites-Details", Description="Find Prerequisites"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Prerequisites-Create", Description="Create Prerequisites"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Prerequisites-Edit", Description="Edit Prerequisites"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Prerequisites-Delete", Description="Delete Prerequisites"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="References-Index", Description="List References"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="References-Details", Description="Find References"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="References-Create", Description="Create References"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="References-Edit", Description="Edit References"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="References-Delete", Description="Delete References"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="CategoryTypes-Index", Description="List Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="CategoryTypes-Details", Description="Find Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="CategoryTypes-Create", Description="Create Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="CategoryTypes-Edit", Description="Edit Roles"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="CategoryTypes-Delete", Description="Delete Roles"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Phases-Index", Description="List CategoryTypes"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Phases-Details", Description="Find CategoryTypes"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Phases-Create", Description="Create CategoryTypes"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Phases-Edit", Description="Edit CategoryTypes"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Phases-Delete", Description="Delete CategoryTypes"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Programs-Index", Description="List Programs"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Programs-Details", Description="Find Programs"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Programs-Create", Description="Create Programs"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Programs-Edit", Description="Edit Programs"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Programs-Delete", Description="Delete Programs"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Stages-Index", Description="List Stages"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Stages-Details", Description="Find Stages"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Stages-Create", Description="Create Stages"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Stages-Edit", Description="Edit Stages"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Stages-Delete", Description="Delete Stages"},
                //End, Curriculum

                //Begin, Enrollement
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="BatchClasses-Index", Description="List BatchClasses"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="BatchClasses-Details", Description="Find BatchClasses"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="BatchClasses-Create", Description="Create BatchClasses"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="BatchClasses-Edit", Description="Edit BatchClasses"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="BatchClasses-Delete", Description="Delete BatchClasses"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Batches-Index", Description="List Batches"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Batches-Details", Description="Find Batches"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Batches-Create", Description="Create Batches"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Batches-Edit", Description="Edit Batches"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Batches-Delete", Description="Delete Batches"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="QualificationTypes-Index", Description="List QualificationTypes"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="QualificationTypes-Details", Description="Find QualificationTypes"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="QualificationTypes-Create", Description="Create QualificationTypes"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="QualificationTypes-Edit", Description="Edit QualificationTypes"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="QualificationTypes-Delete", Description="Delete QualificationTypes"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Instructors-Index", Description="List Instructors"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Instructors-Details", Description="Find Instructors"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Instructors-Create", Description="Create Instructors"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Instructors-Edit", Description="Edit Instructors"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Instructors-Delete", Description="Delete Instructors"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="PeriodTemplates-Index", Description="List PeriodTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="PeriodTemplates-Details", Description="Find PeriodTemplates"},
                //new ApplicationPrivilege(){Id = Guid.NewGuid().ToGetConnectionStringString(), Action="PeriodTemplates-Create", Description="Create PeriodTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="PeriodTemplates-Edit", Description="Edit PeriodTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="PeriodTemplates-Delete", Description="Delete PeriodTemplates"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Periods-Index", Description="List Periods"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Periods-Details", Description="Find Periods"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Periods-Create", Description="Create Periods"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Periods-Edit", Description="Edit Periods"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Periods-Delete", Description="Delete Periods"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="DayTemplates-Index", Description="List DayTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="DayTemplates-Details", Description="Find DayTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="DayTemplates-Create", Description="Create DayTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="DayTemplates-Edit", Description="Edit DayTemplates"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="DayTemplates-Delete", Description="Delete DayTemplates"},

                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Days-Index", Description="List Days"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Days-Details", Description="Find Days"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Days-Create", Description="Create Days"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Days-Edit", Description="Edit Days"},
                new ApplicationPrivilege(){Id = Guid.NewGuid().ToString(), Action="Days-Delete", Description="Delete Days"}
                //End, Enrollement
            };

            _db.ApplicationPrivileges.AddRange(privileges);

            ApplicationRoleManager _roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            success = this.CreateRole(_roleManager, "Admin", "Global access");
            if (!success == true) return success;

            foreach (ApplicationPrivilege ap in privileges)
            {
                _db.ApplicationRolePrivileges.Add(
                    new ApplicationRolePrivilege
                    {
                        PrivilegeId = ap.Id,
                        RoleId = _db.Roles.First(r => r.Name == "Admin").Id
                    });
            }

            _db.SaveChanges();

            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            ApplicationUser user = new ApplicationUser();

            user.UserName = "admin@pts.com";
            user.Email = "admin@pts.com";



            IdentityResult result = userManager.Create(user, "Abcd@1234");

            success = this.AddUserToRole(userManager, user.Id, "Admin");
            if (!success) return success;

            return success;
            //#endif
        }

        /// <summary>
        /// Checks whether a given role exists
        /// </summary>
        /// <param name="roleManager">Role manager in which to look for the role</param>
        /// <param name="name">Role name</param>
        /// <returns></returns>

        public bool RoleExists(ApplicationRoleManager roleManager, string name)
        {
            return roleManager.RoleExists(name);
        }

        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="_roleManager">Role manager in which to create the role</param>
        /// <param name="name">Role name</param>
        /// <param name="description">Role description</param>
        /// <returns></returns>
        public bool CreateRole(ApplicationRoleManager _roleManager, string name, string description = "")
        {
            var idResult = _roleManager.Create<ApplicationRole, string>(new ApplicationRole(name, description));
            return idResult.Succeeded;
        }

        /// <summary>
        /// Adds a user to a given role
        /// </summary>
        /// <param name="_userManager">User manager in which to add user to role</param>
        /// <param name="userId">User id to be added to a role</param>
        /// <param name="roleName">Role to which to add a role</param>
        /// <returns></returns>
        public bool AddUserToRole(ApplicationUserManager _userManager, string userId, string roleName)
        {
            var idResult = _userManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }

        /// <summary>
        /// Clears role of a given user
        /// </summary>
        /// <param name="userManager">User manager in which to clear user to role</param>
        /// <param name="userId">User id to be cleared</param>
        public void ClearUserRoles(ApplicationUserManager userManager, string userId)
        {
            ApplicationDbContext _db = new ApplicationDbContext();
            var user = userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();

            currentRoles.AddRange(user.Roles);



            foreach (var role in currentRoles)
            {
                string name = _db.Roles.FirstOrDefault(x => x.Id == role.RoleId).Name;
                userManager.RemoveFromRole(userId, name);
            }
        }

        /// <summary>
        /// Removes a given user from a given role
        /// </summary>
        /// <param name="userManager">User manager in which to remove user to role</param>
        /// <param name="userId">User id to be removed</param>
        /// <param name="roleName">Role name from which to be removed</param>
        public void RemoveFromRole(ApplicationUserManager userManager, string userId, string roleName)
        {
            userManager.RemoveFromRole(userId, roleName);
        }

        /// <summary>
        /// Deletes a given role
        /// </summary>
        /// <param name="context">Identity database context</param>
        /// <param name="userManager">User manager in which to delete a role</param>
        /// <param name="roleId">Role id to be removed</param>

        public void DeleteRole(ApplicationDbContext context, ApplicationUserManager userManager, string roleId)
        {
            var roleUsers = context.Users.Where(u => u.UserRoles.Any(r => r.RoleId == roleId));
            var role = context.Roles.Find(roleId);

            foreach (var user in roleUsers)
            {
                this.RemoveFromRole(userManager, user.Id, role.Name);
            }
            context.Roles.Remove(role);
            context.SaveChanges();
        }

        /// <summary>
        /// Context Initializer for the initial run
        /// </summary>
        public class CreateIfNotExistsInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
                context.Seed(context);
                base.Seed(context);
            }
        }

        public DbSet<ApplicationPrivilege> ApplicationPrivileges { get; set; }
        //public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationRolePrivilege> ApplicationRolePrivileges { get; set; }

        //public DbSet<Notification> Notifications { get; set; }
        //public DbSet<UserNotification> UserNotifications { get; set; }
    }
}