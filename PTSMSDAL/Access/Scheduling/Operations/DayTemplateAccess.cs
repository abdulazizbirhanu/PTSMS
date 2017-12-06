using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class DayTemplateAccess
    {
        private PTSContext db = new PTSContext();

        public List<DayTemplate> List()
        {
            return db.DayTemplates.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public DayTemplate Details(int id)
        {
            try
            {
                DayTemplate dayTemplate = db.DayTemplates.Find(id);
                if (dayTemplate == null)
                {
                    return null; // Not Found
                }
                return dayTemplate; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(DayTemplate dayTemplate)
        {
            try

            {
                dayTemplate.StartDate = DateTime.Now;
                dayTemplate.EndDate = Constants.EndDate;
                dayTemplate.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                dayTemplate.CreationDate = DateTime.Now;

                db.DayTemplates.Add(dayTemplate);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(DayTemplate dayTemplate)
        {
            try
            {

                dayTemplate.RevisionDate = DateTime.Now;
                dayTemplate.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(dayTemplate).State = EntityState.Modified;
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

                DayTemplate dayTemplate = db.DayTemplates.Find(id);
                dayTemplate.EndDate = DateTime.Now;
                db.DayTemplates.Remove(dayTemplate);
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