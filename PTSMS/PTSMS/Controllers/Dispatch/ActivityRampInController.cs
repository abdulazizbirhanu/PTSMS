using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch;
using PTSMSBAL.Dispatch;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Collections.Generic;
using PTSMSDAL.Models.Dispatch.Master;
using System.Linq;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSBAL.Scheduling.Others;
using System;

namespace PTSMS.Controllers.Dispatch
{
    public class ActivityRampInController : Controller
    {
        private PTSContext db = new PTSContext();
        ActivityRampInLogic activityRampInLogic = new ActivityRampInLogic();
        ActivityRampOutLogic activityRampOutLogic = new ActivityRampOutLogic();
        ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();
        SchedulerUtility schedulerUtility = new SchedulerUtility();
        private ApplicationSignInManager _signInManager;

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

        // GET: ActivityRampIn
        public ActionResult Index()
        {
            return View(activityRampInLogic.List());
        }

        // GET: ActivityRampIn/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityRampIn activityRampIn = activityRampInLogic.Details((int)id);
            if (activityRampIn == null)
            {
                return HttpNotFound();
            }
            return View(activityRampIn);
        }

        // GET: ActivityRampIn/Create
        public ActionResult Create()
        {
            ViewBag.ActivityRampOutId = new SelectList(activityRampOutLogic.List(), "ActivityRampOutId", "ActivityRampOutId");
            return View();
        }

        [HttpGet]
        public PartialViewResult CreateRampIn(int flyingFTDScheduleId)
        {
            ActivityCheckIn activityCheckIn = activityCheckInLogic.CheckInDetailsByScheduleId(flyingFTDScheduleId);
            ActivityRampOut activityRampOut = new ActivityRampOut();
            ActivityRampIn activityRampIn = new ActivityRampIn();
            FlyingFTDSchedule flyingFTDSchedule = schedulerUtility.Details(flyingFTDScheduleId);
            ViewBag.IsFlyingLesson = false;
            if (flyingFTDSchedule != null)
            {
                //Check whether the lesson is Flying or FTD
                if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    ViewBag.IsFlyingLesson = false;
                else if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    ViewBag.IsFlyingLesson = true;

                List<ArrivalTimeReason> arrivalTimeReasonList = db.ArrivalTimeReasons.ToList();
                arrivalTimeReasonList.Add(new ArrivalTimeReason { ArrivalTimeReasonId = 0, ArrivalTimeReasonName = "-- Select Reason --" });

                bool isArrivalTimeReasonAssigned = false;
                if (activityCheckIn != null)
                {
                    //Get last activity ramp-in
                    ActivityRampIn lastActivityRampIn = activityRampInLogic.GetLastActivityRampInDetailsByEquipmentId(flyingFTDSchedule.EquipmentId);

                    activityRampOut = activityRampOutLogic.GetActivityRampOutDetailsByCheckInId(activityCheckIn.ActivityCheckInId);
                    if (activityRampOut != null)
                    {

                        activityRampIn = activityRampInLogic.GetActivityRampInDetailsByRampOutId(activityRampOut.ActivityRampOutId);
                        if (activityRampIn == null)
                        {
                            activityRampIn = new ActivityRampIn();
                            activityRampIn.ActivityRampOutId = activityRampOut.ActivityRampOutId;
                            activityRampIn.AdjustedArrivalTime = flyingFTDSchedule.ScheduleEndTime;
                            ViewBag.AdjustedArrivalTime = activityRampIn.AdjustedArrivalTime;
                            ViewBag.ScheduleEndTime = flyingFTDSchedule.ScheduleEndTime;

                            isArrivalTimeReasonAssigned = true;
                            ViewBag.ArrivalTimeReasonId = new SelectList(arrivalTimeReasonList.OrderBy(x => x.ArrivalTimeReasonId).Select(item => new
                            {
                                ArrivalTimeReasonId = item.ArrivalTimeReasonId > 0 ? item.ArrivalTimeReasonId.ToString() : "",
                                ArrivalTimeReasonName = item.ArrivalTimeReasonName
                            }), "ArrivalTimeReasonId", "ArrivalTimeReasonName", activityRampIn.ArrivalTimeReasonId);
                            if (lastActivityRampIn != null)
                            {
                                activityRampIn.Hobbs = lastActivityRampIn.Hobbs;
                            }
                        }
                        else
                        {
                            ViewBag.AdjustedArrivalTime = activityRampIn.AdjustedArrivalTime;
                            ViewBag.ArrivalTimeReason = activityRampIn.ArrivalTimeReasonId;
                        }
                    }
                }
                if (!isArrivalTimeReasonAssigned)
                {
                    ViewBag.ArrivalTimeReasonId = new SelectList(arrivalTimeReasonList.OrderBy(x => x.ArrivalTimeReasonId).Select(item => new
                    {
                        ArrivalTimeReasonId = item.ArrivalTimeReasonId > 0 ? item.ArrivalTimeReasonId.ToString() : "",
                        ArrivalTimeReasonName = item.ArrivalTimeReasonName
                    }), "ArrivalTimeReasonId", "ArrivalTimeReasonName");
                }
            }
            return PartialView("CreateRampIn", activityRampIn);
        }

