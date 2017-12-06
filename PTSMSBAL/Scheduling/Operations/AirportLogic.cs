using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.Operations
{
    public class AirportLogic
    {
        AirportAccess airportAccess = new AirportAccess();

        public List<Airport> List()
        {
            return airportAccess.List();
        }

        public Airport Details(int id)
        {
            return airportAccess.Details(id);
        }

        public bool Add(Airport airport)
        {

            airport.Status = "Active";
            airport.RevisionNo = 1;
            return airportAccess.Add(airport);
        }

        public bool Revise(Airport airport)
        {
            Airport per = airportAccess.Details(airport.AirportId);
            return airportAccess.Revise(per);
        }

        public bool Delete(int id)
        {
            return airportAccess.Delete(id);
        }
    }
}
