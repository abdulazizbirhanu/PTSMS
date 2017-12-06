using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.Operations
{
    public class RescheduleReasonLogic
    {
        RescheduleReasonAccess rescheduleReasonAccess = new RescheduleReasonAccess();

        public List<RescheduleReason> List()
        {
            return rescheduleReasonAccess.List();
        }

        public RescheduleReason Details(int id)
        {
            return rescheduleReasonAccess.Details(id);
        }

        public bool Add(RescheduleReason rescheduleReason)
        {
            rescheduleReason.Status = "Active";
            rescheduleReason.RevisionNo = 1;
            return rescheduleReasonAccess.Add(rescheduleReason);
        }

        public bool Revise(RescheduleReason rescheduleReason)
        {

            RescheduleReason d = (RescheduleReason)rescheduleReasonAccess.Details(rescheduleReason.RescheduleReasonId);
            return rescheduleReasonAccess.Revise(d);
        }

        public bool Delete(int id)
        {
            return rescheduleReasonAccess.Delete(id);
        }
    }
}
