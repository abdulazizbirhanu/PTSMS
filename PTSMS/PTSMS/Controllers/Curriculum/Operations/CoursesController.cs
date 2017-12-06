using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using System.Collections.Generic;
using PTSMSBAL.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using System;

namespace PTSMS.Controllers
{    
    public class CoursesController : Controller
    {
        private PTSContext db = new PTSContext();

        CourseLogic courseLogic = new CourseLogic();

        public JsonResult ListCourse()
        {
            List<Course> result = (List<Course>)courseLogic.List();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.CourseId,
                    Id = item.RevisionGroupId == null ? item.CourseId : item.RevisionGroupId,

                    Name = item.CourseCode + " - " + item.CourseTitle
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListCourseByCategoryId(int programCategoryId)
        {
            List<Course> result = (List<Course>)courseLogic.List();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.CourseId,
                    Id = item.RevisionGroupId == null ? item.CourseId : item.RevisionGroupId,

                    Name = item.CourseCode + " - " + item.CourseTitle
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListModule(int CourseId)
        {
            List<CourseModule> result = (List<CourseModule>)courseLogic.ListModule(CourseId);
            return Json(new
            {
                items = result.Select(item => new
                {
                    //moduleId = item.ModuleId,
                    moduleId = item.Module.RevisionGroupId == null ? item.Module.ModuleId : item.Module.RevisionGroupId,

                    moduleTitle = item.Module.ModuleCode + " - " + item.Module.ModuleTitle
                })
            });
        }

        public JsonResult AddModule(string ModuleId, int CourseId, int PhaseId)
        {

            if (!(String.IsNullOrEmpty(ModuleId) || String.IsNullOrWhiteSpace(ModuleId)))
            {
                string[] ModuleIdArray = ModuleId.Split(',');
                object result = courseLogic.AddModule(ModuleIdArray.ToList(), CourseId, PhaseId);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Invalid input." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveCourseModule(int courseModuleId)
        {
            object result = courseLogic.RemoveCourseModule(courseModuleId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListExam(int CourseId)
        {
            List<CourseExam> result = (List<CourseExam>)courseLogic.ListExam(CourseId);
            return Json(new
            {
                items = result.Select(item => new
                {
                    //examId = item.ExamId,
                    examId = item.Exam.RevisionGroupId == null ? item.Exam.ExamId : item.Exam.RevisionGroupId,

                    examName = item.Exam.Name,
                })
            });
        }

        public JsonResult AddExam(string ExamId, int CourseId)
        {
            if (!(String.IsNullOrEmpty(ExamId) || String.IsNullOrWhiteSpace(ExamId)))
            {
                string[] ExamIdArray = ExamId.Split(',');
                object result = courseLogic.AddExam(ExamIdArray.ToList(), CourseId);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Invalid input." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveCourseExam(int courseExamId)
        {
            object result = courseLogic.RemoveCourseExam(courseExamId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListPrerequisite(int CourseId)
        {
            List<Prerequisite> result = (List<Prerequisite>)courseLogic.ListPrerequisite(CourseId);
            return Json(new
            {
                items = result.Select(item => new
                {
                    //prerequisiteId = item.PrerequisiteId,
                    prerequisiteId = item.PrerequisiteCourse.RevisionGroupId == null ? item.PrerequisiteCourse.CourseId : item.PrerequisiteCourse.RevisionGroupId,
                    
                    prerequisiteName = item.PrerequisiteCourse.CourseTitle
                })
            });
        }

        public JsonResult AddPrerequisite(int CourseId, string PrerequisiteId)
        {
            if (!(String.IsNullOrEmpty(PrerequisiteId) || String.IsNullOrWhiteSpace(PrerequisiteId)))
            {
                string[] PrerequisiteIdArray = PrerequisiteId.Split(',');
                object result = courseLogic.AddPrerequisite(CourseId, PrerequisiteIdArray.ToList());
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Invalid input." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemovePrerequisite(int prerequisiteId)
        {
            object result = courseLogic.RemovePrerequisite(prerequisiteId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public PartialViewResult CourseDetailPartialView(int courseCategoryId)
        {
            Course course = courseLogic.CourseDetail(courseCategoryId);
            return PartialView("CourseDetailPartialView", course);
        }


        // GET: Courses       
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(courseLogic.List());
        }

        // GET: Courses/Details/5      
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = (Course)courseLogic.Details((int)id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create     
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                if ((bool)courseLogic.Add(course))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", course.RevisionGroupId);
            return View(course);
        }

        // GET: Courses/Edit/5     
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = (Course)courseLogic.Details((int)id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", course.RevisionGroupId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                if ((bool)courseLogic.Revise(course))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode", course.RevisionGroupId);
            return View(course);
        }

        // GET: Courses/Delete/5   
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = (Course)courseLogic.Details((int)id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)courseLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
        /*Nice*/

        public JsonResult FilteredListCourse(string BatchClassId)
        {
            int batchClassId = Convert.ToInt32(BatchClassId);
            List<Course> result = (List<Course>)courseLogic.FilteredListCourse(batchClassId);
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    Id = item.CourseId,
                    Name = item.CourseTitle
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FilteredCourceExam(string CourseId)
        {
            int ECourseId = Convert.ToInt32(CourseId);
            List<Exam> result = (List<Exam>)courseLogic.FilteredCourceExam(ECourseId);
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    Id = item.ExamId,
                    Name = item.Name
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
            /*Nice*/
        }
    }
}
