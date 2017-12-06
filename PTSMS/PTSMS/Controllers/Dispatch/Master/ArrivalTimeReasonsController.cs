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

namespace PTSMS.Controllers.Dispatch.Master
{
    public class ArrivalTimeReasonsController : Controller
    {
       // private PTSContext db = new PTSContext();
        ArrivalTimeReasonLogic arrivalTimeReasonLogic = new ArrivalTimeReasonLogic();
        // GET: ArrivalTimeReasons
        public ActionResult Index()
        {
            var arrivalTimeReason = arrivalTimeReasonLogic.List();
            return View(arrivalTimeReason);
        }

        // GET: ArrivalTimeReasons/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArrivalTimeReason arrivalTimeReason = arrivalTimeReasonLogic.Details(id);
            if (arrivalTimeReason == null)
            {
                return HttpNotFound();
            }
            return View(arrivalTimeReason);
        }

        // GET: ArrivalTimeReasons/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: ArrivalTimeReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArrivalTimeReasonId,ArrivalTimeReasonName,EffectiveDate,RevisionGroupId,RevisionNo,Status")] ArrivalTimeReason arrivalTimeReason)
        {
           // if (ModelState.IsValid)
          //  {
                arrivalTimeReasonLogic.Add(arrivalTimeReason);
                return RedirectToAction("Index");
           // }
            return View(arrivalTimeReason);

           
           
        }

        // GET: ArrivalTimeReasons/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArrivalTimeReason arrivalTimeReason = arrivalTimeReasonLogic.Details(id);
            if (arrivalTimeReason == null)
            {
                return HttpNotFound();
            }
            return View(arrivalTimeReason);
        }

        // POST: ArrivalTimeReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArrivalTimeReasonId,ArrivalTimeReasonName,EffectiveDate,RevisionGroupId,RevisionNo,Status")] ArrivalTimeReason arrivalTimeReason)
        {
            if (ModelState.IsValid)
            {
                arrivalTimeReasonLogic.Revise(arrivalTimeReason);
                return RedirectToAction("Index");
            }
            return View(arrivalTimeReason);
        }

        // GET: ArrivalTimeReasons/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArrivalTimeReason arrivalTimeReason = arrivalTimeReasonLogic.Details(id);
            if (arrivalTimeReason == null)
            {
                return HttpNotFound();
            }
            return View(arrivalTimeReason);
        }

        // POST: ArrivalTimeReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           
            arrivalTimeReasonLogic.Delete(id);
            return RedirectToAction("Index");
        }

      
    }
}
