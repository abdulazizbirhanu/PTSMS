using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMS.Controllers.Scheduling
{
    public class LeaveTypesController : Controller
    {
        private PTSContext db = new PTSContext();

        // GET: LeaveTypes
        public ActionResult Index()
        {
            return View(db.LeaveTypes.ToList());
        }

        // GET: LeaveTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveType leaveType = db.LeaveTypes.Find(id);
            if (leaveType == null)
            {
                return HttpNotFound();
            }
            return View(leaveType);
        }

        // GET: LeaveTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LeaveTypeId,Type,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] LeaveType leaveType)
        {
            if (ModelState.IsValid)
            {
                db.LeaveTypes.Add(leaveType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(leaveType);
        }

        // GET: LeaveTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveType leaveType = db.LeaveTypes.Find(id);
            if (leaveType == null)
            {
                return HttpNotFound();
            }
            return View(leaveType);
        }

        // POST: LeaveTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LeaveTypeId,Type,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] LeaveType leaveType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(leaveType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leaveType);
        }

        // GET: LeaveTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveType leaveType = db.LeaveTypes.Find(id);
            if (leaveType == null)
            {
                return HttpNotFound();
            }
            return View(leaveType);
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LeaveType leaveType = db.LeaveTypes.Find(id);
            db.LeaveTypes.Remove(leaveType);
            db.SaveChanges();
            return RedirectToAction("Index");
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
