using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Curriculum.Operations
{
    public class GroundLessonLogic
    {
        GroundLessonAccess groundLessonAccess = new GroundLessonAccess();
         
        public object List()
        {
            return groundLessonAccess.List();
        }

        public object Details(int id)
        {
            return groundLessonAccess.Details(id);
        }
        public GroundLesson GroundLessonsDetail(int moduleGroundLessonsId)
        {
            return groundLessonAccess.GroundLessonsDetail(moduleGroundLessonsId);
        }

        

        public object Add(GroundLesson groundLesson)
        {
            groundLesson.Status = "Active";
            groundLesson.RevisionNo = 1;
            return groundLessonAccess.Add(groundLesson);
        }

        public object Revise(GroundLesson groundLesson)
        {
            GroundLesson grndLesson = (GroundLesson)groundLessonAccess.Details(groundLesson.GroundLessonId);
            grndLesson.Status = "Replaced";

            groundLesson.RevisionNo = grndLesson.RevisionNo + 1;
            groundLesson.Status = "Active";

            if (grndLesson.RevisionGroupId == null)
                groundLesson.RevisionGroupId = groundLesson.GroundLessonId;
            else
                groundLesson.RevisionGroupId = grndLesson.RevisionGroupId;

            groundLessonAccess.Revise(grndLesson);

            return groundLessonAccess.Add(groundLesson);
        }

        public object Delete(int id)
        {
            return groundLessonAccess.Delete(id);
        }
    }
}
