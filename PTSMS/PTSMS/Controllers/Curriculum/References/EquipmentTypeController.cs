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

namespace PTSMS.Controllers.Curriculum.References
{
    public class EquipmentTypeController : Controller
    {
        EquipmentTypeLogic equipmentTypeLogic = new EquipmentTypeLogic();

        // GET: EquipmentType
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(equipmentTypeLogic.List());
        }

        // GET: EquipmentType/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentType equipmentType = equipmentTypeLogic.Details((int)id);
            if (equipmentType == null)
            {
                return HttpNotFound();
            }
            return View(equipmentType);
        }

        // GET: EquipmentType/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            return View();
        }

        // POST: EquipmentType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create([Bind(Include = "EquipmentTypeId,EquipmentTypeName,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] EquipmentType equipmentType)
        {
            if (ModelState.IsValid)
            {
                equipmentTypeLogic.Add(equipmentType);
                return RedirectToAction("Index");
            }

            return View(equipmentType);
        }

        // GET: EquipmentType/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentType equipmentType = equipmentTypeLogic.Details((int)id);
            if (equipmentType == null)
            {
                return HttpNotFound();
            }
            return View(equipmentType);
        }

        // POST: EquipmentType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit([Bind(Include = "EquipmentTypeId,EquipmentTypeName,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] EquipmentType equipmentType)
        {
            if (ModelState.IsValid)
            {
                equipmentTypeLogic.Revise(equipmentType);
                return RedirectToAction("Index");
            }
            return View(equipmentType);
        }

        // GET: EquipmentType/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentType equipmentType = equipmentTypeLogic.Details((int)id);
            if (equipmentType == null)
            {
                return HttpNotFound();
            }
            return View(equipmentType);
        }

        // POST: EquipmentType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(int id)
        {
            equipmentTypeLogic.Delete(id);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
