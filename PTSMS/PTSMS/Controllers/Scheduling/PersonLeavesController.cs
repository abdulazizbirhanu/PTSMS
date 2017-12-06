using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSBAL.Enrollment.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMS.Controllers.Scheduling
{
    public class PersonLeavesController : Controller
    {
        private PTSContext db = new PTSContext();
        PersonLeaveLogic personLeaveLogic = new PersonLeaveLogic();
        // GET: PersonLeaves
        public ActionResult Index()
        {
            var personLeaves = personLeaveLogic.List();
            return View(personLeaves);
        }

        // GET: PersonLeaves/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonLeave personLeave = personLeaveLogic.Details(id);
            if (personLeave == null)
            {
                return HttpNotFound();
            }
            return View(personLeave);
        }

        // GET: PersonLeaves/Create
        public ActionResult Create()
        {
            ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "LeaveTypeId", "Type");
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId");
            return View();
        }

        // POST: PersonLeaves/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonLeaveId,LeaveTypeId,PersonId,StartDate,EndDate,Description,CreationDate,CreatedBy,RevisionDate,RevisedBy,FromDate,ToDate")] PersonLeave personLeave)
        {
            if (ModelState.IsValid)
            {
                personLeaveLogic.Add(personLeave);
                return RedirectToAction("Index");
            }

            ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "LeaveTypeId", "Type", personLeave.LeaveTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", personLeave.PersonId);
            return View(personLeave);
        }

        // GET: PersonLeaves/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonLeave personLeave = personLeaveLogic.Details(id);
            if (personLeave == null)
            {
                return HttpNotFound();
            }
            ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "LeaveTypeId", "Type", personLeave.LeaveTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", personLeave.PersonId);
            return View(personLeave);
        }

        // POST: PersonLeaves/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PersonLeaveId,LeaveTypeId,PersonId,StartDate,EndDate,Description,CreationDate,CreatedBy,RevisionDate,RevisedBy,,FromDate,ToDate")] PersonLeave personLeave)
        {
            if (ModelState.IsValid)
            {
                personLeaveLogic.Revise(personLeave);
                return RedirectToAction("Index");
            }
            ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "LeaveTypeId", "Type", personLeave.LeaveTypeId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "CompanyId", personLeave.PersonId);
            return View(personLeave);
        }

        // GET: PersonLeaves/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonLeave personLeave = personLeaveLogic.Details(id);
            if (personLeave == null)
            {
                return HttpNotFound();
            }
            return View(personLeave);
        }

        // POST: PersonLeaves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            personLeaveLogic.Delete(id);
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
