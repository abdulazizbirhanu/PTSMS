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
    public class ActivityAuthorizationLogic
    {
        ActivityAuthorizationAccess activityAuthorizationAccess = new ActivityAuthorizationAccess();

        public List<ActivityAuthorization> List()
        {
            return activityAuthorizationAccess.List();
        }

        public ActivityAuthorization Details(int id)
        {
            return activityAuthorizationAccess.Details(id);
        }

        public bool Add(ActivityAuthorization activityAuthorization)
        {
            var activityAuthorizationDetail = activityAuthorizationAccess.Details(activityAuthorization.ActivityAuthorizationId);
            if (activityAuthorizationDetail == null)
            {
                if (activityAuthorizationAccess.Add(activityAuthorization))
                {
                    FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
                    ActivityCheckInLogic activityCheckInLogic = new ActivityCheckInLogic();
                    ActivityCheckIn activityCheckIn = activityCheckInLogic.Details(activityAuthorization.ActivityCheckinId);
                    if (activityAuthorization.Status == Enum.GetName(typeof(ActivityAuthorizationStatus), ActivityAuthorizationStatus.Authorized))
                    {
                        fTDAndFlyingSchedulerAccess.UpdateScheduleStatus(activityCheckIn.FlyingFTDScheduleId, FlyingFTDScheduleStatus.CheckedInAuthorized);
                    }
                    else if (activityAuthorization.Status == Enum.GetName(typeof(ActivityAuthorizationStatus), ActivityAuthorizationStatus.Rejected))
                    {
                        fTDAndFlyingSchedulerAccess.UpdateScheduleStatus(activityCheckIn.FlyingFTDScheduleId, FlyingFTDScheduleStatus.CheckedInRejected);
                    }
                    return true;
                }
                return false;
            }
            else
            {
                activityAuthorizationDetail.ActivityCheckinId = activityAuthorization.ActivityCheckinId;
                activityAuthorizationDetail.Status = activityAuthorization.Status;
                activityAuthorizationDetail.Remark = activityAuthorization.Remark;

                return activityAuthorizationAccess.Revise(activityAuthorizationDetail);
            }
        }

        public bool Revise(ActivityAuthorization activityAuthorization)
        {
            return activityAuthorizationAccess.Revise(activityAuthorization);
        }

        public bool Delete(int id)
        {
            return activityAuthorizationAccess.Delete(id);
        }

        public ActivityAuthorization GetActivityAuthorizationDetailsByCheckInId(int activityCheckInId)
        {
            return activityAuthorizationAccess.GetActivityAuthorizationDetailsByCheckInId(activityCheckInId);
        }
    }
}
