using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class RescheduleReasonAccess
    {

        private PTSContext db = new PTSContext();
        public List<RescheduleReason> List()
        {
            return db.RescheduleReasons.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public RescheduleReason Details(int id)
        {
            try
            {
                return db.RescheduleReasons.Find(id);
            }
            catch (Exception e)
            {
                return new RescheduleReason();
            }
        }


        public bool Add(RescheduleReason rescheduleReason)
        {
            try
            {
                rescheduleReason.StartDate = DateTime.Now;
                rescheduleReason.EndDate = Constants.EndDate;
                rescheduleReason.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                rescheduleReason.CreationDate = DateTime.Now;
                db.RescheduleReasons.Add(rescheduleReason);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Revise(RescheduleReason rescheduleReason)
        {
            try
            {
                rescheduleReason.RevisionDate = DateTime.Now;
                rescheduleReason.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(rescheduleReason).State = EntityState.Modified;
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
                RescheduleReason rescheduleReason = db.RescheduleReasons.Find(id);
                db.Entry(rescheduleReason).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
