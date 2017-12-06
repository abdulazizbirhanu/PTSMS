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
    public class ActivityCheckInAccess
    {
        private PTSContext db = new PTSContext();
        public List<ActivityCheckIn> List()
        {
            return db.ActivityCheckIns.ToList();
        }

        public ActivityCheckIn Details(int id)
        {
            try
            {
                return  db.ActivityCheckIns.Find(id);                
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }


        public bool Add(ActivityCheckIn activityCheckIn)
        {
            try
            {
                activityCheckIn.Sequence = 1;
                db.ActivityCheckIns.Add(activityCheckIn);
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public ActivityCheckIn CheckInDetailsByScheduleId(int flyingFTDScheduleId)
        {
            try
            {
                return  db.ActivityCheckIns.Where(ck => ck.FlyingFTDScheduleId == flyingFTDScheduleId).ToList().FirstOrDefault();
               
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Revise(ActivityCheckIn activityCheckIn)
        {
            try
            {
                db.Entry(activityCheckIn).State = EntityState.Modified;
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
                ActivityCheckIn activityCheckIn = db.ActivityCheckIns.Find(id);
                db.Entry(activityCheckIn).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
