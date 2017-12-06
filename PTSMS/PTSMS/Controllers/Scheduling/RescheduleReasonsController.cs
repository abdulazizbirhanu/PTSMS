using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSBAL.Scheduling.Operations;

namespace PTSMS.Controllers.Scheduling
{
    public class RescheduleReasonsController : Controller
    {
        private PTSContext db = new PTSContext();
        RescheduleReasonLogic rescheduleReasonLogic = new RescheduleReasonLogic();
        // GET: RescheduleReasons
        public ActionResult Index()
        {
          
            return View(rescheduleReasonLogic.List());
        }

        // GET: RescheduleReasons/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RescheduleReason rescheduleReason = rescheduleReasonLogic.Details(id);
            if (rescheduleReason == null)
            {
                return HttpNotFound();
            }
            return View(rescheduleReason);
        }

        // GET: RescheduleReasons/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.RescheduleReasons, "RescheduleReasonId", "RescheduleReasonName");
            return View();
        }

        // POST: RescheduleReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RescheduleReasonId,RescheduleReasonName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] RescheduleReason rescheduleReason)
        {
            if (ModelState.IsValid)
            {
                rescheduleReasonLogic.Add(rescheduleReason);
              
                return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList(db.RescheduleReasons, "RescheduleReasonId", "RescheduleReasonName", rescheduleReason.RevisionGroupId);
            return View(rescheduleReason);
        }

        // GET: RescheduleReasons/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RescheduleReason rescheduleReason = rescheduleReasonLogic.Details(id);
            if (rescheduleReason == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.RescheduleReasons, "RescheduleReasonId", "RescheduleReasonName", rescheduleReason.RevisionGroupId);
            return View(rescheduleReason);
        }

        // POST: RescheduleReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RescheduleReasonId,RescheduleReasonName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] RescheduleReason rescheduleReason)
        {
            if (ModelState.IsValid)
            {
                rescheduleReasonLogic.Revise(rescheduleReason);
                return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.RescheduleReasons, "RescheduleReasonId", "RescheduleReasonName", rescheduleReason.RevisionGroupId);
            return View(rescheduleReason);
        }

        // GET: RescheduleReasons/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RescheduleReason rescheduleReason = rescheduleReasonLogic.Details(id);
            if (rescheduleReason == null)
            {
                return HttpNotFound();
            }
            return View(rescheduleReason);
        }

        // POST: RescheduleReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            rescheduleReasonLogic.Delete(id);
            return RedirectToAction("Index");
        }

      
    }
}
