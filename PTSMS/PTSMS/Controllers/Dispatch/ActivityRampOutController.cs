using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch;
using PTSMSBAL.Dispatch;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSBAL.Scheduling.Others;
using PTSMSDAL.Models.Dispatch.Master;
using System.Collections.Generic;

namespace PTSMS.Controllers.Dispatch
{
    public class ActivityRampOutController : Controller
    {
        private PTSContext db = new PTSContext();
        ActivityRampOutLogic activityRampOutLogic = new ActivityRampOutLogic();
        ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();
        SchedulerUtility schedulerUtility = new SchedulerUtility();
        ActivityAuthorizationLogic activityAuthorizationLogic = new ActivityAuthorizationLogic();
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
        // GET: ActivityRampOut
        public ActionResult Index()
        {
            return View(activityRampOutLogic.List());
        }

        // GET: ActivityRampOut/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityRampOut activityRampOut = activityRampOutLogic.Details((int)id);
            if (activityRampOut == null)
            {
                return HttpNotFound();
            }
            return View(activityRampOut);
        }

        // GET: ActivityRampOut/Create
        public ActionResult Create()
        {
            ViewBag.ActivityCheckinId = new SelectList(activityCheckInLogic.List(), "ActivityCheckInId", "Comments");
            return View();
        }


        [HttpGet]
        public PartialViewResult CreateRampOut(int flyingFTDScheduleId)
        {
            ActivityCheckIn activityCheckIn = activityCheckInLogic.CheckInDetailsByScheduleId(flyingFTDScheduleId);
            ActivityRampOut activityRampOut = new ActivityRampOut();
            ActivityRampOut previousActivityRampOut = new ActivityRampOut();
            ActivityRampIn previousActivityRampIn = new ActivityRampIn();
            ViewBag.IsFlyingLesson = false;
            FlyingFTDSchedule flyingFTDSchedule = schedulerUtility.Details(flyingFTDScheduleId);
            if (flyingFTDSchedule != null)
            {
                //Check whether the lesson is FLYING or FTD
                if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    ViewBag.IsFlyingLesson = false;
                else if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    ViewBag.IsFlyingLesson = true;

                List<DepartureTimeReason> departureTimeReasonList = db.DepartureTimeReasons.ToList();
                departureTimeReasonList.Add(new DepartureTimeReason { DepartureTimeReasonId = 0, DepartureTimeReasonName = "-- Select Reason --" });

                if (activityCheckIn != null)
                {
                    ActivityAuthorization activityAuthorization = activityAuthorizationLogic.GetActivityAuthorizationDetailsByCheckInId(activityCheckIn.ActivityCheckInId);
                    if (activityAuthorization == null)
                        ViewBag.AutorizationMessage = "No authorization has done so far";
                    else
                    {
                        if (activityAuthorization.Status != Enum.GetName(typeof(ActivityAuthorizationStatus), ActivityAuthorizationStatus.Authorized))
                        {
                            ViewBag.AutorizationMessage = "It is not ready for ramp out becuase activity authorization is not made so far.";
                        }
                    }
                    //Get last Ramp In of an Equipment, so that it will be the next minimum ramp-out time
                    ActivityRampInLogic activityRampInLogic = new ActivityRampInLogic();
                    ActivityRampIn lastActivityRampIn = activityRampInLogic.GetLastActivityRampInDetailsByEquipmentId(flyingFTDSchedule.EquipmentId);
                    if (lastActivityRampIn != null)
                    {
                        ViewBag.LastEquipmentRampInDate = lastActivityRampIn.AdjustedArrivalTime;
                    }

                    activityRampOut = activityRampOutLogic.GetActivityRampOutDetailsByCheckInId(activityCheckIn.ActivityCheckInId);
                    //previousActivityRampOut = activityRampOutLogic.GetActivityRampOutDetailsByEquipmentId(flyingFTDSchedule.EquipmentId);                
                    if (activityRampOut == null)
                    {
                        activityRampOut = new ActivityRampOut();
                        activityRampOut.ActivityCheckinId = activityCheckIn.ActivityCheckInId;
                        activityRampOut.AdjustedDepartureTime = flyingFTDSchedule.ScheduleStartTime;
                        ViewBag.AdjustedDepartureTime = flyingFTDSchedule.ScheduleStartTime;
                        ViewBag.AdjustedReasonId = new SelectList(departureTimeReasonList.OrderBy(x => x.DepartureTimeReasonId).Select(item => new
                        {
                            DepartureTimeReasonId = item.DepartureTimeReasonId > 0 ? item.DepartureTimeReasonId.ToString() : "",
                            DepartureTimeReasonName = item.DepartureTimeReasonName
                        }), "DepartureTimeReasonId", "DepartureTimeReasonName");

                        //Assign the prevous ramp-in Hobbs
                        if (lastActivityRampIn != null)
                        {
                            activityRampOut.Hobbs = lastActivityRampIn.Hobbs;
                        }
                    }
                    else
                    {
                        ViewBag.AdjustedDepartureTime = activityRampOut.AdjustedDepartureTime;
                        ViewBag.AdjustedReason = activityRampOut.AdjustedReasonId;
                        ViewBag.AdjustedReasonId = new SelectList(departureTimeReasonList.OrderBy(x => x.DepartureTimeReasonId).Select(item => new
                        {
                            DepartureTimeReasonId = item.DepartureTimeReasonId > 0 ? item.DepartureTimeReasonId.ToString() : "",
                            DepartureTimeReasonName = item.DepartureTimeReasonName
                        }), "DepartureTimeReasonId", "DepartureTimeReasonName", activityRampOut.AdjustedReasonId);
                    }
                }
                else
                {
                    ViewBag.AdjustedReasonId = new SelectList(departureTimeReasonList.OrderBy(x => x.DepartureTimeReasonId).Select(item => new
                    {
                        DepartureTimeReasonId = item.DepartureTimeReasonId > 0 ? item.DepartureTimeReasonId.ToString() : "",
                        DepartureTimeReasonName = item.DepartureTimeReasonName
                    }), "DepartureTimeReasonId", "DepartureTimeReasonName");
                }

            }


            return PartialView("CreateRampOut", activityRampOut);
        }

