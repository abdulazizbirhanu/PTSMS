using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using PTSMSDAL.Generic;
using System.Web;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class DayAccess
    {
        private PTSContext db = new PTSContext();

        public List<Day> List()
        {
            return db.Days.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public Day Details(int id)
        {
            try
            {
                Day day = db.Days.Find(id);
                if (day == null)
                {
                    return null; // Not Found
                }
                return day; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(Day day)
        {
            try
            {
                day.StartDate = DateTime.Now;
                day.EndDate = Constants.EndDate;
                day.CreatedBy = HttpContext.Current.User.Identity.Name;
                day.CreationDate = DateTime.Now;

                db.Days.Add(day);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(Day day)
        {
            try
            {
                day.RevisionDate = DateTime.Now;
                day.RevisedBy = HttpContext.Current.User.Identity.Name;
                db.Entry(day).State = EntityState.Modified;
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
                Day day = db.Days.Find(id);
                db.Days.Remove(day);
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