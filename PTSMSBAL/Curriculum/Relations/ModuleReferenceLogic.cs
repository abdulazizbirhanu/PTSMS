using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSBAL.Logic.Curriculum.Relations
{
    public class ModuleReferenceLogic
    {
        ModuleReferenceAccess moduleReferenceAccess = new ModuleReferenceAccess();

        public object List()
        {
            return moduleReferenceAccess.List();
        }

        public object Details(int id)
        {
            return moduleReferenceAccess.Details(id);
        }

        public object Add(ModuleReference moduleReference)
        {
            return moduleReferenceAccess.Add(moduleReference);
        }

        public object Revise(ModuleReference moduleReference)
        {
            return moduleReferenceAccess.Revise(moduleReference);
        }

        public object Delete(int id)
        {
            return moduleReferenceAccess.Delete(id);
        }
    }
}