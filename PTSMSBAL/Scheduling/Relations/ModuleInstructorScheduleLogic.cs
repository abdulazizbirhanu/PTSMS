using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using System.Collections.Generic;

namespace PTSMSBAL.Logic.Scheduling.Relations
{
    public class ModuleInstructorScheduleLogic
    {
        ModuleInstructorScheduleAccess moduleInstructorScheduleAccess = new ModuleInstructorScheduleAccess();

        public object List()
        {
            return moduleInstructorScheduleAccess.List();
        }

        public List<ModuleInstructorSchedule> ListModuleInstructors(int instructorId, int moduleId)
        {
            return moduleInstructorScheduleAccess.ListModuleInstructors(instructorId, moduleId); 
        }
        public List<ModuleInstructorSchedule> ListInstructorModuleAssociation(int instructorId)
        {
            return moduleInstructorScheduleAccess.ListInstructorModuleAssociation(instructorId);
        }

        public List<InstructorModuleView> ListAll()
        {
            return moduleInstructorScheduleAccess.ListAll();
        }

        public List<InstructorQualification> GetInstructorsByQualification(string qualification)
        { 
            return moduleInstructorScheduleAccess.GetInstructorsByQualification(qualification);
        }
        
        public ModuleInstructorSchedule Details(int id)
        {
            return moduleInstructorScheduleAccess.Details(id);
        }

        public object Add(ModuleInstructorSchedule moduleInstructorSchedule)
        {
            return moduleInstructorScheduleAccess.Add(moduleInstructorSchedule);
        }

        public object Revise(ModuleInstructorSchedule moduleInstructorSchedule)
        {
            return moduleInstructorScheduleAccess.Revise(moduleInstructorSchedule);
        }

        public object Delete(int id)
        {
            return moduleInstructorScheduleAccess.Delete(id);
        }
    }
}