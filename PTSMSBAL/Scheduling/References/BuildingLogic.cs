using PTSMSDAL.Access.Scheduling.References;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSBAL.Logic.Scheduling.References
{
    public class BuildingLogic
    {
        BuildingAccess buildingAccess = new BuildingAccess();

        public object List()
        {
            return buildingAccess.List();
        }

        public object Details(int id)
        {
            return buildingAccess.Details(id);
        }

        public object Add(Building building)
        {
            return buildingAccess.Add(building);
        }

        public object Revise(Building building)
        {
            return buildingAccess.Revise(building);
        }

        public object Delete(int id)
        {
            return buildingAccess.Delete(id);
        }
    }
}