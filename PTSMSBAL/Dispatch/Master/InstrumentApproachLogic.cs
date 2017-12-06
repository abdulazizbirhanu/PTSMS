using PTSMSDAL.Access.Dispatch.Master;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch.Master
{
   public class InstrumentApproachLogic
    {
        InstrumentApproachAccess instrumentApproachAccess = new InstrumentApproachAccess();

        public List<InstrumentApproach> List()
        {
            return instrumentApproachAccess.List();
        }

        public InstrumentApproach Details(int id)
        {
            return instrumentApproachAccess.Details(id);
        }

        public bool Add(InstrumentApproach instrumentApproach)
        {
            instrumentApproach.Status = "Active";
            instrumentApproach.RevisionNo = 1;
            return instrumentApproachAccess.Add(instrumentApproach);
        }

        public bool Revise(InstrumentApproach instrumentApproach)
        {
            InstrumentApproach d = (InstrumentApproach)instrumentApproachAccess.Details(instrumentApproach.InstrumentApproachId);
            return instrumentApproachAccess.Revise(d);
        }

        public bool Delete(int id)
        {
            return instrumentApproachAccess.Delete(id);
        }
    }
}

