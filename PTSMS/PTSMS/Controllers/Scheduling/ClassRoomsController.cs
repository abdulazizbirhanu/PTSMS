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
    public class ClassRoomsController : Controller
    {
        private PTSContext db = new PTSContext();
        ClassRoomLogic classRoomLogic = new ClassRoomLogic();

        // GET: ClassRooms
        public ActionResult Index()
        {
            var classRooms = classRoomLogic.List();
            return View(classRooms);
        }

        // GET: ClassRooms/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassRoom classRoom = classRoomLogic.Details(id);
            if (classRoom == null)
            {
                return HttpNotFound();
            }
            return View(classRoom);
        }

        // GET: ClassRooms/Create
        public ActionResult Create()
        {
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "BuildingName");
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName");
            return View();
        }

        // POST: ClassRooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassRoomId,RoomNo,BuildingId,Capacity,LocationId,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] ClassRoom classRoom)
        {
            if (ModelState.IsValid)
            {
                classRoomLogic.Add(classRoom);
                return RedirectToAction("Index");
            }

            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "BuildingName", classRoom.BuildingId);
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", classRoom.LocationId);
            return View(classRoom);
        }

        // GET: ClassRooms/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassRoom classRoom = classRoomLogic.Details(id);
            if (classRoom == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "BuildingName", classRoom.BuildingId);
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", classRoom.LocationId);
            return View(classRoom);
        }

        // POST: ClassRooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassRoomId,RoomNo,BuildingId,Capacity,LocationId,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] ClassRoom classRoom)
        {
            if (ModelState.IsValid)
            {
                classRoomLogic.Revise(classRoom);
                return RedirectToAction("Index");
            }
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "BuildingName", classRoom.BuildingId);
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", classRoom.LocationId);
            return View(classRoom);
        }

        // GET: ClassRooms/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassRoom classRoom = classRoomLogic.Details(id);
            if (classRoom == null)
            {
                return HttpNotFound();
            }
            return View(classRoom);
        }

        // POST: ClassRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            classRoomLogic.Delete(id);
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
