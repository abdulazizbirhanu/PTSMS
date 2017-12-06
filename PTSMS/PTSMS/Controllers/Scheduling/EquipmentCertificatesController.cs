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
using PTSMSDAL.Models.Scheduling.Relations;

namespace PTSMS.Controllers.Scheduling
{
    public class EquipmentCertificatesController : Controller
    {
        EquipmentCertificateLogic equipmentCertificateLogic = new EquipmentCertificateLogic();
        private PTSContext db = new PTSContext();
        // GET: EquipmentCertificates
        public ActionResult Index()
        {
            var equipmentCertificates = equipmentCertificateLogic.List();
            return View(equipmentCertificates);
        }

        // GET: EquipmentCertificates/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentCertificate equipmentCertificate = equipmentCertificateLogic.Details(id);
            if (equipmentCertificate == null)
            {
                return HttpNotFound();
            }
            return View(equipmentCertificate);
        }

        // GET: EquipmentCertificates/Create
        public ActionResult Create()
        {
            ViewBag.CertificateTypeId = new SelectList(db.CertificateTypes, "CertificateTypeId", "Type");
            return View();
        }

        // POST: EquipmentCertificates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EquipmentLicenseId,EquipmentId,CertificateTypeId,StartDate,EndDate,Description,CreationDate,CreatedBy,RevisionDate,RevisedBy")] EquipmentCertificate equipmentCertificate)
        {
            if (ModelState.IsValid)
            {
                equipmentCertificateLogic.Add(equipmentCertificate);
                return RedirectToAction("Index");
            }

            ViewBag.CertificateTypeId = new SelectList(db.CertificateTypes, "CertificateTypeId", "Type", equipmentCertificate.CertificateTypeId);
            return View(equipmentCertificate);
        }

        // GET: EquipmentCertificates/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentCertificate equipmentCertificate = equipmentCertificateLogic.Details(id);
            if (equipmentCertificate == null)
            {
                return HttpNotFound();
            }
            ViewBag.CertificateTypeId = new SelectList(db.CertificateTypes, "CertificateTypeId", "Type", equipmentCertificate.CertificateTypeId);
            return View(equipmentCertificate);
        }

        // POST: EquipmentCertificates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EquipmentLicenseId,EquipmentId,CertificateTypeId,StartDate,EndDate,Description,CreationDate,CreatedBy,RevisionDate,RevisedBy")] EquipmentCertificate equipmentCertificate)
        {
            if (ModelState.IsValid)
            {
                equipmentCertificateLogic.Revise(equipmentCertificate);
                return RedirectToAction("Index");
            }
            ViewBag.CertificateTypeId = new SelectList(db.CertificateTypes, "CertificateTypeId", "Type", equipmentCertificate.CertificateTypeId);
            return View(equipmentCertificate);
        }

        // GET: EquipmentCertificates/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentCertificate equipmentCertificate = equipmentCertificateLogic.Details(id);
            if (equipmentCertificate == null)
            {
                return HttpNotFound();
            }
            return View(equipmentCertificate);
        }

        // POST: EquipmentCertificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            equipmentCertificateLogic.Delete(id);
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
