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
using PTSMSBAL.Dispatch.Master;

namespace PTSMS.Controllers.Dispatch.Master
{
    public class OverallGradeController : Controller
    {
        private PTSContext db = new PTSContext();
        OverallGradeLogic overallGradeLogic = new OverallGradeLogic();
        // GET: OverallGrade
        public ActionResult Index()
        {
            
            return View(overallGradeLogic.List());
        }

        // GET: OverallGrade/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OverallGrade overallGrade = overallGradeLogic.Details(id);
            if (overallGrade == null)
            {
                return HttpNotFound();
            }
            return View(overallGrade);
        }

        // GET: OverallGrade/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName");
            return View();
        }

        // POST: OverallGrade/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OverallGradeId,OverallGradeName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] OverallGrade overallGrade)
        {
            if (ModelState.IsValid)
            {
                overallGradeLogic.Add(overallGrade);
                return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName", overallGrade.RevisionGroupId);
            return View(overallGrade);
        }

        // GET: OverallGrade/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OverallGrade overallGrade = overallGradeLogic.Details(id);
            if (overallGrade == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName", overallGrade.RevisionGroupId);
            return View(overallGrade);
        }

        // POST: OverallGrade/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OverallGradeId,OverallGradeName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] OverallGrade overallGrade)
        {
            if (ModelState.IsValid)
            {
                overallGradeLogic.Revise(overallGrade);
                return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName", overallGrade.RevisionGroupId);
            return View(overallGrade);
        }

        // GET: OverallGrade/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OverallGrade overallGrade = overallGradeLogic.Details(id);
            if (overallGrade == null)
            {
                return HttpNotFound();
            }
            return View(overallGrade);
        }

        // POST: OverallGrade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            overallGradeLogic.Delete(id);
            return RedirectToAction("Index");
        }

       
    }
}
