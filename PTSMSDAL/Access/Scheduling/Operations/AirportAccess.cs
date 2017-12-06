using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class AirportAccess
    {
        public List<Airport> List()
        {
            PTSContext db = new PTSContext();
            return db.Airports.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public Airport Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                Airport airport = db.Airports.Find(id);
                if (airport == null)
                {
                    return null; // Not Found
                }
                return airport; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(Airport airport)
        {
            try
            {
                PTSContext db = new PTSContext();

                airport.StartDate = DateTime.Now;
                airport.EndDate = Constants.EndDate;
                airport.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                airport.CreationDate = DateTime.Now;
                db.Airports.Add(airport);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(Airport airport)
        {
            try
            {
                PTSContext db = new PTSContext();
                airport.RevisionDate = DateTime.Now;
                airport.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(airport).State = EntityState.Modified;
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
                Airport airport = db.Airports.Find(id);
                db.Airports.Remove(airport);
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
