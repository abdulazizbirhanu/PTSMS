using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;

namespace PTSMS.Controllers
{
    public class PrerequisitesController : Controller
    {
        PrerequisiteLogic prerequisiteLogic = new PrerequisiteLogic();
        CourseLogic courseLogic = new CourseLogic();

        public JsonResult ListPrerequisitesCourse()
        {
            List<Prerequisite> result = (List<Prerequisite>)prerequisiteLogic.List();
            return Json(new
            {
                items = result.Select(item => new
                {
                    //prerequisiteId = item.PrerequisiteId,
                    prerequisiteId = item.PrerequisiteCourse.RevisionGroupId == null ? item.PrerequisiteId : item.PrerequisiteCourse.RevisionGroupId,

                    prerequisiteTitle = item.PrerequisiteCourse.CourseTitle
                })
            });
        }


        [HttpGet]
        public PartialViewResult PrerequisiteDetailPartialView(int prerequisiteId)
        {
            Course course = (Course)prerequisiteLogic.PrerequisiteDetailPartialView(prerequisiteId);
            return PartialView("PrerequisiteDetailPartialView", course);
        }


        // GET: Prerequisites
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(prerequisiteLogic.List());
        }

        // GET: Prerequisites/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prerequisite prerequisite = (Prerequisite)prerequisiteLogic.Details((int)id);
            if (prerequisite == null)
            {
                return HttpNotFound();
            }
            return View(prerequisite);
        }

        // GET: Prerequisites/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode");
            ViewBag.PrerequisiteCourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode");
            return View();
        }

        // POST: Prerequisites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(Prerequisite prerequisite)
        {
            if (ModelState.IsValid)
            {
                if ((bool)prerequisiteLogic.Add(prerequisite))
                    return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", prerequisite.PrerequisiteCourseId);
            ViewBag.PrerequisiteCourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", prerequisite.PrerequisiteCourseId);
            return View(prerequisite);
        }

        // GET: Prerequisites/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prerequisite prerequisite = (Prerequisite)prerequisiteLogic.Details((int)id);
            if (prerequisite == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", prerequisite.PrerequisiteCourseId);
            ViewBag.PrerequisiteCourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", prerequisite.PrerequisiteCourseId);
            return View(prerequisite);
        }

        // POST: Prerequisites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(Prerequisite prerequisite)
        {
            if (ModelState.IsValid)
            {
                if ((bool)prerequisiteLogic.Revise(prerequisite))
                    return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", prerequisite.PrerequisiteCourseId);
            ViewBag.PrerequisiteCourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", prerequisite.PrerequisiteCourseId);
            return View(prerequisite);
        }

        // GET: Prerequisites/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prerequisite prerequisite = (Prerequisite)prerequisiteLogic.Details((int)id);
            if (prerequisite == null)
            {
                return HttpNotFound();
            }
            return View(prerequisite);
        }

        // POST: Prerequisites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)prerequisiteLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }
}
