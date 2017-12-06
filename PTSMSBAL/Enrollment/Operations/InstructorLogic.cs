using System.Collections.Generic;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Relations;

namespace PTSMSBAL.Logic.Enrollment.Operations
{
    public class InstructorLogic
    {
        InstructorAccess instructorAccess = new InstructorAccess();

        public List<Instructor> List()
        {
            return instructorAccess.List();
        }

        public Instructor Details(int id)
        {
            return instructorAccess.Details(id);
        }

        public bool Add(Instructor instructor, List<InstructorQualification> instructorQualification)
        {
            return instructorAccess.Add(instructor, instructorQualification);
        }

        public object Revise(Instructor instructor)
        {
            return instructorAccess.Revise(instructor);
        }

        public object Delete(int id)
        {
            return instructorAccess.Delete(id);
        }

        public List<QualificationType> GetQulaificationType()
        {
            return instructorAccess.GetQulaificationType();
        }
    }
}