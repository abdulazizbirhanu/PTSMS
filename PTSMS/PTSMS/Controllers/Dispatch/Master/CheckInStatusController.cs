using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch.Master;

namespace PTSMS.Controllers.Dispatch.Master
{
    public class CheckInStatusController : Controller
    {
        private PTSContext db = new PTSContext();

        // GET: CheckInStatus
        public ActionResult Index()
        {
            var checkInStatuss = db.CheckInStatuss.Include(c => c.PreviousCheckInStatus);
            return View(checkInStatuss.ToList());
        }

        // GET: CheckInStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckInStatus checkInStatus = db.CheckInStatuss.Find(id);
            if (checkInStatus == null)
            {
                return HttpNotFound();
            }
            return View(checkInStatus);
        }

        // GET: CheckInStatus/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.CheckInStatuss, "CheckInStatusId", "CheckInStatusName");
            return View();
        }

        // POST: CheckInStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckInStatusId,CheckInStatusName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] CheckInStatus checkInStatus)
        {
            if (ModelState.IsValid)
            {
                db.CheckInStatuss.Add(checkInStatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList(db.CheckInStatuss, "CheckInStatusId", "CheckInStatusName", checkInStatus.RevisionGroupId);
            return View(checkInStatus);
        }

        // GET: CheckInStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckInStatus checkInStatus = db.CheckInStatuss.Find(id);
            if (checkInStatus == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.CheckInStatuss, "CheckInStatusId", "CheckInStatusName", checkInStatus.RevisionGroupId);
            return View(checkInStatus);
        }

        // POST: CheckInStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CheckInStatusId,CheckInStatusName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] CheckInStatus checkInStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(checkInStatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.CheckInStatuss, "CheckInStatusId", "CheckInStatusName", checkInStatus.RevisionGroupId);
            return View(checkInStatus);
        }

        // GET: CheckInStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckInStatus checkInStatus = db.CheckInStatuss.Find(id);
            if (checkInStatus == null)
            {
                return HttpNotFound();
            }
            return View(checkInStatus);
        }

        // POST: CheckInStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CheckInStatus checkInStatus = db.CheckInStatuss.Find(id);
            db.CheckInStatuss.Remove(checkInStatus);
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
