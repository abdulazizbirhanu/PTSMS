using PTSMSDAL.Context;
using PTSMSDAL.Models.Others.School;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Others
{
    public class SchoolAccess
    {
        private PTSContext db = new PTSContext();
        public List<School> List()
        {
            return db.Schools.ToList();          
        }

        public School Details(int id)
        {
            try
            {
                return db.Schools.Find(id);
            }
            catch (Exception e)
            {
                return new School();
            }
        }


        public bool Add(School school)
        {
            try
            {
                db.Schools.Add(school);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Revise(School school)
        {
            try
            {
                School schoolObj = db.Schools.Find(school.SchoolId);
                schoolObj.SchoolCode = school.SchoolCode;
                schoolObj.SchoolName = school.SchoolName;
                schoolObj.DatabaseName = school.DatabaseName;
                schoolObj.Server = school.Server;
                schoolObj.Username = school.Username;
                schoolObj.Password = school.Password;
                schoolObj.RevisionDate = DateTime.Now;
                schoolObj.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(schoolObj).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public School SchoolDetails(string schoolCode)
        {
            try
            {
                return db.Schools.Where(s=>s.SchoolCode == schoolCode).ToList().FirstOrDefault();
            }
            catch (Exception e)
            {
                return new School();
            }
        }

        public bool Delete(int id)
        {
            try
            {
                School school = db.Schools.Find(id);
                db.Entry(school).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
