using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Dispatch.Master
{
   public class DestinationAccess
    {
        private PTSContext db = new PTSContext();

        public List<Destination> List()
        {
            return db.Destinations.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public Destination Details(int id)
        {
            try
            {
                Destination destination = db.Destinations.Find(id);
                if (destination == null)
                {
                    return null; // Not Found
                }
                return destination; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(Destination destination)
        {
            try
            {

                destination.StartDate = DateTime.Now;
                destination.EndDate = Constants.EndDate;
                destination.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                destination.CreationDate = DateTime.Now;

                db.Destinations.Add(destination);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(Destination destination)
        {
            try
            {
                destination.RevisionDate = DateTime.Now;
                destination.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(destination).State = EntityState.Modified;
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
                Destination destination = db.Destinations.Find(id);
                destination.EndDate = DateTime.Now;
                db.Entry(destination).State = EntityState.Modified;
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
