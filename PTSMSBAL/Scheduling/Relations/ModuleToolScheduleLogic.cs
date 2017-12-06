using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.Relations;

namespace PTSMSBAL.Logic.Scheduling.Relations
{
    public class ModuleToolScheduleLogic
    {
        ModuleToolScheduleAccess moduleToolScheduleAccess = new ModuleToolScheduleAccess();

        public object List()
        {
            return moduleToolScheduleAccess.List();
        }

        public object Details(int id)
        {
            return moduleToolScheduleAccess.Details(id);
        }

        public object Add(ModuleToolSchedule moduleToolSchedule)
        {
            return moduleToolScheduleAccess.Add(moduleToolSchedule);
        }

        public object Revise(ModuleToolSchedule moduleToolSchedule)
        {
            return moduleToolScheduleAccess.Revise(moduleToolSchedule);
        }

        public object Delete(int id)
        {
            return moduleToolScheduleAccess.Delete(id);
        }
    }
}