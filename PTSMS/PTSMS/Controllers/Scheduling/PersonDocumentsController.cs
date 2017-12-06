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
    public class PersonDocumentsController : Controller
    {
        private PTSContext db = new PTSContext();
        PersonDocumentLogic personDocumentLogic = new PersonDocumentLogic();
        // GET: PersonDocuments
        public ActionResult Index()
        {
            var personDocuments = personDocumentLogic.List();
            return View(personDocuments);
        }

        // GET: PersonDocuments/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonDocument personDocument = personDocumentLogic.Details(id);
            if (personDocument == null)
            {
                return HttpNotFound();
            }
            return View(personDocument);
        }

        // GET: PersonDocuments/Create
        public ActionResult Create()
        {
            ViewBag.DocumentTypeId = new SelectList(db.Documents, "DocumentId", "DocumentTypeName");
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId");
            return View();
        }

        // POST: PersonDocuments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonDocumentId,DocumentTypeId,PersonId,Description,DocumentURL,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] PersonDocument personDocument)
        {
            if (ModelState.IsValid)
            {
                personDocumentLogic.Add(personDocument);
                return RedirectToAction("Index");
            }

            ViewBag.DocumentTypeId = new SelectList(db.Documents, "DocumentId", "DocumentTypeName", personDocument.DocumentTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", personDocument.PersonId);
            return View(personDocument);
        }

        // GET: PersonDocuments/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonDocument personDocument = personDocumentLogic.Details(id);
            if (personDocument == null)
            {
                return HttpNotFound();
            }
            ViewBag.DocumentTypeId = new SelectList(db.Documents, "DocumentId", "DocumentTypeName", personDocument.DocumentTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", personDocument.PersonId);
            return View(personDocument);
        }

        // POST: PersonDocuments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PersonDocumentId,DocumentTypeId,PersonId,Description,DocumentURL,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] PersonDocument personDocument)
        {
            if (ModelState.IsValid)
            {
                personDocumentLogic.Revise(personDocument);
                return RedirectToAction("Index");
            }
            ViewBag.DocumentTypeId = new SelectList(db.Documents, "DocumentId", "DocumentTypeName", personDocument.DocumentTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", personDocument.PersonId);
            return View(personDocument);
        }

        // GET: PersonDocuments/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonDocument personDocument = personDocumentLogic.Details(id);
            if (personDocument == null)
            {
                return HttpNotFound();
            }
            return View(personDocument);
        }

        // POST: PersonDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            personDocumentLogic.Delete(id);
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
