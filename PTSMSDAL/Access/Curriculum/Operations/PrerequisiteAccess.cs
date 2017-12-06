using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class PrerequisiteAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.Prerequisites.Where(c => c.EndDate > DateTime.Now).ToList();
        }

        public object Details(int id)
        {
            try
            {
                Prerequisite prerequisite = db.Prerequisites.Find(id);
                if (prerequisite == null)
                {
                    return false; // Not Found
                }
                return prerequisite; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public Course PrerequisiteDetailPartialView(int prerequisiteId)
        {
            try
            {
                var prerequisite = db.Prerequisites.Find(prerequisiteId);

                if (prerequisite != null)
                {
                    var course = db.Courses.Where(c => ((c.RevisionGroupId == null && c.CourseId == prerequisite.PrerequisiteCourseId)
                        || (c.RevisionGroupId != null && c.RevisionGroupId == prerequisite.PrerequisiteCourseId)) && c.Status == "Active").ToList();
                    if (course.Count > 0)
                        return course.FirstOrDefault();
                }
                return new Course();
            }
            catch (System.Exception e)
            {
                return new Course();
            }
        }

        public object Add(Prerequisite prerequisite)
        {
            try
            {
                prerequisite.StartDate = DateTime.Now;
                prerequisite.EndDate = Constants.EndDate;
                prerequisite.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                prerequisite.CreationDate = DateTime.Now;

                db.Prerequisites.Add(prerequisite);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Prerequisite prerequisite)
        {
            try
            {
                prerequisite.RevisionDate = DateTime.Now;
                prerequisite.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(prerequisite).State = EntityState.Modified;
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
                Prerequisite prerequisite = db.Prerequisites.Find(id);
                prerequisite.EndDate = DateTime.Now;
                db.Entry(prerequisite).State = EntityState.Modified;
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