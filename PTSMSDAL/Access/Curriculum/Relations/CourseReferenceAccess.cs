using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.Relations
{
    public class CourseReferenceAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.CourseReferences.ToList();
        }

        public object Details(int id)
        {
            try
            {
                CourseReference courseReference = db.CourseReferences.Find(id);
                if (courseReference == null)
                {
                    return false; // Not Found
                }
                return courseReference; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(CourseReference courseReference)
        {
            try
            {
                db.CourseReferences.Add(courseReference);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(CourseReference courseReference)
        {
            try
            {
                db.Entry(courseReference).State = EntityState.Modified;
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
                CourseReference courseReference = db.CourseReferences.Find(id);
                db.CourseReferences.Remove(courseReference);
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