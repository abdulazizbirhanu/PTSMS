using System;
using System.Collections.Generic;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMSBAL.Logic.Enrollment.Operations
{
    public class LicenseLogic
    {
        LicenseAccess licenseAccess = new LicenseAccess();

        public List<License> List()
        {
            return licenseAccess.List();
        }
        public List<License> ListExpiredLicence(DateTime expirationDate)
        {
            return licenseAccess.ListExpiredLicence(expirationDate);
        }
        public License Details(int id)
        {
            return licenseAccess.Details(id);
        }
        public List<License> GetLicense(int personId)
        {
            return licenseAccess.GetLicense(personId);
        }
        public object Add(License license)
        {
            return licenseAccess.Add(license);
        }

        public object Revise(License license)
        {
            return licenseAccess.Revise(license);
        }

        public object Delete(int id)
        {
            return licenseAccess.Delete(id);
        }
    }
}