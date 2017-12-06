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
    public class PeriodTemplatesController : Controller
    {
        private PTSContext db = new PTSContext();
        PeriodTemplateLogic periodTemplateLogic = new PeriodTemplateLogic();
        // GET: PeriodTemplates
        public ActionResult Index()
        {
            var periodTemplates = periodTemplateLogic.List();
            return View(periodTemplates);
        }

        // GET: PeriodTemplates/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodTemplate periodTemplate = periodTemplateLogic.Details(id);
            if (periodTemplate == null)
            {
                return HttpNotFound();
            }
            return View(periodTemplate);
        }

        // GET: PeriodTemplates/Create
        public ActionResult Create()
        {
            ViewBag.PreviousRevisionId = new SelectList(db.PeriodTemplates, "PeriodTemplateId", "PeriodTemplateName");
            return View();
        }

        // POST: PeriodTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PeriodTemplateId,PeriodTemplateName,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] PeriodTemplate periodTemplate)
        {
            if (ModelState.IsValid)
            {
                periodTemplateLogic.Add(periodTemplate);
                return RedirectToAction("Index");
            }

            return View(periodTemplate);
        }

        // GET: PeriodTemplates/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodTemplate periodTemplate = periodTemplateLogic.Details(id);
            if (periodTemplate == null)
            {
                return HttpNotFound();
            }
            return View(periodTemplate);
        }

        // POST: PeriodTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PeriodTemplateId,PeriodTemplateName,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] PeriodTemplate periodTemplate)
        {
            if (ModelState.IsValid)
            {
                periodTemplateLogic.Revise(periodTemplate);
                return RedirectToAction("Index");
            }
            return View(periodTemplate);
        }

        // GET: PeriodTemplates/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodTemplate periodTemplate = periodTemplateLogic.Details(id);
            if (periodTemplate == null)
            {
                return HttpNotFound();
            }
            return View(periodTemplate);
        }

        // POST: PeriodTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            periodTemplateLogic.Delete(id);
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
