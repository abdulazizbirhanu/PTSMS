using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class GroundLessonAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.GroundLessons.Include(c => c.PreviousGroundLesson).Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public object Details(int id)
        {
            try
            {
                GroundLesson groundLesson = db.GroundLessons.Find(id);
                if (groundLesson == null)
                {
                    return false; // Not Found
                }
                return groundLesson; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public GroundLesson GroundLessonsDetail(int moduleGroundLessonId)
        {
            try
            {
                ModuleGroundLesson moduleGroundLesson = db.ModuleGroundLessons.Find(moduleGroundLessonId);
                if (moduleGroundLesson != null)
                {
                    var groundLesson = db.GroundLessons.Where(c => ((c.RevisionGroupId == null && c.GroundLessonId == moduleGroundLesson.GroundLessonId)
                      || (c.RevisionGroupId != null && c.RevisionGroupId == moduleGroundLesson.GroundLessonId)) && c.Status == "Active").ToList();
                    if (groundLesson.Count > 0)
                        return groundLesson.FirstOrDefault();
                }
                return new GroundLesson();
            }
            catch (Exception e)
            {
                return new GroundLesson();
            }
        }
        


        public object Add(GroundLesson groundLesson)
        {
            try
            {
                groundLesson.StartDate = DateTime.Now;
                groundLesson.EndDate = Constants.EndDate;
                groundLesson.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                groundLesson.CreationDate = DateTime.Now;

                db.GroundLessons.Add(groundLesson);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(GroundLesson groundLesson)
        {
            try
            {
                groundLesson.RevisionDate = DateTime.Now;
                groundLesson.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(groundLesson).State = EntityState.Modified;
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
                GroundLesson groundLesson = db.GroundLessons.Find(id);
                groundLesson.EndDate = DateTime.Now;
                groundLesson.LessonCode += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(groundLesson).State = EntityState.Modified;
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
