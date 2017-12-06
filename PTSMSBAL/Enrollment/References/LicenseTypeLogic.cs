using PTSMSDAL.Access.Enrollment.References;
using PTSMSDAL.Models.Enrollment.References;

namespace PTSMSBAL.Logic.Enrollment.References
{
    public class LicenseTypeLogic
    {
        LicenseTypeAccess licenseTypeAccess = new LicenseTypeAccess();

        public object List()
        {
            return licenseTypeAccess.List();
        }

        public object Details(int id)
        {
            return licenseTypeAccess.Details(id);
        }

        public object Add(LicenseType licenseType)
        {
            return licenseTypeAccess.Add(licenseType);
        }

        public object Revise(LicenseType licenseType)
        {
            return licenseTypeAccess.Revise(licenseType);


        }

        public object Delete(int id)
        {
            return licenseTypeAccess.Delete(id);
        }
    }
}