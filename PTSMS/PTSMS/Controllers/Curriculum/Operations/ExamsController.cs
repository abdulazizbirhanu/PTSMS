using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;

namespace PTSMS.Controllers
{
    public class ExamsController : Controller
    {
        ExamLogic examLogic = new ExamLogic();


        [HttpGet]
        public PartialViewResult ExamDetailPartialView(int idNumber, string examType)
        {
            Exam exam = examLogic.ExamDetail(idNumber, examType);
            return PartialView("ExamDetailPartialView", exam);
        }

        

        public JsonResult ListExam()
        {
            List<Exam> result = (List<Exam>)examLogic.List();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.ExamId,
                    Id = item.RevisionGroupId == null ? item.ExamId : item.RevisionGroupId,

                    Name = item.Name,
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: Exams
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(examLogic.List());
        }

        // GET: Exams/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = (Exam)examLogic.Details((int)id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // GET: Exams/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList((List<Exam>)examLogic.List(), "ExamId", "Name");
            return View();
        }

        // POST: Exams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(Exam exam)
        {
            if (ModelState.IsValid)
            {
                if ((bool)examLogic.Add(exam))
                    return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList((List<Exam>)examLogic.List(), "ExamId", "Name", exam.RevisionGroupId);
            return View(exam);
        }

        // GET: Exams/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = (Exam)examLogic.Details((int)id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList((List<Exam>)examLogic.List(), "ExamId", "Name", exam.RevisionGroupId);
            return View(exam);
        }

        // POST: Exams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(Exam exam)
        {
            if (ModelState.IsValid)
            {
                if ((bool)examLogic.Revise(exam))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList((List<Exam>)examLogic.List(), "ExamId", "Name", exam.RevisionGroupId);
            return View(exam);
        }

        // GET: Exams/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = (Exam)examLogic.Details((int)id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)examLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }
}
