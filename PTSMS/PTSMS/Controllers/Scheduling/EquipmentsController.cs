using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSBAL.Scheduling.Relations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSBAL.Logic.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSBAL.Logic.Scheduling.Operations;

namespace PTSMS.Controllers.Scheduling
{
    public class EquipmentsController : Controller
    {
        private PTSContext db = new PTSContext();
        EquipmentLogic equipmentLogic = new EquipmentLogic();
        LocationLogic locationLogic = new LocationLogic();
        ClassRoomLogic classRoomLogic = new ClassRoomLogic();
        BuildingLogic buildingLogic = new BuildingLogic();
        // GET: Equipments
        public ActionResult Index()
        {
            return View(equipmentLogic.List());
        }

        // GET: Equipments/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = equipmentLogic.Details(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // GET: Equipments/Create
        public ActionResult Create()
        {
            ViewBag.EquipmentStatusId = new SelectList(db.EquipmentStatuss.ToList(), "EquipmentStatusId", "EquipmentStatusName");
            //SelectList ClassRoomSelectList = new SelectList(classRoomLogic.List(), "ClassRoomId", "RoomNo");
            ViewBag.LocationId = new SelectList((List<Location>)locationLogic.List(), "LocationId", "LocationName");
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels.ToList(), "EquipmentModelId", "EquipmentModelName");

            ViewBag.Building = new SelectList((List<Building>)buildingLogic.List(), "BuildingId", "BuildingName");
            ViewBag.RoomNo = new SelectList(classRoomLogic.List(), "ClassRoomId", "RoomNo");

            return View();
        }

        // POST: Equipments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Equipment equipment)
        {
            ViewBag.EquipmentStatusId = new SelectList(db.EquipmentStatuss.ToList(), "EquipmentStatusId", "EquipmentStatusName");

            if (ModelState.IsValid)
            {
                equipmentLogic.Add(equipment);
                return RedirectToAction("Index");
            }
            ViewBag.EquipmentStatusId = new SelectList(db.EquipmentStatuss.ToList(), "EquipmentStatusId", "EquipmentStatusName");
            ViewBag.LocationId = new SelectList((List<Location>)locationLogic.List(), "LocationId", "LocationName");
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels.ToList(), "EquipmentModelId", "EquipmentModelName");
            ViewBag.RoomNo = new SelectList(classRoomLogic.List(), "ClassRoomId", "RoomNo");
            ViewBag.Building = new SelectList((List<Building>)buildingLogic.List(), "BuildingId", "BuildingName");


            return View(equipment);
        }

        // GET: Equipments/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = equipmentLogic.Details(id);
            ViewBag.EquipmentStatusId = new SelectList(db.EquipmentStatuss.ToList(), "EquipmentStatusId", "EquipmentStatusName", equipment.EquipmentStatusId);
            ViewBag.LocationId = new SelectList((List<Location>)locationLogic.List(), "LocationId", "LocationName", equipment.LocationId);
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels.ToList(), "EquipmentModelId", "EquipmentModelName", equipment.EquipmentModelId);

            ViewBag.RoomNo = new SelectList(classRoomLogic.List(), "ClassRoomId", "RoomNo", equipment.RoomNo);
            ViewBag.Building = new SelectList((List<Building>)buildingLogic.List(), "BuildingId", "BuildingName", equipment.Building);

            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                equipmentLogic.Revise(equipment);
                return RedirectToAction("Index");
            }
            ViewBag.EquipmentStatusId = new SelectList(db.EquipmentStatuss.ToList(), "EquipmentStatusId", "EquipmentStatusName", equipment.EquipmentStatusId);
            ViewBag.LocationId = new SelectList((List<Location>)locationLogic.List(), "LocationId", "LocationName", equipment.LocationId);
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels.ToList(), "EquipmentModelId", "EquipmentModelName", equipment.EquipmentModelId);
            ViewBag.RoomNo = new SelectList(classRoomLogic.List(), "ClassRoomId", "RoomNo", equipment.RoomNo);
            ViewBag.Building = new SelectList((List<Building>)buildingLogic.List(), "BuildingId", "BuildingName", equipment.Building);

            return View(equipment);
        }

        [HttpPost]
        public ActionResult Update(Equipment equipment)
        {
            Equipment equip = equipmentLogic.Details(equipment.EquipmentId);

            equip.TotalFlyingHours = equipment.TotalFlyingHours;/////
            equip.ActualRemainingHours = equipment.ActualRemainingHours;
            equip.EstimatedRemainingHours = equipment.EstimatedRemainingHours;
            equip.EnableAutoForcast = equipment.EnableAutoForcast;

            equipmentLogic.Revise(equip);
            //return RedirectToAction("Index");
            return View("Index",equipmentLogic.List());

        }

        // GET: Equipments/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = equipmentLogic.Details(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            equipmentLogic.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
