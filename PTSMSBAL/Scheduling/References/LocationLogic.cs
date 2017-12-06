using PTSMSDAL.Access.Scheduling.References;
using PTSMSDAL.Models.Scheduling.References;
using System;

namespace PTSMSBAL.Logic.Scheduling.References
{
    public class LocationLogic
    {
        LocationAccess locationAccess = new LocationAccess();

        public object List()
        {
            return locationAccess.List();
        }

        public object Details(int id)
        {
            return locationAccess.Details(id);
        }

        public object Add(Location location)
        {
            return locationAccess.Add(location);
        }

        public object Revise(Location location)
        {
            Location loc = (Location)locationAccess.Details(location.LocationId);
            loc.Status = "Replaced";
            loc.LocationName = loc.LocationName + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss");

            location.RevisionNo = loc.RevisionNo + 1;
            location.Status = "Active";
            locationAccess.Revise(loc);

            return locationAccess.Add(location); 
        }

        public object Delete(int id)
        {
            return locationAccess.Delete(id);
        }
    }
}