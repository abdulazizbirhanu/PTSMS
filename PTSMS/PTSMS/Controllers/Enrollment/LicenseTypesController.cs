using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.References;
using PTSMSBAL.Logic.Enrollment.References;

namespace PTSMS.Controllers.Enrollment
{
    public class LicenseTypesController : Controller
    {
        // private PTSContext db = new PTSContext();
        LicenseTypeLogic LicenseTypeLogic = new LicenseTypeLogic();
        // GET: LicenseTypes
        public ActionResult Index()
        {
          
            return View(LicenseTypeLogic.List());
        }

        // GET: LicenseTypes/Details/5
        public ActionResult Details(int id)
        {

            //////////
            if (id ==0 )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenseType licenseType = (LicenseType)LicenseTypeLogic.Details(id);
            if (licenseType == null)
            {
                return HttpNotFound();
            }
            return View(licenseType);
        }

        // GET: LicenseTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LicenseTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LicenseTypeId,Type,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] LicenseType licenseType)
        {
            

            if (ModelState.IsValid)
            {
                LicenseTypeLogic.Add(licenseType);
                return RedirectToAction("Index");
            }

            return View(licenseType);
        }

        // GET: LicenseTypes/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenseType licenseType = (LicenseType)LicenseTypeLogic.Details(id);
            if (licenseType == null)
            {
                return HttpNotFound();
            }
            return View(licenseType);
        }

        // POST: LicenseTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LicenseTypeId,Type,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] LicenseType licenseType)
        {
            if (ModelState.IsValid)
            {
                LicenseTypeLogic.Revise(licenseType);
                return RedirectToAction("Index");
            }
            return View(licenseType);
        }

        // GET: LicenseTypes/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenseType licenseType = (LicenseType)LicenseTypeLogic.Details(id);
            if (licenseType == null)
            {
                return HttpNotFound();
            }
            return View(licenseType);
        }

        // POST: LicenseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LicenseTypeLogic.Delete(id);
            return RedirectToAction("Index");
        }

     
    }
}
