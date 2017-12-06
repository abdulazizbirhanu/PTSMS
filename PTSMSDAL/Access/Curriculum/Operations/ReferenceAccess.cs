using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class ReferenceAccess
    {
        private PTSContext db = new PTSContext();

        public List<Reference> List()
        {
            return db.References.Include(c => c.PreviousReference).Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public Reference Details(int id)
        {
            try
            {
                Reference reference = db.References.Find(id);
                if (reference == null)
                {
                    return null; // Not Found
                }
                return reference; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }


        public bool Add(Reference reference)
        {
            try
            {
                reference.StartDate = DateTime.Now;
                reference.EndDate = Constants.EndDate;
                reference.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                reference.CreationDate = DateTime.Now;

                db.References.Add(reference);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(Reference reference)
        {
            try
            {
                reference.RevisionDate = DateTime.Now;
                reference.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(reference).State = EntityState.Modified;
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
                Reference reference = db.References.Find(id);
                reference.EndDate = DateTime.Now;
                reference.ReferenceName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(reference).State = EntityState.Modified;
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