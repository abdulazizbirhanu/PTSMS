using PTSMS.Models;
using PTSMSBAL.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.Scheduling
{
    public class AirportController : Controller
    {

        AirportLogic airportLogic = new AirportLogic();

        // GET: Periods
        public ActionResult Index()
        {
            var periods = airportLogic.List();
            return View(periods);
        }

        // GET: Periods/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Airport airport = airportLogic.Details(id);
            if (airport == null)
            {
                return HttpNotFound();
            }
            return View(airport);
        }

        // GET: Periods/Create
        public ActionResult Create()
        {
            return View();
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Airport airport)
        {
            if (ModelState.IsValid)
            {
                airportLogic.Add(airport);
                return RedirectToAction("Index");
            }
            return View(airport);
        }


        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Airport airport = airportLogic.Details(id);
            if (airport == null)
            {
                return HttpNotFound();
            }            
            return View(airport);
        }

        // POST: Periods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Airport airport)
        {
            if (ModelState.IsValid)
            {
                airportLogic.Revise(airport);
                return RedirectToAction("Index");
            }
            return View(airport);
        }

        // GET: Periods/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Airport airport = airportLogic.Details(id);
            if (airport == null)
            {
                return HttpNotFound();
            }
            return View(airport);
        }

        // POST: Periods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            airportLogic.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
