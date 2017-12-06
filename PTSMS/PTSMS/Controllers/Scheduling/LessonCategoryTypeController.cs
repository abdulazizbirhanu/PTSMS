using PTSMSBAL.Scheduling.References;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.Scheduling
{
    public class LessonCategoryTypeController : Controller
    {
        LessonCategoryTypeLogic lessonCategoryTypeLogic = new LessonCategoryTypeLogic();
        // GET: LessonCategoryType
        public ActionResult Index()
        {
            var lessonCategoryTypeList = lessonCategoryTypeLogic.List();
            return View(lessonCategoryTypeList);
        }

        // GET: LessonCategoryType/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LessonCategoryType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LessonCategoryType/Create
        [HttpPost]
        public ActionResult Create(LessonCategoryType LessonCategoryType)
        {
            try
            {
                // TODO: Add insert logic here
                lessonCategoryTypeLogic.Add(LessonCategoryType);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LessonCategoryType/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LessonCategoryType/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LessonCategoryType/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LessonCategoryType/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
