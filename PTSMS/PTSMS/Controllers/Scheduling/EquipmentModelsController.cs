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

namespace PTSMS.Controllers.Scheduling
{
    public class EquipmentModelsController : Controller
    {
        private PTSContext db = new PTSContext();

        // GET: EquipmentModels
        public ActionResult Index()
        {
            var equipmentModels = db.EquipmentModels.Include(e => e.EquipmentType);
            return View(equipmentModels.ToList());
        }

        // GET: EquipmentModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentModel equipmentModel = db.EquipmentModels.Find(id);
            if (equipmentModel == null)
            {
                return HttpNotFound();
            }
            return View(equipmentModel);
        }

        // GET: EquipmentModels/Create
        public ActionResult Create()
        {
            ViewBag.EquipmentTypeId = new SelectList(db.EquipmentTypes, "EquipmentTypeId", "EquipmentTypeName");
            return View();
        }

        // POST: EquipmentModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EquipmentModelId,EquipmentTypeId,EquipmentModelName,IsMultiEngine,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] EquipmentModel equipmentModel)
        {
            equipmentModel.CreationDate = DateTime.Now;
            equipmentModel.RevisionDate = DateTime.Now;
            equipmentModel.StartDate = DateTime.Now;
            equipmentModel.EndDate = Convert.ToDateTime("1/11/9999");
            if (ModelState.IsValid)
            {
                try
                {
                    db.EquipmentModels.Add(equipmentModel);
                    db.SaveChanges();
                }
                catch (Exception ee)
                {

                    throw;
                }
                return RedirectToAction("Index");
            }

            ViewBag.EquipmentTypeId = new SelectList(db.EquipmentTypes, "EquipmentTypeId", "EquipmentTypeName", equipmentModel.EquipmentTypeId);
            return View(equipmentModel);
        }

        // GET: EquipmentModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentModel equipmentModel = db.EquipmentModels.Find(id);
            if (equipmentModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.EquipmentTypeId = new SelectList(db.EquipmentTypes, "EquipmentTypeId", "EquipmentTypeName", equipmentModel.EquipmentTypeId);
            return View(equipmentModel);
        }

        // POST: EquipmentModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EquipmentModelId,EquipmentTypeId,EquipmentModelName,IsMultiEngine,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] EquipmentModel equipmentModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipmentModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EquipmentTypeId = new SelectList(db.EquipmentTypes, "EquipmentTypeId", "EquipmentTypeName", equipmentModel.EquipmentTypeId);
            return View(equipmentModel);
        }

        // GET: EquipmentModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentModel equipmentModel = db.EquipmentModels.Find(id);
            if (equipmentModel == null)
            {
                return HttpNotFound();
            }
            return View(equipmentModel);
        }

        // POST: EquipmentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EquipmentModel equipmentModel = db.EquipmentModels.Find(id);
            db.EquipmentModels.Remove(equipmentModel);
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
