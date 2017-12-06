using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMS.Controllers
{

    public class BatchClassesController : Controller
    {
        BatchClassLogic batchClassLogic = new BatchClassLogic();
        BatchLogic batchLogic = new BatchLogic();
        [PTSAuthorizeAttribute]
        public JsonResult CreateBatchClass(int BatchId, string BatchClassName)
        {
            BatchClass batchClass = new BatchClass();
            batchClass.BatchId = BatchId;
            batchClass.BatchClassName = BatchClassName;

            if (ModelState.IsValid)
            {
                if ((bool)batchClassLogic.Add(batchClass))
                    return Json(new { Result = new { status = true } }, JsonRequestBehavior.AllowGet);
                return Json(new { Result = new { status = false, message = "Error occured." } }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = new { status = false, message = "Invalid Input." } }, JsonRequestBehavior.AllowGet);
        }


        // GET: BatchClasses
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(batchClassLogic.List());
        }

        // GET: BatchClasses/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.BatchClassSelectList = new SelectList((List<BatchClass>)batchClassLogic.List(((BatchClass)batchClassLogic.Details((int)id)).BatchId), "BatchClassId", "BatchClassName");
            ViewBag.TraineeList = batchLogic.ListTraineeToBatchClass((int)id);
            BatchClass batchClass = (BatchClass)batchClassLogic.Details((int)id);
            if (batchClass == null)
            {
                return HttpNotFound();
            }
            return View(batchClass);
        }

        // GET: BatchClasses/Create
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Create(int batchId)
        {
            if (batchId == 0)
                ViewBag.BatchId = null;
            else
                ViewBag.BatchId = batchId;
            ViewBag.BatchId = new SelectList((List<Batch>)batchLogic.List(), "BatchId", "BatchName");
            return View();
        }
        // POST: BatchClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(BatchClass batchClass)
        {
            if (ModelState.IsValid)
            {
                if ((bool)batchClassLogic.Add(batchClass))
                    return RedirectToAction("Index");
            }

            ViewBag.BatchId = new SelectList((List<Batch>)batchLogic.List(), "BatchId", "BatchName", batchClass.BatchId);
            return View(batchClass);
        }

        // GET: BatchClasses/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BatchClass batchClass = (BatchClass)batchClassLogic.Details((int)id);
            if (batchClass == null)
            {
                return HttpNotFound();
            }
            ViewBag.BatchId = new SelectList((List<Batch>)batchLogic.List(), "BatchId", "BatchName", batchClass.BatchId);

            return View(batchClass);
        }

        // POST: BatchClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(BatchClass batchClass)
        {
            if (ModelState.IsValid)
            {
                if ((bool)batchClassLogic.Revise(batchClass))
                    return RedirectToAction("Index");
            }
            ViewBag.BatchId = new SelectList((List<Batch>)batchLogic.List(), "BatchId", "BatchName", batchClass.BatchId);
            return View(batchClass);
        }

        // GET: BatchClasses/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BatchClass batchClass = (BatchClass)batchClassLogic.Details((int)id);
            if (batchClass == null)
            {
                return HttpNotFound();
            }
            return View(batchClass);
        }

        // POST: BatchClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)batchClassLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }
}
