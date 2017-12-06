using PTSMS.Models;
using PTSMSBAL.Others;
using PTSMSDAL.Models.Others;
using PTSMSDAL.Models.Others.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PTSMS.Others
{
    public class HelperClass
    {
        /// <summary>
        /// Get user privilege list. Then this will be used to contruct a user based run time Menu.  
        /// </summary>
        /// <returns></returns>
        public static List<UserMenu> GetMenuList()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    List<UserMenu> userMenuList = new List<UserMenu>();
                    ApplicationDbContext db = new ApplicationDbContext();
                    List<ApplicationUserRole> Roles = new List<ApplicationUserRole>();

                    ApplicationUser user = db.Users.Where(u => u.UserName == HttpContext.Current.User.Identity.Name).FirstOrDefault();
                    //Get User role list. Enrollment, curicullum or scheduler ,instructor trainee, dispacher
                    if (user != null)
                    {
                        foreach (var userRole in user.Roles)
                        {
                            Roles.Add(new ApplicationUserRole { RoleId = userRole.RoleId });
                        }
                    }

                    //For every role of that user, get privileges (ControllerName - ActionName)
                    foreach (ApplicationUserRole userRole in Roles)
                    {

                        string roleId = userRole.RoleId;
                        var role = db.Roles.Where(r => r.Id == roleId).ToList().FirstOrDefault();

                        List<ApplicationRolePrivilege> rolePrivilegeList = db.ApplicationRolePrivileges.Include("Privilage").Where(r => r.RoleId == roleId).ToList();
                        foreach (var rolePrivilege in rolePrivilegeList)
                        {
                            int i = 1;
                            var privilege = db.ApplicationPrivileges.Where(p => rolePrivilege.PrivilegeId == p.Id).ToList().FirstOrDefault();
                          
                            if (privilege != null)
                            {
                                if (!String.IsNullOrEmpty(privilege.Action))
                                {
                                    string[] controllerActionName = privilege.Action.Split('-');
                                    userMenuList.Add(new UserMenu
                                    {
                                        Id = i++,//privilege.Id != null ? Convert.ToInt16(privilege.Id) : 0,
                                        LinkText = controllerActionName[0],
                                        ControllerName = controllerActionName[0],
                                        ActionName = controllerActionName[1],
                                        Roles = role.Name
                                    });
                                }
                            }
                        }
                    }
                    return userMenuList;
                }
                return new List<UserMenu>();
            }
            catch (Exception ex)
            {
                return new List<UserMenu>();
            }

        }

        public static UserNotifications GetUserNotification()
        {
            NotificationLogic notificationLogic = new NotificationLogic();
            if (HttpContext.Current.User.Identity.Name != null)
            {
                return notificationLogic.GetUserNotification(HttpContext.Current.User.Identity.Name);
            }
            return new UserNotifications();
        }


        public static UserMessages GetUserMessages()
        {
            MessageLogic messageLogic = new MessageLogic();
            if (HttpContext.Current.User.Identity.Name != null)
            {
                return messageLogic.GetUserMessages(HttpContext.Current.User.Identity.Name);
            }
            return new UserMessages();
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

    }
}