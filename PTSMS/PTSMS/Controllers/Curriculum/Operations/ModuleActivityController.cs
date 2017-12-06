using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;

namespace PTSMS.Controllers.Curriculum.Operations
{
    public class ModuleActivityController : Controller
    {
        private PTSContext db = new PTSContext();
        ModuleActivityAccess moduleActivityAccess = new ModuleActivityAccess();
        ModuleLogic moduleLogic = new ModuleLogic();
        // GET: ModuleActivity
        public ActionResult Index()
        {
            var moduleActivitys = db.ModuleActivitys.Include(m => m.Module);
            return View(moduleActivitys.ToList());
        }

        public PartialViewResult ModuleActivityList(int moduleId)
        {
            ViewBag.ModuleId = moduleId;
            ViewBag.ModuleCode = db.Modules.Find(moduleId).ModuleCode + " - " + db.Modules.Find(moduleId).ModuleTitle;
            var moduleActivitys = db.ModuleActivitys.Include(m => m.Module).Where(m => m.ModuleId == moduleId).ToList();
            return PartialView(moduleActivitys.ToList());
        }

        public PartialViewResult CreateModuleActivity(int moduleId)
        {
            ModuleActivity moduleActivity = new ModuleActivity();
            ViewBag.ModuleId = moduleId;
            ViewBag.ModuleCode = db.Modules.Find(moduleId).ModuleCode + " - " + db.Modules.Find(moduleId).ModuleTitle;
            return PartialView(moduleActivity);
        }

        public PartialViewResult EditModuleActivity(int moduleActivityId)
        {            
            ModuleActivity moduleActivity = moduleActivityAccess.Details(moduleActivityId);   
            ViewBag.ModuleId = new SelectList((List<Module>)moduleLogic.List(), "ModuleId", "ModuleCode", moduleActivity.ModuleId);  
            return PartialView(moduleActivity);
        }

        // GET: ModuleActivity/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.ModuleActivitys.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }
            return View(moduleActivity);
        }

        // GET: ModuleActivity/Create
        public ActionResult SaveModuleActivity(int moduleId, string moduleActivityList)
        {
            try
            {
                if (!string.IsNullOrEmpty(moduleActivityList) && moduleId > 0)
                {
                    string[] moduleActivityArray = moduleActivityList.Split('~');

                    foreach (var moduleActivity in moduleActivityArray)
                    {
                        if (!string.IsNullOrEmpty(moduleActivity))
                        {
                            string[] moduleActivityValuePair = moduleActivity.Split('-');

                            if (!string.IsNullOrEmpty(moduleActivityValuePair[0]) && !string.IsNullOrEmpty(moduleActivityValuePair[1]))
                            {
                                db.ModuleActivitys.Add(new ModuleActivity
                                {
                                    ModuleId = moduleId,
                                    EstimatedDuration = Convert.ToDouble(moduleActivityValuePair[1]),
                                    ModuleActivityName = moduleActivityValuePair[0],
                                    CreationDate = DateTime.Now,
                                    StartDate = DateTime.Now,
                                    EndDate = DateTime.MaxValue,
                                    RevisionDate = DateTime.Now
                                });
                                db.SaveChanges();
                            }
                        }
                    }
                    TempData["ModuleMessage"] = "Module activity successfully saved.";
                }
                else
                    TempData["ModuleMessage"] = "Invalid input for module activity.";
                return RedirectToAction("Index", "Modules", new { });

            }
            catch (Exception)
            {
                TempData["ModuleMessage"] = "Failed to save module activity.";
                return RedirectToAction("Index", "Modules", new { });
            }

        }
        public ActionResult Create()
        {
            ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "ModuleCode");
            return View();
        }

        // POST: ModuleActivity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ModuleActivityId,ModuleActivityName,EstimatedDuration,ModuleId,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] ModuleActivity moduleActivity)
        {
            if (ModelState.IsValid)
            {
                db.ModuleActivitys.Add(moduleActivity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "ModuleCode", moduleActivity.ModuleId);
            return View(moduleActivity);
        }

        // GET: ModuleActivity/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.ModuleActivitys.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }
            ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "ModuleCode", moduleActivity.ModuleId);
            return View(moduleActivity);
        }

        // POST: ModuleActivity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModuleActivityId,ModuleActivityName,EstimatedDuration,ModuleId,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] ModuleActivity moduleActivity)
        {
            if (ModelState.IsValid)
            {
                if (moduleActivityAccess.Revise(moduleActivity))
                    TempData["ModuleMessage"] = "Module activity successfully edited";
                else
                    TempData["ModuleMessage"] = "Failed to edit module activity.";
                return RedirectToAction("Index", "Modules", new { });
            }
            TempData["ModuleMessage"] = "Failed to edit module activity due to invalid input.";
            return RedirectToAction("Index", "Modules", new { });
        }



        // GET: ModuleActivity/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.ModuleActivitys.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }
            return View(moduleActivity);
        }

        public PartialViewResult DeleteModuleActivity(int moduleActivityId)
        {
            ModuleActivity moduleActivity = db.ModuleActivitys.Find(moduleActivityId);
            ViewBag.ModuleCode = db.Modules.Find(moduleActivity.ModuleId).ModuleCode + " - " + db.Modules.Find(moduleActivity.ModuleId).ModuleTitle;
            return PartialView(moduleActivity);
        }


        // POST: ModuleActivity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModuleActivity moduleActivity = db.ModuleActivitys.Find(id);
            db.ModuleActivitys.Remove(moduleActivity);
            db.SaveChanges();
            return RedirectToAction("Index", "Modules", new { });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
