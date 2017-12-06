using PTSMSDAL.Access.Curriculum.References;
using PTSMSDAL.Models.Curriculum.References;

namespace PTSMSBAL.Curriculum.References
{
    public class StageLogic
    {
        StageAccess stageAccess = new StageAccess();
                
        public object List()
        {
            return stageAccess.List();
        }

        public object Details(int id)
        {
            return stageAccess.Details(id);
        }

        public object Add(Stage stage)
        {
            return stageAccess.Add(stage);
        }

        public object Revise(Stage stage)
        {                        
            return stageAccess.Revise(stage);
        }

        public object Delete(int id)
        {
            return stageAccess.Delete(id);
        }
    }
}