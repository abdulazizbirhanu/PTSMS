using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSBAL.Logic.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMS.Controllers.Scheduling
{
    [PTSAuthorizeAttribute]
    public class PeriodsController : Controller
    {
        private PTSContext db = new PTSContext();
        PeriodLogic periodLogic = new PeriodLogic(); 

        // GET: Periods
        public ActionResult Index()
        {
            var periods = periodLogic.List();
            return View(periods);
        }

        // GET: Periods/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = periodLogic.Details(id);
            if (period == null)
            {
                return HttpNotFound();
            }
            return View(period);
        }

        // GET: Periods/Create
        public ActionResult Create()
        {
            ViewBag.PeriodTemplateId = new SelectList(db.PeriodTemplates, "PeriodTemplateId", "PeriodTemplateName");
            ViewBag.PreviousRevisionId = new SelectList(db.Periods, "PeriodId", "PeriodName");
            return View();
        }

        // POST: Periods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PeriodId,PeriodName,PeriodTemplateId,StartTime,EndTime,AMorPM,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Period period)
        {
            if (ModelState.IsValid)
            {
                periodLogic.Add(period);
                return RedirectToAction("Index");
            }

            ViewBag.PeriodTemplateId = new SelectList(db.PeriodTemplates, "PeriodTemplateId", "PeriodTemplateName", period.PeriodTemplateId);
            return View(period);
        }

        // GET: Periods/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = periodLogic.Details(id);
            if (period == null)
            {
                return HttpNotFound();
            }
            ViewBag.PeriodTemplateId = new SelectList(db.PeriodTemplates, "PeriodTemplateId", "PeriodTemplateName", period.PeriodTemplateId);
            return View(period);
        }

        // POST: Periods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PeriodId,PeriodName,PeriodTemplateId,StartTime,EndTime,AMorPM,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Period period)
        {
            if (ModelState.IsValid)
            {
                periodLogic.Revise(period);
                return RedirectToAction("Index");
            }
            ViewBag.PeriodTemplateId = new SelectList(db.PeriodTemplates, "PeriodTemplateId", "PeriodTemplateName", period.PeriodTemplateId);
            return View(period);
        }

        // GET: Periods/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = periodLogic.Details(id);
            if (period == null)
            {
                return HttpNotFound();
            }
            return View(period);
        }

        // POST: Periods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            periodLogic.Delete(id);
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
