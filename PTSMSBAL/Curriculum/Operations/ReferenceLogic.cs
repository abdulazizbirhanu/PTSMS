using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;
using System.Collections.Generic;

namespace PTSMSBAL.Curriculum.Operations
{
    public class ReferenceLogic
    {
        ReferenceAccess referenceAccess = new ReferenceAccess();

        public List<Reference> List()
        {
            return referenceAccess.List();
        }

        public Reference Details(int id)
        {
            return referenceAccess.Details(id);
        }

        public bool Add(Reference reference)
        {
            reference.Status = "Active";
            reference.RevisionNo = 1;
            return referenceAccess.Add(reference);
        }

        public bool Revise(Reference reference)
        {
            Reference refer = (Reference)referenceAccess.Details(reference.ReferenceId);
            return referenceAccess.Revise(refer);
        }

        public bool Delete(int id)
        {
            return referenceAccess.Delete(id);
        }
    }
}