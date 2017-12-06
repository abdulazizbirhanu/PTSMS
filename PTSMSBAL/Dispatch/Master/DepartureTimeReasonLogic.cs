using PTSMSDAL.Access.Dispatch.Master;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch.Master
{
   public class DepartureTimeReasonLogic
    {
        DepartureTimeReasonAccess departureTimeReasonAccess = new DepartureTimeReasonAccess();

        public List<DepartureTimeReason> List()
        {
            return departureTimeReasonAccess.List();
        }

        public DepartureTimeReason Details(int id)
        {
            return departureTimeReasonAccess.Details(id);
        }

        public bool Add(DepartureTimeReason departureTimeReason)
        {
            departureTimeReason.Status = "Active";
            departureTimeReason.RevisionNo = 1;
            return departureTimeReasonAccess.Add(departureTimeReason);
        }

        public bool Revise(DepartureTimeReason departureTimeReason)
        {

            DepartureTimeReason d = (DepartureTimeReason)departureTimeReasonAccess.Details(departureTimeReason.DepartureTimeReasonId);
            return departureTimeReasonAccess.Revise(d);

        }

        public bool Delete(int id)
        {
            return departureTimeReasonAccess.Delete(id);
        }
    }
}

