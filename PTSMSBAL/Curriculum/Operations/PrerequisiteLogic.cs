using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMSBAL.Curriculum.Operations
{
    public class PrerequisiteLogic
    {
        PrerequisiteAccess prerequisiteAccess = new PrerequisiteAccess();

        public object List()
        {
            return prerequisiteAccess.List();
        }

        public object Details(int id)
        {
            return prerequisiteAccess.Details(id);
        }
        public Course PrerequisiteDetailPartialView(int prerequisiteId)
        {
            return prerequisiteAccess.PrerequisiteDetailPartialView(prerequisiteId);
        }

        public object Add(Prerequisite prerequisite)
        {
            return prerequisiteAccess.Add(prerequisite);
        }

        public object Revise(Prerequisite prerequisite)
        {
            return prerequisiteAccess.Revise(prerequisite);
        }

        public object Delete(int id)
        {
            return prerequisiteAccess.Delete(id);
        }
    }
}