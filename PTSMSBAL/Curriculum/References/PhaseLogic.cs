using PTSMSDAL.Access.Curriculum.References;
using PTSMSDAL.Models.Curriculum.References;

namespace PTSMSBAL.Curriculum.References
{
    public class PhaseLogic
    {
        PhaseAccess phaseAccess = new PhaseAccess();
                
        public object List()
        {
            return phaseAccess.List();
        }

        public object Details(int id)
        {
            return phaseAccess.Details(id);
        }

        public object Add(Phase phase)
        {
            return phaseAccess.Add(phase);
        }

        public object Revise(Phase phase)
        {                        
            return phaseAccess.Revise(phase);
        }

        public object Delete(int id)
        {
            return phaseAccess.Delete(id);
        }
    }
}