using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSBAL.Logic.Curriculum.Relations
{
    public class CourseReferenceLogic
    {
        CourseReferenceAccess courseReferenceAccess = new CourseReferenceAccess();

        public object List()
        {
            return courseReferenceAccess.List();
        }

        public object Details(int id)
        {
            return courseReferenceAccess.Details(id);
        }

        public object Add(CourseReference courseReference)
        {
            return courseReferenceAccess.Add(courseReference);
        }

        public object Revise(CourseReference courseReference)
        {
            return courseReferenceAccess.Revise(courseReference);
        }

        public object Delete(int id)
        {
            return courseReferenceAccess.Delete(id);
        }
    }
}