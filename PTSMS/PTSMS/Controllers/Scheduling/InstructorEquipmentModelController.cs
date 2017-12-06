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
    public class InstructorEquipmentModelController : Controller
    {
        private PTSContext db = new PTSContext();
        InstructorEquipmentModelLogic instructorEquipmentModelLogic = new InstructorEquipmentModelLogic();
        // GET: InstructorEquipmentModel
        public ActionResult Index()
        {
            var instructors = instructorEquipmentModelLogic.List();
            return View(instructors);
        }

        // GET: InstructorEquipmentModel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstructorEquipmentModel instructorEquipmentModel = instructorEquipmentModelLogic.Details((int)id);
            if (instructorEquipmentModel == null)
            {
                return HttpNotFound();
            }
            return View(instructorEquipmentModel);
        }

        // GET: InstructorEquipmentModel/Create
        public ActionResult Create()
        {
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName");

            ViewBag.InstructorId = new SelectList(db.InstructorQualifications.Where(IQ => IQ.QualificationType.Type.ToUpper() == "FTD INSTRUCTOR" || IQ.QualificationType.Type.ToUpper() == "FLYING INSTRUCTOR").ToList().Select(item => new
            {
                InstructorId = item.InstructorId,
                InstructorName = item.Instructor.Person.FirstName + " " + item.Instructor.Person.MiddleName.Substring(0, 1)
            }).GroupBy(instructor => new { instructor.InstructorId }).Select(item => item.FirstOrDefault()).ToList(), "InstructorId", "InstructorName");
            return View();
        }

        // POST: InstructorEquipmentModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InstructorEquipmentModelId,EquipmentModelId,InstructorId")] InstructorEquipmentModel instructorEquipmentModel)
        {
            var equipmentModelIds = Request.Form["EquipmentModelId"].ToString();
            var equipmentModelIdsArray = equipmentModelIds.Split(',');

            if (ModelState.IsValid)
            {
                foreach (var item in equipmentModelIdsArray)
                {
                    instructorEquipmentModel.EquipmentModelId = Convert.ToInt32(item);
                    instructorEquipmentModelLogic.Add(instructorEquipmentModel);
                }
                return RedirectToAction("Index");
            }

            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName", instructorEquipmentModel.EquipmentModelId);
            ViewBag.InstructorId = new SelectList(db.Instructors, "InstructorId", "CreatedBy", instructorEquipmentModel.InstructorId);
            return View(instructorEquipmentModel);
        }

        // GET: InstructorEquipmentModel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstructorEquipmentModel instructorEquipmentModel = instructorEquipmentModelLogic.Details((int)id);
            if (instructorEquipmentModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName", instructorEquipmentModel.EquipmentModelId);
            ViewBag.InstructorId = new SelectList(db.Instructors.ToList().Select(item => new
            {
                InstructorId = item.InstructorId,
                InstructorName = item.Person.FirstName + " " + item.Person.MiddleName.Substring(0, 1)
            }), "InstructorId", "InstructorName", instructorEquipmentModel.InstructorId);
            //ViewBag.InstructorId = new SelectList(db.Instructors, "InstructorId", "CreatedBy", instructorEquipmentModel.InstructorId);
            return View(instructorEquipmentModel);
        }

        // POST: InstructorEquipmentModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstructorEquipmentModelId,EquipmentModelId,InstructorId")] InstructorEquipmentModel instructorEquipmentModel)
        {
            if (ModelState.IsValid)
            {
                instructorEquipmentModelLogic.Revise(instructorEquipmentModel);
                return RedirectToAction("Index");
            }
            ViewBag.EquipmentModelId = new SelectList(db.EquipmentModels, "EquipmentModelId", "EquipmentModelName", instructorEquipmentModel.EquipmentModelId);
            ViewBag.InstructorId = new SelectList(db.Instructors.ToList().Select(item => new
            {
                InstructorId = item.InstructorId,
                InstructorName = item.Person.FirstName + " " + item.Person.MiddleName.Substring(0, 1)
            }), "InstructorId", "InstructorName", instructorEquipmentModel.InstructorId);
            //ViewBag.InstructorId = new SelectList(db.Instructors, "InstructorId", "CreatedBy", instructorEquipmentModel.InstructorId);
            return View(instructorEquipmentModel);
        }

        // GET: InstructorEquipmentModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstructorEquipmentModel instructorEquipmentModel = instructorEquipmentModelLogic.Details((int)id);
            if (instructorEquipmentModel == null)
            {
                return HttpNotFound();
            }
            return View(instructorEquipmentModel);
        }

        // POST: InstructorEquipmentModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            instructorEquipmentModelLogic.Delete(id);
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
