using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class PeriodAccess
    {
        public List<Period> List()
        {
            PTSContext db = new PTSContext();
            return db.Periods.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public Period Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                Period period = db.Periods.Find(id);
                if (period == null)
                {
                    return null; // Not Found
                }
                return period; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(Period period)
        {
            try
            {
                PTSContext db = new PTSContext();

                period.StartDate = DateTime.Now;
                period.EndDate = Constants.EndDate;
                period.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                period.CreationDate = DateTime.Now;
                db.Periods.Add(period);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(Period period)
        {
            try
            {
                PTSContext db = new PTSContext();
                period.RevisionDate = DateTime.Now;
                period.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(period).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Delete(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                Period period = db.Periods.Find(id);
                db.Periods.Remove(period);
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
    }
}