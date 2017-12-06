using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSBAL.Logic.Curriculum.Relations
{
    public class LessonEvaluationTemplateLogic
    {
        LessonEvaluationTemplateAccess lessonEvaluationTemplateAccess = new LessonEvaluationTemplateAccess();

        public object List()
        {
            return lessonEvaluationTemplateAccess.List();
        }

        public object Details(int id)
        {
            return lessonEvaluationTemplateAccess.Details(id);
        }

        public object Add(LessonEvaluationTemplate lessonEvaluationTemplate)
        {
            return lessonEvaluationTemplateAccess.Add(lessonEvaluationTemplate);
        }

        public object Revise(LessonEvaluationTemplate lessonEvaluationTemplate)
        {
            return lessonEvaluationTemplateAccess.Revise(lessonEvaluationTemplate);
        }

        public object Delete(int id)
        {
            return lessonEvaluationTemplateAccess.Delete(id);
        }
    }
}