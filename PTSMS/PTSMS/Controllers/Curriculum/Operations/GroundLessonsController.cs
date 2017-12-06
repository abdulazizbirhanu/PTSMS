using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;

namespace PTSMS.Controllers.Curriculum.Operations
{
    public class GroundLessonsController : Controller
    {
        GroundLessonLogic groundLessonLogic = new GroundLessonLogic();
        private PTSContext db = new PTSContext();

        public JsonResult ListGroundLessons()
        {
            List<GroundLesson> result = (List<GroundLesson>)groundLessonLogic.List();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.GroundLessonId,
                    Id = item.RevisionGroupId == null ? item.GroundLessonId : item.RevisionGroupId,

                    Name = item.LessonName,
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult GroundLessonsDetailPartialView(int moduleGroundLessonId)
        {
            GroundLesson groundLesson = groundLessonLogic.GroundLessonsDetail(moduleGroundLessonId);
            return PartialView("GroundLessonsDetailPartialView", groundLesson);
        }

        // GET: GroundLessons
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(groundLessonLogic.List());
        }

        // GET: GroundLessons/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroundLesson groundLesson = (GroundLesson)groundLessonLogic.Details((int)id);
            if (groundLesson == null)
            {
                return HttpNotFound();
            }
            return View(groundLesson);
        }

        

        // GET: GroundLessons/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList(db.GroundLessons, "GroundLessonId", "LessonCode");
            return View();
        }

        // POST: GroundLessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(GroundLesson groundLesson)
        {
            if (ModelState.IsValid)
            {
                if ((bool)groundLessonLogic.Add(groundLesson))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.GroundLessons, "GroundLessonId", "LessonCode", groundLesson.RevisionGroupId);
            return View(groundLesson);
        }

        // GET: GroundLessons/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroundLesson groundLesson = (GroundLesson)groundLessonLogic.Details((int)id);
            if (groundLesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList(db.GroundLessons, "GroundLessonId", "LessonCode", groundLesson.RevisionGroupId);
            return View(groundLesson);
        }

        // POST: GroundLessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(GroundLesson groundLesson)
        {
            if (ModelState.IsValid)
            {
                if ((bool)groundLessonLogic.Revise(groundLesson))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList(db.GroundLessons, "GroundLessonId", "LessonCode", groundLesson.RevisionGroupId);
            return View(groundLesson);
        }

        // GET: GroundLessons/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroundLesson groundLesson = (GroundLesson)groundLessonLogic.Details((int)id);
            if (groundLesson == null)
            {
                return HttpNotFound();
            }
            return View(groundLesson);
        }

        // POST: GroundLessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {            
            if ((bool)groundLessonLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
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
