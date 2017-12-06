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
    public class DayTemplatesController : Controller
    {
        private PTSContext db = new PTSContext();
        DayTemplateLogic dayTemplateLogic = new DayTemplateLogic();
        // GET: DayTemplates
        public ActionResult Index()
        {
            var dayTemplates = dayTemplateLogic.List();
            return View(dayTemplates);
        }

        // GET: DayTemplates/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DayTemplate dayTemplate = dayTemplateLogic.Details(id);
            if (dayTemplate == null)
            {
                return HttpNotFound();
            }
            return View(dayTemplate);
        }

        // GET: DayTemplates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DayTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DayTemplateId,DayTemplateName,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] DayTemplate dayTemplate)
        {
            if (ModelState.IsValid)
            {
                dayTemplateLogic.Add(dayTemplate);
                return RedirectToAction("Index");
            }
            return View(dayTemplate);
        }

        // GET: DayTemplates/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DayTemplate dayTemplate = dayTemplateLogic.Details(id);
            if (dayTemplate == null)
            {
                return HttpNotFound();
            }
            return View(dayTemplate);
        }

        // POST: DayTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DayTemplateId,DayTemplateName,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] DayTemplate dayTemplate)
        {
            if (ModelState.IsValid)
            {
                dayTemplateLogic.Revise(dayTemplate);
                return RedirectToAction("Index");
            }
            return View(dayTemplate);
        }

        // GET: DayTemplates/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DayTemplate dayTemplate = dayTemplateLogic.Details(id);
            if (dayTemplate == null)
            {
                return HttpNotFound();
            }
            return View(dayTemplate);
        }

        // POST: DayTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            dayTemplateLogic.Delete(id);
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
