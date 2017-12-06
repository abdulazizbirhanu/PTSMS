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
    public class OperationAreasController : Controller
    {
        private PTSContext db = new PTSContext();
        OperationAreaLogic operationAreaLogic = new OperationAreaLogic();
        // GET: OperationAreas
        public ActionResult Index()
        {
           
            return View(operationAreaLogic.List());
        }

        // GET: OperationAreas/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationArea operationArea = operationAreaLogic.Details(id);
            if (operationArea == null)
            {
                return HttpNotFound();
            }
            return View(operationArea);
        }

        // GET: OperationAreas/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.OperationAreas, "OperationAreaId", "OperationAreaName");
            return View();
        }

        // POST: OperationAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OperationAreaId,OperationAreaName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] OperationArea operationArea)
        {
            if (ModelState.IsValid)
            {
                operationAreaLogic.Add(operationArea);
               
                return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList(db.OperationAreas, "OperationAreaId", "OperationAreaName", operationArea.RevisionGroupId);
            return View(operationArea);
        }

        // GET: OperationAreas/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationArea operationArea = operationAreaLogic.Details(id);
            if (operationArea == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.OperationAreas, "OperationAreaId", "OperationAreaName", operationArea.RevisionGroupId);
            return View(operationArea);
        }

        // POST: OperationAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OperationAreaId,OperationAreaName,EffectiveDate,RevisionGroupId,RevisionNo,Status,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] OperationArea operationArea)
        {
            if (ModelState.IsValid)
            {
                operationAreaLogic.Revise(operationArea);
                return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.OperationAreas, "OperationAreaId", "OperationAreaName", operationArea.RevisionGroupId);
            return View(operationArea);
        }

        // GET: OperationAreas/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationArea operationArea = operationAreaLogic.Details(id);
            if (operationArea == null)
            {
                return HttpNotFound();
            }
            return View(operationArea);
        }

        // POST: OperationAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            operationAreaLogic.Delete(id);
           
            return RedirectToAction("Index");
        }

       
    }
}
