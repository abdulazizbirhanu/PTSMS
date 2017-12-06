using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSBAL.Logic.Curriculum.Relations
{
    public class LessonReferenceLogic
    {
        LessonReferenceAccess lessonReferenceAccess = new LessonReferenceAccess();

        public object List()
        {
            return lessonReferenceAccess.List();
        }

        public object Details(int id)
        {
            return lessonReferenceAccess.Details(id);
        }

        public object Add(LessonReference lessonReference)
        {
            return lessonReferenceAccess.Add(lessonReference);
        }

        public object Revise(LessonReference lessonReference)
        {
            return lessonReferenceAccess.Revise(lessonReference);
        }

        public object Delete(int id)
        {
            return lessonReferenceAccess.Delete(id);
        }
    }
}