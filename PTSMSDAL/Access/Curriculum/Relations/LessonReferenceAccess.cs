using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PTSMSDAL.Access.Curriculum.Relations
{
    public class LessonReferenceAccess
    {
        private PTSContext db = new PTSContext();

        public List<LessonReference> List()
        {
            return db.LessonReferences.ToList();
        }
        
        public List<LessonReference> GetLessonReferencesByLessonId(int lessonId)
        {
            return db.LessonReferences.Where(lr => lr.LessonId == lessonId).ToList();
        }
        public LessonReference GetLessonReferencesByLessonIdANdFileName(int lessonId, string fileName)
        {
            var references = db.LessonReferences.Where(mr => mr.LessonId == lessonId && mr.FileName == fileName).ToList();
            if (references.Count > 0)
                return references.FirstOrDefault();
            else
                return null;
        }

        public LessonReference Details(int id)
        {
            try
            {
                LessonReference lessonReference = db.LessonReferences.Find(id);
                return lessonReference; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(LessonReference lessonReference)
        {
            try
            {
                lessonReference.StartDate = DateTime.Now;
                lessonReference.EndDate = new DateTime(9999, 12, 31);
                lessonReference.CreationDate = DateTime.Now;
                lessonReference.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                lessonReference.RevisionDate = DateTime.Now;
                lessonReference.RevisedBy = HttpContext.Current.User.Identity.Name;

                db.LessonReferences.Add(lessonReference);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(LessonReference lessonReference)
        {
            try
            {
                db.Entry(lessonReference).State = EntityState.Modified;
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
                LessonReference lessonReference = db.LessonReferences.Find(id);
                db.LessonReferences.Remove(lessonReference);
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