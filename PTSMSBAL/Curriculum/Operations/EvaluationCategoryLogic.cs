using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMSBAL.Curriculum.Operations
{
    public class EvaluationCategoryLogic
    {
        EvaluationCategoryAccess evaluationCategoryAccess = new EvaluationCategoryAccess();

        public object List()
        {
            return evaluationCategoryAccess.List();
        }

        public object Details(int id)
        {
            return evaluationCategoryAccess.Details(id);
        }

        public object Add(EvaluationCategory evaluationCategory)
        {
            return evaluationCategoryAccess.Add(evaluationCategory);
        }

        public object Revise(EvaluationCategory evaluationCategory)
        {
            return evaluationCategoryAccess.Revise(evaluationCategory);
        }

        public object Delete(int id)
        {
            return evaluationCategoryAccess.Delete(id);
        }
    }
}