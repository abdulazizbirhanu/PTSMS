using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch.Master;
using PTSMSBAL.Dispatch.Master;

namespace PTSMS.Controllers.Dispatch.Master
{
    public class DestinationsController : Controller
    {
         private PTSContext db = new PTSContext();
        DestinationLogic destinationLogic = new DestinationLogic();
        // GET: Destinations
        public ActionResult Index()
        {
           
            return View(destinationLogic.List());
        }

        // GET: Destinations/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination destination = destinationLogic.Details(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            return View(destination);
        }

        // GET: Destinations/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.Destinations, "DestinationId", "DestinationName");
            return View();
        }

        // POST: Destinations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DestinationId,DestinationName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Destination destination)
        {
            if (ModelState.IsValid)
            {
                destinationLogic.Add(destination);
                return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList(db.Destinations, "DestinationId", "DestinationName", destination.RevisionGroupId);
            return View(destination);
        }

        // GET: Destinations/Edit/5
        public ActionResult Edit(int id)
        {
            if (id ==0 )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination destination = destinationLogic.Details(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.Destinations, "DestinationId", "DestinationName", destination.RevisionGroupId);
            return View(destination);
        }

        // POST: Destinations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DestinationId,DestinationName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Destination destination)
        {
            if (ModelState.IsValid)
            {
                destinationLogic.Revise(destination);
                return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.Destinations, "DestinationId", "DestinationName", destination.RevisionGroupId);
            return View(destination);
        }

        // GET: Destinations/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination destination = destinationLogic.Details(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            return View(destination);
        }

        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            destinationLogic.Delete(id);
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
