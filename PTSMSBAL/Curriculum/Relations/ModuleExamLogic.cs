using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSBAL.Logic.Curriculum.Relations
{
    public class ModuleExamLogic
    {
        ModuleExamAccess moduleExamAccess = new ModuleExamAccess();

        public object List()
        {
            return moduleExamAccess.List();
        }

        public object Details(int id)
        {
            return moduleExamAccess.Details(id);
        }

        public object Add(ModuleExam moduleExam)
        {
            return moduleExamAccess.Add(moduleExam);
        }

        public object Revise(ModuleExam moduleExam)
        {
            return moduleExamAccess.Revise(moduleExam);
        }

        public object Delete(int id)
        {
            return moduleExamAccess.Delete(id);
        }
    }
}