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
    public class DaysController : Controller
    {
        private PTSContext db = new PTSContext();
        DayLogic dayLogic = new DayLogic();
        // GET: Days
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            var days = dayLogic.List();
            return View(days);
        }

        // GET: Days/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Day day = dayLogic.Details(id);
            if (day == null)
            {
                return HttpNotFound();
            }
            return View(day);
        }

        // GET: Days/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.DayTemplateId = new SelectList(db.DayTemplates, "DayTemplateId", "DayTemplateName");
            ViewBag.PreviousRevisionId = new SelectList(db.Days, "DayId", "Status");
            return View();
        }

        // POST: Days/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create([Bind(Include = "DayId,DayTemplateId,DayName,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Day day)
        {
            if (ModelState.IsValid)
            {
                dayLogic.Add(day);
                return RedirectToAction("Index");
            }

            ViewBag.DayTemplateId = new SelectList(db.DayTemplates, "DayTemplateId", "DayTemplateName", day.DayTemplateId);
            return View(day);
        }

        // GET: Days/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Day day = dayLogic.Details(id);
            if (day == null)
            {
                return HttpNotFound();
            }
            ViewBag.DayTemplateId = new SelectList(db.DayTemplates, "DayTemplateId", "DayTemplateName", day.DayTemplateId);
            return View(day);
        }

        // POST: Days/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit([Bind(Include = "DayId,DayTemplateId,DayName,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Day day)
        {
            if (ModelState.IsValid)
            {
                dayLogic.Revise(day);
                return RedirectToAction("Index");
            }
            ViewBag.DayTemplateId = new SelectList(db.DayTemplates, "DayTemplateId", "DayTemplateName", day.DayTemplateId);
            return View(day);
        }

        // GET: Days/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Day day = dayLogic.Details(id);
            if (day == null)
            {
                return HttpNotFound();
            }
            return View(day);
        }

        // POST: Days/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(int id)
        {
            dayLogic.Delete(id);
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
