using PTSMSDAL.Access.Dispatch;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch
{
   public class ArrivalTimeReasonLogic
    {
        ArrivalTimeReasonAccess arrivalTimeReasonAccess = new ArrivalTimeReasonAccess();

        public List<ArrivalTimeReason> List()
        {
            return arrivalTimeReasonAccess.List();
        }

        public ArrivalTimeReason Details(int id)
        {
            return arrivalTimeReasonAccess.Details(id);
        }

        public bool Add(ArrivalTimeReason arrivalTimeReason)
        {

            arrivalTimeReason.Status = "Active";
            arrivalTimeReason.RevisionNo = 1;
            return arrivalTimeReasonAccess.Add(arrivalTimeReason);
        }

        public bool Revise(ArrivalTimeReason arrivalTimeReason)
        {

            ArrivalTimeReason per = arrivalTimeReasonAccess.Details(arrivalTimeReason.ArrivalTimeReasonId);

            return arrivalTimeReasonAccess.Revise(per);

        }

        public bool Delete(int id)
        {
            return arrivalTimeReasonAccess.Delete(id);
        }
    }
}
