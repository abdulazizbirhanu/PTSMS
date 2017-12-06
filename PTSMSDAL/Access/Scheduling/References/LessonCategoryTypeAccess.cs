using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.References
{
    public class LessonCategoryTypeAccess
    {
        private PTSContext db = new PTSContext();

        public List<LessonCategoryType> List()
        {
            return db.LessonCategoryTypes.ToList();
        }

        public LessonCategoryType Details(int id)
        {
            try
            {
                LessonCategoryType lessonCategoryType = db.LessonCategoryTypes.Find(id);
                if (lessonCategoryType == null)
                {
                    return new LessonCategoryType(); // Not Found
                }
                return lessonCategoryType; // Success
            }
            catch (Exception e)
            {
                return new LessonCategoryType(); // Exception
            }
        }

        public bool Add(LessonCategoryType lessonCategoryType)
        {
            try
            {
                lessonCategoryType.CreatedBy = "New";
                lessonCategoryType.CreationDate = DateTime.Now;
                lessonCategoryType.EndDate = DateTime.MaxValue;
                lessonCategoryType.RevisedBy = "New";
                lessonCategoryType.StartDate = DateTime.Now;
                lessonCategoryType.RevisionDate = DateTime.Now;
                db.LessonCategoryTypes.Add(lessonCategoryType);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(LessonCategoryType lessonCategoryType)
        {
            try
            {
                db.Entry(lessonCategoryType).State = EntityState.Modified;
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
                LessonCategoryType lessonCategoryType = db.LessonCategoryTypes.Find(id);
                db.LessonCategoryTypes.Remove(lessonCategoryType);
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
