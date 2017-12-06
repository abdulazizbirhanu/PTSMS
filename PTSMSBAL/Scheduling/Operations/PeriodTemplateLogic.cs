using System.Collections.Generic;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMSBAL.Logic.Scheduling.Operations
{
    public class PeriodTemplateLogic
    {
        PeriodTemplateAccess periodTemplateAccess = new PeriodTemplateAccess();

        public List<PeriodTemplate> List()
        {
            return periodTemplateAccess.List();
        }

        public PeriodTemplate Details(int id)
        {
            return periodTemplateAccess.Details(id);
        }

        public bool Add(PeriodTemplate periodTemplate)
        {
            periodTemplate.Status = "Active";
            periodTemplate.RevisionNo = 1;
            return periodTemplateAccess.Add(periodTemplate);
        }

        public bool Revise(PeriodTemplate PeriodTemplate)
        {
            PeriodTemplate per = (PeriodTemplate)periodTemplateAccess.Details(PeriodTemplate.PeriodTemplateId);
            return periodTemplateAccess.Revise(per);
        }

        public bool Delete(int id)
        {
            return periodTemplateAccess.Delete(id);
        }
    }
}