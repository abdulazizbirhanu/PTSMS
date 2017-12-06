using System.Collections.Generic;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMSBAL.Logic.Enrollment.Operations
{
    public class PersonDocumentLogic
    {
        PersonDocumentAccess traineeAccess = new PersonDocumentAccess();

        public List<PersonDocument> List()
        {
            return traineeAccess.List();
        }

        public PersonDocument Details(int id)
        {
            return traineeAccess.Details(id);
        }

        public object Add(PersonDocument trainee)
        {
            return traineeAccess.Add(trainee);
        }

        public object Revise(PersonDocument trainee)
        {
            return traineeAccess.Revise(trainee);
        }

        public object Delete(int id)
        {
            return traineeAccess.Delete(id);
        }
    }
}