using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using PTSMSBAL.Dispatch;
using PTSMSBAL.Enrollment.Operations;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSBAL.Logic.Scheduling.Relations;
using PTSMSBAL.Scheduling.Operations;
using PTSMSBAL.Scheduling.Others;
using PTSMSBAL.Scheduling.References;
using PTSMSBAL.Scheduling.Relations;
using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMS.Controllers.Scheduling
{
    public class SchedulerController : Controller
    {
        private ApplicationSignInManager _signInManager;



        ModuleScheduleLogic moduleScheduleLogic = new ModuleScheduleLogic();
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult OrderSchedule()
        {
            return View();
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult OrderSchedule(string LessonCategoryName)
        {
            SchedulerLogic schedulerLogic = new SchedulerLogic();
            if (LessonCategoryName.Equals("Ground"))
            {
                schedulerLogic.TimeTableScheduler();
            }
            else
            {
                schedulerLogic.TimeTableScheduler();
            }
            return Json(new { isSuccess = true, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        #region Ground Class Scheduler
        /*Begin Newly Added*/
        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult GetGroundSchedulerResource()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.GetGroundSchedulerResource();

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

        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult BatchClassColorList()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<BatchClassColor> BatchClassColorlist = schedulerUtility.BatchClassColorList();
            return Json(new { resultData = BatchClassColorlist, hasList = BatchClassColorlist.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        /*End, Newly Added*/
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult GroundClassSchedule()
        {
            if (TempData["ScheduleMessage"] != null)
            {
                //ViewBag.ScheduleMessage = TempData["ScheduleMessage"];
                TempData["ScheduleMessage"] = null;
            }
            ViewBag.ScheduleAddMessage = null;
            return View();
        }

        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult getScheduledEvent(string month, string year)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.ScheduledEventList(month, year);

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

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult getScheduledEventForRefreshFullCalendar(string month, string year)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.ScheduledEventList(month, year);

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
                        start = Convert.ToDateTime(e.EventStart.ToString("g")),
                        end = Convert.ToDateTime(e.EventEnd.ToString("g")),
                        allDay = Convert.ToBoolean(e.IsAllDay)
                    });
            }

            return Json(scheduledEventList.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetEquipmentEventForInstructorUtilizations(string month, string year)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.GetEquipmentEventForInstructorUtilizations(month, year);

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
            PersonLeaveLogic PLL = new PersonLeaveLogic();
            var instructorLeave = PLL.List();

            InstructorLogic instructorLogic = new InstructorLogic();
            List<Instructor> instructors = instructorLogic.List();
            foreach (PersonLeave pl in instructorLeave)
            {

                scheduledEventList.Add(
                    new
                    {
                        id = pl.PersonLeaveId,
                        resourceId = pl.Person.CompanyId,
                        title = "Leave",
                        description = pl.LeaveType.Description,
                        start = pl.FromDate,
                        end = pl.ToDate,
                        allDay = false
                    });
            }

            LicenseLogic license = new LicenseLogic();
            List<License> licenseList = license.List();

            foreach (License li in licenseList)
            {

                scheduledEventList.Add(
                   new
                   {
                       id = li.LicenseId,
                       resourceId = li.Person.CompanyId,
                       title = "License Issue",
                       description = li.Description,
                       start = li.ExpiryDate,
                       end = Convert.ToDateTime("12/31/9999"),
                       allDay = false
                   });
            }

            return Json(scheduledEventList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetEquipmentEventForInstructorUtilizationsNew(string day, string month, string year)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.GetEquipmentEventForInstructorUtilizations(month, year);
            DateTime startingDate = Convert.ToDateTime(month + "/" + day + "/" + year);

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
            PersonLeaveLogic PLL = new PersonLeaveLogic();
            var instructorLeave = PLL.List();

            InstructorLogic instructorLogic = new InstructorLogic();
            List<Instructor> instructors = instructorLogic.List();

            foreach (PersonLeave pl in instructorLeave)
            {
                Instructor Instructor = instructors.Where(i => i.PersonId == pl.PersonId).FirstOrDefault();
                scheduledEventList.Add(
                    new
                    {
                        id = pl.PersonLeaveId,
                        resourceId = Instructor.InstructorId,
                        title = "Leave",
                        description = pl.LeaveType.Description,
                        start = pl.FromDate,
                        end = pl.ToDate,
                        allDay = false
                    });
            }

            LicenseLogic license = new LicenseLogic();
            List<License> licenseList = license.ListExpiredLicence(startingDate);

            foreach (License li in licenseList)
            {
                Instructor ins = instructors.Where(i => i.PersonId == li.PersonId).FirstOrDefault();
                if (ins != null)
                {
                    scheduledEventList.Add(
                       new
                       {
                           id = li.LicenseId,
                           resourceId = ins.InstructorId,
                           title = "License Issue",
                           description = li.LicenseType.Description,
                           start = li.ExpiryDate,
                           end = Convert.ToDateTime("12/31/9999"),
                           allDay = false
                       });
                }
            }

            return Json(scheduledEventList.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult get_FreeTimeSlotAndRoom(int ModuleScheduleId)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<UnscheduledResource> unscheduledResource = schedulerUtility.get_FreeTimeSlotAndRoom(ModuleScheduleId);
            return new JsonResult
            {
                MaxJsonLength = Int32.MaxValue,
                Data = new { resultData = unscheduledResource, hasList = unscheduledResource.Count > 0 },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public ActionResult UpdateModuleSchedule(FormCollection formCollection)
        {
            //txtEventID,dpdDate,dpdPeriod,dpdClassRoom,dpdInstructor
            string moduleScheduleId = Request.Form["txtEventID"];
            string periodId = Request.Form["dpdPeriod"];
            string classRoomId = Request.Form["dpdClassRoom"];
            string date = Request.Form["dpdDate"];
            string instructorId = Request.Form["dpdInstructor"];

            if (!(String.IsNullOrEmpty(moduleScheduleId) || String.IsNullOrEmpty(periodId) || String.IsNullOrEmpty(classRoomId) || String.IsNullOrEmpty(date) || String.IsNullOrEmpty(instructorId)))
            {
                if (moduleScheduleLogic.UpdateModuleSchedule(Convert.ToInt16(moduleScheduleId), Convert.ToInt16(periodId), Convert.ToInt16(classRoomId), date, Convert.ToInt16(instructorId)))
                {
                    TempData["ScheduleMessage"] = "Event successfully updated.";
                    return RedirectToAction("GroundClassSchedule");
                }
                else
                {
                    TempData["ScheduleMessage"] = "Failed to update the event.";
                    return RedirectToAction("GroundClassSchedule");
                }
            }
            else
            {
                TempData["ScheduleMessage"] = "Failed to update the event due to invalid input.";
                return RedirectToAction("GroundClassSchedule");
            }
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult AddNewModuleSchedule(int phaseScheduledId, int moduleId, string date, int noOfDays, int periodId, int classRoomId, int instructorId)
        {
            if (!(String.IsNullOrEmpty(date) || String.IsNullOrWhiteSpace(date)) && (phaseScheduledId != 0) && (moduleId != 0) && (noOfDays > 0) && (periodId != 0) && (classRoomId != 0) && (instructorId != 0))
            {
                List<FreePeriodList> freePeriodList = moduleScheduleLogic.AddModuleSchedule(phaseScheduledId, moduleId, date, noOfDays, periodId, classRoomId, instructorId);
                if (freePeriodList.Count() > 0)
                {
                    ViewBag.ScheduleMessage = freePeriodList;
                    //TempData["ScheduleMessage"] = freePeriodList;// "Event successfully Added.";
                    return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["ScheduleMessage"] = "Failed to add the event.";
                }
            }
            else
            {
                TempData["ScheduleMessage"] = "Failed to add the event due to invalid input.";
            }
            TempData["displayDate"] = date;
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        //AddNewModuleSchedule
        [PTSAuthorizeAttribute]
        public JsonResult GetFilterValue(string filterBy)
        {
            if (!(String.IsNullOrEmpty(filterBy) || String.IsNullOrWhiteSpace(filterBy)))
            {
                SchedulerUtility schedulerUtility = new SchedulerUtility();

                var result = schedulerUtility.Filter(filterBy);
                return Json(new
                {
                    resultData = result.Select(item => new
                    {
                        Id = item.Id,
                        Name = item.Name
                    }),
                    hasList = result.Count() > 0
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                resultData = new List<FilterView>(),
                hasList = false
            }, JsonRequestBehavior.AllowGet);
        }
        [PTSAuthorizeAttribute]
        public JsonResult GetAttendaceData(int moduleScheduleId)
        {
            if (moduleScheduleId != 0)
            {
                SchedulerUtility schedulerUtility = new SchedulerUtility();
                AttendanceView attendanceView = schedulerUtility.GetAttendaceData(moduleScheduleId);
                return Json(new { resultData = attendanceView, hasList = (attendanceView.Trainees.Count > 0 && attendanceView.ClassRooms.Count > 0 && attendanceView.PotentialInstructors.Count > 0) }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                resultData = new AttendanceView(),
                hasList = false
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult Filter(string FilterBy, string FilterValue)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.FilterScheduledEventList(FilterBy, FilterValue);

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
        [PTSAuthorizeAttribute]
        public JsonResult ListPhaseCourses(int instructorId, int batchId) //javascipt need to be updated accordingly
        {
            PhaseScheduleLogic phaseScheduleLogic = new PhaseScheduleLogic();
            var result = phaseScheduleLogic.ListPhaseCourses(instructorId, batchId);

            return Json(new { resultData = result, hasList = result.Count() > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetPhaseModules(int instructorId, int phaseCourseId, int phaseScheduleId)
        {
            PhaseScheduleLogic phaseScheduleLogic = new PhaseScheduleLogic();
            List<PhaseModules> phaseModuleList = phaseScheduleLogic.ListPhaseModules(instructorId, phaseCourseId, phaseScheduleId);
            return Json(new { resultData = phaseModuleList, hasList = phaseModuleList.Count() > 0 }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult get_FreeTimeSlotAndRoomForSpecificDate(string date, int instructorId, int phaseScheduleId, int phseModuleId, int noOfDays)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            //List<FreePeriod> FreePeriodList = schedulerUtility.get_FreeTimeSlotAndRoomForSpecificDate(Convert.ToDateTime(date + " 12:00:00"), instructorId, phaseScheduleId, phseModuleId, noOfDays);
            List<FreePeriodList> FreePeriodList = schedulerUtility.get_FreeTimeSlotAndRoomForSpecificDate(Convert.ToDateTime(date + " 12:00:00"), instructorId, phaseScheduleId, phseModuleId, noOfDays);
            return Json(new { resultData = FreePeriodList, hasList = FreePeriodList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetFreeInstructorForSpecificDate(string date, int batchClassId)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<FreeInstructor> FreeInstructorList = schedulerUtility.GetFreeInstructorForSpecificDate(Convert.ToDateTime(date + " 12:00:00"), batchClassId);
            return Json(new { resultData = FreeInstructorList, hasList = FreeInstructorList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult AddAttendance(int moduleSchedule, string takenDate, int instructorId, int classRoomId, string trainees, string Note, string password, string moduleActivities)
        {
            AttendanceExceptionLogic attendanceExceptionLogic = new AttendanceExceptionLogic();
            if (HttpContext.User.Identity.Name != null)
            {
                var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

                if (userValidationResult.Result.ToString() == "Success" || userValidationResult.Result.ToString() == "Failure")
                {
                    return Json(new
                    {
                        isSuccess = attendanceExceptionLogic.Add(moduleSchedule, takenDate, instructorId, classRoomId, trainees, Note, moduleActivities)
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Failed to save Attendance  due to incorrect password."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "The password you entered is no the password of the active user."
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        /// <summary>
        /// Used to get instructor specific color for the scheduled event
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult InstructorColorList()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<InstructorColor> ScheduledInstructorList = schedulerUtility.ListScheduledInstructors();
            return Json(new { resultData = ScheduledInstructorList, hasList = ScheduledInstructorList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetGroundScheduleDetail(int moduleScheduleId)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            ModuleSchedule moduleSchedule = schedulerUtility.ModuleScheduleDetails(moduleScheduleId);
            return Json(new { resultData = moduleSchedule, hasValue = moduleSchedule != null }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[PTSAuthorizeAttribute]
        //public JsonResult GetValidBatchClassToBeMerged(int moduleScheduleId)
        //{
        //    SchedulerUtility schedulerUtility = new SchedulerUtility();

        //    return Json(new
        //    {
        //        resultData = batchClassViewList,
        //        hasList = batchClassViewList.Count > 0
        //    }, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult MergeBatchClass(int moduleScheduleId, string batchClassList, int moduleId)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            if (!string.IsNullOrEmpty(batchClassList) && moduleScheduleId > 0)
                TempData["ScheduleMessage"] = schedulerUtility.MergeBatchClass(moduleScheduleId, batchClassList, moduleId);
            else
                TempData["ScheduleMessage"] = "Invalid Input";
            return RedirectToAction("GroundClassSchedule", "Scheduler");
        }

        //CancelSchedule
        [HttpPost]
        [PTSAuthorizeAttribute]
        public ActionResult CancelGroundSchedule(FormCollection formCollection)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            string reason = Request.Form["cancelReason"];
            string remark = Request.Form["cancelRemark"];
            string moduleScheduleId = Request.Form["cancelModuleScheduleId"];
            if (schedulerUtility.CancelGroundSchedule(reason, remark, moduleScheduleId))
                TempData["FTDAndFlyingScheduleMessage"] = "Scheduled has been canceled";
            else
                TempData["FTDAndFlyingScheduleMessage"] = "Failed to cancel schedule";
            return RedirectToAction("GroundClassSchedule");
        }
        #endregion

        #region FTD and Flying Scheduler


        /*Begin, New Code */
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult EquipmentScheduler()
        {
            if (TempData["FTDAndFlyingScheduleMessage"] != null)
            {
                ViewBag.FTDAndFlyingScheduleMessage = TempData["FTDAndFlyingScheduleMessage"];
                TempData["FTDAndFlyingScheduleMessage"] = null;
            }
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<BatchClass> PhaseScheduleList = schedulerUtility.GetBatchClass();



            ViewBag.BatchClassList = PhaseScheduleList.Select(item => new BatchView
            {
                Id = item.BatchClassId,
                Name = item.BatchClassName
            }).OrderBy(x => x.Name).ToList();

            if (TempData["BatchClassId"] != null)
            {
                ViewBag.BatchClassId = TempData["BatchClassId"];
                TempData["BatchClassId"] = null;
            }
            if (TempData["LessonType"] != null)
            {
                ViewBag.LessonType = TempData["LessonType"];
                TempData["LessonType"] = null;
            }
            RescheduleReasonAccess rescheduleReasonAccess = new RescheduleReasonAccess();
            List<RescheduleReason> checkInStatusList = rescheduleReasonAccess.List();
            checkInStatusList.Add(new RescheduleReason { RescheduleReasonId = 0, RescheduleReasonName = "-- Select Reschedule Reason --" });
            ViewBag.RescheduleReasonId = new SelectList(checkInStatusList.OrderBy(x => x.RescheduleReasonId).Select(item => new
            {
                RescheduleReasonId = item.RescheduleReasonId > 0 ? item.RescheduleReasonId.ToString() : "",
                RescheduleReasonName = item.RescheduleReasonName
            }), "RescheduleReasonId", "RescheduleReasonName");
            return View();
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetSchedulerResource(string EquiList)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            List<SchedulerResource> events = schedulerUtility.GetSchedulerResource();
            var schedulerResourceList = new List<object>();

            SchedulerResource eventsTemp = new SchedulerResource();

            if (EquiList != null && EquiList != "")
            {
                string[] EquiLists = EquiList.Split('~');
                foreach (string e in EquiLists)
                {
                    if (e != "")
                    {
                        int eID = Convert.ToInt16(e);
                        eventsTemp = events.Where(eq => eq.EquipmentId == eID).FirstOrDefault();
                        try
                        {
                            schedulerResourceList.Add(
                                           new
                                           {
                                               id = eventsTemp.EquipmentId,
                                               equipmentModel = eventsTemp.EquipmentModel,
                                               equiDisplayName = eventsTemp.EquipmentName + "/" + eventsTemp.EstimatedRemainingHours,
                                               EquipmentName = eventsTemp.EquipmentName,
                                               WorkingHours = eventsTemp.WorkingHours,
                                               EstimatedRemainingHours = eventsTemp.EstimatedRemainingHours
                                           });
                        }
                        catch (Exception rr)
                        {

                        }
                    }
                }
            }
            else
            {
                foreach (var e in events)
                {
                    schedulerResourceList.Add(
                        new
                        {
                            id = e.EquipmentId,
                            equipmentModel = e.EquipmentModel,
                            equiDisplayName = e.EquipmentName + "/" + e.EstimatedRemainingHours,
                            EquipmentName = e.EquipmentName,
                            WorkingHours = e.WorkingHours,
                            EstimatedRemainingHours = e.EstimatedRemainingHours
                        });
                }
            }






            return Json(schedulerResourceList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Instructor utilization
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult GetSchedulerInstructorResource()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.GetSchedulerInstructorResource();

            var schedulerResourceList = new List<object>();

            foreach (var e in events)
            {
                schedulerResourceList.Add(
                    new
                    {
                        id = e.InstructorId,
                        InstructorName = e.InstructorName,
                        EquipmentType = e.EquipmentType
                    });
            }

            return Json(schedulerResourceList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult FlyingAndFTDInstructorColorList()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<InstructorColor> ScheduledInstructorList = schedulerUtility.FlyingAndFTDInstructorColorList();
            return Json(new { resultData = ScheduledInstructorList, hasList = ScheduledInstructorList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult InstructorUtilization()
        {
            if (TempData["FTDAndFlyingScheduleMessage"] != null)
            {
                ViewBag.FTDAndFlyingScheduleMessage = TempData["FTDAndFlyingScheduleMessage"];
                TempData["FTDAndFlyingScheduleMessage"] = null;
            }
            return View();
        }

        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult GetEquipmentEventForInstructorUtilization()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.GetEquipmentEventForInstructorUtilization();

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



        /////////////End Instructor Utilization 
        /// <summary>
        ///Get all lessons that trainees can learn from a specifically clicked equipment, 
        /// Validated Constraints.
        /// 1. Check whether the trainee has already taken the mentioned lesson
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="traineeId"></param>
        /// <param name="equipmentId"></param>
        /// <returns></returns>
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetTraineeLessons(string date, string time, int traineeId, int equipmentId)
        {
            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time);// DateTime.ParseExact(date + time, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<LessonView> LessonList = schedulerUtility.GetTraineeLessonList(startingTime, traineeId, equipmentId);
            return Json(new { resultData = LessonList, hasList = LessonList.Count > 0 }, JsonRequestBehavior.AllowGet);
           
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetTraineeLessonList(int equipmentId, int traineeId, string ActualclickedDate)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            DateTime clickedDate = Convert.ToDateTime(ActualclickedDate);
            List<PhaseLessonView> LessonList = schedulerUtility.GetTraineeLessonList(equipmentId, traineeId, clickedDate);
            return Json(new { resultData = LessonList, hasList = LessonList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  Validated Constraints.
        /// 1. Is the time in Between Equipment Working Hour
        /// 2. Is the time in between equipment Downtime
        /// 3. Is the time in between equipment recuring Downtime
        /// 4. Is Equipment Free
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <param name="lessonId"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult CheckConstraints(int equipmentId, int lessonId, string date, string time, string traineeId)
        {// IsInBetweenEquipmentWorkingHour
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            FlyingFTDSchedule schedule = new FlyingFTDSchedule();
            time = " " + time;
            schedule.EquipmentId = equipmentId;
            schedule.LessonId = lessonId;
            List<int> traineeIDlist = new List<int>();
            string[] traineeIdSplited = traineeId.Split('~');
            foreach (string t in traineeIdSplited)
            {
                string[] temp = t.Split('-');
                if (int.TryParse(temp[0], out int x))
                    traineeIDlist.Add(x);
            }
            schedule.ScheduleStartTime = Convert.ToDateTime(date + time);

            string message = "";
            bool IsInBetweenEquipmentWorkingHour = false;
            bool IsInBetweenEquipmentDowntime = true;
            bool IsInBetweenRecurringDowntimeSchedule = true;
            bool IsEquipmentFree = false;
            bool IsTraineeFree = false;
            bool IsInBetweenEquipmentMaintainanceTime = true;

            //Constraint Is In Between Equipment Working Hour
            if (schedulerUtility.IsInBetweenEquipmentMaintainanceTime(schedule))
                message = "Schedule time should not be in beetween the equipment maintainance hours.";
            else
                IsInBetweenEquipmentMaintainanceTime = false;


            //Constraint Is In Between Equipment Working Hour
            IsInBetweenEquipmentWorkingHour = true;
            /*  if (schedulerUtility.IsInBetweenEquipmentWorkingHour(schedule))
                  IsInBetweenEquipmentWorkingHour = true;
              else
                  message = "Schedule time should be in beetween the selected equipment working hours.";*/

            //Constraint is in between equipment Downtime
            if (schedulerUtility.IsInBetweenEquipmentDowntime(schedule))
                message = message + " Schedule time should not be in beetween equipment downtime.";
            else
                IsInBetweenEquipmentDowntime = false;

            //Constraint is in between equipment recuring Downtime
            if (schedulerUtility.IsInBetweenRecurringDowntimeSchedule(schedule))
                message = message + " Schedule time should not be in beetween recurring downtime schedule.";
            else
                IsInBetweenRecurringDowntimeSchedule = false;

            //Constraint:- Is Equipment Free
            if (schedulerUtility.IsEquipmentFree(schedule))
                IsEquipmentFree = true;
            else
                message = message + " Equipment has already scheduled.";

            //Constraint:- check Trainee Freeness
            foreach(int traineeid in traineeIDlist)
            {
                schedule.TraineeId = traineeid;
                if (schedulerUtility.IsTraineeFree(schedule))
                    IsTraineeFree = true;
                else
                {
                    message = message + " The selected trainee is not free.";
                    break;
                }
            }
           


            return Json(new { isInBetweenEquipmentWorkingHour = IsInBetweenEquipmentWorkingHour, isInBetweenEquipmentDowntime = IsInBetweenEquipmentDowntime, isInBetweenRecurringDowntimeSchedule = IsInBetweenRecurringDowntimeSchedule, isEquipmentFree = IsEquipmentFree, isTraineeFree = IsTraineeFree, isInBetweenEquipmentMaintainanceTime = IsInBetweenEquipmentMaintainanceTime, Message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetReservedDateTimes(int equipmentId, string date)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            DateTime startingDate = Convert.ToDateTime(date);
            List<FlyingFTDSchedule> ScheduleList = schedulerUtility.GetReservedDateTimes(equipmentId, date);
            List<EquipmentDowntimeSchedule> EquipmentDownTimeScheduleList = schedulerUtility.GetEquipmentDowntimeSchedule(equipmentId, startingDate);
            List<FTDRecurringDownTime> FTDRecurringDownTimeList = schedulerUtility.GetEquipmentRecurringDowntimeSchedule(equipmentId, startingDate);

            List<DisplayDropDownOption> displayDropDownOptionList = new List<DisplayDropDownOption>();

            if (ScheduleList.Count > 0)
            {
                displayDropDownOptionList.Add(new DisplayDropDownOption
                {
                    Id = 0,
                    Name = "Equipment Working Hours: " + (ScheduleList.FirstOrDefault().ScheduleStartTime.Date + ScheduleList.FirstOrDefault().Equipment.StartTime).ToString("MM/dd/yyyy HH:mm") + "-" + ((ScheduleList.FirstOrDefault().ScheduleStartTime.Date + ScheduleList.FirstOrDefault().Equipment.StartTime).AddHours((double)ScheduleList.FirstOrDefault().Equipment.WorkingHours)).ToString("MM/dd/yyyy HH:mm")
                });
            }
            else
            {
                EquipmentLogic equipmentLogic = new EquipmentLogic();
                DateTime datetim = Convert.ToDateTime(date);
                Equipment equipment = equipmentLogic.Details(equipmentId);

                displayDropDownOptionList.Add(new DisplayDropDownOption
                {
                    Id = 0,
                    Name = "Equipment Working Hours: " + (datetim.Date + equipment.StartTime).ToString("MM/dd/yyyy HH:mm") + "-" + ((datetim.Date + equipment.StartTime).AddHours((double)equipment.WorkingHours)).ToString("MM/dd/yyyy HH:mm")
                });
            }
            //Equipment Downtime
            if (EquipmentDownTimeScheduleList.Count > 0)
            {
                foreach (var item in EquipmentDownTimeScheduleList)
                {
                    displayDropDownOptionList.Add(new DisplayDropDownOption
                    {
                        Id = item.EquipmentId,
                        Name = "Equipment Downtime due to " + item.DowntimeReason.DowntimeReasonName + " is :" + item.ScheduleStartDate.ToString("MM/dd/yyyy HH:mm") + "-" + item.ScheduleEndDate.ToString("MM/dd/yyyy HH:mm")
                    });
                }
            }

            //FTD Recurring Downtime
            if (FTDRecurringDownTimeList.Count > 0)
            {
                foreach (var item in FTDRecurringDownTimeList)
                {
                    displayDropDownOptionList.Add(new DisplayDropDownOption
                    {
                        Id = item.FTDRecurringDownTimeId,
                        Name = "FTD Recurring Downtime due to " + item.BreakName + " is :" + item.StartingDate.ToString("HH:mm") + "-" + item.EndingDate.ToString("HH:mm")
                    });
                }
            }

            //Schedule lesson list
            if (ScheduleList.Count > 0)
            {
                foreach (var item in ScheduleList)
                {
                    displayDropDownOptionList.Add(new DisplayDropDownOption
                    {
                        Id = item.EquipmentId,
                        Name = item.ScheduleStartTime.ToString("HH:mm") + " -" + item.ScheduleEndTime.ToString("HH:mm")
                    });
                }
            }
            return Json(new { resultData = displayDropDownOptionList, hasList = displayDropDownOptionList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetFTDandFlyingInstructors(int equipmentId, string date, string time, string traineeLesson)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time);
            string[] TraineeLessonIdArray = traineeLesson.Split('~');
            List<FTDandFlyingInstructorView> InstructorList = schedulerUtility.GetFTDandFlyingInstructors(equipmentId, startingTime, TraineeLessonIdArray);
            return Json(new { resultData = InstructorList, hasList = InstructorList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetFlyingAndFTDScheduleDetail(int flyingAndFTDScheduleId)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            var equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
            var FTDScheduleId = new int();
            EquipmentScheduleBriefingDebriefing briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(flyingAndFTDScheduleId, true, false);
            EquipmentScheduleBriefingDebriefing debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(flyingAndFTDScheduleId, false, true);

            if (briefing != null)
            {
                FTDScheduleId = briefing.FlyingFTDScheduleId;
                debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(FTDScheduleId, false, true);
            }
            else if (debriefing != null)
            {
                FTDScheduleId = debriefing.FlyingFTDScheduleId;
                briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(FTDScheduleId, true, false);
            }
            else
            {
                FTDScheduleId = flyingAndFTDScheduleId;
                briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(flyingAndFTDScheduleId, true, false);
                debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(flyingAndFTDScheduleId, false, true);
            }
            FlyingFTDSchedule flyingFTDSchedule = schedulerUtility.Details(FTDScheduleId);

            return Json(new { ScheduledEventData = flyingFTDSchedule, Briefing = briefing, Debriefing = debriefing, foundScheduledEventDetail = flyingFTDSchedule != null, foundBriefingDetail = briefing != null, foundDebriefingDetail = debriefing != null }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetEquipmentInstructors(int equipmentId, string date, string time, int fylingAndFTDScheduleId)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time);

            PTSContext dbContext = new PTSContext();
            FlyingFTDSchedule flyingFTDSchedule = dbContext.FlyingFTDSchedules.Find(fylingAndFTDScheduleId);
            string traineeLesson = flyingFTDSchedule.TraineeId + "-" + flyingFTDSchedule.LessonId;
            string[] TraineeLessonIdArray = traineeLesson.Split('~');
            List<FTDandFlyingInstructorView> InstructorList = schedulerUtility.GetFTDandFlyingInstructors(equipmentId, startingTime, TraineeLessonIdArray);
            return Json(new { resultData = InstructorList, hasList = InstructorList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult AddNewTraineeLessonSchedule1(string date, string StartingTime)
        {
            return Json(new
            {
                message = TempData["FTDAndFlyingScheduleMessage"],
                resultData = "",
                hasList = true,
                isSuccessAdd = false
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult AddNewTraineeLessonSchedule(string date, string StartingTime, string Equipmentid, string TraineeLessonId, string InstructorId, string isCustomBriefingTime, string isCustomDebriefingTime, string briefingStartingTime, string debriefingStartingTime, string briefingTimeId, string debriefingTimeId, int batchClassId, string lessonType, string rescheduledReasonId)
        {
            bool isSuccessAdd = false;
            TraineeEvaluationTemplateAccess TEvTaccess = new TraineeEvaluationTemplateAccess();
            string returnMessage = string.Empty;
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            if (!((String.IsNullOrEmpty(date) || String.IsNullOrWhiteSpace(date)) && (String.IsNullOrEmpty(StartingTime) || String.IsNullOrWhiteSpace(StartingTime)) && (String.IsNullOrEmpty(Equipmentid) || String.IsNullOrWhiteSpace(Equipmentid)) && (String.IsNullOrEmpty(TraineeLessonId) || String.IsNullOrWhiteSpace(TraineeLessonId)) && (String.IsNullOrEmpty(InstructorId) || String.IsNullOrWhiteSpace(InstructorId))))
            {
                StartingTime = " " + StartingTime;
                DateTime startingTime = Convert.ToDateTime(date + StartingTime);// DateTime.ParseExact(date + StartingTime, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);

                List<FlyingFTDSchedule> flyingFTDScheduleList = new List<FlyingFTDSchedule>();
                string[] TraineeLessonIdPair = null;
                string[] traineeLessonIdArray = TraineeLessonId.Split('~');
                foreach (var item in traineeLessonIdArray)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        TraineeLessonIdPair = item.Split('-');
                        if (!(String.IsNullOrEmpty(TraineeLessonIdPair[0]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[0]) || String.IsNullOrEmpty(TraineeLessonIdPair[1]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[1])))
                        {

                            int traineeId = Convert.ToInt16(TraineeLessonIdPair[0]);
                            int lessonId = Convert.ToInt16(TraineeLessonIdPair[1]);
                            List<TraineeLesson> objTraineeEvaluationTemplate = TEvTaccess.selectTraineeEvaluationTemplate(lessonId, traineeId);
                            int sequence = 1;
                            if (objTraineeEvaluationTemplate.Count > 0)
                                sequence = objTraineeEvaluationTemplate.Max(o => o.Sequence);

                            flyingFTDScheduleList.Add(new FlyingFTDSchedule
                            {
                                EquipmentId = Convert.ToInt16(Equipmentid),
                                InstructorId = Convert.ToInt16(InstructorId),
                                LessonId = Convert.ToInt16(lessonId),
                                TraineeId = Convert.ToInt16(traineeId),
                                //ScheduleStartTime = startingTime,
                                IsNotified = false,
                                Sequence = 1,
                                Status = Enum.GetName(typeof(FlyingFTDScheduleStatus), 0)
                                //, RescheduleReasonId = (rescheduledReason != 0 ? rescheduledReason : null)
                            });

                        }
                    }
                }
                TempData["displayDate"] = date;
                if (batchClassId > 0)
                    TempData["BatchClassId"] = batchClassId;
                if (!string.IsNullOrEmpty(lessonType))
                    TempData["LessonType"] = lessonType;
                var result = schedulerUtility.GetLeaveAndLicenceExpiry(startingTime, Convert.ToInt16(InstructorId), traineeLessonIdArray);
                if (result != string.Empty)
                {
                    returnMessage = "Failed to add the event due to invalid input.";
                    isSuccessAdd = false;
                }
                returnMessage = schedulerUtility.AddNewTraineeLessonSchedule(flyingFTDScheduleList, traineeLessonIdArray, startingTime, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriefingTime), briefingTimeId, debriefingTimeId, briefingStartingTime, debriefingStartingTime, rescheduledReasonId);

                TempData["FTDAndFlyingScheduleMessage"] = returnMessage;
                if (returnMessage.Contains("successfully scheduled"))
                    isSuccessAdd = true;


            }
            else
            {
                TempData["ScheduleMessage"] = "Failed to add the event due to invalid input.";
            }
            var events = GetFTDandFlyingScheduledEvent(Convert.ToDateTime(date).Month.ToString(), Convert.ToDateTime(date).Year.ToString());

            return Json(new
            {
                message = returnMessage,
                resultData = events,
                hasList = true,
                isSuccessAdd = isSuccessAdd
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public ActionResult UpdateTraineeLessonScheduleForm(FormCollection formCollection)
        {

            string date = Request.Form["txtEditDate"];
            string time = Request.Form["txtEditStartingTime"];
            string equipmentId = Request.Form["dpdEditEquipment"];
            string instructorId = Request.Form["dpdEditInstructor"];
            string fylingAndFTDScheduleId = Request.Form["txtfylingAndFTDScheduleId"];
            string isReschedule = Request.Form["txtIsReschedule"];

            ////////////////Custom Briefing////////////////////////////
            string isCustomBriefingTime = Request.Form["txtEditCustomBriefing"];
            string isCustomDebriefingTime = Request.Form["txtEditCustomDebriefing"];

            string briefingStartingTime = Request.Form["txtEditBriefingStartingTime"];
            string debriefingStartingTime = Request.Form["txtEditDebriefingStartingTime"];
            ////////////////Custom Debriefing /////////////////////////

            string briefingTimeId = Request.Form["dpdEditBriefingTime"];
            string debriefingTimeId = Request.Form["dpdEditDebriefingTime"];
            ////////////////informations to stay on current state of view//////

            string batch = Request.Form["dpdScheduleBatchClass"];
            string lessonType = Request.Form["txtLessonType"];

            if (!string.IsNullOrEmpty(batch))
            {
                int batchClassId = int.Parse(batch);
                TempData["BatchClassId"] = batchClassId;
            }
            if (!string.IsNullOrEmpty(lessonType))
                TempData["LessonType"] = lessonType;

            TempData["displayDate"] = date;
            ////////////////  end of  informations to stay on current state of view//////
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            if (!((String.IsNullOrEmpty(date) || String.IsNullOrWhiteSpace(date)) && (String.IsNullOrEmpty(time) || String.IsNullOrWhiteSpace(time)) && (String.IsNullOrEmpty(equipmentId) || String.IsNullOrWhiteSpace(equipmentId)) && (String.IsNullOrEmpty(instructorId) || String.IsNullOrWhiteSpace(instructorId)) && (String.IsNullOrEmpty(fylingAndFTDScheduleId) || String.IsNullOrWhiteSpace(fylingAndFTDScheduleId) || String.IsNullOrWhiteSpace(isReschedule))))
            {
                string[] dateArray = date.Split('/');
                time = " " + time;
                DateTime startingTime = Convert.ToDateTime(date + time);
                startingTime = new DateTime(Convert.ToInt16(dateArray[2]), Convert.ToInt16(dateArray[0]), Convert.ToInt16(dateArray[1]), startingTime.Hour, startingTime.Minute, 0);

                TempData["FTDAndFlyingScheduleMessage"] = schedulerUtility.UpdateTraineeLessonSchedule(startingTime, Convert.ToInt16(fylingAndFTDScheduleId), Convert.ToInt16(equipmentId), Convert.ToInt16(instructorId), Convert.ToBoolean(isReschedule), briefingTimeId, debriefingTimeId, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriefingTime), briefingStartingTime, debriefingStartingTime, date);
                return RedirectToAction("EquipmentScheduler");
            }
            else
            {
                TempData["ScheduleMessage"] = "Failed to update the schedule due to invalid input.";
                return RedirectToAction("EquipmentScheduler");
            }
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult UpdateTraineeLessonSchedule(string date, string time, string instructorId, string traineeLesson, string equipmentId, string briefingTimeId, string debriefingTimeId, string fTDAndFlyingScheduleId, string isReschedule, string isCustomBriefingTime, string isCustomDebriefingTime, string briefingStartingTime, string debriefingStartingTime, string batch, string lessonType)
        {
            bool isSuccessUpdated;

            if (!string.IsNullOrEmpty(batch))
            {
                int batchClassId = int.Parse(batch);
                TempData["BatchClassId"] = batchClassId;
            }
            if (!string.IsNullOrEmpty(lessonType))
                TempData["LessonType"] = lessonType;

            TempData["displayDate"] = date;
            DateTime currentDate = Convert.ToDateTime(date);
            ////////////////  end of  informations to stay on current state of view//////
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            if (!((String.IsNullOrEmpty(date) || String.IsNullOrWhiteSpace(date)) && (String.IsNullOrEmpty(time) || String.IsNullOrWhiteSpace(time)) && (String.IsNullOrEmpty(equipmentId) || String.IsNullOrWhiteSpace(equipmentId)) && (String.IsNullOrEmpty(instructorId) || String.IsNullOrWhiteSpace(instructorId)) && (String.IsNullOrEmpty(fTDAndFlyingScheduleId) || String.IsNullOrWhiteSpace(fTDAndFlyingScheduleId) || String.IsNullOrWhiteSpace(isReschedule))))
            {
                string[] dateArray = date.Split('/');
                time = " " + time;
                DateTime startingTime = Convert.ToDateTime(date + time);
                startingTime = new DateTime(Convert.ToInt16(dateArray[2]), Convert.ToInt16(dateArray[0]), Convert.ToInt16(dateArray[1]), startingTime.Hour, startingTime.Minute, 0);

                TempData["FTDAndFlyingScheduleMessage"] = schedulerUtility.UpdateTraineeLessonSchedule(startingTime, Convert.ToInt16(fTDAndFlyingScheduleId), Convert.ToInt16(equipmentId), Convert.ToInt16(instructorId), Convert.ToBoolean(isReschedule), briefingTimeId, debriefingTimeId, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriefingTime), briefingStartingTime, debriefingStartingTime, date);
                if (TempData["FTDAndFlyingScheduleMessage"].ToString().Contains("successfully"))
                    isSuccessUpdated = true;
                else
                    isSuccessUpdated = false;
            }
            else
            {
                TempData["ScheduleMessage"] = "Failed to update the schedule due to invalid input.";
                isSuccessUpdated = false;
            }
            string month = Convert.ToDateTime(date).Month.ToString();
            string year = Convert.ToDateTime(date).Year.ToString();
            var events = GetFTDandFlyingScheduledEvent(month, year);
            //var events = schedulerUtility.GetFTDandFlyingScheduledEvent(Convert.ToInt32(fTDAndFlyingScheduleId));
            return Json(new
            {
                message = TempData["FTDAndFlyingScheduleMessage"],
                resultData = events,
                hasList = true,
                isSuccessUpdated = isSuccessUpdated
            }, JsonRequestBehavior.AllowGet);
        }
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            FlyingFTDSchedule flyingFTDSchedule = schedulerUtility.Details((int)id);
            if (flyingFTDSchedule == null)
            {
                return HttpNotFound();
            }
            return View(flyingFTDSchedule);
        }

        // [HttpPost]
       // [PTSAuthorizeAttribute]
        //public ActionResult UpdateScheduleFromDragAndDropForm(FormCollection formCollection)
        //{
        //    string date = Request.Form["txtDragDate"];
        //    string batch = Request.Form["txtBatchIdCurrent"];
        //    string lessonType = Request.Form["txtLessonTypeCurrent"];

        //    string time = Request.Form["txtDragStartingTime"];
        //    string equipmentId = Request.Form["txtDragEquipment"];
        //    string instructorId = Request.Form["txtDragInstructorId"];
        //    string isReschedule = "false";


        //    ////////////////Custom Briefing////////////////////////////
        //    string isCustomBriefingTime = Request.Form["txtDragCustomBriefing"];
        //    string isCustomDebriefingTime = Request.Form["txtDragCustomDebriefing"];

        //    string briefingStartingTime = Request.Form["txtDragBriefingStartingTime"];
        //    string debriefingStartingTime = Request.Form["txtDragDebriefingStartingTime"];
        //    ////////////////Custom Debriefing /////////////////////////

        //    string briefingTimeId = Request.Form["dpdDragBriefingTime"];
        //    string debriefingTimeId = Request.Form["dpdDragDebriefingTime"];
        //    //var traineeLesson = Request.Form["txtDragTraineeId"] + '-' + Request.Form["txtDragLessonId"] + "~";

        //    string TempId = Request.Form["txtDragfylingAndFTDScheduleId"];

        //    int tempID2 = new int();
        //    if (TempId != "")
        //        tempID2 = int.Parse(TempId);

        //    string EventApplyingType = Request.Form["txtDragAndDropEventType"];
        //    string fylingAndFTDScheduleId = null;
        //    var equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
        //    SchedulerUtility schedulerUtility = new SchedulerUtility();



        //    if (!((String.IsNullOrEmpty(date) || String.IsNullOrWhiteSpace(date)) && (String.IsNullOrEmpty(time) || String.IsNullOrWhiteSpace(time)) && (String.IsNullOrEmpty(equipmentId) || String.IsNullOrWhiteSpace(equipmentId)) && (String.IsNullOrEmpty(instructorId) || String.IsNullOrWhiteSpace(instructorId)) && (String.IsNullOrEmpty(fylingAndFTDScheduleId) || String.IsNullOrWhiteSpace(fylingAndFTDScheduleId) || String.IsNullOrWhiteSpace(isReschedule))))
        //    {
        //        time = " " + time;
        //        DateTime startingTime = Convert.ToDateTime(date + time);
        //        if (EventApplyingType == "Lesson")
        //        {
        //            fylingAndFTDScheduleId = TempId;
        //            int id = int.Parse(fylingAndFTDScheduleId);

        //            var flyingFTDSchedule = schedulerUtility.Details((int)id);

        //            string TraineeFname = flyingFTDSchedule.Trainee.Person.FirstName;
        //            string TraineeLname = flyingFTDSchedule.Trainee.Person.LastName;

        //            string tempMessage = schedulerUtility.UpdateTraineeLessonSchedule(startingTime, Convert.ToInt16(fylingAndFTDScheduleId), Convert.ToInt16(equipmentId), Convert.ToInt16(instructorId), Convert.ToBoolean(isReschedule), briefingTimeId, debriefingTimeId, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriefingTime), briefingStartingTime, debriefingStartingTime, date);

        //            if (TempData["FTDAndFlyingScheduleMessage"] == null)
        //                TempData["FTDAndFlyingScheduleMessage"] += tempMessage + "   for " + TraineeFname + " " + TraineeLname;
        //            else
        //                TempData["FTDAndFlyingScheduleMessage"] += " and " + tempMessage + " for " + TraineeFname + " " + TraineeLname;


        //        }
        //        else
        //        {
        //            List<EquipmentScheduleBriefingDebriefing> briefingLessons = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingList(tempID2, true, false);
        //            List<EquipmentScheduleBriefingDebriefing> debriefingLessons = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingList(tempID2, false, true);
        //            EquipmentScheduleBriefingDebriefing tempBerDbr = new EquipmentScheduleBriefingDebriefing();
        //            if (briefingLessons != null && briefingLessons.Count > 0)
        //            {
        //                foreach (EquipmentScheduleBriefingDebriefing br in briefingLessons)
        //                {
        //                    fylingAndFTDScheduleId = br.FlyingFTDScheduleId.ToString();

        //                    int id = int.Parse(fylingAndFTDScheduleId);

        //                    var flyingFTDSchedule = schedulerUtility.Details((int)id);

        //                    string TraineeFname = flyingFTDSchedule.Trainee.Person.FirstName;
        //                    string TraineeLname = flyingFTDSchedule.Trainee.Person.LastName;

        //                    tempBerDbr = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt16(fylingAndFTDScheduleId), false, true);
        //                    startingTime = tempBerDbr.FlyingFTDSchedule.ScheduleStartTime;
        //                    debriefingStartingTime = tempBerDbr.BriefingAndDebriefing.StartingTime.ToString();
        //                    debriefingTimeId = tempBerDbr.BriefingAndDebriefingId.ToString();

        //                    string tempMessage = schedulerUtility.UpdateTraineeLessonSchedule(startingTime, Convert.ToInt16(fylingAndFTDScheduleId), Convert.ToInt16(equipmentId), Convert.ToInt16(instructorId), Convert.ToBoolean(isReschedule), briefingTimeId, debriefingTimeId, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriefingTime), briefingStartingTime, debriefingStartingTime, date);

        //                    if (TempData["FTDAndFlyingScheduleMessage"] == null)
        //                        TempData["FTDAndFlyingScheduleMessage"] += tempMessage + " for " + TraineeFname + " " + TraineeLname;
        //                    else
        //                        TempData["FTDAndFlyingScheduleMessage"] += " and " + tempMessage + " for " + TraineeFname + " " + TraineeLname;

        //                    tempBerDbr = new EquipmentScheduleBriefingDebriefing();
        //                }
        //            }

        //            else if (debriefingLessons != null && debriefingLessons.Count > 0)
        //            {
        //                foreach (EquipmentScheduleBriefingDebriefing dbr in debriefingLessons)
        //                {
        //                    fylingAndFTDScheduleId = dbr.FlyingFTDScheduleId.ToString();

        //                    int id = int.Parse(fylingAndFTDScheduleId);

        //                    var flyingFTDSchedule = schedulerUtility.Details((int)id);

        //                    string TraineeFname = flyingFTDSchedule.Trainee.Person.FirstName;
        //                    string TraineeLname = flyingFTDSchedule.Trainee.Person.LastName;

        //                    tempBerDbr = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt16(fylingAndFTDScheduleId), true, false);


        //                    startingTime = tempBerDbr.FlyingFTDSchedule.ScheduleStartTime;
        //                    briefingStartingTime = tempBerDbr.BriefingAndDebriefing.StartingTime.ToString();
        //                    briefingTimeId = tempBerDbr.BriefingAndDebriefingId.ToString();

        //                    string tempMessage = schedulerUtility.UpdateTraineeLessonSchedule(startingTime, Convert.ToInt16(fylingAndFTDScheduleId), Convert.ToInt16(equipmentId), Convert.ToInt16(instructorId), Convert.ToBoolean(isReschedule), briefingTimeId, debriefingTimeId, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriefingTime), briefingStartingTime, debriefingStartingTime, date);

        //                    if (TempData["FTDAndFlyingScheduleMessage"] == null)
        //                        TempData["FTDAndFlyingScheduleMessage"] += tempMessage + "   for " + TraineeFname + " " + TraineeLname;
        //                    else
        //                        TempData["FTDAndFlyingScheduleMessage"] += " and " + tempMessage + " for " + TraineeFname + " " + TraineeLname;

        //                    tempBerDbr = new EquipmentScheduleBriefingDebriefing();
        //                }
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(batch))
        //        {
        //            int batchClassId = int.Parse(batch);
        //            TempData["BatchClassId"] = batchClassId;
        //        }
        //        if (!string.IsNullOrEmpty(lessonType))
        //            TempData["LessonType"] = lessonType;

        //        TempData["displayDate"] = date;
        //        return RedirectToAction("EquipmentScheduler");
        //    }
        //    else
        //    {
        //        TempData["ScheduleMessage"] = "Failed to move the schedule due to invalid input.";
        //        return RedirectToAction("EquipmentScheduler");
        //    }

        //}
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult UpdateScheduleFromDragAndDrop(string date, string time, string instructorId, string traineeLesson, string equipmentId, string briefingtime, string debriefingTime, string fTDAndFlyingScheduleId, string isReschedule, string isCustomBriefingTime, string isCustomDebriengTime, string briefingStartingTime, string debriefingStartTime, string batch, string lessonType, string EventApplyingType)
        {
            ////////////////Custom Briefing////////////////////////////

            //var traineeLesson = Request.Form["txtDragTraineeId"] + '-' + Request.Form["txtDragLessonId"] + "~";
            EquipmentScheduleBriefingDebriefingAccess brieDebriefAccess = new EquipmentScheduleBriefingDebriefingAccess();

            bool isSuccessUpdated = false;
            bool briefUpdated = false;
            bool deBriefUpdated = false;
            int oldBriefing = 0, oldDebriefing = 0, UpdateBriefing = 0, UpdateDebriefing = 0;
            int tempID2 = new int();
            if (fTDAndFlyingScheduleId != "")
                tempID2 = int.Parse(fTDAndFlyingScheduleId);

            string fylingAndFTDScheduleId = null;
            var equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
            SchedulerUtility schedulerUtility = new SchedulerUtility();



            if (!((String.IsNullOrEmpty(date) || String.IsNullOrWhiteSpace(date)) && (String.IsNullOrEmpty(time) || String.IsNullOrWhiteSpace(time)) && (String.IsNullOrEmpty(equipmentId) || String.IsNullOrWhiteSpace(equipmentId)) && (String.IsNullOrEmpty(instructorId) || String.IsNullOrWhiteSpace(instructorId)) && (String.IsNullOrEmpty(fylingAndFTDScheduleId) || String.IsNullOrWhiteSpace(fylingAndFTDScheduleId) || String.IsNullOrWhiteSpace(isReschedule))))
            {
                time = " " + time;
                DateTime startingTime = Convert.ToDateTime(date + time);
                if (EventApplyingType == "Lesson")
                {
                    fylingAndFTDScheduleId = fTDAndFlyingScheduleId;
                    oldBriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(tempID2, true, false).BriefingAndDebriefingId;
                    oldDebriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(tempID2, false, true).BriefingAndDebriefingId;
                    int id = int.Parse(fylingAndFTDScheduleId);
                    tempID2 = id;
                    var flyingFTDSchedule = schedulerUtility.Details((int)id);

                    string TraineeFname = flyingFTDSchedule.Trainee.Person.FirstName;
                    string TraineeLname = flyingFTDSchedule.Trainee.Person.LastName;

                    string tempMessage = schedulerUtility.UpdateTraineeLessonSchedule(startingTime, Convert.ToInt16(fylingAndFTDScheduleId), Convert.ToInt16(equipmentId), Convert.ToInt16(instructorId), Convert.ToBoolean(isReschedule), briefingtime, debriefingTime, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriengTime), briefingStartingTime, debriefingStartTime, date);

                    if (TempData["FTDAndFlyingScheduleMessage"] == null)
                        TempData["FTDAndFlyingScheduleMessage"] += tempMessage + "   for " + TraineeFname + " " + TraineeLname;
                    else
                        TempData["FTDAndFlyingScheduleMessage"] += " and " + tempMessage + " for " + TraineeFname + " " + TraineeLname;
                    if (tempMessage.Contains("successfully"))
                    {
                        isSuccessUpdated = true;
                        UpdateBriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), true, false).BriefingAndDebriefingId;
                        UpdateDebriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), false, true).BriefingAndDebriefingId;
                        if (oldBriefing != UpdateBriefing)
                            briefUpdated = true;
                        if (oldDebriefing != UpdateDebriefing)
                            deBriefUpdated = true;
                    }
                    else
                    {
                        isSuccessUpdated = false;
                        UpdateBriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), true, false).BriefingAndDebriefingId;
                        UpdateDebriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), false, true).BriefingAndDebriefingId;

                    }
                }
                else
                {
                    List<EquipmentScheduleBriefingDebriefing> briefingLessons = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingList(tempID2, true, false);
                    List<EquipmentScheduleBriefingDebriefing> debriefingLessons = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingList(tempID2, false, true);
                    EquipmentScheduleBriefingDebriefing tempBerDbr = new EquipmentScheduleBriefingDebriefing();
                    if (briefingLessons != null && briefingLessons.Count > 0)
                    {
                        oldBriefing = tempID2;
                        foreach (EquipmentScheduleBriefingDebriefing br in briefingLessons)
                        {
                            fylingAndFTDScheduleId = br.FlyingFTDScheduleId.ToString();

                            int id = int.Parse(fylingAndFTDScheduleId);
                            tempID2 = id;
                            var flyingFTDSchedule = schedulerUtility.Details((int)id);

                            string TraineeFname = flyingFTDSchedule.Trainee.Person.FirstName;
                            string TraineeLname = flyingFTDSchedule.Trainee.Person.LastName;

                            tempBerDbr = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt16(fylingAndFTDScheduleId), false, true);

                            startingTime = tempBerDbr.FlyingFTDSchedule.ScheduleStartTime;
                            debriefingStartTime = tempBerDbr.BriefingAndDebriefing.StartingTime.ToString();
                            debriefingTime = tempBerDbr.BriefingAndDebriefingId.ToString();

                            string tempMessage = schedulerUtility.UpdateTraineeLessonSchedule(startingTime, Convert.ToInt16(fylingAndFTDScheduleId), Convert.ToInt16(equipmentId), Convert.ToInt16(instructorId), Convert.ToBoolean(isReschedule), briefingtime, debriefingTime, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriengTime), briefingStartingTime, debriefingStartTime, date);

                            if (TempData["FTDAndFlyingScheduleMessage"] == null)
                                TempData["FTDAndFlyingScheduleMessage"] += tempMessage + " for " + TraineeFname + " " + TraineeLname;
                            else
                                TempData["FTDAndFlyingScheduleMessage"] += " and " + tempMessage + " for " + TraineeFname + " " + TraineeLname;

                            tempBerDbr = new EquipmentScheduleBriefingDebriefing();
                            if (tempMessage.Contains("successfully"))
                            {
                                isSuccessUpdated = true;
                                UpdateBriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), true, false).BriefingAndDebriefingId;
                                UpdateDebriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), false, true).BriefingAndDebriefingId;
                                if (oldBriefing != UpdateBriefing)
                                    briefUpdated = true;
                            }
                            else
                            {
                                isSuccessUpdated = false;
                                UpdateBriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), true, false).BriefingAndDebriefingId;
                                UpdateDebriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), false, true).BriefingAndDebriefingId;
                            }
                        }
                    }

                    else if (debriefingLessons != null && debriefingLessons.Count > 0)
                    {
                        oldDebriefing = tempID2;
                        foreach (EquipmentScheduleBriefingDebriefing dbr in debriefingLessons)
                        {
                            fylingAndFTDScheduleId = dbr.FlyingFTDScheduleId.ToString();

                            int id = int.Parse(fylingAndFTDScheduleId);
                            tempID2 = id;
                            var flyingFTDSchedule = schedulerUtility.Details((int)id);

                            string TraineeFname = flyingFTDSchedule.Trainee.Person.FirstName;
                            string TraineeLname = flyingFTDSchedule.Trainee.Person.LastName;

                            tempBerDbr = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt16(fylingAndFTDScheduleId), true, false);
                            oldBriefing = tempBerDbr.BriefingAndDebriefingId;

                            startingTime = tempBerDbr.FlyingFTDSchedule.ScheduleStartTime;
                            briefingStartingTime = tempBerDbr.BriefingAndDebriefing.StartingTime.ToString();
                            briefingtime = tempBerDbr.BriefingAndDebriefingId.ToString();

                            string tempMessage = schedulerUtility.UpdateTraineeLessonSchedule(startingTime, Convert.ToInt16(fylingAndFTDScheduleId), Convert.ToInt16(equipmentId), Convert.ToInt16(instructorId), Convert.ToBoolean(isReschedule), briefingtime, debriefingTime, Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriengTime), briefingStartingTime, debriefingStartTime, date);

                            if (TempData["FTDAndFlyingScheduleMessage"] == null)
                                TempData["FTDAndFlyingScheduleMessage"] += tempMessage + "   for " + TraineeFname + " " + TraineeLname;
                            else
                                TempData["FTDAndFlyingScheduleMessage"] += " and " + tempMessage + " for " + TraineeFname + " " + TraineeLname;

                            tempBerDbr = new EquipmentScheduleBriefingDebriefing();
                            if (tempMessage.Contains("successfully"))
                            {
                                isSuccessUpdated = true;
                                UpdateBriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), true, false).BriefingAndDebriefingId;
                                UpdateDebriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), false, true).BriefingAndDebriefingId;
                                if (oldDebriefing != UpdateDebriefing)
                                    deBriefUpdated = true;
                            }
                            else
                            {
                                isSuccessUpdated = false;
                                UpdateBriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), true, false).BriefingAndDebriefingId;
                                UpdateDebriefing = brieDebriefAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(Convert.ToInt32(fylingAndFTDScheduleId), false, true).BriefingAndDebriefingId;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(batch))
                {
                    int batchClassId = int.Parse(batch);
                    TempData["BatchClassId"] = batchClassId;
                }
                if (!string.IsNullOrEmpty(lessonType))
                    TempData["LessonType"] = lessonType;

                TempData["displayDate"] = date;
            }
            else
            {
                TempData["ScheduleMessage"] = "Failed to move the schedule due to invalid input.";
                isSuccessUpdated = false;
            }
            //var events = schedulerUtility.GetFTDandFlyingScheduledEvent(tempID2);
            string month = Convert.ToDateTime(date).Month.ToString();
            string year = Convert.ToDateTime(date).Year.ToString();
            var events = GetFTDandFlyingScheduledEvent(month, year);
            return Json(new
            {
                message = TempData["ScheduleMessage"],
                resultData = events,
                oldBriefing = oldBriefing.ToString(),
                UpdateBriefing = UpdateBriefing.ToString(),
                oldDebriefing = oldDebriefing.ToString(),
                UpdateDebriefing = UpdateDebriefing.ToString(),
                briefUpdated = briefUpdated,
                deBriefUpdated = deBriefUpdated,
                hasList = true,
                isSuccessUpdated = isSuccessUpdated
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Used to get Equipment specific color for the scheduled event
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult ListScheduledEquipment()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<EquipmentColor> ScheduledInstructorList = schedulerUtility.ListScheduledEquipment();
            return Json(new { resultData = ScheduledInstructorList, hasList = ScheduledInstructorList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [PTSAuthorizeAttribute]
        public JsonResult GetFTDandFlyingScheduledEvent(string month, string year)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            var events = schedulerUtility.GetFTDandFlyingScheduledEvent(month, year);

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
            EquipmentMaintenancesLogic equipMent = new EquipmentMaintenancesLogic();
            var equipmentsNotWork = equipMent.List();

            foreach (EquipmentMaintenance eq in equipmentsNotWork)
            {
                if (eq.ScheduledCalanderStartDate != null && eq.ScheduledCalanderEndDate != null)
                {
                    scheduledEventList.Add(
                       new
                       {
                           id = "",
                           resourceId = eq.EquipmentId,
                           title = "On maintenance - " + eq.MaintenanceName,
                           description = eq.Remark,
                           start = eq.ScheduledCalanderStartDate,
                           end = eq.ScheduledCalanderEndDate,
                           allDay = false
                       });
                }
            }

            return Json(scheduledEventList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        /*Begin, methods for edit*/
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetFreeEquipment(int flyingFTDScheduleId, string date, string time)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time);//, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);

            List<EquipmentsView> EquipmentList = schedulerUtility.GetFreeEquipment(flyingFTDScheduleId, startingTime);
            return Json(new { resultData = EquipmentList, hasList = EquipmentList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetFreeEquipmentAndInstructors(int flyingFTDScheduleId, string date, string time)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time);//, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);

            List<EquipmentsViewForEdit> EquipmentInstructorList = schedulerUtility.GetFreeEquipmentAndInstructors(flyingFTDScheduleId, startingTime);
            return Json(new { resultData = EquipmentInstructorList, hasList = EquipmentInstructorList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFreeInstructors(int flyingFTDScheduleId, string date, string time)
        {

            SchedulerUtility schedulerUtility = new SchedulerUtility();
            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time);//, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);

           

            List<FTDandFlyingInstructorView> InstructorList = schedulerUtility.GetFTDandFlyingInstructors(flyingFTDScheduleId, date, time);
            return Json(new { resultData = InstructorList, hasList = InstructorList.Count > 0 }, JsonRequestBehavior.AllowGet);


          }
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetListOffEquipment()
        {
            EquipmentAccess EquipmentAccess = new EquipmentAccess();


            List<Equipment> EquipmentList = new List<Equipment>();

            EquipmentList = EquipmentAccess.List();
            return Json(new { resultData = EquipmentList, hasList = EquipmentList.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        /*
        [HttpPost]
        public JsonResult GetBatchTrainee(int lessonId, int equipmentId, string date, string time)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            time = " " + time;
            DateTime startingTime = DateTime.ParseExact(date + time, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);
            List<TraineeView> traineeViewList = schedulerUtility.GetBatchTrainee(lessonId, equipmentId, startingTime);

            return Json(new
            {
                resultData =
                traineeViewList.Select(trainee => new
                {
                    Id = trainee.TraineeId,
                    Name = trainee.TraineeNameAndCompanyId
                }).ToList(),
                hasList = traineeViewList.Count > 0
            }, JsonRequestBehavior.AllowGet);

        }
        */

        /// <summary>
        /// Get all trainees who are able to take any lesson to which they are assign for on that equipment. STEPS
        /// 1. Check whether the trainee has already a period in the mentioned date and time.
        /// 2. Check trainee has briefing and debriefing in the mentioned date and time. 
        /// 3. Check whether the trainee has already taken the mentioned lesson
        /// </summary>         
        /// <param name="equipmentId"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>      


        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetTraineeList(int equipmentId, string date, string time)
        {
            time = " " + time;
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            DateTime startingTime = Convert.ToDateTime(date + time);// DateTime.ParseExact(date + time, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);
            List<TraineeView> traineeViewList = schedulerUtility.GetTraineeList(equipmentId, startingTime);

            return Json(new
            {
                resultData =
                traineeViewList.Select(trainee => new
                {
                    Id = trainee.TraineeId,
                    Name = trainee.TraineeNameAndLesson
                }).ToList(),
                hasList = traineeViewList.Count > 0
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [PTSAuthorizeAttribute]
        public PartialViewResult TraineeEvaluationTemplatePartialView(int traineeId, int lessonId, int sequence, int flyingFTDScheduleId)
        {
            ActivityRampInLogic activityRampInLogic = new ActivityRampInLogic();
            ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();
            ActivityRampOutLogic activityRampOutLogic = new ActivityRampOutLogic();

            TraineeEvaluationTemplateLogic traineeEvaluationTemplateLogic = new TraineeEvaluationTemplateLogic();
            List<TraineeEvaluationTemplateView> TraineeEvaluationTemplateToView = new List<TraineeEvaluationTemplateView>();

            List<TraineeEvaluationTemplateView> TraineeEvaluationTemplate = traineeEvaluationTemplateLogic.EvaluationTemplateList(traineeId, lessonId, sequence);


            //Check weather Ramp In has done before starting evaluation template. 
            ViewBag.IsRampInDone = false;
            ActivityCheckIn activityCheckIn = activityCheckInLogic.CheckInDetailsByScheduleId(flyingFTDScheduleId);
            ActivityRampOut activityRampOut = new ActivityRampOut();
            ActivityRampIn activityRampIn = new ActivityRampIn();
            if (activityCheckIn != null)
            {
                activityRampOut = activityRampOutLogic.GetActivityRampOutDetailsByCheckInId(activityCheckIn.ActivityCheckInId);
                if (activityRampOut != null)
                {
                    activityRampIn = activityRampInLogic.GetActivityRampInDetailsByRampOutId(activityRampOut.ActivityRampOutId);
                    if (activityRampIn != null)
                        ViewBag.IsRampInDone = true;
                }
            }
            PTSContext db = new PTSContext();
            if (TraineeEvaluationTemplate[0].OverallGradeId != null && TraineeEvaluationTemplate[0].OverallGradeId > 0)
                ViewBag.OverallGradeId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName", TraineeEvaluationTemplate[0].OverallGradeId);
            else
                ViewBag.OverallGradeId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName");
            TraineeAccess tr = new TraineeAccess();

            SchedulerUtility schedulerUtility = new SchedulerUtility();
            var flyingFTDSchedule = schedulerUtility.Details(flyingFTDScheduleId);
            ViewBag.trainee = tr.Details(traineeId);
            ViewBag.flyingFTDSchedule = flyingFTDSchedule;
            var traineeLessonResult = db.TraineeLessons.Where(TL => TL.LessonId == lessonId && TL.TraineeId == traineeId).ToList();
            TraineeLesson traineeLesson = traineeLessonResult.FirstOrDefault();

          

            ViewBag.TimeIn = activityRampOut.AdjustedDepartureTime.Hour+":"+ activityRampOut.AdjustedDepartureTime.Minute;
            ViewBag.TimeOut = activityRampIn.AdjustedArrivalTime.Hour + ":" + activityRampIn.AdjustedArrivalTime.Minute;
            ViewBag.FlightTime = (activityRampIn.AdjustedArrivalTime- activityRampOut.AdjustedDepartureTime).Hours+":"+ ( activityRampIn.AdjustedArrivalTime- activityRampOut.AdjustedDepartureTime ).Minutes;
            ViewBag.FlightDate = activityRampOut.AdjustedDepartureTime;
           
            return PartialView("TraineeEvaluationTemplatePartialView", TraineeEvaluationTemplate);
        }

        /*End, methods for edit*/

        /*End, New Code */


        /*UNUSED METHODS*/
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult FTDAndFlyingScheduler()
        {
            if (TempData["FTDAndFlyingScheduleMessage"] != null)
            {
                ViewBag.FTDAndFlyingScheduleMessage = TempData["FTDAndFlyingScheduleMessage"];
                TempData["FTDAndFlyingScheduleMessage"] = null;
            }
            return View();
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetBatchClass()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<BatchClass> PhaseScheduleList = schedulerUtility.GetBatchClass();
            return Json(new
            {
                resultData = PhaseScheduleList.Select(item => new
                {
                    Id = item.BatchClassId,
                    Name = item.BatchClassName
                }),
                hasList = PhaseScheduleList.Count > 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult get_EquipmentType()
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<EquipmentType> EquipmentTypeList = schedulerUtility.get_EquipmentType();
            return Json(new
            {
                resultData = EquipmentTypeList.Select(item => new
                {
                    Id = item.EquipmentTypeId,
                    Name = item.EquipmentTypeName
                }),
                hasList = EquipmentTypeList.Count > 0
            },
            JsonRequestBehavior.AllowGet);
        }

        //CancelSchedule
        [HttpPost]
        [PTSAuthorizeAttribute]
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
            return RedirectToAction("EquipmentScheduler");
        }

        [HttpPost]
        public JsonResult GetBriefingAndDebriefing(string date, string time, int instructorId, int equipmentId, string traineeLesson)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time); //DateTime.ParseExact(date + time, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);

            string[] TraineeLessonIdArray = traineeLesson.Split('~');
            var result = schedulerUtility.GetBriefingAndDebriefing(startingTime, instructorId, equipmentId, TraineeLessonIdArray);

            return Json(new
            {
                resultData = result,
                hasList = result.Briefing.Count > 0 || result.Debriefing.Count > 0
            },
            JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetLeaveAndLicenceExpiry(string date, string time, int instructorId, int equipmentId, string traineeLesson)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time); //DateTime.ParseExact(date + time, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);

            string[] TraineeLessonIdArray = traineeLesson.Split('~');
            var result = schedulerUtility.GetLeaveAndLicenceExpiry(startingTime, instructorId, TraineeLessonIdArray);

            return Json(new
            {
                resultData = result
            },
            JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        // public JsonResult IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTime(string date, string time, int instructorId, string traineeLesson, int equipmentId, string briefingtime, string debriefingTime, int fTDAndFlyingScheduleId, string isReschedule, string isCustomBriefingTime, string isCustomDebriengTime, string briefingStartingTime, string debriefingStartTime)
        public JsonResult IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTime(string date,string time,int instructorId,string traineeLesson,
            int equipmentId,string briefingtime,string debriefingTime,int fTDAndFlyingScheduleId,string isReschedule,string isCustomBriefingTime,
            string isCustomDebriengTime,string briefingStartingTime,string debriefingStartTime,string EventApplyingType)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time); //DateTime.ParseExact(date + time, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);
            string[] TraineeLessonIdArray = traineeLesson.Split('~');

            var result = schedulerUtility.IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTime(EventApplyingType, date,startingTime, instructorId, TraineeLessonIdArray, equipmentId, briefingtime, debriefingTime, fTDAndFlyingScheduleId, Convert.ToBoolean(isReschedule), Convert.ToBoolean(isCustomBriefingTime), Convert.ToBoolean(isCustomDebriengTime), briefingStartingTime, debriefingStartTime);
            return Json(new { resultData = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        // public JsonResult IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTime(string date, string time, int instructorId, string traineeLesson, int equipmentId, string briefingtime, string debriefingTime, int fTDAndFlyingScheduleId, string isReschedule, string isCustomBriefingTime, string isCustomDebriengTime, string briefingStartingTime, string debriefingStartTime)
        public JsonResult IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTimeForUpdate(string date, string time, int instructorId, string traineeLesson,int fTDAndFlyingScheduleId, string EventApplyingType,bool isReschedule)
        {
            FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            time = " " + time;
            DateTime startingTime = Convert.ToDateTime(date + time); //DateTime.ParseExact(date + time, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);
            string[] TraineeLessonIdArray = traineeLesson.Split('~');
            FlyingFTDSchedule flyingFTDSchedule = fTDAndFlyingSchedulerAccess.Details(fTDAndFlyingScheduleId);

            EquipmentScheduleBriefingDebriefingAccess BREAccess = new EquipmentScheduleBriefingDebriefingAccess();

            EquipmentScheduleBriefingDebriefing Briefing = BREAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(fTDAndFlyingScheduleId,true,false);
            EquipmentScheduleBriefingDebriefing Debriefing = BREAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(fTDAndFlyingScheduleId,false,true);



            var result = schedulerUtility.IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTime(EventApplyingType, date, startingTime, instructorId, TraineeLessonIdArray, flyingFTDSchedule.EquipmentId, Briefing.BriefingAndDebriefingId.ToString(), Debriefing.BriefingAndDebriefingId.ToString(), fTDAndFlyingScheduleId, Convert.ToBoolean(isReschedule), Convert.ToBoolean(false), Convert.ToBoolean(false), Briefing.BriefingAndDebriefing.StartingTime.ToString(), Debriefing.BriefingAndDebriefing.StartingTime.ToString());
            return Json(new { resultData = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult IsTraineeInstructorAndEquipmentFee(string date, string startingTime, string endingTime, int flyingAndFTDScheduleId, int equipmentId)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            startingTime = " " + startingTime;
            endingTime = " " + endingTime;
            DateTime startingDateTime = Convert.ToDateTime(date + startingTime);// DateTime.ParseExact(date + startingTime, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);
            DateTime endingDateTime = Convert.ToDateTime(date + endingTime);// DateTime.ParseExact(date + endingTime, "MM/dd/yyyy HH:mm", CultureInfo.InstalledUICulture);

            var result = schedulerUtility.IsTraineeInstructorAndEquipmentFee(startingDateTime, endingDateTime, flyingAndFTDScheduleId, equipmentId);
            return Json(new { resultData = result }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// A method used to get all trainees registered in the selected batch class
        /// </summary>
        /// <param name="batchClassId"></param>       
        /// <returns></returns>

        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetBatchClassTrainees(int batchClassId, string lessonType)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<TraineeView> traineeViewList = schedulerUtility.GetBatchClassTrainees(batchClassId, lessonType);

            return Json(new
            {
                resultData =
                traineeViewList.Select(trainee => new
                {
                    Id = trainee.TraineeId,
                    Name = trainee.TraineeName,
                    NameAndLesson= trainee.TraineeNameAndLesson,
                    NearFutureLesson = trainee.NearestFutureLessonSequence
                }).ToList(),
                hasList = traineeViewList.Count > 0
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult GetBatchClassTraineesForEdit(int batchClassId, int ScheduleID)
        {
            FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
            FlyingFTDSchedule flyingFTDSchedule = fTDAndFlyingSchedulerAccess.Details(ScheduleID);
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<TraineeView> traineeViewListToReturn = new List<TraineeView>();
            string lessonType = flyingFTDSchedule.Lesson.EquipmentType.EquipmentTypeName;
            if (flyingFTDSchedule.Status == "New")
            {
                List<TraineeView> traineeViewList = schedulerUtility.GetBatchClassTraineesForTraineeUpdate(batchClassId, lessonType);
                TraineeAccess traineeAccess = new TraineeAccess();
                foreach (TraineeView tv in traineeViewList)
                {
                    string[] TraineeLessonIdPair = tv.TraineeId.ToString().Split('-');
                    flyingFTDSchedule.TraineeId = tv.TraineeId;
                    flyingFTDSchedule.Trainee = traineeAccess.Details(flyingFTDSchedule.TraineeId);
                    if (schedulerUtility.IsTraineeFree(flyingFTDSchedule))
                        traineeViewListToReturn.Add(tv);
                }

                return Json(new
                {
                    resultData =
                    traineeViewListToReturn.Select(trainee => new
                    {
                        Id = trainee.TraineeId,
                        Name = trainee.TraineeNameAndLesson,
                        NearFutureLesson = trainee.NearestFutureLessonSequence
                    }).ToList(),
                    hasList = traineeViewList.Count > 0
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                resultData = "not-new",
                hasList = false
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult UpdateTrainee(int ScheduleID, int traineeID)
        {
            FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
            TraineeAccess traineeAccess = new TraineeAccess();
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            FlyingFTDSchedule flyingFTDSchedule = fTDAndFlyingSchedulerAccess.Details(Convert.ToInt32(ScheduleID));
            flyingFTDSchedule.Trainee = traineeAccess.Details(Convert.ToInt32(traineeID));
            flyingFTDSchedule.TraineeId = Convert.ToInt32(traineeID);

            TempData["FTDAndFlyingScheduleMessage"] = schedulerUtility.UpdateTraineeLessonSchedule(flyingFTDSchedule);
            var events = schedulerUtility.GetFTDandFlyingScheduledEvent(flyingFTDSchedule.FlyingFTDScheduleId);
            return Json(new
            {
                message = TempData["FTDAndFlyingScheduleMessage"],
                resultData = events,
                hasList = true
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PTSAuthorizeAttribute]
        public JsonResult UpdateLesson(int ScheduleID, int LessonID)
        {
            FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
            LessonAccess lessonAccess = new LessonAccess();
            SchedulerUtility schedulerUtility = new SchedulerUtility();

            FlyingFTDSchedule flyingFTDSchedule = fTDAndFlyingSchedulerAccess.Details(Convert.ToInt32(ScheduleID));
            flyingFTDSchedule.Lesson = lessonAccess.DetailsLesson(LessonID);
            flyingFTDSchedule.LessonId = LessonID;

            TempData["FTDAndFlyingScheduleMessage"] = schedulerUtility.UpdateTraineeLessonSchedule(flyingFTDSchedule);
            var events = schedulerUtility.GetFTDandFlyingScheduledEvent(flyingFTDSchedule.FlyingFTDScheduleId);
            return Json(new
            {
                message = TempData["FTDAndFlyingScheduleMessage"],
                resultData = events,
                hasList = true
            }, JsonRequestBehavior.AllowGet);
        }

    }
}