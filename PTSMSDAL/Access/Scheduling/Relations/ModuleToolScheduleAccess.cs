using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class ModuleToolScheduleAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.ModuleToolSchedules.ToList();
        }

        public object Details(int id)
        {
            try
            {
                ModuleToolSchedule moduleToolSchedule = db.ModuleToolSchedules.Find(id);
                if (moduleToolSchedule == null)
                {
                    return false; // Not Found
                }
                return moduleToolSchedule; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(ModuleToolSchedule moduleToolSchedule)
        {
            try
            {
                db.ModuleToolSchedules.Add(moduleToolSchedule);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(ModuleToolSchedule moduleToolSchedule)
        {
            try
            {
                db.Entry(moduleToolSchedule).State = EntityState.Modified;
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
                ModuleToolSchedule moduleToolSchedule = db.ModuleToolSchedules.Find(id);
                db.ModuleToolSchedules.Remove(moduleToolSchedule);
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