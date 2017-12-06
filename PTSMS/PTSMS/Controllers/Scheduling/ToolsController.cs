using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSBAL.Logic.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMS.Controllers.Scheduling
{
    public class ToolsController : Controller
    {
        private PTSContext db = new PTSContext();
        ToolLogic toolLogic = new ToolLogic();
        // GET: Tools
        public ActionResult Index()
        {
            var tools = toolLogic.List();
            return View(tools);
        }

        // GET: Tools/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = toolLogic.Details(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        // GET: Tools/Create
        public ActionResult Create()
        {
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName");
            ViewBag.PreviousRevisionId = new SelectList(db.Tools, "ToolId", "ToolName");
            return View();
        }

        // POST: Tools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ToolId,ToolName,ToolDescription,ToolStatus,LocationId,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Tool tool)
        {
            if (ModelState.IsValid)
            {
                toolLogic.Add(tool);
                return RedirectToAction("Index");
            }

            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", tool.LocationId);
            ViewBag.PreviousRevisionId = new SelectList(db.Tools, "ToolId", "ToolName", tool.PreviousRevisionId);
            return View(tool);
        }

        // GET: Tools/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = toolLogic.Details(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", tool.LocationId);
            ViewBag.PreviousRevisionId = new SelectList(db.Tools, "ToolId", "ToolName", tool.PreviousRevisionId);
            return View(tool);
        }

        // POST: Tools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ToolId,ToolName,ToolDescription,ToolStatus,LocationId,EffectiveDate,PreviousRevisionId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Tool tool)
        {
            if (ModelState.IsValid)
            {
                toolLogic.Revise(tool);
                return RedirectToAction("Index");
            }
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", tool.LocationId);
            ViewBag.PreviousRevisionId = new SelectList(db.Tools, "ToolId", "ToolName", tool.PreviousRevisionId);
            return View(tool);
        }

        // GET: Tools/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = toolLogic.Details(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        // POST: Tools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            toolLogic.Delete(id);
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
