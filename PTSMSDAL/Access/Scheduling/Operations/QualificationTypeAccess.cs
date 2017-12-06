using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class QualificationTypeAccess
    {

        public List<QualificationType> List()
        {
            PTSContext db = new PTSContext();
            return db.QualificationTypes.ToList();
        }

        public QualificationType Details(int id)
        {
            try
            {
                PTSContext db = new PTSContext();
                QualificationType qualificationType = db.QualificationTypes.Find(id);
                if (qualificationType == null)
                {
                    return null; // Not Found
                }
                return qualificationType; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(QualificationType qualificationType)
        {
            try
            {
                PTSContext db = new PTSContext();
                qualificationType.StartDate = DateTime.Now;
                qualificationType.EndDate = new DateTime(9999, 12, 31);
                qualificationType.CreationDate = DateTime.Now;
                qualificationType.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                qualificationType.RevisionDate = DateTime.Now;
                qualificationType.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;
                db.QualificationTypes.Add(qualificationType);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(QualificationType qualificationType)
        {
            try
            {
                PTSContext db = new PTSContext();
                qualificationType.RevisionDate = DateTime.Now;
                qualificationType.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(qualificationType).State = EntityState.Modified;
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
                PTSContext db = new PTSContext();
                QualificationType qualificationType = db.QualificationTypes.Find(id);
                db.QualificationTypes.Remove(qualificationType);
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
