using System.Collections.Generic;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMSBAL.Logic.Scheduling.Operations
{
    public class PeriodLogic
    {
        PeriodAccess periodAccess = new PeriodAccess();

        public List<Period> List()
        {
            return periodAccess.List();
        }

        public Period Details(int id)
        {
            return periodAccess.Details(id);
        }

        public bool Add(Period period)
        {

            period.Status = "Active";
            period.RevisionNo = 1;
            return periodAccess.Add(period);
        }

        public bool Revise(Period period)
        {
            Period per = (Period)periodAccess.Details(period.PeriodId);
            return periodAccess.Revise(per);
        }

        public bool Delete(int id)
        {
            return periodAccess.Delete(id);
        }
    }
}