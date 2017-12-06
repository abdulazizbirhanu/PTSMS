using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class ExamAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.Exams.Include(c => c.PreviousExam).Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public object Details(int id)
        {
            try
            {
                Exam exam = db.Exams.Find(id);
                if (exam == null)
                {
                    return false; // Not Found
                }
                return exam; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public Exam ExamDetail(int idNumber, string examType)
        {
            try
            {
                if (examType == "Course")
                {
                    CourseExam courseExam = db.CourseExams.Find(idNumber);
                    if (courseExam != null)
                    {
                        var exam = db.Exams.Where(c => ((c.RevisionGroupId == null && c.ExamId == courseExam.ExamId)
                          || (c.RevisionGroupId != null && c.RevisionGroupId == courseExam.ExamId)) && c.Status == "Active").ToList();
                        if (exam.Count > 0)
                            return exam.FirstOrDefault();
                    }
                }
                else
                {
                    ModuleExam moduleExam = db.ModuleExams.Find(idNumber);
                    if (moduleExam != null)
                    {
                        var exam = db.Exams.Where(c => ((c.RevisionGroupId == null && c.ExamId == moduleExam.ExamId)
                          || (c.RevisionGroupId != null && c.RevisionGroupId == moduleExam.ExamId)) && c.Status == "Active").ToList();
                        if (exam.Count > 0)
                            return exam.FirstOrDefault();
                    }
                }
                return new Exam();
            }
            catch (System.Exception e)
            {
                return new Exam();
            }
        }


        public object Add(Exam exam)
        {
            try
            {
                exam.StartDate = DateTime.Now;
                exam.EndDate = Constants.EndDate;
                exam.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                exam.CreationDate = DateTime.Now;

                db.Exams.Add(exam);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Exam exam)
        {
            try
            {
                exam.RevisionDate = DateTime.Now;
                exam.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(exam).State = EntityState.Modified;
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
                Exam exam = db.Exams.Find(id);
                exam.EndDate = DateTime.Now;
                exam.Name += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(exam).State = EntityState.Modified;
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