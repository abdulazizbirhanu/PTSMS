using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using PTSMS.Models;

namespace PTSMS.Controllers
{
    public class PrivilegesController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        //
        // GET: /Privileges/Index
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(_db.ApplicationPrivileges.ToList());
        }

        //
        // GET: /Privileges/Create
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Create(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }

        //
        // POST: /Privileges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create([Bind(Include =
            "Id,Action,Description")]ApplicationPrivilege model)
        {
            string message = "That privilege name has already been used";
            if (ModelState.IsValid)
            {
                ApplicationPrivilege pre = _db.ApplicationPrivileges.Find(model.Id);
                if (pre != null)
                {
                    return View(message);
                }
                else
                {
                    model.Id = Guid.NewGuid().ToString();
                    _db.ApplicationPrivileges.Add(model);
                    _db.SaveChanges();
                    return RedirectToAction("Index", "Privileges");
                }
            }
            return View();
        }

        //
        // GET: /Privileges/Edit
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(string paction)
        {
            if (paction == null)
            {
                return RedirectToAction("BadRequest", "Errors");
            }
            ApplicationPrivilege previlege = _db.ApplicationPrivileges.First(p => p.Action == paction);
            if (previlege == null)
            {
                return RedirectToAction("NotFound", "Errors");
            }
            return View(previlege);
        }

        //
        // POST: /Privileges/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit([Bind(Include =
            "Id,Action,Description")] ApplicationPrivilege model)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(model).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        //
        // GET: /Roles/Delete        
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Delete(string paction)
        {
            if (paction == null)
            {
                return RedirectToAction("BadRequest", "Errors");
            }
            ApplicationPrivilege privilege = _db.ApplicationPrivileges.First(r => r.Action == paction);
            return View(privilege);
        }

        //
        // POST: /Roles/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(string paction)
        {
            ApplicationPrivilege privilege = _db.ApplicationPrivileges.First(p => p.Action == paction);
            List<ApplicationRolePrivilege> rolePrivilege =
                _db.ApplicationRolePrivileges.Where(rp => rp.PrivilegeId == privilege.Id).ToList();
            _db.ApplicationPrivileges.Remove(privilege);
            _db.ApplicationRolePrivileges.RemoveRange(rolePrivilege);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
