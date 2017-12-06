using PTSMSDAL.Access.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.References
{
    public class LessonCategoryTypeLogic
    {
        LessonCategoryTypeAccess lessonCategoryTypeAccess = new LessonCategoryTypeAccess();

        public List<LessonCategoryType> List()
        {
            return lessonCategoryTypeAccess.List();
        }

        public LessonCategoryType Details(int id)
        {
            return lessonCategoryTypeAccess.Details(id);
        }

        public bool Add(LessonCategoryType lessonCategoryType)
        {
            return lessonCategoryTypeAccess.Add(lessonCategoryType);
        }

        public bool Revise(LessonCategoryType lessonCategoryType)
        {
            return lessonCategoryTypeAccess.Revise(lessonCategoryType);
        }

        public bool Delete(int id)
        {
            return lessonCategoryTypeAccess.Delete(id);
        }
    }
}
