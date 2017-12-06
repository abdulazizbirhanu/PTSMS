using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSBAL.Logic.Scheduling.References;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMS.Controllers.Scheduling
{
    public class LocationsController : Controller
    {
        private PTSContext db = new PTSContext();
        LocationLogic locationlogic = new LocationLogic();
        // GET: Locations
        public ActionResult Index()
        {
            var locations = locationlogic.List();
            return View(locations);
        }

        // GET: Locations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = (Location)locationlogic.Details((int)id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);

            
        }

        // GET: Locations/Create
        public ActionResult Create()
        {
            ViewBag.PreviousRevisionId = new SelectList(db.Locations, "LocationId", "LocationName");
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LocationId,LocationName,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Location location)
        {
            if (ModelState.IsValid)
            {
                locationlogic.Add(location);
                return RedirectToAction("Index");
            }

            ViewBag.PreviousRevisionId = new SelectList(db.Locations, "LocationId", "LocationName", location.PreviousRevisionId);
            return View(location);
        }

        // GET: Locations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = (Location)locationlogic.Details((int)id);
            if (location == null)
            {
                return HttpNotFound();
            }
            ViewBag.PreviousRevisionId = new SelectList(db.Locations, "LocationId", "LocationName", location.PreviousRevisionId);
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LocationId,LocationName,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Location location)
        {
            if (ModelState.IsValid)
            {
                locationlogic.Revise(location);
                return RedirectToAction("Index");
            }
            ViewBag.PreviousRevisionId = new SelectList(db.Locations, "LocationId", "LocationName", location.PreviousRevisionId);
            return View(location);
        }

        // GET: Locations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = (Location)locationlogic.Details((int)id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            locationlogic.Delete(id);
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
