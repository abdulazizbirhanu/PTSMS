using PTSMSDAL.Access.Dispatch.Master;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Dispatch.Master
{
  public  class ParkingSpotLogic
    {
        ParkingSpotAccess parkingSpotAccess = new ParkingSpotAccess();

        public List<ParkingSpot> List()
        {
            return parkingSpotAccess.List();
        }

        public ParkingSpot Details(int id)
        {
            return parkingSpotAccess.Details(id);
        }

        public bool Add(ParkingSpot parkingSpot)
        {
            parkingSpot.Status = "Active";
            parkingSpot.RevisionNo = 1;
            return parkingSpotAccess.Add(parkingSpot);
        }

        public bool Revise(ParkingSpot parkingSpot)
        {
            ParkingSpot d = (ParkingSpot)parkingSpotAccess.Details(parkingSpot.ParkingSpotId);
            return parkingSpotAccess.Revise(d);
        }

        public bool Delete(int id)
        {
            return parkingSpotAccess.Delete(id);
        }
    }
}
