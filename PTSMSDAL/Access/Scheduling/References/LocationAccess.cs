using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Scheduling.References
{
    public class LocationAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.Locations.ToList();
        }

        public object Details(int id)
        {
            try
            {
                Location location = db.Locations.Find(id);
                if (location == null)
                {
                    return false; // Not Found
                }
                return location; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(Location location)
        {
            try
            {
                location.Status = "Active";
                location.RevisionNo = 1;
                location.StartDate = DateTime.Now;
                location.EndDate = Constants.EndDate;
                location.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                location.CreationDate = DateTime.Now;
                db.Locations.Add(location);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Location location)
        {
            try
            {

                location.RevisionDate = DateTime.Now;
                location.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(location).State = EntityState.Modified;
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
                Location location = db.Locations.Find(id);
                db.Locations.Remove(location);
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