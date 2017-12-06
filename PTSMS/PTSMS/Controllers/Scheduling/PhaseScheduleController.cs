using PTSMSBAL.Logic.Scheduling.Relations;
using PTSMSBAL.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.Scheduling
{
    public class PhaseScheduleController : Controller
    {
        // GET: PhaseSchedule
        PhaseScheduleLogic phaseScheduleLogic = new PhaseScheduleLogic();
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            LessonCategoryTypeLogic lessonCategoryTypeLogic = new LessonCategoryTypeLogic();
            ViewBag.LessonCategoryTypeId = new SelectList(lessonCategoryTypeLogic.List(), "LessonCategoryTypeId", "LessonCategoryTypName");
            if (TempData["PhaseMessage"] != null)
            {
                ViewBag.PhaseMessage = TempData["PhaseMessage"];
                TempData["PhaseMessage"] = null;
            }
            ViewBag.PhaseSchedule = phaseScheduleLogic.ListBatchPhase();
            return View();
        }

        public JsonResult ListCourseModule(int batchClassId, int phaseId, int lessonCategoryTypeId)
        {
            bool isCourseModuleSequenceFound = false;
            var result = phaseScheduleLogic.ListCourseModule(batchClassId, phaseId, lessonCategoryTypeId, ref isCourseModuleSequenceFound);
            return Json(new { resultData = result, hasList = result.Count > 0, isSequenceAssigned = isCourseModuleSequenceFound }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListLessons(int batchClassId, int phaseId, int lessonCategoryTypeId)
        {
            bool isLessonSequenceFound = false;
            var result = phaseScheduleLogic.ListLessons(batchClassId, phaseId, lessonCategoryTypeId, ref isLessonSequenceFound);
            return Json(new { resultData = result, hasList = result.Count > 0, isSequenceAssigned = isLessonSequenceFound }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCourseModuleAndLessonSequence(FormCollection formCollection)
        {
            bool isLessonSequenceFound = false;
            string batchId = Request.Form["BatchId"];
            string phaseId = Request.Form["PhaseId"];
            string locationId = Request.Form["LocationId"];
            string startDate = Request.Form["StartingDate"];
            string categoryTypeId = Request.Form["TypeId"];
            string categoryTypeName = Request.Form["TypeName"];

            if (!(String.IsNullOrWhiteSpace(phaseId) && String.IsNullOrWhiteSpace(batchId) && String.IsNullOrWhiteSpace(locationId) && String.IsNullOrWhiteSpace(startDate) && String.IsNullOrWhiteSpace(categoryTypeId)))
            {
                PhaseSchedule phaseSchedule = new PhaseSchedule();
                phaseSchedule.BatchId = Convert.ToInt32(batchId);
                phaseSchedule.PhaseId = Convert.ToInt32(phaseId);
                phaseSchedule.LocationId = Convert.ToInt32(locationId);
                phaseSchedule.StartingDate = Convert.ToDateTime(startDate);
                phaseSchedule.LessonCategoryTypeId = Convert.ToInt16(categoryTypeId);

                if (categoryTypeName.Equals("Ground"))
                {
                    bool isCourseModuleSequenceFound = false;
                    var result = phaseScheduleLogic.ListCourseModule(Convert.ToInt32(batchId), Convert.ToInt32(phaseId), Convert.ToInt32(categoryTypeId), ref isCourseModuleSequenceFound);
                    string courseSequence = String.Empty;
                    string moduleSequence = String.Empty;
                    foreach (var courses in result)
                    {
                        courseSequence = Request.Form[("Course" + courses.Course.Id)];
                        if (!(String.IsNullOrEmpty(courseSequence) && String.IsNullOrWhiteSpace(courseSequence)))
                        {
                            if (Convert.ToInt32(courseSequence) >= 0)
                                courses.Course.Sequence = Convert.ToInt32(courseSequence);
                            else
                                courses.Course.Sequence = 0;
                        }
                        else
                            courses.Course.Sequence = 0;
                        foreach (var module in courses.Modules)
                        {
                            moduleSequence = Request.Form[("Module" + module.Id)];
                            if (!(String.IsNullOrEmpty(moduleSequence) && String.IsNullOrWhiteSpace(moduleSequence)))
                            {
                                if (Convert.ToInt32(moduleSequence) >= 0)
                                    module.Sequence = Convert.ToInt32(moduleSequence);
                                else
                                    module.Sequence = 0;
                            }
                            else
                                module.Sequence = 0;
                        }
                    }
                    if (phaseScheduleLogic.SaveCourseModuleSequence(result, phaseSchedule))
                    {
                        TempData["PhaseMessage"] = "Phase Schedule has been inserted successfully.";
                    }
                    else
                    {
                        TempData["PhaseMessage"] = "Error occured while inserting Phase Schedule.";
                    }
                }
                else
                {
                    string lessonSequence = String.Empty;
                    var result = phaseScheduleLogic.ListLessons(Convert.ToInt32(batchId), Convert.ToInt32(phaseId), Convert.ToInt32(categoryTypeId), ref isLessonSequenceFound);

                    foreach (var lesson in result)
                    {
                        lessonSequence = Request.Form[("Lesson" + lesson.Id)];
                        if (!(String.IsNullOrEmpty(lessonSequence) && String.IsNullOrWhiteSpace(lessonSequence)))
                        {
                            if (Convert.ToInt32(lessonSequence) >= 0)
                                lesson.Sequence = Convert.ToInt32(lessonSequence);
                            else
                                lesson.Sequence = 0;
                        }
                        else
                            lesson.Sequence = 0;
                    }
                    if (phaseScheduleLogic.SaveLessonSequence(result, phaseSchedule))
                    {
                        TempData["PhaseMessage"] = "Lesson sequence has been inserted successfully.";
                    }
                    else
                    {
                        TempData["PhaseMessage"] = "Error occured while inserting Lesson sequence.";
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Incorrect Input.";
            }
            return RedirectToAction("Index");
        }
    }
}