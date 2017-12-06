using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSBAL.Logic.Enrollment.Operations;

namespace PTSMS.Controllers.Enrollment
{
    public class PersonController : Controller
    {
        private PTSContext db = new PTSContext();
        PersonLogic personLogic = new PersonLogic();
        // GET: Person
        public ActionResult Index()
        {
            return View(personLogic.List());
        }

        // GET: Person/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = (Person)personLogic.Details(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: Person/Create
        public ActionResult Create()
        {
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName");
           
            return View();
        }

        // POST: Person/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonId,CompanyId,FirstName,MiddleName,LastName,Sex,BirthDate,Position,RegistrationDate,LocationId,Status,Nationality,Address,City,Phone,Email,PhotoURL,ContactPerson1Name,ContactPerson1Address,ContactPerson1Phone,ContactPerson2Name,ContactPerson2Address,ContactPerson2Phone,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Person person)
        {
           // if (ModelState.IsValid)
           // {
                personLogic.Add(person);
              
                return RedirectToAction("Index");
          //  }

            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", person.LocationId);
            return View(person);
        }

        // GET: Person/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = (Person)personLogic.Details(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", person.LocationId);
            return View(person);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PersonId,CompanyId,FirstName,MiddleName,LastName,Sex,BirthDate,Position,RegistrationDate,LocationId,Status,Nationality,Address,City,Phone,Email,PhotoURL,ContactPerson1Name,ContactPerson1Address,ContactPerson1Phone,ContactPerson2Name,ContactPerson2Address,ContactPerson2Phone,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", person.LocationId);
            return View(person);
        }

        // GET: Person/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = db.Persons.Find(id);
            db.Persons.Remove(person);
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