        // POST: ActivityRampOut/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ActivityRampOut activityRampOut)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string password = Request.Form["txtActivityRampOutPassword"];

                var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

                if (userValidationResult.Result.ToString() == "Success")
                {
                    if (ModelState.IsValid)
                    {
                        if (activityRampOutLogic.Add(activityRampOut))
                            TempData["FTDAndFlyingScheduleMessage"] = "You have made a successful ramp out.";
                        else
                            TempData["FTDAndFlyingScheduleMessage"] = "You have got failure while making ramp out.";
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
        public JsonResult CreateJson(int ActivityCheckinId, int ActivityRampOutId,int Hobbs, string AdjustedDepartureTime)
        {
            bool isSuccess = false;
            ActivityRampOut activityRampOut = new ActivityRampOut();
            if(ActivityRampOutId!=0)
                activityRampOut= activityRampOutLogic.Details(ActivityRampOutId);
            activityRampOut.ActivityCheckinId = ActivityCheckinId;
            activityRampOut.Hobbs = Hobbs;
            if (!string.IsNullOrEmpty(AdjustedDepartureTime))
            activityRampOut.AdjustedDepartureTime = Convert.ToDateTime(AdjustedDepartureTime);
            //if (HttpContext.User.Identity.Name != null)
            //{
            //    string password = Request.Form["txtActivityRampOutPassword"];

            //    var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

            //    if (userValidationResult.Result.ToString() == "Success")
            //    {
            //        if (ModelState.IsValid)
            //        {
            if (activityRampOutLogic.Add(activityRampOut))
            {
                TempData["FTDAndFlyingScheduleMessage"] = "You have made a successful ramp out.";
                isSuccess = true;
            }
            else
            {
                TempData["FTDAndFlyingScheduleMessage"] = "You have got failure while making ramp out.";
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
        // GET: ActivityRampOut/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityRampOut activityRampOut = activityRampOutLogic.Details((int)id);
            if (activityRampOut == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityCheckinId = new SelectList(activityCheckInLogic.List(), "ActivityCheckInId", "Comments", activityRampOut.ActivityCheckinId);
            return View(activityRampOut);
        }

        // POST: ActivityRampOut/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityRampOutId,ActivityCheckinId,Hobbs,Tach,AdjustedDepartureTime,AdjustedReasonId")] ActivityRampOut activityRampOut)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activityRampOut).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActivityCheckinId = new SelectList(activityCheckInLogic.List(), "ActivityCheckInId", "Comments", activityRampOut.ActivityCheckinId);
            return View(activityRampOut);
        }

        // GET: ActivityRampOut/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityRampOut activityRampOut = activityRampOutLogic.Details((int)id);
            if (activityRampOut == null)
            {
                return HttpNotFound();
            }
            return View(activityRampOut);
        }

        // POST: ActivityRampOut/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            activityRampOutLogic.Delete(id);
            return RedirectToAction("Index");
        }

    }
}
