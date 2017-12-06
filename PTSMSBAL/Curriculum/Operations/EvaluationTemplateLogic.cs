using System;
using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.View;
using System.Collections.Generic;

namespace PTSMSBAL.Curriculum.Operations
{
    public class EvaluationTemplateLogic
    {
        EvaluationTemplateAccess evaluationTemplateAccess = new EvaluationTemplateAccess();

        public List<EvaluationTemplate> List()
        {
            return evaluationTemplateAccess.List();
        }

        public object Details(int id)
        {
            return evaluationTemplateAccess.Details(id);
        }
        public EvaluationTemplateView EvaluationTemplateDetail(int lessonEvaluationTemplateId)
        {
            return evaluationTemplateAccess.EvaluationTemplateDetail(lessonEvaluationTemplateId);
        }       

        public bool Add(EvaluationTemplate evaluationTemplate)
        {
            evaluationTemplate.Status = "Active";
            evaluationTemplate.RevisionNo = 1;
            return evaluationTemplateAccess.Add(evaluationTemplate);
        }

        public bool Revise(EvaluationTemplate evaluationTemplate)
        {
            EvaluationTemplate eval = (EvaluationTemplate)evaluationTemplateAccess.Details(evaluationTemplate.EvaluationTemplateId);
            return evaluationTemplateAccess.Revise(eval);
        }

        public bool Delete(int id)
        {
            return evaluationTemplateAccess.Delete(id);
        }

        public EvaluationTemplateView GetLessonEvaluationTemplate(int lessonId)
        {
            var lessonEvaluationTemplate = evaluationTemplateAccess.GetLessonEvaluationTemplate(lessonId);
            return evaluationTemplateAccess.EvaluationTemplateDetail(lessonEvaluationTemplate.LessonEvaluationTemplateId);
        }
    }
}