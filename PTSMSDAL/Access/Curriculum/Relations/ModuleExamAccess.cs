using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.Relations
{
    public class ModuleExamAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.ModuleExams.ToList();
        }

        public object Details(int id)
        {
            try
            {
                ModuleExam moduleExam = db.ModuleExams.Find(id);
                if (moduleExam == null)
                {
                    return false; // Not Found
                }
                return moduleExam; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(ModuleExam moduleExam)
        {
            try
            {
                db.ModuleExams.Add(moduleExam);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(ModuleExam moduleExam)
        {
            try
            {
                db.Entry(moduleExam).State = EntityState.Modified;
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
                ModuleExam moduleExam = db.ModuleExams.Find(id);
                db.ModuleExams.Remove(moduleExam);
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