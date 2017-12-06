using PTSMSDAL.Context;
using PTSMSDAL.Models.Others;
using PTSMSDAL.Models.Others.Messaging;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Others
{
    public class NotificationAccess
    {

        public List<NotificationView> GetConcernedBodies(string lessonType, DateTime startAt, DateTime endAt)
        {
            PTSContext db = new PTSContext();
            List<NotificationView> notificationListView = new List<NotificationView>();
            try
            {
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                if (lessonType.ToUpper() == "FTD")
                {
                   
                    var FTDSchedules = db.FlyingFTDSchedules.Where(s => s.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD" && s.Status != statusName).ToList();
                    var scheduleList = FTDSchedules.Where(s => s.ScheduleStartTime.Date >= startAt.Date && s.ScheduleEndTime.Date <= endAt.Date).ToList();

                    int notificationIndex = 1;

                    NotificationView notificationView = null;
                    foreach (var schedule in scheduleList)
                    {
                        notificationView = new NotificationView();

                        notificationView.NotificationId = notificationIndex++;
                        //notificationView.EmailMessage = GetHtmlMessageBody(schedule);
                        //notificationView.SMSMessage = "Lesson Name: " + schedule.Lesson.LessonName + ", Time: " + schedule.ScheduleStartTime.ToString("MM/dd/yyy") + " " + schedule.ScheduleStartTime.ToString("HH:ss") + " " + schedule.ScheduleEndTime.ToString("HH:ss") + ", Location: " + schedule.Equipment.Location.LocationName;
                        notificationView.Name = schedule.Lesson.LessonName;
                        int recipientId = 1;
                        List<ConcernedBodies> ConcernedBodyList = new List<ConcernedBodies>();
                        ConcernedBodyList.Add(new ConcernedBodies
                        {
                            ScheduleId = schedule.FlyingFTDScheduleId,
                            NotificationId = recipientId++,
                            FirstName = schedule.Instructor.Person.FirstName,
                            LastName = schedule.Instructor.Person.MiddleName,
                            PersonId = schedule.Instructor.Person.PersonId,
                            Email = schedule.Instructor.Person.Email,
                            PhoneNumber = schedule.Instructor.Person.Phone,
                            HasEmailAddress = (!string.IsNullOrEmpty(schedule.Instructor.Person.Email) ? true : false),
                            HasMobileNumber = (!string.IsNullOrEmpty(schedule.Instructor.Person.Phone) ? true : false)
                        });
                        ConcernedBodyList.Add(new ConcernedBodies
                        {
                            ScheduleId = schedule.FlyingFTDScheduleId,
                            NotificationId = recipientId,
                            FirstName = schedule.Trainee.Person.FirstName,
                            LastName = schedule.Trainee.Person.MiddleName,
                            PersonId = schedule.Trainee.Person.PersonId,
                            Email = schedule.Trainee.Person.Email,
                            PhoneNumber = schedule.Trainee.Person.Phone,
                            HasEmailAddress = (!string.IsNullOrEmpty(schedule.Trainee.Person.Email) ? true : false),
                            HasMobileNumber = (!string.IsNullOrEmpty(schedule.Trainee.Person.Phone) ? true : false)
                        });
                        notificationView.ConcernedBodyList = ConcernedBodyList;
                        notificationListView.Add(notificationView);
                    }
                    return notificationListView;
                }
                else if (lessonType.ToUpper() == "FLYING")
                {
                    var FTDSchedules = db.FlyingFTDSchedules.Where(s => s.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING" && s.Status != statusName).ToList();
                    var scheduleList = FTDSchedules.Where(s => s.ScheduleStartTime.Date >= startAt.Date && s.ScheduleEndTime.Date <= endAt.Date).ToList();

                    int notificationIndex = 1; int recipientId = 1;
                    NotificationView notificationView = null;
                    foreach (var schedule in scheduleList)
                    {
                        notificationView = new NotificationView();

                        notificationView.NotificationId = notificationIndex++;
                        // notificationView.EmailMessage = GetHtmlMessageBody(schedule);
                        //notificationView.SMSMessage = "Lesson Name: " + schedule.Lesson.LessonName + ", Time: " + schedule.ScheduleStartTime.ToString("MM/dd/yyy") + " " + schedule.ScheduleStartTime.ToString("HH:ss") + " " + schedule.ScheduleEndTime.ToString("HH:ss") + ", Location: " + schedule.Equipment.Location.LocationName;
                        notificationView.Name = schedule.Lesson.LessonName;

                        List<ConcernedBodies> ConcernedBodyList = new List<ConcernedBodies>();
                        ConcernedBodyList.Add(new ConcernedBodies
                        {
                            ScheduleId = schedule.FlyingFTDScheduleId,
                            NotificationId = recipientId,
                            FirstName = schedule.Instructor.Person.FirstName,
                            LastName = schedule.Instructor.Person.MiddleName,
                            PersonId = schedule.Instructor.Person.PersonId,
                            Email = schedule.Instructor.Person.Email,
                            PhoneNumber = schedule.Instructor.Person.Phone,
                            HasEmailAddress = (schedule.Instructor.Person.Email != null ? true : false),
                            HasMobileNumber = (schedule.Instructor.Person.Phone != null ? true : false)
                        });
                        ConcernedBodyList.Add(new ConcernedBodies
                        {
                            ScheduleId = schedule.FlyingFTDScheduleId,
                            NotificationId = recipientId++,
                            FirstName = schedule.Trainee.Person.FirstName,
                            LastName = schedule.Trainee.Person.MiddleName,
                            PersonId = schedule.Trainee.Person.PersonId,
                            Email = schedule.Trainee.Person.Email,
                            PhoneNumber = schedule.Trainee.Person.Phone,
                            HasEmailAddress = (schedule.Trainee.Person.Email != null ? true : false),
                            HasMobileNumber = (schedule.Trainee.Person.Phone != null ? true : false)
                        });
                        notificationView.ConcernedBodyList = ConcernedBodyList;
                        notificationListView.Add(notificationView);
                    }
                    return notificationListView;
                }
                else if (lessonType.ToUpper() == "GROUND")
                {

                }
                return new List<NotificationView>();
            }
            catch (Exception ex)
            {
                return new List<NotificationView>();
            }
        }

        public UserNotifications GetUserNotification(string recipientId)
        {
            try
            {
                PTSContext db = new PTSContext();
                UserNotifications userNotifications = new UserNotifications();

                List<UserNotificationView> userNotificationList = new List<UserNotificationView>();
                var userNotification = (
                                       from N in db.Notifications
                                       join UN in db.UserNotifications on N.NotificationId equals UN.NotificationId
                                       where UN.RecipientId == recipientId && UN.NotificationState != NotificationState.READ
                                       select new
                                       {
                                           N,
                                           UN
                                       }).ToList();
                int unreadNotification = userNotification.Where(n => n.UN.NotificationState == NotificationState.UNREAD).ToList().Count();
                int seenNotification = userNotification.Where(n => n.UN.NotificationState == NotificationState.SEEN).ToList().Count();

                foreach (var notif in userNotification)
                {
                    userNotificationList.Add(new UserNotificationView
                    {
                        ObjectURL = notif.N.ObjectURL,
                        TimeSent = notif.N.TimeSent,
                        Description = notif.N.Description,
                        NotificationType = notif.N.NotificationType,

                        ReadDate = notif.UN.ReadDate,
                        SeenDate = notif.UN.SeenDate,
                        UserNotificationId = notif.UN.UserNotificationId,
                        NotificationState = notif.UN.NotificationState
                    });
                }
                userNotifications.UserNotificationList = userNotificationList;
                userNotifications.SeenNotifications = seenNotification;
                userNotifications.UnreadNotifications = unreadNotification;
                return userNotifications;
            }
            catch (Exception ex)
            {
                return new UserNotifications();
            }
        }

        public string GetHtmlMessageBody(FlyingFTDSchedule newFlyingFTDSchedule)
        {
            try
            {
                string messageBody = "<font><b>Schedule Notification:</b> </font><br><br>";

                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style =\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style =\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";

                messageBody += htmlTableStart;
                //Construct Header
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "No." + htmlTdEnd;
                messageBody += htmlTdStart + "Info" + htmlTdEnd;
                messageBody += htmlTdStart + "Detail" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                //Contstuct table body               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "1. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Trainee Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + newFlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                //       
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "2. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Instructor Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Instructor.Person.FirstName.Substring(0, 3) + " " + newFlyingFTDSchedule.Instructor.Person.MiddleName.Substring(0, 1) + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //       
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "3. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Equipment" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Equipment.NameOrSerialNo + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "4. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Location" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Equipment.Location.LocationName + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "5. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Room No" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Equipment.RoomNo + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "6. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Lesson Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Lesson.LessonName + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "7. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Duration" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.ScheduleStartTime + " - " + newFlyingFTDSchedule.ScheduleEndTime + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "8. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Status" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Status + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //
                messageBody = messageBody + htmlTableEnd;


                return messageBody;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        PTSContext db = new PTSContext();

        public List<Notification> GetNotificationListBySenderId(string senderid)
        {
            return db.Notifications.Where(n => n.SenderId == senderid).ToList();
        }

        public Notification Details(int notificationId)
        {
            try
            {
                return db.Notifications.Find(notificationId);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool Add(Notification notification)
        {
            try
            {
                db.Notifications.Add(notification);
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(Notification notification)
        {
            try
            {
                db.Entry(notification).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                Notification notification = db.Notifications.Find(id);
                db.Notifications.Remove(notification);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
