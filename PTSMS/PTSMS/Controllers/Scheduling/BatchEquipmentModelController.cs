using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSBAL.Scheduling.Relations;

namespace PTSMS.Controllers.Scheduling
{
    public class BatchEquipmentModelController : Controller
    {
        private PTSContext db = new PTSContext();
        BatchEquipmentModelLogic batchEquipmentModelLogic = new BatchEquipmentModelLogic();
        // GET: BatchEquipmentModel
        public ActionResult Index()
        {
            var Batchequipment = batchEquipmentModelLogic.List();
            return View(Batchequipment);
        }

        // GET: BatchEquipmentModel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BatchEquipmentModel batchEquipmentModel = batchEquipmentModelLogic.Details((int)id);
            if (batchEquipmentModel == null)
            {
                return HttpNotFound();
            }
            return View(batchEquipmentModel);
        }

        // GET: BatchEquipmentModel/Create
        public ActionResult Create()
        {
            ViewBag.BatchId = new SelectList(db.Batches, "BatchId", "BatchName");
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName");
            return View();
        }

        // POST: BatchEquipmentModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BatchEquipmentModelId,EquipmentModelId,BatchId,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] BatchEquipmentModel batchEquipmentModel)
        {
            var equipmentModelIds = Request.Form["EquipmentModelId"].ToString();
            var equipmentModelIdsArray = equipmentModelIds.Split(',');

            if (ModelState.IsValid)
            {
                foreach (var item in equipmentModelIdsArray)
                {
                    batchEquipmentModel.EquipmentModelId = Convert.ToInt32(item);
                    batchEquipmentModelLogic.Add(batchEquipmentModel);
                }
                return RedirectToAction("Index");

            }
            ViewBag.BatchId = new SelectList(db.Batches, "BatchId", "BatchName", batchEquipmentModel.BatchId);
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName", batchEquipmentModel.EquipmentModelId);
            return View(batchEquipmentModel);
        }

        // GET: BatchEquipmentModel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BatchEquipmentModel batchEquipmentModel = batchEquipmentModelLogic.Details((int)id);
            if (batchEquipmentModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.BatchId = new SelectList(db.Batches, "BatchId", "BatchName", batchEquipmentModel.BatchId);
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName", batchEquipmentModel.EquipmentModelId);
            return View(batchEquipmentModel);
        }

        // POST: BatchEquipmentModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BatchEquipmentModelId,EquipmentModelId,BatchId,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] BatchEquipmentModel batchEquipmentModel)
        {
            if (ModelState.IsValid)
            {
                batchEquipmentModelLogic.Revise(batchEquipmentModel);
                return RedirectToAction("Index");
            }
            ViewBag.BatchId = new SelectList(db.Batches, "BatchId", "BatchName", batchEquipmentModel.BatchId);
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName", batchEquipmentModel.EquipmentModelId);
            return View(batchEquipmentModel);
        }

        // GET: BatchEquipmentModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BatchEquipmentModel batchEquipmentModel = batchEquipmentModelLogic.Details((int)id);
            if (batchEquipmentModel == null)
            {
                return HttpNotFound();
            }
            return View(batchEquipmentModel);
        }

        // POST: BatchEquipmentModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BatchEquipmentModel batchEquipmentModel = batchEquipmentModelLogic.Details(id);
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
