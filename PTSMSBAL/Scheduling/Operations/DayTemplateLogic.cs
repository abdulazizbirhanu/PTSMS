using System.Collections.Generic;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMSBAL.Logic.Scheduling.Operations
{
    public class DayTemplateLogic
    {
        DayTemplateAccess dayTemplateAccess = new DayTemplateAccess();

        public List<DayTemplate> List()
        {
            return dayTemplateAccess.List();
        }

        public DayTemplate Details(int id)
        {
            return dayTemplateAccess.Details(id);
        }

        public bool Add(DayTemplate dayTemplate)
        {

            dayTemplate.Status = "Active";
            dayTemplate.RevisionNo = 1;
            return dayTemplateAccess.Add(dayTemplate);
        }

        public bool Revise(DayTemplate dayTemplate)
        {
            DayTemplate Dtemp = ( DayTemplate )dayTemplateAccess.Details(dayTemplate.DayTemplateId);
            return dayTemplateAccess.Revise(Dtemp);
    }

        public bool Delete(int id)
        {
            return dayTemplateAccess.Delete(id);
        }
    }
}