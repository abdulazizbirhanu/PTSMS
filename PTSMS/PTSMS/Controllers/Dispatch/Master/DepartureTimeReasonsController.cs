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
    public class DepartureTimeReasonsController : Controller
    {
        private PTSContext db = new PTSContext();

        // GET: DepartureTimeReasons
        public ActionResult Index()
        {
            var departureTimeReasons = db.DepartureTimeReasons.Include(d => d.PreviousDepartureTimeReason);
            return View(departureTimeReasons.ToList());
        }

        // GET: DepartureTimeReasons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartureTimeReason departureTimeReason = db.DepartureTimeReasons.Find(id);
            if (departureTimeReason == null)
            {
                return HttpNotFound();
            }
            return View(departureTimeReason);
        }

        // GET: DepartureTimeReasons/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.DepartureTimeReasons, "DepartureTimeReasonId", "DepartureTimeReasonName");
            return View();
        }

        // POST: DepartureTimeReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartureTimeReasonId,DepartureTimeReasonName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] DepartureTimeReason departureTimeReason)
        {
            if (ModelState.IsValid)
            {
                db.DepartureTimeReasons.Add(departureTimeReason);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList(db.DepartureTimeReasons, "DepartureTimeReasonId", "DepartureTimeReasonName", departureTimeReason.RevisionGroupId);
            return View(departureTimeReason);
        }

        // GET: DepartureTimeReasons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartureTimeReason departureTimeReason = db.DepartureTimeReasons.Find(id);
            if (departureTimeReason == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.DepartureTimeReasons, "DepartureTimeReasonId", "DepartureTimeReasonName", departureTimeReason.RevisionGroupId);
            return View(departureTimeReason);
        }

        // POST: DepartureTimeReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartureTimeReasonId,DepartureTimeReasonName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] DepartureTimeReason departureTimeReason)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departureTimeReason).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.DepartureTimeReasons, "DepartureTimeReasonId", "DepartureTimeReasonName", departureTimeReason.RevisionGroupId);
            return View(departureTimeReason);
        }

        // GET: DepartureTimeReasons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartureTimeReason departureTimeReason = db.DepartureTimeReasons.Find(id);
            if (departureTimeReason == null)
            {
                return HttpNotFound();
            }
            return View(departureTimeReason);
        }

        // POST: DepartureTimeReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DepartureTimeReason departureTimeReason = db.DepartureTimeReasons.Find(id);
            db.DepartureTimeReasons.Remove(departureTimeReason);
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
