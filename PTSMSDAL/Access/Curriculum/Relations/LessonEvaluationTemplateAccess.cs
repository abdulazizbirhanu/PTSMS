using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.Relations
{
    public class LessonEvaluationTemplateAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.LessonEvaluationTemplates.ToList();
        }

        public object Details(int id)
        {
            try
            {
                LessonEvaluationTemplate lessonEvaluationTemplate = db.LessonEvaluationTemplates.Find(id);
                if (lessonEvaluationTemplate == null)
                {
                    return false; // Not Found
                }
                return lessonEvaluationTemplate; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(LessonEvaluationTemplate lessonEvaluationTemplate)
        {
            try
            {
                db.LessonEvaluationTemplates.Add(lessonEvaluationTemplate);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(LessonEvaluationTemplate lessonEvaluationTemplate)
        {
            try
            {
                db.Entry(lessonEvaluationTemplate).State = EntityState.Modified;
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
                LessonEvaluationTemplate lessonEvaluationTemplate = db.LessonEvaluationTemplates.Find(id);
                db.LessonEvaluationTemplates.Remove(lessonEvaluationTemplate);
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