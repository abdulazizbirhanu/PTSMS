using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMSBAL.Logic.Enrollment.Operations
{
    public class PersonLogic
    {
        PersonAccess personAccess = new PersonAccess();

        public object List()
        {
            return personAccess.List();
        }

        public object Details(int id)
        {
            return personAccess.Details(id);
        }

        public object PersonDetail(string companyId)
        {
            return personAccess.PersonDetail(companyId);
        }

        public object Add(Person person)
        {
            person.Status = "Active";
            return personAccess.Add(person);
        }

        public object Revise(Person person)
        {
            return personAccess.Revise(person);
        }

        public object Delete(int id)
        {
            return personAccess.Delete(id);
        }
    }
}