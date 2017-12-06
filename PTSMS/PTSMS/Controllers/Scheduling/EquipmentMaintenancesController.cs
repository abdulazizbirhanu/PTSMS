using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSBAL.Scheduling.References;
using PTSMSBAL.Scheduling.Relations;

namespace PTSMS.Controllers.Scheduling
{
    public class EquipmentMaintenancesController : Controller
    {
        EquipmentMaintenancesLogic equipmentMaintenancesLogic = new EquipmentMaintenancesLogic();
        EquipmentLogic equipmentLogic = new EquipmentLogic();

        //private PTSContext db = new PTSContext();

        // GET: EquipmentMaintenances
        public ActionResult Index()
        {
            //var equipmentMaintenances = db.EquipmentMaintenances.Include(e => e.Equipment);
            //var equipmentList = equipmentLogic.List();

            return View(equipmentMaintenancesLogic.List());
        }

        // GET: EquipmentMaintenances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentMaintenance equipmentMaintenance = equipmentMaintenancesLogic.Details(id);// db.EquipmentMaintenances.Find(id);
            if (equipmentMaintenance == null)
            {
                return HttpNotFound();
            }
            return View(equipmentMaintenance);
        }

        // GET: EquipmentMaintenances/Create
        public ActionResult Create()
        {
            ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo");
            return View();
        }
        [HttpGet]
        public PartialViewResult CreateEquiMentainance(string EquipmentId)
        {
            ViewBag.EquipmentId = EquipmentId;
            return PartialView("CreateEquiMentainance");
        }
        // POST: EquipmentMaintenances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( EquipmentMaintenance equipmentMaintenance)
        {
            if (ModelState.IsValid)
            {
               equipmentMaintenancesLogic.Add(equipmentMaintenance);
                return RedirectToAction("Index");
            }

            ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo", equipmentMaintenance.EquipmentId);
            return View(equipmentMaintenance);
        }

        // GET: EquipmentMaintenances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentMaintenance equipmentMaintenance = equipmentMaintenancesLogic.Details(id); //db.EquipmentMaintenances.Find(id);
            if (equipmentMaintenance == null)
            {
                return HttpNotFound();
            }
            ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo", equipmentMaintenance.EquipmentId);
            return View(equipmentMaintenance);
        }

        // POST: EquipmentMaintenances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( EquipmentMaintenance equipmentMaintenance)
        {
            if (ModelState.IsValid)
            {
                equipmentMaintenancesLogic.Revise(equipmentMaintenance);
                
                return RedirectToAction("Index");
            }
            ViewBag.EquipmentId = new SelectList(equipmentLogic.List(), "EquipmentId", "NameOrSerialNo", equipmentMaintenance.EquipmentId);
            return View(equipmentMaintenance);
        }

        public ActionResult Cancle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentMaintenance equipmentMaintenance =equipmentMaintenancesLogic.Details(id);// db.EquipmentMaintenances.Find(id);
            if (equipmentMaintenance == null)
            {
                return HttpNotFound();
            }
            equipmentMaintenance.Status = StatusType.Canceled;
            equipmentMaintenancesLogic.ReviseUpdate(equipmentMaintenance);
            //db.Entry(equipmentMaintenance).State = EntityState.Modified;
            //db.SaveChanges();

            return RedirectToAction("Index");            
        }

        [HttpPost]
        public ActionResult Clear(EquipmentMaintenance equipmentMaintenance)
        {

            if (equipmentMaintenance.EquipmentMaintenanceId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentMaintenance equipMaintenancee = equipmentMaintenancesLogic.Details(equipmentMaintenance.EquipmentMaintenanceId);// db.EquipmentMaintenances.Find(equipmentMaintenance.EquipmentMaintenanceId);
            if (equipMaintenancee == null)
            {
                return HttpNotFound();
            }
            equipMaintenancee.Status = StatusType.Completed;
            equipMaintenancee.ActualCalanderStartDate = equipmentMaintenance.ActualCalanderStartDate;
            equipMaintenancee.ActualCalanderEndDate = equipmentMaintenance.ActualCalanderEndDate;
            equipMaintenancee.ActualMaintenanceHour = equipmentMaintenance.ActualMaintenanceHour;

            equipmentMaintenancesLogic.ReviseUpdate(equipMaintenancee);
            //db.Entry(equipMaintenancee).State = EntityState.Modified;
            //db.SaveChanges();

            return RedirectToAction("Index");
        }
        // GET: EquipmentMaintenances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentMaintenance equipmentMaintenance = equipmentMaintenancesLogic.Details(id);// db.EquipmentMaintenances.Find(id);
            if (equipmentMaintenance == null)
            {
                return HttpNotFound();
            }
            return View(equipmentMaintenance);
        }

        // POST: EquipmentMaintenances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EquipmentMaintenance equipmentMaintenance = equipmentMaintenancesLogic.Details(id);// db.EquipmentMaintenances.Find(id);
            equipmentMaintenancesLogic.Delete(equipmentMaintenance);

            //db.EquipmentMaintenances.Remove(equipmentMaintenance);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                equipmentMaintenancesLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
