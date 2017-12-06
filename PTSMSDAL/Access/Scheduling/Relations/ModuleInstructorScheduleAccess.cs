using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class ModuleInstructorScheduleAccess
    {
        private PTSContext db = new PTSContext();

        public List<ModuleInstructorSchedule> List()
        {
            return db.ModuleInstructorSchedules.ToList();
        }
        public List<ModuleInstructorSchedule> ListModuleInstructors(int instructorId, int moduleId)
        {
            return db.ModuleInstructorSchedules.Where(MIS => MIS.InstructorId == instructorId && MIS.ModuleId == moduleId).ToList();
        }
        public List<ModuleInstructorSchedule> ListInstructorModuleAssociation(int instructorId)
        {
            return db.ModuleInstructorSchedules.Where(MIS => MIS.InstructorId == instructorId).ToList();
        }

        public List<InstructorModuleView> ListAll()
        {
            var result = (from MIS in db.ModuleInstructorSchedules
                          join M in db.Modules on MIS.ModuleId equals (M.RevisionGroupId == null ? M.ModuleId : M.RevisionGroupId)
                          where M.Status.Equals("Active")
                          select new InstructorModuleView
                          {
                              InstructorId=MIS.InstructorId,
                              Instructor=MIS.Instructor,
                              ModuleId=M.ModuleId,
                              Module=M,
                              ModuleInstructorScheduleId=MIS.ModuleInstructorScheduleId
                          }).ToList();

            return result;
        }

        public List<InstructorQualification> GetInstructorsByQualification(string qualification)
        { 
            return db.InstructorQualifications
                .Where(IQ => IQ.QualificationType.Type.ToUpper().Contains(qualification.ToUpper()))
                .ToList();
        }

        public ModuleInstructorSchedule Details(int id)
        {
            try
            {
                ModuleInstructorSchedule moduleInstructorSchedule = db.ModuleInstructorSchedules.Find(id);
                if (moduleInstructorSchedule == null)
                {
                    return null; // Not Found
                }
                return moduleInstructorSchedule; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public object Add(ModuleInstructorSchedule moduleInstructorSchedule)
        {
            try
            {
                moduleInstructorSchedule.StartDate = DateTime.Now;
                moduleInstructorSchedule.EndDate = Constants.EndDate;
                moduleInstructorSchedule.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                moduleInstructorSchedule.CreationDate = DateTime.Now;

                db.ModuleInstructorSchedules.Add(moduleInstructorSchedule);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(ModuleInstructorSchedule moduleInstructorSchedule)
        {
            try
            {
                moduleInstructorSchedule.RevisionDate = DateTime.Now;
                moduleInstructorSchedule.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;
                db.Entry(moduleInstructorSchedule).State = EntityState.Modified;
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
                PTSContext db = new PTSContext();
                ModuleInstructorSchedule moduleInstructorSchedule = db.ModuleInstructorSchedules.Find(id);
                moduleInstructorSchedule.EndDate = DateTime.Now;
                moduleInstructorSchedule.CreatedBy += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(moduleInstructorSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }      
        /* Fisha*/
        public ModuleInstructorSchedule GetInstructorSchedule(int moduleId, int instructorId)
        {
            PTSContext db = new PTSContext();
            try
            {
                var result = db.ModuleInstructorSchedules.Where(ms => ms.InstructorId == instructorId && ms.ModuleId == moduleId).ToList();
                if (result.Count > 0)
                    return result.FirstOrDefault(); // Success
                return new ModuleInstructorSchedule();
            }
            catch (System.Exception e)
            {
                return new ModuleInstructorSchedule(); // Exception
            }
        }

        public List<ModuleInstructorSchedule> PotentialInstructorList(int moduleId)
        {
            PTSContext db = new PTSContext();
            var result = db.ModuleInstructorSchedules.Where(ins => ins.ModuleId == moduleId).ToList();
            if (result.Count > 0)
                return result.ToList();
            return new List<ModuleInstructorSchedule>();
        }
        public List<ModuleInstructorSchedule> List(int instructorId)
        {
            PTSContext db = new PTSContext();
            var result = db.ModuleInstructorSchedules.Where(ins => ins.InstructorId == instructorId).ToList();
            if (result.Count > 0)
                return result.ToList();
            return new List<ModuleInstructorSchedule>();
        }
        /* Fisha*/
    }
}