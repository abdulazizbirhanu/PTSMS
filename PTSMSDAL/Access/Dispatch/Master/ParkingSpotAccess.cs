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
    public class ParkingSpotAccess
    {
        private PTSContext db = new PTSContext();

        public List<ParkingSpot> List()
        {
            return db.ParkingSpots.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public ParkingSpot Details(int id)
        {
            try
            {
                ParkingSpot parkingSpot = db.ParkingSpots.Find(id);
                if (parkingSpot == null)
                {
                    return null; // Not Found
                }
                return parkingSpot; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(ParkingSpot parkingSpot)
        {
            try
            {

                parkingSpot.StartDate = DateTime.Now;
                parkingSpot.EndDate = Constants.EndDate;
                parkingSpot.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                parkingSpot.CreationDate = DateTime.Now;

                db.ParkingSpots.Add(parkingSpot);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(ParkingSpot parkingSpot)
        {
            try
            {
                parkingSpot.RevisionDate = DateTime.Now;
                parkingSpot.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(parkingSpot).State = EntityState.Modified;
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
                ParkingSpot parkingSpot = db.ParkingSpots.Find(id);
                parkingSpot.EndDate = DateTime.Now;
                db.Entry(parkingSpot).State = EntityState.Modified;
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
