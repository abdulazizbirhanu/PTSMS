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
    public class InstrumentApproachesController : Controller
    {
        private PTSContext db = new PTSContext();
        InstrumentApproachLogic instrumentApproachLogic = new InstrumentApproachLogic();
        // GET: InstrumentApproaches
        public ActionResult Index()
        {
           
            return View(instrumentApproachLogic.List());
        }

        // GET: InstrumentApproaches/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstrumentApproach instrumentApproach = instrumentApproachLogic.Details(id);
            if (instrumentApproach == null)
            {
                return HttpNotFound();
            }
            return View(instrumentApproach);
        }

        // GET: InstrumentApproaches/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.InstrumentApproachs, "InstrumentApproachId", "InstrumentApproachName");
            return View();
        }

        // POST: InstrumentApproaches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InstrumentApproachId,InstrumentApproachName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] InstrumentApproach instrumentApproach)
        {
            if (ModelState.IsValid)
            {
                instrumentApproachLogic.Add(instrumentApproach);
                return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList(db.InstrumentApproachs, "InstrumentApproachId", "InstrumentApproachName", instrumentApproach.RevisionGroupId);
            return View(instrumentApproach);
        }

        // GET: InstrumentApproaches/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstrumentApproach instrumentApproach = instrumentApproachLogic.Details(id);
            if (instrumentApproach == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.InstrumentApproachs, "InstrumentApproachId", "InstrumentApproachName", instrumentApproach.RevisionGroupId);
            return View(instrumentApproach);
        }

        // POST: InstrumentApproaches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstrumentApproachId,InstrumentApproachName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] InstrumentApproach instrumentApproach)
        {
            if (ModelState.IsValid)
            {
                instrumentApproachLogic.Revise(instrumentApproach);
                return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.InstrumentApproachs, "InstrumentApproachId", "InstrumentApproachName", instrumentApproach.RevisionGroupId);
            return View(instrumentApproach);
        }

        // GET: InstrumentApproaches/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstrumentApproach instrumentApproach = instrumentApproachLogic.Details(id);
            if (instrumentApproach == null)
            {
                return HttpNotFound();
            }
            return View(instrumentApproach);
        }

        // POST: InstrumentApproaches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            instrumentApproachLogic.Delete(id);
            return RedirectToAction("Index");
        }

      
    }
}
