using PTSMSBAL.Scheduling.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.Scheduling
{
    public class StudentScheduleController : Controller
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
        public JsonResult GetGroundSchedulerResourceForTrainee()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            string companyId = HttpContext.User.Identity.Name;

            var events = schedulerUtility.GetGroundSchedulerResourceForTrainee(companyId);

            var schedulerResourceList = new List<object>();

            foreach (var e in events)
            {
                schedulerResourceList.Add(new
                {
                    id = e.BatchId, //require double checking
                    SerialNumber = e.SerialNumber,
                    BatchClassName = e.BatchClassName
                });
            }
            return Json(schedulerResourceList.ToArray(), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetScheduledEventForTrainee()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            string companyId = HttpContext.User.Identity.Name;

            var events = schedulerUtility.GetScheduledEventForTrainee(companyId);

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
                        allDay = Convert.ToBoolean(e.IsAllDay)
                    });
            }
            return Json(scheduledEventList.ToArray(), JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult GetSchedulerResourceForTrainee()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            string companyId = HttpContext.User.Identity.Name;

            var events = schedulerUtility.GetSchedulerResourceForTrainee(companyId);

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
        public JsonResult GetFTDandFlyingScheduledEventForTrainee()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            string companyId = HttpContext.User.Identity.Name;

            var events = schedulerUtility.GetFTDandFlyingScheduledEventForTrainee(companyId);

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
    }
}