using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSBAL.Curriculum.References;

namespace PTSMS.Controllers.Curriculum.References
{
    public class StagesController : Controller
    {
        StageLogic stageLogic = new StageLogic();

        // GET: Stages
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(stageLogic.List());
        }

        // GET: Stages/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stage stage = (Stage)stageLogic.Details((int)id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            return View(stage);
        }

        // GET: Stages/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create([Bind(Include = "StageId,Name,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Stage stage)
        {
            if (ModelState.IsValid)
            {
                stageLogic.Add(stage);              
                return RedirectToAction("Index");
            }

            return View(stage);
        }

        // GET: Stages/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stage stage = (Stage)stageLogic.Details((int)id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            return View(stage);
        }

        // POST: Stages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit([Bind(Include = "StageId,Name,Description,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Stage stage)
        {
            if (ModelState.IsValid)
            {
                stageLogic.Revise(stage);
                return RedirectToAction("Index");
            }
            return View(stage);
        }

        // GET: Stages/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stage stage =(Stage) stageLogic.Details((int)id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            return View(stage);
        }

        // POST: Stages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(int id)
        {
            stageLogic.Delete((int)id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
