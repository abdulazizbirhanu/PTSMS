using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSBAL.Utility;
using PTSMSDAL.Access.Others;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Access.Utility;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Others;
using PTSMSDAL.Models.Others.Messaging;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Others
{
    public class NotificationLogic
    {
        NotificationAccess notificationAccess = new NotificationAccess();
        public List<NotificationView> GetConcernedBodies(string lessonType, DateTime startAt, DateTime endAt)
        {
            return notificationAccess.GetConcernedBodies(lessonType, startAt, endAt);
        }

        public OperationResult SendScheduleNotification(string[] recipientArray, string senderUserName)
        {
            try
            {
                string message = "";
                PTSContext db = new PTSContext();
                EmailLogic emailLogic = new EmailLogic();
                var email = "";
                var phoneNumber = "";
                var HasEmail = "";
                var HasPhoneNumber = "";
                var SMSMessage = "";
                var EmailMessage = "";
                var ScheduleId = "";
                var personId = "";
                FTDAndFlyingSchedulerAccess flyingFTDScheduleAccess = new FTDAndFlyingSchedulerAccess();
                foreach (var recipient in recipientArray)
                {
                    if (!(String.IsNullOrEmpty(recipient)))
                    {
                        string[] recipientDetail = recipient.Split('-');
                        email = recipientDetail[0];
                        phoneNumber = recipientDetail[1];
                        HasPhoneNumber = recipientDetail[2];
                        HasEmail = recipientDetail[3];
                        ScheduleId = recipientDetail[5];
                        personId = recipientDetail[4];
                        EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
                        FlyingFTDSchedule flyingFTDSchedule = flyingFTDScheduleAccess.Details(Int16.Parse(ScheduleId));
                        EquipmentScheduleBriefingDebriefing briefingAndDebriefing = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDScheduleId == flyingFTDSchedule.FlyingFTDScheduleId && b.BriefingAndDebriefing.IsBriefing).ToList().FirstOrDefault();

                        EmailMessage = notificationAccess.GetHtmlMessageBody(flyingFTDSchedule);

                        if (flyingFTDSchedule.Instructor.PersonId == Int16.Parse(personId))
                            SMSMessage = "Lesson: " + flyingFTDSchedule.Lesson.LessonName + " Briefing: " + briefingAndDebriefing.BriefingAndDebriefing.StartingTime.ToString("MM/dd/yyy") + " " + briefingAndDebriefing.BriefingAndDebriefing.StartingTime.ToString("HH:mm") + "-" + briefingAndDebriefing.BriefingAndDebriefing.EndingTime.ToString("HH:mm") + ", Lesson Time: " + flyingFTDSchedule.ScheduleStartTime.ToString("HH:mm") + "-" + flyingFTDSchedule.ScheduleEndTime.ToString("HH:mm") + ", Location: " + flyingFTDSchedule.Equipment.Location.LocationName + ", " + "Trainee: " + flyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + flyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". Equipment: " + flyingFTDSchedule.Equipment.NameOrSerialNo;
                        else
                            SMSMessage = "Lesson: " + flyingFTDSchedule.Lesson.LessonName + " Briefing: " + briefingAndDebriefing.BriefingAndDebriefing.StartingTime.ToString("MM/dd/yyy") + " " + briefingAndDebriefing.BriefingAndDebriefing.StartingTime.ToString("HH:mm") + "-" + briefingAndDebriefing.BriefingAndDebriefing.EndingTime.ToString("HH:mm") + ", Lesson Time: " + flyingFTDSchedule.ScheduleStartTime.ToString("HH:mm") + "-" + flyingFTDSchedule.ScheduleEndTime.ToString("HH:mm") + ", Location: " + flyingFTDSchedule.Equipment.Location.LocationName + ", " + "Instructor: " + flyingFTDSchedule.Instructor.Person.FirstName.Substring(0, 3) + " " + flyingFTDSchedule.Instructor.Person.MiddleName.Substring(0, 1) + ". Equipment: " + flyingFTDSchedule.Equipment.NameOrSerialNo;
                        bool isNotifiedUpdated = false;
                        if (!String.IsNullOrEmpty(HasPhoneNumber))
                        {
                            if (Boolean.Parse(HasPhoneNumber.ToLower()))
                            {
                                if (emailLogic.SendSMS(SMSMessage, phoneNumber))
                                {
                                    flyingFTDSchedule.IsNotified = true;
                                    if(flyingFTDScheduleAccess.Revise(flyingFTDSchedule))
                                    {
                                        isNotifiedUpdated = true;
                                    }
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(HasEmail))
                        {
                            if (Boolean.Parse(HasEmail.ToLower()))
                            {
                                if(emailLogic.SendEmail(EmailMessage, email, "Pilot School Schedule Notification", ""))
                                {
                                    if(!isNotifiedUpdated)
                                    {
                                        flyingFTDSchedule.IsNotified = true;
                                        if (flyingFTDScheduleAccess.Revise(flyingFTDSchedule))
                                        {
                                            isNotifiedUpdated = true;
                                        }
                                    }
                                }
                            }
                        }
                        //Save NOTIFICATION
                        if (!String.IsNullOrEmpty(personId))
                        {
                            PersonLogic personLogic = new PersonLogic();

                            Person person = (Person)personLogic.Details(Convert.ToInt16(personId));
                            if (person != null)
                            {
                                ///NotificationAccess notificationAccess = new NotificationAccess();
                                //Save Notification
                                Notification notification = new Notification
                                {
                                    SenderId = senderUserName,
                                    TimeSent = DateTime.Now,
                                    Description = SMSMessage,
                                    NotificationType = NotificationType.New_Schedule,
                                    ObjectURL = "/Scheduler/Details?id=" + Int16.Parse(ScheduleId)
                                };

                                if (notificationAccess.Add(notification))
                                {
                                    UtilityClass utilityClass = new UtilityClass();
                                    int notificationId = utilityClass.GetLatestIdNumber("NOTIFICATION");

                                    UserNotification userNotification = new UserNotification
                                    {
                                        NotificationId = notificationId,
                                        NotificationState = NotificationState.UNREAD,
                                        RecipientId = person.CompanyId
                                    };
                                    UserNotificationAccess userNotificationAccess = new UserNotificationAccess();
                                    userNotificationAccess.Add(userNotification);
                                }
                            }
                        }
                    }
                }
                if (message == "")
                    return new OperationResult { Message = "Notification has successfully sent to all recipient.", IsSuccess = true };
                return new OperationResult { Message = message, IsSuccess = false };
            }
            catch (Exception e)
            {
                return new OperationResult { Message = e.Message, IsSuccess = false };
            }
        }

        public bool NotificationHasSeen(string recipientId)
        {
            try
            {
                UserNotificationAccess userNotificationAccess = new UserNotificationAccess();
                List<UserNotification> userNotificationList = userNotificationAccess.GetUserNotification(recipientId);

                foreach (var notific in userNotificationList)
                {
                    userNotificationAccess.NotificationHasSeen(notific.UserNotificationId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public UserNotifications GetUserNotification(string recipientId)
        {
            return notificationAccess.GetUserNotification(recipientId);
        }
    }
}
