using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSBAL.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMS.Controllers.Scheduling
{
    [PTSAuthorizeAttribute]
    public class QualificationTypesController : Controller
    {
        private PTSContext db = new PTSContext();
        QualificationTypeLogic qualificationTypeLogic = new QualificationTypeLogic();

        // GET: QualificationTypes
        public ActionResult Index()
        {
            return View(qualificationTypeLogic.List());
        }

        // GET: QualificationTypes/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualificationType qualificationType = qualificationTypeLogic.Details(id);
            if (qualificationType == null)
            {
                return HttpNotFound();
            }
            return View(qualificationType);
        }

        // GET: QualificationTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QualificationTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QualificationTypeId,Type,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] QualificationType qualificationType)
        {
            if (ModelState.IsValid)
            {
                qualificationTypeLogic.Add(qualificationType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(qualificationType);
        }

        // GET: QualificationTypes/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualificationType qualificationType = qualificationTypeLogic.Details(id); 
            if (qualificationType == null)
            {
                return HttpNotFound();
            }
            return View(qualificationType);
        }

        // POST: QualificationTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QualificationTypeId,Type,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] QualificationType qualificationType)
        {
            if (ModelState.IsValid)
            {
                qualificationTypeLogic.Revise(qualificationType);
                return RedirectToAction("Index");
            }
            return View(qualificationType);
        }

        // GET: QualificationTypes/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualificationType qualificationType = qualificationTypeLogic.Details(id);
            if (qualificationType == null)
            {
                return HttpNotFound();
            }
            return View(qualificationType);
        }

        // POST: QualificationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            qualificationTypeLogic.Delete(id);
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
