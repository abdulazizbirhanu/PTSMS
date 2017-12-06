using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Dispatch
{
   public class ArrivalTimeReasonAccess
    {
        public List<ArrivalTimeReason> List()
        {
            PTSContext db = new PTSContext();
            return db.ArrivalTimeReasons.Where(c => c.Status == "Active").ToList();
        }

        public ArrivalTimeReason Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                ArrivalTimeReason arrivalTimeReason = db.ArrivalTimeReasons.Find(id);
                if (arrivalTimeReason == null)
                {
                    return null; // Not Found
                }
                return arrivalTimeReason; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(ArrivalTimeReason arrivalTimeReason)
        {
            try
            {
                PTSContext db = new PTSContext();

               
                db.ArrivalTimeReasons.Add(arrivalTimeReason);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(ArrivalTimeReason arrivalTimeReason)
        {
            try
            {
                PTSContext db = new PTSContext();
               
                db.Entry(arrivalTimeReason).State = EntityState.Modified;
                db.SaveChanges();
                return true;
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
                PTSContext db = new PTSContext();
                ArrivalTimeReason arrivalTimeReason = db.ArrivalTimeReasons.Find(id);
                db.ArrivalTimeReasons.Remove(arrivalTimeReason);
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
