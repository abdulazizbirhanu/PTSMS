using PTSMSDAL.Access.Dispatch.Master;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch.Master
{
 public   class DestinationLogic
    {
        DestinationAccess destinationAccess = new DestinationAccess();

        public List<Destination> List()
        {
            return destinationAccess.List();
        }

        public Destination Details(int id)
        {
            return destinationAccess.Details(id);
        }

        public bool Add(Destination destination)
        {
            destination.Status = "Active";
            destination.RevisionNo = 1;
            return destinationAccess.Add(destination);
        }

        public bool Revise(Destination destination)
        {
            Destination d = (Destination)destinationAccess.Details(destination.DestinationId);
            return destinationAccess.Revise(d);
        }

        public bool Delete(int id)
        {
            return destinationAccess.Delete(id);
        }
    }
}

