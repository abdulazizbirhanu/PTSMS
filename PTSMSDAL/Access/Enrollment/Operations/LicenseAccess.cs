using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMSDAL.Access.Enrollment.Operations
{
    public class LicenseAccess
    {
        private PTSContext db = new PTSContext();

        public List<License> List()
        {
            return db.Licenses.ToList();
        }
        public List<License> ListExpiredLicence(DateTime expirationDate)
        {
            return db.Licenses.Where(l => l.ExpiryDate <= expirationDate).ToList();
        }
        public List<License> ListExpiredLicenceWithId(DateTime expirationDate, int PersonID)
        {
            return db.Licenses.Where(l => l.ExpiryDate <= expirationDate && l.PersonId == PersonID).ToList();
        }
        public License Details(int id)
        {
            try
            {
                License license = db.Licenses.Find(id);
                if (license == null)
                {
                    return null; // Not Found
                }
                return license; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public List<License> GetLicense(int personId)
        {
            try
            {
                var licenses = db.Licenses.Where(l => l.PersonId == personId && l.EndDate > DateTime.Now).ToList();

                if (licenses == null)
                {
                    return new List<License>();
                }
                return licenses;
            }
            catch (Exception e)
            {
                return new List<License>();
            }
        }

        public object Add(License license)
        {
            try
            {
                license.StartDate = DateTime.Now;
                license.EndDate = DateTime.MaxValue;
                license.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                license.CreationDate = DateTime.Now;
                db.Licenses.Add(license);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(License license)
        {
            try
            {
                license.RevisionDate = DateTime.Now;
                license.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(license).State = EntityState.Modified;
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
                License license = db.Licenses.Find(id);
                db.Licenses.Remove(license);
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