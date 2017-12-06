using Microsoft.AspNet.Identity.Owin;
using PTSMSBAL.Dispatch;
using PTSMSBAL.Scheduling.Relations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch;
using PTSMSDAL.Models.Scheduling.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Access.Scheduling.Operations;

namespace PTSMS.Controllers.Grading
{
    public class TraineeEvaluationTemplateController : Controller
    {
        private ApplicationSignInManager _signInManager;

  

        [HttpPost]
        public JsonResult SaveTraineeEvaluationTemplateItems(string evaluationTemplateItems,int overAllGradeId,string traineeId,string lessonId,string remark,int fylingFTDScheduleId,string TimeIn,string TimeOut,string FlightTime,string FlightDate)
        {
            OperationResult result = new OperationResult();
            if (!(String.IsNullOrEmpty(evaluationTemplateItems) || String.IsNullOrWhiteSpace(evaluationTemplateItems) || overAllGradeId == 0 || String.IsNullOrWhiteSpace(traineeId) || String.IsNullOrWhiteSpace(lessonId) || fylingFTDScheduleId < 1))
            {
                TraineeEvaluationTemplateLogic traineeEvaluationTemplateLogic = new TraineeEvaluationTemplateLogic();
                string[] templateItemIdsArray = evaluationTemplateItems.Split('~');
                result = traineeEvaluationTemplateLogic.SaveTraineeEvaluationTemplateItems(templateItemIdsArray, overAllGradeId, Convert.ToInt32(traineeId), Convert.ToInt32(lessonId), remark, fylingFTDScheduleId, TimeIn, TimeOut, FlightTime, FlightDate);
                TempData["FTDAndFlyingScheduleMessage"] = result.Message;
            }
            return Json(new
            {
                message = TempData["FTDAndFlyingScheduleMessage"],
                IsSuccess= result.IsSuccess,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsTraineeLessonHasEvaluationItem(int traineeId, int lessonId, int sequence, int flyingFTDScheduleId)
        {
            TraineeEvaluationTemplateLogic traineeEvaluationTemplateLogic = new TraineeEvaluationTemplateLogic();


            var IsThereEvaluationItem = traineeEvaluationTemplateLogic.IsTraineeLessonHasEvaluationItem(traineeId, lessonId, sequence);
            var IsAlreadyEvaluated = traineeEvaluationTemplateLogic.IsAlreadyEvaluated(traineeId, lessonId, sequence);

            //start, Check weather Ramp In has done before starting evaluation template. 
            ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();
            ActivityRampOutLogic activityRampOutLogic = new ActivityRampOutLogic();
            ActivityRampInLogic activityRampInLogic = new ActivityRampInLogic();
            bool IsRampInDone = false;
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
                        IsRampInDone = true;
                }
                else
                {
                    IsRampInDone = false;
                }
            }
            
            //end, Check weather Ramp In has done before starting evaluation template. 
            return Json(new { IsThereEvaluationItem = IsThereEvaluationItem, IsAlreadyEvaluated = IsAlreadyEvaluated, IsRampInDone = IsRampInDone }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PopulateEvaluationItemForTraineeLesson(int traineeId, int lessonId, int sequence)
        {
            TraineeEvaluationTemplateLogic traineeEvaluationTemplateLogic = new TraineeEvaluationTemplateLogic();
            var IsEvaluationItemGenerated = traineeEvaluationTemplateLogic.PopulateEvaluationItemForTraineeLesson(traineeId, lessonId, sequence);
            return Json(new { IsEvaluationItemGenerated = IsEvaluationItemGenerated }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult TraineeEvaluationTemplate(int traineeId, int lessonId, int sequence)
        {
            //GetTraineeEvaluationTemplate
            TraineeEvaluationTemplateLogic traineeEvaluationTemplateLogic = new TraineeEvaluationTemplateLogic();

            List<TraineeEvaluationTemplateView> TraineeEvaluationTemplate = traineeEvaluationTemplateLogic.EvaluationTemplateList(traineeId, lessonId, sequence);
            //return Json(new { resultData = TraineeEvaluationTemplate, hasList = TraineeEvaluationTemplate.Id != 0 }, JsonRequestBehavior.AllowGet);

            PTSContext db = new PTSContext();
            if (TraineeEvaluationTemplate[0].OverallGradeId != null && TraineeEvaluationTemplate[0].OverallGradeId > 0)
                ViewBag.OverallGradeId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName", TraineeEvaluationTemplate[0].OverallGradeId);
            else
                ViewBag.OverallGradeId = new SelectList(db.OverallGrades, "OverallGradeId", "OverallGradeName");

            return PartialView("TraineeEvaluationTemplate", TraineeEvaluationTemplate);
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

        [HttpGet]
        public ActionResult AcceptEvaluationTemplate(string traineeId, string lessonId, string password, string isAccepted, int flyingFTDScheduleId)
        {
            if (!(String.IsNullOrWhiteSpace(traineeId) || String.IsNullOrWhiteSpace(lessonId) || String.IsNullOrWhiteSpace(isAccepted) || flyingFTDScheduleId < 1))
            {
                TraineeEvaluationTemplateLogic traineeEvaluationTemplateLogic = new TraineeEvaluationTemplateLogic();

                if (HttpContext.User.Identity.Name != null)
                {
                    var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

                    if (userValidationResult.Result.ToString() == "Success")
                    {
                        string result = traineeEvaluationTemplateLogic.AcceptEvaluationTemplate(Convert.ToInt32(traineeId), Convert.ToInt32(lessonId), Boolean.Parse(isAccepted), flyingFTDScheduleId);
                        TempData["FTDAndFlyingScheduleMessage"] = result;
                    }
                    else
                    {
                        TempData["FTDAndFlyingScheduleMessage"] = "Failed to save your agreement due to incorrect password.";
                    }
                }
                else
                {
                    TempData["FTDAndFlyingScheduleMessage"] = "The password you entered is no the password of the active user.";
                }
                return RedirectToAction("Equipment", "StudentSchedule");
            }
            TempData["FTDAndFlyingScheduleMessage"] = "Invalid input.";
            return RedirectToAction("EquipmentScheduler", "Scheduler");
        }
    }
}