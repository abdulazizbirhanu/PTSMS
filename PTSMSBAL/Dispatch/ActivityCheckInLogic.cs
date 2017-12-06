using PTSMSDAL.Access.Dispatch;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch;
using PTSMSDAL.Models.Dispatch.Master;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch
{
    public class ActivityCheckInLogic
    {
        ActivityCheckInAccess activityCheckInAccess = new ActivityCheckInAccess();
        PTSContext db = new PTSContext();

        public List<ActivityCheckIn> List()
        {
            return activityCheckInAccess.List();
        }

        public ActivityCheckIn Details(int id)
        {
            return activityCheckInAccess.Details(id);
        }

        public bool Add(ActivityCheckIn activityCheckIn)
        {
            var activityCheckInDetail = activityCheckInAccess.Details(activityCheckIn.ActivityCheckInId);
            if (activityCheckInDetail == null)
            {
                if (activityCheckInAccess.Add(activityCheckIn))
                {
                    FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
                    FlyingFTDScheduleStatus status = FlyingFTDScheduleStatus.CheckedIn;//Check - In
                    if (activityCheckIn.CheckInStatusId != null && activityCheckIn.CheckInStatusId > 0)
                    {
                        CheckInStatus checkInStatus = db.CheckInStatuss.Find(activityCheckIn.CheckInStatusId);
                        if (checkInStatus != null)
                        {
                            if (!string.IsNullOrEmpty(checkInStatus.CheckInStatusName))
                            {
                                if (!checkInStatus.CheckInStatusName.ToUpper().Equals("CHECK-IN"))
                                    status = FlyingFTDScheduleStatus.Unattended;
                            }
                        }
                    }
                    fTDAndFlyingSchedulerAccess.UpdateScheduleStatus(activityCheckIn.FlyingFTDScheduleId, status);
                    return true;
                }
                return false;
            }
            else
            {
                activityCheckInDetail.ArrivalAirportId = activityCheckIn.ArrivalAirportId;
                activityCheckInDetail.DepartureAirportId = activityCheckIn.DepartureAirportId;
                activityCheckInDetail.DestinationId = activityCheckIn.DestinationId;
                activityCheckInDetail.EquipmentId = activityCheckIn.EquipmentId;
                activityCheckInDetail.InstructorId = activityCheckIn.InstructorId;
                activityCheckInDetail.CheckInStatusId = activityCheckIn.CheckInStatusId;
                activityCheckInDetail.ObserverId = activityCheckIn.ObserverId;
                activityCheckInDetail.ParkingSpotId = activityCheckIn.ParkingSpotId;
                activityCheckInDetail.CheckInTime = activityCheckIn.CheckInTime;
                activityCheckInDetail.OperationAreaId = activityCheckIn.OperationAreaId;
                activityCheckInDetail.Comments = activityCheckIn.Comments;

                return activityCheckInAccess.Revise(activityCheckInDetail);
            }
        }

        public bool Revise(ActivityCheckIn activityCheckIn)
        {
            return activityCheckInAccess.Revise(activityCheckIn);
        }

        public bool Delete(int id)
        {
            return activityCheckInAccess.Delete(id);
        }

        public ActivityCheckIn CheckInDetailsByScheduleId(int flyingFTDScheduleId)
        {
            return activityCheckInAccess.CheckInDetailsByScheduleId(flyingFTDScheduleId);
        }
    }
}
