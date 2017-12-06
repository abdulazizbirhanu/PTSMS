using System.Collections.Generic;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMSBAL.Logic.Scheduling.Operations
{
    public class DayLogic
    {
        DayAccess dayAccess = new DayAccess();

        public List<Day> List()
        {
            return dayAccess.List();
        }

        public Day Details(int id)
        {
            return dayAccess.Details(id);
        }

        public bool Add(Day day)
        {
            day.Status = "Active";
            day.RevisionNo = 1;
            return dayAccess.Add(day);
        }

        public bool Revise(Day day)
        {
         
            Day d = (Day)dayAccess.Details(day.DayId);
            return dayAccess.Revise(d);
        }

        public bool Delete(int id)
        {
            return dayAccess.Delete(id);
        }
    }
}