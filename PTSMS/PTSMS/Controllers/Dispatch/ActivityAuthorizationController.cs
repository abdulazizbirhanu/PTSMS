using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using PTSMSBAL.Dispatch;
using PTSMSBAL.Scheduling.Others;
using PTSMSDAL.Access.Dispatch;
using PTSMSDAL.Models.Dispatch;
using PTSMSDAL.Models.Scheduling.Relations;

namespace PTSMS.Controllers.Dispatch
{
    public class ActivityAuthorizationController : Controller
    {
        ActivityCheckInAccess activityCheckInAccess = new ActivityCheckInAccess();
        ActivityAuthorizationLogic activityAuthorizationLogic = new ActivityAuthorizationLogic();
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

        // GET: ActivityAuthorization
        public ActionResult Index()
        {
            return View(activityAuthorizationLogic.List());
        }

        // GET: ActivityAuthorization/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityAuthorization activityAuthorization = activityAuthorizationLogic.Details((int)id);
            if (activityAuthorization == null)
            {
                return HttpNotFound();
            }
            return View(activityAuthorization);
        }

        // GET: ActivityAuthorization/Create
        public ActionResult Create()
        {
            ViewBag.ActivityCheckinId = new SelectList(activityCheckInAccess.List(), "ActivityCheckInId", "Comments");
            return View();
        }

        [HttpGet]
        public PartialViewResult CreateAuthorization(int flyingFTDScheduleId)
        {
            ActivityCheckIn activityCheckIn = activityCheckInLogic.CheckInDetailsByScheduleId(flyingFTDScheduleId);
            ActivityAuthorization activityAuthorization = new ActivityAuthorization();
            ViewBag.IsCheckedIn = false;
            ViewBag.IsFlyingLesson = false;
            //Check whether the lesson is FLYING or FTD
            FlyingFTDSchedule flyingFTDSchedule = schedulerUtility.Details(flyingFTDScheduleId);

            if (flyingFTDSchedule != null)
            {
                if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    ViewBag.IsFlyingLesson = false;
                else if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    ViewBag.IsFlyingLesson = true;

                if (activityCheckIn != null)
                {
                    activityAuthorization = activityAuthorizationLogic.GetActivityAuthorizationDetailsByCheckInId(activityCheckIn.ActivityCheckInId);
                    if (activityAuthorization == null)
                    {
                        activityAuthorization = new ActivityAuthorization();
                        activityAuthorization.ActivityCheckinId = activityCheckIn.ActivityCheckInId;
                    }
                    ViewBag.ActivityCheckIn = activityCheckIn;

                    if (activityCheckIn.CheckInStatus.CheckInStatusName.ToUpper() == "CHECK-IN")
                        ViewBag.IsCheckedIn = true;
                    else
                        ViewBag.IsCheckedIn = false;
                }
                else
                {
                    activityCheckIn = new ActivityCheckIn();
                }
            }
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Value = "",
                Text = "--Select Status--",
                Selected = false
            });

            foreach (ActivityAuthorizationStatus status in Enum.GetValues(typeof(ActivityAuthorizationStatus)))
            {
                items.Add(new SelectListItem
                {
                    Value = status.ToString(),
                    Text = status.ToString()
                });
            }
            ViewBag.Status = items;

            return PartialView("CreateAuthorization", activityAuthorization);
        }

        // POST: ActivityAuthorization/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        public JsonResult CreateJson(int ActivityCheckinId, int ActivityAuthorizationId,string Status,string Remark)
        {
            bool isSuccess = false;
            ActivityAuthorization activityAuthorization =new ActivityAuthorization();
            if(ActivityAuthorizationId!=0)
                activityAuthorization = activityAuthorizationLogic.Details(ActivityAuthorizationId);
            activityAuthorization.ActivityCheckinId = ActivityCheckinId;
            activityAuthorization.Status = Status;
            activityAuthorization.Remark = Remark;
            //if (HttpContext.User.Identity.Name != null)
            //{
            //    string password = Request.Form["txtActivityAuthorizationPassword"];

            //    var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

            //    if (userValidationResult.Result.ToString() == "Success")
            //    {
                    if (activityAuthorizationLogic.Add(activityAuthorization))
                    {
                        TempData["FTDAndFlyingScheduleMessage"] = "You have made a successful authorization.";
                        isSuccess = true;
                    }
                    else
                    {
                        TempData["FTDAndFlyingScheduleMessage"] = "You have got failure while making authorization.";
                        isSuccess = false;
                    }
            //    }
            //    else
            //    {
            //        TempData["FTDAndFlyingScheduleMessage"] = "Failed to authorization due to incorrect password.";
            //        isSuccess = false;
            //    }
            //}
            //else
            //{
            //    TempData["FTDAndFlyingScheduleMessage"] = "The password you entered is not the password of the active user.";
            //    isSuccess = false;
            //}
            return Json(new { isSuccess = isSuccess, Message = TempData["FTDAndFlyingScheduleMessage"].ToString() }, JsonRequestBehavior.AllowGet);
        }
        // GET: ActivityAuthorization/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityAuthorization activityAuthorization = activityAuthorizationLogic.Details((int)id);
            if (activityAuthorization == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityCheckinId = new SelectList(activityCheckInAccess.List(), "ActivityCheckInId", "Comments", activityAuthorization.ActivityCheckinId);
            return View(activityAuthorization);
        }

        // POST: ActivityAuthorization/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityAuthorizationId,ActivityCheckinId,Remark,Status")] ActivityAuthorization activityAuthorization)
        {
            if (ModelState.IsValid)
            {
                activityAuthorizationLogic.Revise(activityAuthorization);
                return RedirectToAction("Index");
            }
            ViewBag.ActivityCheckinId = new SelectList(activityCheckInAccess.List(), "ActivityCheckInId", "Comments", activityAuthorization.ActivityCheckinId);
            return View(activityAuthorization);
        }

        // GET: ActivityAuthorization/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityAuthorization activityAuthorization = activityAuthorizationLogic.Details((int)id);
            if (activityAuthorization == null)
            {
                return HttpNotFound();
            }
            return View(activityAuthorization);
        }

        // POST: ActivityAuthorization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            activityAuthorizationLogic.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
