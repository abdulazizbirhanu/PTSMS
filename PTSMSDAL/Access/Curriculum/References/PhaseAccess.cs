using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.References
{
    public class PhaseAccess
    {
        private PTSContext db = new PTSContext();
        
        public object List()
        {
            return db.Phases.Where(p => p.EndDate > DateTime.Now).ToList();
        }

        public object Details(int id)
        {
            try
            {
                Phase phase = db.Phases.Find(id);
                if (phase == null)
                {
                    return null; // Not Found
                }
                return phase; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(Phase phase)
        {
            try
            {
                phase.StartDate = DateTime.Now;
                phase.EndDate = Constants.EndDate;
                phase.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                phase.CreationDate = DateTime.Now;

                db.Phases.Add(phase);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Phase phase)
        {
            try
            {
                phase.RevisionDate = DateTime.Now;
                phase.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(phase).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Delete(int id)
        {
            try
            {
                Phase phase = db.Phases.Find(id);
                phase.EndDate = DateTime.Now;
                phase.Name += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(phase).State = EntityState.Modified;
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