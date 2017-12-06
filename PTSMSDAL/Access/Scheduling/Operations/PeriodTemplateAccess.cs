using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class PeriodTemplateAccess
    {
        public List<PeriodTemplate> List()
        {
            PTSContext db = new PTSContext();
            return db.PeriodTemplates.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public PeriodTemplate Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                PeriodTemplate periodTemplate = db.PeriodTemplates.Find(id);
                if (periodTemplate == null)
                {
                    return null; // Not Found
                }
                return periodTemplate; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(PeriodTemplate periodTemplate)
        {
            try
            {
                PTSContext db = new PTSContext();
                periodTemplate.StartDate = DateTime.Now;
                periodTemplate.EndDate = Constants.EndDate;
                periodTemplate.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                periodTemplate.CreationDate = DateTime.Now;
                periodTemplate.PeriodTemplateId = 0;
                db.PeriodTemplates.Add(periodTemplate);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(PeriodTemplate periodTemplate)
        {
            try
            {
                PTSContext db = new PTSContext();
                periodTemplate.RevisionDate = DateTime.Now;
                periodTemplate.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(periodTemplate).State = EntityState.Modified;
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
                PeriodTemplate periodTemplate = db.PeriodTemplates.Find(id);
                db.PeriodTemplates.Remove(periodTemplate);
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