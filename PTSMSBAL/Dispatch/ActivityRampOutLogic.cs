using PTSMSDAL.Access.Dispatch;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch
{
    public class ActivityRampOutLogic
    {
        ActivityRampOutAccess activityRampOutAccess = new ActivityRampOutAccess();

        public List<ActivityRampOut> List()
        {
            return activityRampOutAccess.List();
        }

        public ActivityRampOut Details(int id)
        {
            return activityRampOutAccess.Details(id);
        }

        public bool Add(ActivityRampOut activityRampOut)
        {
            var activityRampOutDetail = activityRampOutAccess.Details(activityRampOut.ActivityRampOutId);

            if (activityRampOutDetail == null)
            {                
                if (activityRampOutAccess.Add(activityRampOut))
                {
                    FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
                    ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();
                    ActivityCheckIn activityCheckIn = activityCheckInLogic.Details(activityRampOut.ActivityCheckinId);
                    fTDAndFlyingSchedulerAccess.UpdateScheduleStatus(activityCheckIn.FlyingFTDScheduleId, FlyingFTDScheduleStatus.RampOut);

                    //start, 	When Ramp out is recorder subtract lesson duration from actual remaining maintenance hours (ActualMaintenanceHour) 
                    PTSContext db = new PTSContext();
                    var flyingFTDSchedule = db.FlyingFTDSchedules.Find(activityCheckIn.FlyingFTDScheduleId);
                    var equipmentObj = db.Equipments.Find(flyingFTDSchedule.EquipmentId);
                    var lesson = db.Lessons.Where(l => l.LessonId == flyingFTDSchedule.LessonId).ToList();
                    double lessonDuration = 0;
                    if (lesson.Count > 0 && lesson.Count > 0)
                    {
                        if (equipmentObj.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                        {
                            lessonDuration = (double)lesson.FirstOrDefault().FTDTime;
                        }
                        else if (equipmentObj.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                        {
                            lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;

                        }
                    }
                    else
                    {
                        lessonDuration = 1;
                    }
                    if (equipmentObj != null)
                    {
                        equipmentObj.ActualRemainingHours = equipmentObj.ActualRemainingHours - (float)lessonDuration;
                        db.Entry(equipmentObj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    ///////////////////////end, 	When Ramp out is recorder subtract lesson duration from actual remaining maintenance hours (ActualMaintenanceHour) 
                    return true;
                }
                return false;
            }
            else
            {
                activityRampOutDetail.Hobbs = activityRampOut.Hobbs;
                activityRampOutDetail.DepartureTimeReason = activityRampOut.DepartureTimeReason;
                activityRampOutDetail.AdjustedDepartureTime = activityRampOut.AdjustedDepartureTime;
                activityRampOutDetail.AdjustedReasonId = activityRampOut.AdjustedReasonId;

                return activityRampOutAccess.ReviseAdd(activityRampOutDetail);
            }
        }

        public ActivityRampOut GetActivityRampOutDetailsByEquipmentId(int equipmentId)
        {
            return activityRampOutAccess.GetActivityRampOutDetailsByEquipmentId(equipmentId);
        }

        public bool Revise(ActivityRampOut activityRampOut)
        {
            return activityRampOutAccess.Revise(activityRampOut);
        }

        public bool Delete(int id)
        {
            return activityRampOutAccess.Delete(id);
        }

        public ActivityRampOut GetActivityRampOutDetailsByCheckInId(int activityCheckInId)
        {
            return activityRampOutAccess.GetActivityRampOutDetailsByCheckInId(activityCheckInId);
        }
    }
}
