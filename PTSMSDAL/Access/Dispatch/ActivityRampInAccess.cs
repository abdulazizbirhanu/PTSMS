using PTSMSDAL.Context;
using PTSMSDAL.Models.Dispatch;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Dispatch
{
    public class ActivityRampInAccess
    {
        private PTSContext db = new PTSContext();
        public List<ActivityRampIn> List()
        {
            return db.ActivityRampIns.ToList();
        }

        public ActivityRampIn Details(int id)
        {
            try
            {
                return db.ActivityRampIns.Find(id);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public bool Add(ActivityRampIn activityRampIn)
        {
            try
            {
                db.ActivityRampIns.Add(activityRampIn);
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public ActivityRampIn GetActivityRampInDetailsByRampOutId(int activityRampOutId)
        {

            try
            {
                return db.ActivityRampIns.Where(RN => RN.ActivityRampOutId == activityRampOutId).ToList().FirstOrDefault();
            }
            catch (System.Exception e)
            {
                return new ActivityRampIn();
            }
        }

        public bool Revise(ActivityRampIn activityRampIn)
        {
            try
            {
                db.Entry(activityRampIn).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                ActivityRampIn activityRampIn = db.ActivityRampIns.Find(id);
                db.Entry(activityRampIn).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ActivityRampIn GetLastActivityRampInDetailsByEquipmentId(int equipmentId)
        {
            try
            {
                var activityRampInList = db.ActivityRampIns.Where(ARI => ARI.ActivityRampOut.ActivityCheckIn.EquipmentId == equipmentId).ToList();
                return activityRampInList.OrderByDescending(x => x.AdjustedArrivalTime).FirstOrDefault();

            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
