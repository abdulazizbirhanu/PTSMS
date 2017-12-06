using PTSMSBAL.Scheduling.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.Scheduling
{
    public class InstructorScheduleController : Controller
    {
        [HttpGet]
        //[PTSAuthorizeAttribute]
        public ActionResult GroundClass()
        {
            if (TempData["ScheduleMessage"] != null)
            {
                ViewBag.ScheduleMessage = TempData["ScheduleMessage"];
                TempData["ScheduleMessage"] = null;
            }
            return View();
        }

        [HttpGet]
        //[PTSAuthorizeAttribute]
        public ActionResult Equipment()
        {
            if (TempData["FTDAndFlyingScheduleMessage"] != null)
            {
                ViewBag.FTDAndFlyingScheduleMessage = TempData["FTDAndFlyingScheduleMessage"];
                TempData["FTDAndFlyingScheduleMessage"] = null;
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetGroundSchedulerResourceForInstructor()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            string companyId = HttpContext.User.Identity.Name;

            var events = schedulerUtility.GetGroundSchedulerResourceForInstructor(companyId);

            var schedulerResourceList = new List<object>();

            foreach (var e in events)
            {
                schedulerResourceList.Add(new
                {
                    id = e.BatchId,
                    SerialNumber = e.SerialNumber,
                    BatchClassName = e.BatchClassName
                });
            }
            return Json(schedulerResourceList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CancelSchedule(FormCollection formCollection)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            string reason = Request.Form["cancelReason"];
            string remark = Request.Form["cancelRemark"];
            string flyingAndFTDScheduleId = Request.Form["cancelFlyingAndScheduling"];
            string date = Request.Form["txtEditDate"];
            string batch = Request.Form["txtBatchIdCurrent"];
            string lessonType = Request.Form["txtLessonTypeCurrent"];
            if (!string.IsNullOrEmpty(batch))
            {
                int batchClassId = int.Parse(batch);
                TempData["BatchClassId"] = batchClassId;
            }
            if (!string.IsNullOrEmpty(lessonType))
                TempData["LessonType"] = lessonType;

            TempData["displayDate"] = date;
            if (schedulerUtility.CancelSchedule(reason, remark, flyingAndFTDScheduleId))
                TempData["FTDAndFlyingScheduleMessage"] = "Scheduled has been canceled";
            else
                TempData["FTDAndFlyingScheduleMessage"] = "Failed to cancel schedule";
            return RedirectToAction("Equipment");
        }
        [HttpGet]
        public JsonResult GetGroundScheduledEventForInstructor(string month, string year)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            string companyId = HttpContext.User.Identity.Name;

            var events = schedulerUtility.GetScheduledEventForInstructor(companyId, month, year);

            var scheduledEventList = new List<object>();

            foreach (var e in events)
            {
                scheduledEventList.Add(
                    new
                    {
                        id = e.EventID,
                        resourceId = e.ResourceId,
                        title = e.Title,
                        description = e.Description,
                        start = e.EventStart.AddHours(3),
                        end = e.EventEnd.AddHours(3),
                        allDay = Convert.ToBoolean(e.IsAllDay),

                        moduleID = e.ModuleID,
                        moduleCode = e.ModuleCode,

                        courseID = e.CourseID,
                        courseCode = e.CourseCode,

                        batchID = e.BatchID,
                        batchClassName = e.BatchClassName
                    });
            }
            return Json(scheduledEventList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSchedulerResourceForInstructor()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            string companyId = HttpContext.User.Identity.Name;

            var events = schedulerUtility.GetSchedulerResourceForInstructor(companyId);

            var schedulerResourceList = new List<object>();

            foreach (var e in events)
            {
                schedulerResourceList.Add(
                    new
                    {
                        id = e.EquipmentId,
                        equipmentModel = e.EquipmentModel,
                        EquipmentName = e.EquipmentName,
                        WorkingHours = e.WorkingHours
                    });
            }
            return Json(schedulerResourceList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFTDandFlyingScheduledEventForInstructor(string month, string year)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            string companyId = HttpContext.User.Identity.Name;

            var events = schedulerUtility.GetFTDandFlyingScheduledEventForInstructor(companyId, month, year);

            var scheduledEventList = new List<object>();
            //{ id: '1', resourceId: 'b', start: TODAY + 'T02:00:00', end: TODAY + 'T05:00:00', title: 'DAN' },
            foreach (var e in events)
            {
                scheduledEventList.Add(
                    new
                    {
                        id = e.EventID,
                        resourceId = e.ResourceId,
                        title = e.Title,
                        description = e.Description,
                        start = e.EventStart.AddHours(3),
                        end = e.EventEnd.AddHours(3),
                        allDay = Convert.ToBoolean(e.IsAllDay)
                    });
            }
            return Json(scheduledEventList.ToArray(), JsonRequestBehavior.AllowGet);
        }
        /*Begin, methods for edit*/

    }
}