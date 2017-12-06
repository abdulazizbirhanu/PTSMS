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
using PTSMSBAL.Dispatch;
using PTSMSDAL.Access.Scheduling.Operations;

namespace PTSMS.Controllers.Dispatch
{
    public class OverallGradeUpdateRequestController : Controller
    {
        private PTSContext db = new PTSContext();
        OverallGradeUpdateRequestLogic overallGradeUpdateRequestLogic = new OverallGradeUpdateRequestLogic();

        // GET: OverallGradeUpdateRequest
        public ActionResult Index()
        {
            var overallGradeUpdateRequests = db.OverallGradeUpdateRequests.Include(o => o.FlyingFTDSchedule).Include(o => o.NewOverallGrade);
            if (TempData["OverallGradeUpdateRequest"] != null)
            {
                ViewBag.OverallGradeUpdateRequest = TempData["OverallGradeUpdateRequest"];
                TempData["OverallGradeUpdateRequest"] = null;
            }
            return View(overallGradeUpdateRequests.ToList());
        }

        // GET: OverallGradeUpdateRequest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OverallGradeUpdateRequest overallGradeUpdateRequest = db.OverallGradeUpdateRequests.Find(id);
            if (overallGradeUpdateRequest == null)
            {
                return HttpNotFound();
            }
            return View(overallGradeUpdateRequest);
        }

        // GET: OverallGradeUpdateRequest/Create
        public ActionResult Create()
        {
            ViewBag.FlyingFTDScheduleId = new SelectList(db.FlyingFTDSchedules, "FlyingFTDScheduleId", "Status");
            ViewBag.NewOverallGradeId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName");
            return View();
        }



        [HttpGet]
        public ActionResult OverallGradeUpdateRequestApproval(int OverallGradeUpdateRequestId, string IsApprove)
        {
            if (OverallGradeUpdateRequestId > 0 && !string.IsNullOrEmpty(IsApprove))
            {
                if (overallGradeUpdateRequestLogic.OverallGradeUpdateRequestApproval(OverallGradeUpdateRequestId, bool.Parse(IsApprove)))
                    TempData["OverallGradeUpdateRequest"] = "Your approval successfully saved.";
                else
                    TempData["OverallGradeUpdateRequest"] = "Failed to save the approval process.";
            }
            else
                TempData["OverallGradeUpdateRequest"] = "Invalid input/s.";
            return RedirectToAction("Index");
        }



        [HttpGet]
        public PartialViewResult CreateOverallGradeUpdateRequest(int flyingFTDScheduleId, string scheduleDetail)
        {
            //ViewBag.FlyingFTDScheduleId = new SelectList(db.FlyingFTDSchedules, "FlyingFTDScheduleId", "Status");
            ViewBag.NewOverallGradeId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName");
            ViewBag.FlyingFTDScheduleId = flyingFTDScheduleId;
            ViewBag.ScheduleDetail = scheduleDetail;

            return PartialView("CreateOverallGradeUpdateRequest");
        }

        // POST: OverallGradeUpdateRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OverallGradeUpdateRequest overallGradeUpdateRequest)
        {
            OperationResult operationResult = overallGradeUpdateRequestLogic.Add(overallGradeUpdateRequest);
            return RedirectToAction("Equipment", "InstructorSchedule",new { });

        }

        // GET: OverallGradeUpdateRequest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OverallGradeUpdateRequest overallGradeUpdateRequest = db.OverallGradeUpdateRequests.Find(id);
            if (overallGradeUpdateRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.FlyingFTDScheduleId = new SelectList(db.FlyingFTDSchedules, "FlyingFTDScheduleId", "Status", overallGradeUpdateRequest.FlyingFTDScheduleId);
            ViewBag.NewOverallGradeId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName", overallGradeUpdateRequest.NewOverallGradeId);
            return View(overallGradeUpdateRequest);
        }

        // POST: OverallGradeUpdateRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OverallGradeUpdateRequestId,FlyingFTDScheduleId,NewOverallGradeId,Status,RequestedDate")] OverallGradeUpdateRequest overallGradeUpdateRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(overallGradeUpdateRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FlyingFTDScheduleId = new SelectList(db.FlyingFTDSchedules, "FlyingFTDScheduleId", "Status", overallGradeUpdateRequest.FlyingFTDScheduleId);
            ViewBag.NewOverallGradeId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName", overallGradeUpdateRequest.NewOverallGradeId);
            return View(overallGradeUpdateRequest);
        }

        // GET: OverallGradeUpdateRequest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OverallGradeUpdateRequest overallGradeUpdateRequest = db.OverallGradeUpdateRequests.Find(id);
            if (overallGradeUpdateRequest == null)
            {
                return HttpNotFound();
            }
            return View(overallGradeUpdateRequest);
        }

        // POST: OverallGradeUpdateRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OverallGradeUpdateRequest overallGradeUpdateRequest = db.OverallGradeUpdateRequests.Find(id);
            db.OverallGradeUpdateRequests.Remove(overallGradeUpdateRequest);
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