        // POST: ActivityRampIn/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ActivityRampIn activityRampIn)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string password = Request.Form["txtActivityRampInPassword"];

                var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

                if (userValidationResult.Result.ToString() == "Success")
                {
                    if (ModelState.IsValid)
                    {
                        if (activityRampInLogic.Add(activityRampIn))
                            TempData["FTDAndFlyingScheduleMessage"] = "You have made a successful ramp In.";
                        else
                            TempData["FTDAndFlyingScheduleMessage"] = "You have got failure while making Ramp In.";
                    }
                }
                else
                {
                    TempData["FTDAndFlyingScheduleMessage"] = "Failed to ramp in due to incorrect password.";
                }
            }
            else
            {
                TempData["FTDAndFlyingScheduleMessage"] = "The password you entered is not the password of the active user.";
            }
            return RedirectToAction("EquipmentScheduler", "Scheduler", new { });
        }
        [HttpPost]
        public JsonResult CreateJson(int ActivityRampOutId,int ActivityRampInId,int Hobbs,string AdjustedArrivalTime)
        {
            bool isSuccess = false;
            ActivityRampIn activityRampIn = new ActivityRampIn();
            if(ActivityRampInId!=0)
            activityRampIn=activityRampInLogic.Details(ActivityRampInId);
            activityRampIn.ActivityRampOutId = ActivityRampOutId;
            activityRampIn.Hobbs = Hobbs;
            if(!string.IsNullOrEmpty(AdjustedArrivalTime))
            activityRampIn.AdjustedArrivalTime = Convert.ToDateTime(AdjustedArrivalTime);

            //if (HttpContext.User.Identity.Name != null)
            //{
            //    string password = Request.Form["txtActivityRampInPassword"];

            //    var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

            //    if (userValidationResult.Result.ToString() == "Success")
            //    {
            //        if (ModelState.IsValid)
            //        {
            if (activityRampInLogic.Add(activityRampIn))
            {
                TempData["FTDAndFlyingScheduleMessage"] = "You have made a successful ramp In.";
                isSuccess = true;
            }
            else
            {
                TempData["FTDAndFlyingScheduleMessage"] = "You have got failure while making Ramp In.";
                isSuccess = false;
            }
            //        }
            //    }
            //    else
            //    {
            //        TempData["FTDAndFlyingScheduleMessage"] = "Failed to ramp in due to incorrect password.";
            //    }
            //}
            //else
            //{
            //    TempData["FTDAndFlyingScheduleMessage"] = "The password you entered is not the password of the active user.";
            //}
            return Json(new { isSuccess = isSuccess, Message = TempData["FTDAndFlyingScheduleMessage"].ToString() }, JsonRequestBehavior.AllowGet);
        }
        // GET: ActivityRampIn/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityRampIn activityRampIn = activityRampInLogic.Details((int)id);
            if (activityRampIn == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityRampOutId = new SelectList(activityRampOutLogic.List(), "ActivityRampOutId", "ActivityRampOutId", activityRampIn.ActivityRampOutId);
            return View(activityRampIn);
        }

        // POST: ActivityRampIn/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityRampInId,ActivityRampOutId,Hobbs,Tach")] ActivityRampIn activityRampIn)
        {
            if (ModelState.IsValid)
            {
                activityRampInLogic.Revise(activityRampIn);
                return RedirectToAction("Index");
            }
            ViewBag.ActivityRampOutId = new SelectList(activityRampOutLogic.List(), "ActivityRampOutId", "ActivityRampOutId", activityRampIn.ActivityRampOutId);
            return View(activityRampIn);
        }

        // GET: ActivityRampIn/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityRampIn activityRampIn = activityRampInLogic.Details((int)id);
            if (activityRampIn == null)
            {
                return HttpNotFound();
            }
            return View(activityRampIn);
        }

        // POST: ActivityRampIn/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            activityRampInLogic.Delete(id);
            return RedirectToAction("Index");
        }

    }
}
