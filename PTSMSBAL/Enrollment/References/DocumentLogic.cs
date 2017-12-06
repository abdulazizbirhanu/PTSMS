using PTSMSDAL.Access.Enrollment.References;
using PTSMSDAL.Models.Enrollment.References;

namespace PTSMSBAL.Logic.Enrollment.References
{
    public class DocumentLogic
    {
        DocumentAccess documentAccess = new DocumentAccess();

        public object List()
        {
            return documentAccess.List();
        }

        public object Details(int id)
        {
            return documentAccess.Details(id);
        }

        public object Add(DocumentType document)
        {
            return documentAccess.Add(document);
        }

        public object Revise(DocumentType document)
        {
            return documentAccess.Revise(document);
        }

        public object Delete(int id)
        {
            return documentAccess.Delete(id);
        }
    }
}