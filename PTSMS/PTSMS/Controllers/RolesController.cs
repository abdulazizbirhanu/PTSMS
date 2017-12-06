using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using PTSMS.Models;

namespace PTSMS.Controllers
{
    public class RolesController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        //
        // GET: /Roles/Index
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            var rolesList = new List<RoleViewModel>();
            foreach (var role in _db.Roles)
            {
                var roleModel = new RoleViewModel((ApplicationRole)role);
                rolesList.Add(roleModel);
            }
            if (TempData["RoleMessage"] != null)
            {
                ViewBag.RoleMessage = TempData["RoleMessage"];
                TempData["RoleMessage"] = null;
            }
            rolesList = rolesList.OrderBy(r => r.RoleName).ToList();
            return View(rolesList);
        }

        //
        // GET: /Roles/Create
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Create(string message = "")
        {
            //var privilegeList = _db.ApplicationPrivileges.OrderBy(x=>x.Action).ToList();

            ViewBag.Privileges = _db.ApplicationPrivileges.ToList();
            //ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
            ViewBag.Message = message;
            return View();
        }

        //
        // POST: /Roles/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create([Bind(Include =
            "RoleName,Description")]RoleViewModel model)
        {
            string message = "That role name has already been used";
            string messagePrivilege = "The role must have at least one corresponding privilege";
            string pri = Request.Form["privilege"];
            if (ModelState.IsValid && pri != null)
            {
                ApplicationRoleManager _roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(_db));
                if (_db.RoleExists(_roleManager, model.RoleName))
                {
                    return View(message);
                }
                else
                {
                    if (_db.CreateRole(_roleManager, model.RoleName, model.Description))
                    {
                        var role = _db.Roles.First(r => r.Name == model.RoleName);
                        string[] privileges = pri.Split(',');
                        foreach (var item in privileges)
                        {
                            _db.ApplicationRolePrivileges.Add(new ApplicationRolePrivilege { RoleId = role.Id, PrivilegeId = item });
                        }
                        _db.SaveChanges();
                    }
                    TempData["RoleMessage"] = "Role successfully inserted.";
                    return RedirectToAction("Index", "Roles");
                }
            }
            return View(messagePrivilege);
        }

        //
        // GET: /Roles/Edit
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(string name)
        {
            string selected = "";
            var role = _db.Roles.First(r => r.Name == name);
            List<ApplicationRolePrivilege> p = _db.ApplicationRolePrivileges.Where(x => x.RoleId == role.Id).ToList();
            foreach (var item in p)
            {
                selected += item.PrivilegeId + ",";
            }
            ViewBag.Selected = selected;
            ViewBag.Privileges = _db.ApplicationPrivileges.OrderBy(r => r.Action).ToList();
            var model = new RoleViewModel();
            model.RoleName = role.Name;
            return View(model);
        }

        //
        // POST: /Roles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(RoleViewModel model)
        {
            string privilege = Request.Form["privilege"];
            if (privilege != null)
            {
                var role = _db.Roles.FirstOrDefault(x => x.Name == model.RoleName);
                if (role != null)
                {
                    List<ApplicationRolePrivilege> p = _db.ApplicationRolePrivileges.Where(x => x.RoleId == role.Id).ToList();
                    _db.ApplicationRolePrivileges.RemoveRange(p);
                    string[] roles = privilege.Split(',');
                    foreach (var item in roles)
                    {
                        ApplicationRolePrivilege pri = new ApplicationRolePrivilege();
                        pri.PrivilegeId = item;
                        pri.RoleId = role.Id;
                        _db.ApplicationRolePrivileges.Add(pri);
                    }
                    _db.SaveChanges();
                    TempData["RoleMessage"] = "Role successfully updated.";
                    return RedirectToAction("Index", "Roles");
                }
            }
            return View(model);
        }

        //
        // GET: /Roles/Delete        
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Delete(string name)
        {
            RoleViewModel model;
            if (name == null)
            {
                return RedirectToAction("BadRequest", "Errors");
            }
            var role = _db.Roles.First(r => r.Name == name);
            var exist = role.Users.FirstOrDefault(u => u.RoleId == role.Id);
            if (exist != null)
            {
                TempData["RoleMessage"] = "You cannot delete this role. It is given to some users.";
                return RedirectToAction("Index", "Roles");
            }
            else
            {
                model = new RoleViewModel((ApplicationRole)role);
            }
            return View(model);
        }

        //
        // POST: /Roles/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(string name)
        {
            var role = _db.Roles.First(r => r.Name == name);
            var privilege = _db.ApplicationRolePrivileges.Where(p => p.RoleId == role.Id);
            foreach (var item in privilege)
                _db.ApplicationRolePrivileges.Remove(item);
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_db));
            _db.DeleteRole(_db, userManager, role.Id);
            return RedirectToAction("Index");
        }
    }
}