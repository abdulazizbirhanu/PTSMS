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
    public class InstrumentApproachAccess
    {
        private PTSContext db = new PTSContext();

        public List<InstrumentApproach> List()
        {
            return db.InstrumentApproachs.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public InstrumentApproach Details(int id)
        {
            try
            {
                InstrumentApproach instrumentApproach = db.InstrumentApproachs.Find(id);
                if (instrumentApproach == null)
                {
                    return null; // Not Found
                }
                return instrumentApproach; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(InstrumentApproach instrumentApproach)
        {
            try
            {

                instrumentApproach.StartDate = DateTime.Now;
                instrumentApproach.EndDate = Constants.EndDate;
                instrumentApproach.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                instrumentApproach.CreationDate = DateTime.Now;

                db.InstrumentApproachs.Add(instrumentApproach);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(InstrumentApproach instrumentApproach)
        {
            try
            {
                instrumentApproach.RevisionDate = DateTime.Now;
                instrumentApproach.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(instrumentApproach).State = EntityState.Modified;
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
                InstrumentApproach instrumentApproach = db.InstrumentApproachs.Find(id);
                instrumentApproach.EndDate = DateTime.Now;
                db.Entry(instrumentApproach).State = EntityState.Modified;
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

