using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class ModuleScheduleAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.ModuleSchedules.ToList();
        }

        public object Details(int id)
        {
            try
            {
                ModuleSchedule moduleSchedule = db.ModuleSchedules.Find(id);
                if (moduleSchedule == null)
                {
                    return false; // Not Found
                }
                return moduleSchedule; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(ModuleSchedule moduleSchedule)
        {
            try
            {
                db.ModuleSchedules.Add(moduleSchedule);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public bool UpdateModuleSchedule(int moduleScheduleId, int periodId, int classRoomId, string date, int instructorId)
        {
            try
            {
                ModuleInstructorScheduleAccess moduleInstructorScheduleAccess = new ModuleInstructorScheduleAccess();

                ModuleSchedule moduleSchedule = (ModuleSchedule)Details(moduleScheduleId);
                
                moduleSchedule.PeriodId = periodId;
                moduleSchedule.ClassRoomId = classRoomId;
                moduleSchedule.InstructorId = instructorId;               
                moduleSchedule.Date = DateTime.ParseExact(date + " 12:00:00", "dd/MM/yyyy hh:mm:ss", CultureInfo.InstalledUICulture);
                db.Entry(moduleSchedule).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                    return true;// Success
                return false;
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(ModuleSchedule moduleSchedule)
        {
            try
            {
                db.Entry(moduleSchedule).State = EntityState.Modified;
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
                ModuleSchedule moduleSchedule = db.ModuleSchedules.Find(id);
                db.ModuleSchedules.Remove(moduleSchedule);
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