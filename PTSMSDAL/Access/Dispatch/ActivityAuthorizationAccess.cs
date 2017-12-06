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
    public class ActivityAuthorizationAccess
    {
        private PTSContext db = new PTSContext();
        public List<ActivityAuthorization> List()
        {
            return db.ActivityAuthorizations.ToList();
        }

        public ActivityAuthorization Details(int id)
        {
            try
            {
                return db.ActivityAuthorizations.Find(id);                 
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public bool Add(ActivityAuthorization activityAuthorization)
        {
            try
            {                
                db.ActivityAuthorizations.Add(activityAuthorization);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ActivityAuthorization GetActivityAuthorizationDetailsByCheckInId(int activityCheckInId)
        {
            try
            {
                return db.ActivityAuthorizations.Where(AA => AA.ActivityCheckinId == activityCheckInId).ToList().FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool Revise(ActivityAuthorization activityAuthorization)
        {
            try
            {
                db.Entry(activityAuthorization).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                ActivityAuthorization activityAuthorization = db.ActivityAuthorizations.Find(id);
                db.Entry(activityAuthorization).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
