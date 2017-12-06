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
    public class DepartureTimeReasonAccess
    {
        private PTSContext db = new PTSContext();

        public List<DepartureTimeReason> List()
        {
            return db.DepartureTimeReasons.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public DepartureTimeReason Details(int id)
        {
            try
            {
                DepartureTimeReason departureTimeReason = db.DepartureTimeReasons.Find(id);
                if (departureTimeReason == null)
                {
                    return null; // Not Found
                }
                return departureTimeReason; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(DepartureTimeReason departureTimeReason)
        {
            try
            {

                departureTimeReason.StartDate = DateTime.Now;
                departureTimeReason.EndDate = Constants.EndDate;
                departureTimeReason.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                departureTimeReason.CreationDate = DateTime.Now;

                db.DepartureTimeReasons.Add(departureTimeReason);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(DepartureTimeReason departureTimeReason)
        {
            try
            {
                departureTimeReason.RevisionDate = DateTime.Now;
                departureTimeReason.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(departureTimeReason).State = EntityState.Modified;
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
                DepartureTimeReason departureTimeReason = db.DepartureTimeReasons.Find(id);
                departureTimeReason.EndDate = DateTime.Now;
                db.Entry(departureTimeReason).State = EntityState.Modified;
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
