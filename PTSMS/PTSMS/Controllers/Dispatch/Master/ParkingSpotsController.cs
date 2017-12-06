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
    public class ParkingSpotsController : Controller
    {
        private PTSContext db = new PTSContext();
        ParkingSpotLogic parkingSpotLogic = new ParkingSpotLogic();
        // GET: ParkingSpots
        public ActionResult Index()
        {
          
            return View(parkingSpotLogic.List());
        }

        // GET: ParkingSpots/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParkingSpot parkingSpot = (ParkingSpot)parkingSpotLogic.Details(id);
            if (parkingSpot == null)
            {
                return HttpNotFound();
            }
            return View(parkingSpot);
        }

        // GET: ParkingSpots/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.ParkingSpots, "ParkingSpotId", "ParkingSpotName");
            return View();
        }

        // POST: ParkingSpots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParkingSpotId,ParkingSpotName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] ParkingSpot parkingSpot)
        {
            if (ModelState.IsValid)
            {
                parkingSpotLogic.Add(parkingSpot);
                
                return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList(db.ParkingSpots, "ParkingSpotId", "ParkingSpotName", parkingSpot.RevisionGroupId);
            return View(parkingSpot);
        }

        // GET: ParkingSpots/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParkingSpot parkingSpot = (ParkingSpot)parkingSpotLogic.Details(id);
            if (parkingSpot == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.ParkingSpots, "ParkingSpotId", "ParkingSpotName", parkingSpot.RevisionGroupId);
            return View(parkingSpot);
        }

        // POST: ParkingSpots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ParkingSpotId,ParkingSpotName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] ParkingSpot parkingSpot)
        {
            if (ModelState.IsValid)
            {
                parkingSpotLogic.Revise(parkingSpot);
                return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.ParkingSpots, "ParkingSpotId", "ParkingSpotName", parkingSpot.RevisionGroupId);
            return View(parkingSpot);
        }

        // GET: ParkingSpots/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParkingSpot parkingSpot = (ParkingSpot)parkingSpotLogic.Details(id);
            if (parkingSpot == null)
            {
                return HttpNotFound();
            }
            return View(parkingSpot);
        }

        // POST: ParkingSpots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            parkingSpotLogic.Delete(id);
            return RedirectToAction("Index");
        }

       
    }
}
