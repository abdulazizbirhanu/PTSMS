using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.Relations
{
    public class CourseCategoryAccess
    {
        private PTSContext db = new PTSContext();



        public object List()
        {
            return db.CourseCategories.ToList();
        }

        public object Details(int id)
        {
            try
            {
                CourseCategory courseExam = db.CourseCategories.Find(id);
                if (courseExam == null)
                {
                    return false; // Not Found
                }
                return courseExam;
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(CourseCategory courseExam)
        {
            try
            {
                db.CourseCategories.Add(courseExam);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(CourseCategory courseExam)
        {
            try
            {
                db.Entry(courseExam).State = EntityState.Modified;
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
                CourseCategory courseExam = db.CourseCategories.Find(id);
                db.CourseCategories.Remove(courseExam);
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