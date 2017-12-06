using PTSMSDAL.Access.Others;
using PTSMSDAL.Models.Others.School;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace PTSMSBAL.Others
{
    public class SchoolLogic
    {
        SchoolAccess schoolAccess = new SchoolAccess();

        public List<School> List()
        {
            return schoolAccess.List();
        }

        public School Details(int id)
        {
            return schoolAccess.Details(id);
        }

        public bool Add(School school)
        {
            school.CreationDate = DateTime.Now;
            school.RevisionDate = DateTime.Now;
            school.StartDate = DateTime.Now;
            school.EndDate = DateTime.MaxValue;
            return schoolAccess.Add(school);
        }

        public bool Revise(School school)
        {
            return schoolAccess.Revise(school);
        }

        public bool Delete(int id)
        {
            return schoolAccess.Delete(id);
        }
        public bool ChangeConnectionStringInWebConfig(int schoolId)
        {
            try
            {
                School school = schoolAccess.Details(schoolId);
                if (school != null)
                {
                    //string path = System.Web.HttpContext.Current.Server.MapPath("~/Web.Config");
                    string path = System.Web.HttpContext.Current.Server.MapPath("~/Web.config");
                    var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                    var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");

                    string connectionString = "Data Source=" + school.Server + ";Initial Catalog=" + school.DatabaseName + ";User ID=" + school.Username + ";Password=" + school.Password + "";

                    section.ConnectionStrings["PTSContext"].ConnectionString = connectionString;
                    configuration.Save();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
