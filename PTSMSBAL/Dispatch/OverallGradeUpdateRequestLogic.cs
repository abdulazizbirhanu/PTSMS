using PTSMSDAL.Access.Dispatch;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch.Master;
using PTSMSDAL.Models.Enrollment.Relations;
using System;
using System.Web;
using System.Linq;
using System.Data.Entity;
using PTSMSDAL.Access.Others;
using PTSMSDAL.Models.Others.Messaging;
using PTSMSDAL.Access.Utility;
using System.Collections.Generic;
using PTSMSDAL.Access.Scheduling.Operations;

namespace PTSMSBAL.Dispatch
{
    public class OverallGradeUpdateRequestLogic
    {
        OverallGradeUpdateRequestAccess overallGradeUpdateRequestAccess = new OverallGradeUpdateRequestAccess();
        public bool OverallGradeUpdateRequestApproval(int overallGradeUpdateRequestId, bool isApproved)
        {
            try
            {
                string status = string.Empty;
                if (isApproved)
                    status = Enum.GetName(typeof(OverallGradeUpdateRequestStatus), (int)OverallGradeUpdateRequestStatus.Approved);
                else
                    status = Enum.GetName(typeof(OverallGradeUpdateRequestStatus), (int)OverallGradeUpdateRequestStatus.Rejected);

                OverallGradeUpdateRequest overallGradeUpdateRequest = overallGradeUpdateRequestAccess.Details(overallGradeUpdateRequestId);
                if (overallGradeUpdateRequest != null)
                {
                    overallGradeUpdateRequest.Status = status;
                    overallGradeUpdateRequest.ApprovedBy = HttpContext.Current.User.Identity.Name;
                    overallGradeUpdateRequest.ApprovedDate = DateTime.Now;

                    //Update overallGradeUpdateRequest
                    if (overallGradeUpdateRequestAccess.Revise(overallGradeUpdateRequest))
                    {
                        if (isApproved)
                        {
                            //Update Trainee Lesson OverAll Grade.
                            PTSContext db = new PTSContext();
                            TraineeLesson traineeLesson = db.TraineeLessons.Where(TL => TL.LessonId == overallGradeUpdateRequest.FlyingFTDSchedule.LessonId && TL.TraineeId == overallGradeUpdateRequest.FlyingFTDSchedule.TraineeId).ToList().FirstOrDefault();
                            if (traineeLesson != null)
                            {
                                traineeLesson.OverallGradeId = overallGradeUpdateRequest.NewOverallGradeId;
                                db.Entry(traineeLesson).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        //Save and send Notification
                        UtilityClass utilityClass = new UtilityClass();
                        NotificationAccess notificationAccess = new NotificationAccess();
                        UserNotificationAccess userNotificationAccess = new UserNotificationAccess();

                        string description = "Overall Grade Update Request has been " + status + " for " + overallGradeUpdateRequest.FlyingFTDSchedule.Lesson.LessonName + " Lesson";
                        Notification notification = new Notification
                        {
                            SenderId = HttpContext.Current.User.Identity.Name,
                            TimeSent = DateTime.Now,
                            Description = description,
                            NotificationType = NotificationType.Overall_Grade_Change,
                            ObjectURL = "/OverallGradeUpdateRequest/Details?id=" + overallGradeUpdateRequestId,
                        };

                        if (notificationAccess.Add(notification))
                        {
                            int notificationId = utilityClass.GetLatestIdNumber("NOTIFICATION");
                            userNotificationAccess.Add(new UserNotification
                            {
                                NotificationId = notificationId,
                                NotificationState = NotificationState.UNREAD,
                                RecipientId = overallGradeUpdateRequest.FlyingFTDSchedule.Instructor.Person.CompanyId
                            });

                            userNotificationAccess.Add(new UserNotification
                            {
                                NotificationId = notificationId,
                                NotificationState = NotificationState.UNREAD,
                                RecipientId = overallGradeUpdateRequest.FlyingFTDSchedule.Trainee.Person.CompanyId
                            });
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<OverallGradeUpdateRequest> List()
        {
            return overallGradeUpdateRequestAccess.List();
        }

        public OverallGradeUpdateRequest Details(int id)
        {
            return overallGradeUpdateRequestAccess.Details(id);
        }

        public OperationResult Add(OverallGradeUpdateRequest overallGradeUpdateRequest)
        {
            overallGradeUpdateRequest.Status = Enum.GetName(typeof(OverallGradeUpdateRequestStatus), (int)OverallGradeUpdateRequestStatus.Requested);
            overallGradeUpdateRequest.RequestedDate = DateTime.Now;
            var overallGradeUpdateRequestDetail = overallGradeUpdateRequestAccess.OverallGradeUpdateRequestDetails(overallGradeUpdateRequest);
            if (overallGradeUpdateRequestDetail == null)
            {
                return new OperationResult { IsSuccess = false, Message = "You have already made a request for this schedule with the same overall grade." };
            }
            if (overallGradeUpdateRequestAccess.Add(overallGradeUpdateRequest))
                return new OperationResult { IsSuccess = true, Message = "Overall grade change request has been made successfully." };
            return new OperationResult { IsSuccess = false, Message = "Failed to change over all grade." };
        }

        public bool Revise(OverallGradeUpdateRequest overallGradeUpdateRequest)
        {
            return overallGradeUpdateRequestAccess.Revise(overallGradeUpdateRequest);
        }

        public bool Delete(int id)
        {
            return overallGradeUpdateRequestAccess.Delete(id);
        }
    }
}
