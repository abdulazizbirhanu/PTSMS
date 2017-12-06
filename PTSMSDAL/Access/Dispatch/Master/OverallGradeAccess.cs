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
    public class OverallGradeAccess
    {
        
             private PTSContext db = new PTSContext();

        public List<OverallGrade> List()
        {
            return db.OverallGrades.Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public OverallGrade Details(int id)
        {
            try
            {
                OverallGrade overallGrade = db.OverallGrades.Find(id);
                if (overallGrade == null)
                {
                    return null; // Not Found
                }
                return overallGrade; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(OverallGrade overallGrade)
        {
            try
            {

                overallGrade.StartDate = DateTime.Now;
                overallGrade.EndDate = Constants.EndDate;
                overallGrade.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                overallGrade.CreationDate = DateTime.Now;

                db.OverallGrades.Add(overallGrade);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(OverallGrade overallGrade)
        {
            try
            {
                overallGrade.RevisionDate = DateTime.Now;
                overallGrade.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(overallGrade).State = EntityState.Modified;
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
                OverallGrade overallGrade = db.OverallGrades.Find(id);
                overallGrade.EndDate = DateTime.Now;
                db.Entry(overallGrade).State = EntityState.Modified;
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
