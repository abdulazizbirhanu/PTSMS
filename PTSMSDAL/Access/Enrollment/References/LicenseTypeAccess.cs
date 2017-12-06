using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.References;
using System;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Enrollment.References
{
    public class LicenseTypeAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.LicenseTypes.ToList();
        }

        public object Details(int id)
        {
            try
            {
                LicenseType licenseType = db.LicenseTypes.Find(id);
                if (licenseType == null)
                {
                    return false; // Not Found
                }
                return licenseType; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(LicenseType licenseType)
        {
            try
            {
                licenseType.StartDate = DateTime.Now;
                licenseType.EndDate = Constants.EndDate;
                licenseType.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                licenseType.CreationDate = DateTime.Now;
                db.LicenseTypes.Add(licenseType);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(LicenseType licenseType)
        {
            try
            {
                db.Entry(licenseType).State = EntityState.Modified;
                licenseType.RevisionDate = DateTime.Now;
                licenseType.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
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
                LicenseType licenseType = db.LicenseTypes.Find(id);
                db.LicenseTypes.Remove(licenseType);
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