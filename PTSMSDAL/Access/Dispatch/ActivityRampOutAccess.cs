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
    public class ActivityRampOutAccess
    {
        private PTSContext db = new PTSContext();
        public List<ActivityRampOut> List()
        {
            return db.ActivityRampOuts.ToList();
        }

        public ActivityRampOut Details(int id)
        {
            try
            {
                return db.ActivityRampOuts.Find(id);
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(ActivityRampOut activityRampOut)
        {
            try
            {
                db.ActivityRampOuts.Add(activityRampOut);
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public bool ReviseAdd(ActivityRampOut activityRampOutDetail)
        {
            try
            {
                db.Entry(activityRampOutDetail).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ActivityRampOut GetActivityRampOutDetailsByCheckInId(int activityCheckInId)
        {
            try
            {
                return db.ActivityRampOuts.Where(R => R.ActivityCheckinId == activityCheckInId).ToList().FirstOrDefault();
            }
            catch (System.Exception e)
            {
                return new ActivityRampOut();
            }
        }

        public bool Revise(ActivityRampOut activityRampOut)
        {
            try
            {
                db.Entry(activityRampOut).State = EntityState.Modified;
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
                ActivityRampOut activityRampOut = db.ActivityRampOuts.Find(id);
                db.Entry(activityRampOut).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ActivityRampOut GetActivityRampOutDetailsByEquipmentId(int equipmentId)
        {
            try
            {
                return db.ActivityRampOuts.Where(ARO => ARO.ActivityCheckIn.EquipmentId == equipmentId).ToList().OrderByDescending(AR => AR.AdjustedDepartureTime).ToList().FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
