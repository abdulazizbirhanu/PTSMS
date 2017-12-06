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
    public class ArrivalTimeReasonAccess
    {
        private PTSContext db = new PTSContext();

        public List<ArrivalTimeReason> List()
        {
            return db.ArrivalTimeReasons.Where(c => c.Status == "Active" ).ToList();
        }

        public ArrivalTimeReason Details(int id)
        {
            try
            {
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

        public object Add(ArrivalTimeReason arrivalTimeReason)
        {
            try
            {
                

                db.ArrivalTimeReasons.Add(arrivalTimeReason);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(ArrivalTimeReason arrivalTimeReason)
        {
            try
            {
              
                db.Entry(arrivalTimeReason).State = EntityState.Modified;
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
                ArrivalTimeReason arrivalTimeReason = db.ArrivalTimeReasons.Find(id);
           
                db.Entry(arrivalTimeReason).State = EntityState.Modified;
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
