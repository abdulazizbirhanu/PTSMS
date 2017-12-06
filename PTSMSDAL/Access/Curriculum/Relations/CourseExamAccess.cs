using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;

namespace PTSMSDAL.Access.Curriculum.Relations
{
    public class CourseExamAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.CourseExams.Include(c => c.CourseCategory).Include(e => e.Exam).ToList();
        }

        public object Details(int id)
        {
            try
            {
                CourseExam courseExam = db.CourseExams.Find(id);
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

        public object Add(CourseExam courseExam)
        {
            try
            {
                db.CourseExams.Add(courseExam);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public List<CourseExam> CourseExamList(int courseId)
        {
            return db.CourseExams.Include(c => c.CourseCategory).Include(e => e.Exam).Where(ce => ce.CourseCategory.CourseId == courseId).ToList();
        }

        public object Revise(CourseExam courseExam)
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
                CourseExam courseExam = db.CourseExams.Find(id);
                db.CourseExams.Remove(courseExam);
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