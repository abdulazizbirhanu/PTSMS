using PTSMSBAL.Others;
using PTSMSDAL.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Text;
using System.Collections.Specialized;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMS.Models;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Others.Messaging;

namespace PTSMS.Controllers.Others
{
    public class NotificationController : Controller
    {
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

        // GET: Notification
        /// <summary>
        /// This method is used to send schedule to concerned bodies scheduled at the selected date range.
        /// </summary>
        /// <returns></returns>
        public ActionResult ScheduleNotification()
        {
            if (TempData["NotificationMessage"] != null)
            {
                ViewBag.NotificationMessage = TempData["NotificationMessage"];
                TempData["NotificationMessage"] = null;
            }
            return View();
        }

        public PartialViewResult GetConcernedBodies(string lessonType, string dateRange)
        {
            if (!(String.IsNullOrEmpty(lessonType) || String.IsNullOrWhiteSpace(dateRange)))
            {
                NotificationLogic notificationLogic = new NotificationLogic();

                string[] takenDateArray = dateRange.Split('-');

                DateTime startAt = Convert.ToDateTime(takenDateArray[0]);
                DateTime endAt = Convert.ToDateTime(takenDateArray[1]);
                var notifications = notificationLogic.GetConcernedBodies(lessonType, startAt, endAt);
                return PartialView(notifications);
            }
            return PartialView("", new List<NotificationView>());
        }

        [HttpGet]
        public ActionResult SendScheduleNotification(string recipientList, string password)
        {
            if (!String.IsNullOrEmpty(recipientList) && !String.IsNullOrEmpty(password))
            {
                NotificationLogic notificationLogic = new NotificationLogic();
                if (HttpContext.User.Identity.Name != null)
                {
                    var userValidationResult = SignInManager.PasswordSignInAsync(HttpContext.User.Identity.Name, password, false, shouldLockout: false);

                    //if (userValidationResult.Result.ToString() == "Success")
                    //{
                        string[] recipientArray = recipientList.Split('~');

                        OperationResult result = notificationLogic.SendScheduleNotification(recipientArray, HttpContext.User.Identity.Name);

                        TempData["NotificationMessage"] = result.Message;
                        return RedirectToAction("ScheduleNotification");
                    //}
                    //else
                    //{
                    //    TempData["NotificationMessage"] = "Failed to send notification due to incorrect password.";
                   // }
                }
                else
                {
                    TempData["NotificationMessage"] = "The password you entered is no the password of the active user.";
                }
            }
            TempData["NotificationMessage"] = "Invalid input.";
            return RedirectToAction("ScheduleNotification");
        }


        [HttpPost]
        public JsonResult NotificationHasSeen()
        {
            NotificationLogic notificationLogic = new NotificationLogic();
            bool isSuccess = false;
            if (HttpContext.User.Identity.Name != null)
            {
                isSuccess = notificationLogic.NotificationHasSeen(HttpContext.User.Identity.Name);
            }
            return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }







        /*
        public Dictionary<string, string> GetReciever(string[] recipientArray)
        {
            try
            {
                 //Get reciever User Id
                foreach (var recipient in recipientArray)
                {
                    if (!(String.IsNullOrEmpty(recipient)))
                    {
                        string[] recipientDetail = recipient.Split('-');
                        string personId = recipientDetail[4];

                        if (!string.IsNullOrEmpty(personId))
                        {
                            PersonLogic personLogic = new PersonLogic();
                            Person person = (Person)personLogic.Details(Int16.Parse(personId));

                            var user = _db.Users.Where(u => u.UserName == person.CompanyId).ToList().FirstOrDefault();
                            if (user == null)
                            {
                                Notification notification = new Notification();
                            }
                        }
                    }
                }
                return recipientPersonAndUserId;
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }*/



    }
}