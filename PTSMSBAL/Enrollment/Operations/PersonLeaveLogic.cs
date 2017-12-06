using System;
using System.Collections.Generic;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMSBAL.Enrollment.Operations
{
    public class PersonLeaveLogic
    {
        PersonLeaveAccess personLeaveAccess = new PersonLeaveAccess();

        public List<PersonLeave> List()
        {
            return personLeaveAccess.List();
        }

        public PersonLeave Details(int id)
        {
            return personLeaveAccess.Details(id);
        }

        public object Add(PersonLeave personLeave)
        {
            return personLeaveAccess.Add(personLeave);
        }

        public object Revise(PersonLeave personLeave)
        {
            return personLeaveAccess.Revise(personLeave);
        }

        public object Delete(int id)
        {
            return personLeaveAccess.Delete(id);
        }

        public List<PersonLeave> GetLeave(int personId)
        {
            return personLeaveAccess.GetLeave(personId);
        }
        public List<PersonLeave> GetLeave(DateTime FromDate, DateTime ToDate)
        {
            return personLeaveAccess.GetLeave(FromDate, ToDate);
        }
    }
}
