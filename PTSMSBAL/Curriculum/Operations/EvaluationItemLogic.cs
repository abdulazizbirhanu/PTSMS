using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMSBAL.Curriculum.Operations
{
    public class EvaluationItemLogic
    {
        EvaluationItemAccess evaluationItemAccess = new EvaluationItemAccess();

        public object List()
        {
            return evaluationItemAccess.List();
        }

        public object Details(int id)
        {
            return evaluationItemAccess.Details(id);
        }

        public object Add(EvaluationItem evaluationItem)
        {
            return evaluationItemAccess.Add(evaluationItem);
        }

        public object Revise(EvaluationItem evaluationItem)
        {
            return evaluationItemAccess.Revise(evaluationItem);
        }

        public object Delete(int id)
        {
            return evaluationItemAccess.Delete(id);
        }
    }
}