using PTSMSDAL.Access.Dispatch;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Dispatch;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch
{
    public class ActivityRampInLogic
    {
        ActivityRampInAccess activityRampInAccess = new ActivityRampInAccess();

        public List<ActivityRampIn> List()
        {
            return activityRampInAccess.List();
        }

        public ActivityRampIn Details(int id)
        {
            return activityRampInAccess.Details(id);
        }

        public bool Add(ActivityRampIn activityRampIn)
        {
            var activityRampInDetail = activityRampInAccess.Details(activityRampIn.ActivityRampInId);

            if (activityRampInDetail == null)
            {
                if (activityRampInAccess.Add(activityRampIn))
                {
                    FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
                    ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();
                    ActivityRampOutLogic activityRampOutLogic = new ActivityRampOutLogic();
                    ActivityRampOut activityRampOut = activityRampOutLogic.Details(activityRampIn.ActivityRampOutId);
                    if (activityRampOut != null)
                    {
                        ActivityCheckIn activityCheckIn = activityCheckInLogic.Details(activityRampOut.ActivityCheckinId);
                        if (activityCheckIn != null)
                        {
                            fTDAndFlyingSchedulerAccess.UpdateScheduleStatus(activityCheckIn.FlyingFTDScheduleId, FlyingFTDScheduleStatus.RampIn);
                        }
                    }
                    return true;
                }
                return false;
            }
            else
            {
                activityRampInDetail.Hobbs = activityRampIn.Hobbs;
                activityRampInDetail.Remark = activityRampIn.Remark;
                activityRampInDetail.ArrivalTimeReasonId = activityRampIn.ArrivalTimeReasonId;
                return activityRampInAccess.Revise(activityRampInDetail);
            }
        }

        public bool Revise(ActivityRampIn activityRampIn)
        {
            return activityRampInAccess.Revise(activityRampIn);
        }

        public bool Delete(int id)
        {
            return activityRampInAccess.Delete(id);
        }

        public ActivityRampIn GetActivityRampInDetailsByRampOutId(int activityRampOutId)
        {
            return activityRampInAccess.GetActivityRampInDetailsByRampOutId(activityRampOutId);
        }

        public ActivityRampIn GetLastActivityRampInDetailsByEquipmentId(int equipmentId)
        {
            return activityRampInAccess.GetLastActivityRampInDetailsByEquipmentId(equipmentId);
        }
    }
}
