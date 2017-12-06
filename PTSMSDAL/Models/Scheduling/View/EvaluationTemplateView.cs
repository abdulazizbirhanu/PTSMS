using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.View
{
    public class TraineeEvaluationTemplateView
    {
        public TraineeEvaluationTemplateView()
        {
            this.EvaluationCategory = new List<EvaluationCategoriesView>();
            this.LessonScores = new List<LessonScoreView>();
        }
        public int Id { get; set; }
        public string Name { get; set; }           
        public string Remark { get; set; }
        public int? OverallGradeId { get; set; }
        public bool IsEvaluated { get; set; }
        public List<LessonScoreView> LessonScores { get; set; }
        public List<EvaluationCategoriesView> EvaluationCategory { get; set; }
    }
    public class EvaluationCategoriesView
    {
        public EvaluationCategoriesView()
        {
            this.EvaluationItem = new List<EvaluationItemsView>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int sequenceNo { get; set; }
        public List<EvaluationItemsView> EvaluationItem { get; set; }
    }
    public class EvaluationItemsView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ScoreLevelId { get; set; }
    }

    public class LessonScoreView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Score { get; set; }
    }

}
