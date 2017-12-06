using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMS.Controllers.Scheduling
{
    public class LicensesController : Controller
    {
        private PTSContext db = new PTSContext();
        LicenseLogic licenseLogic = new LicenseLogic(); 
        // GET: Licenses
        public ActionResult Index()
        {
            var licenses = licenseLogic.List();
            return View(licenses);
        }

        // GET: Licenses/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = licenseLogic.Details(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            return View(license);
        }

        // GET: Licenses/Create
        public ActionResult Create()
        {
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "Type");
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId");
            ViewBag.PersonDocumentId = new SelectList(db.PersonDocuments, "PersonDocumentId", "Description");
            return View();
        }

        // POST: Licenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LicenseId,PersonId,LicenseTypeId,LicenseNo,Description,IssueDate,ExpiryDate,Threshold,PersonDocumentId,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] License license)
        {
            if (ModelState.IsValid)
            {
                licenseLogic.Add(license);
                return RedirectToAction("Index");
            }

            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "Type", license.LicenseTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", license.PersonId);
            ViewBag.PersonDocumentId = new SelectList(db.PersonDocuments, "PersonDocumentId", "Description", license.PersonDocumentId);
            return View(license);
        }

        // GET: Licenses/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = licenseLogic.Details(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "Type", license.LicenseTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", license.PersonId);
            ViewBag.PersonDocumentId = new SelectList(db.PersonDocuments, "PersonDocumentId", "Description", license.PersonDocumentId);
            return View(license);
        }

        // POST: Licenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LicenseId,PersonId,LicenseTypeId,LicenseNo,Description,IssueDate,ExpiryDate,Threshold,PersonDocumentId,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] License license)
        {
            if (ModelState.IsValid)
            {
                licenseLogic.Revise(license);
                return RedirectToAction("Index");
            }
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "Type", license.LicenseTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", license.PersonId);
            ViewBag.PersonDocumentId = new SelectList(db.PersonDocuments, "PersonDocumentId", "Description", license.PersonDocumentId);
            return View(license);
        }

        // GET: Licenses/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = licenseLogic.Details(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            return View(license);
        }

        // POST: Licenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            licenseLogic.Delete(id);
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
