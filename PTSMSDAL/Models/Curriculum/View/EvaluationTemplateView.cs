using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Curriculum.View
{
    public class EvaluationTemplateView
    {
        public EvaluationTemplateView()
        {
            this.EvaluationTemplate = new EvaluationTemplate();
            this.EvaluationCategories = new List<EvaluationCategoryView>();
        }
        public EvaluationTemplate EvaluationTemplate { get; set; }
        public List<EvaluationCategoryView> EvaluationCategories { get; set; }
    }
    public class EvaluationCategoryView
    {
        public EvaluationCategoryView()
        {
            this.EvaluationCategory = new EvaluationCategory();
            this.EvaluationItems = new List<EvaluationItem>();
        }
        public EvaluationCategory EvaluationCategory { get; set; }
        public List<EvaluationItem> EvaluationItems { get; set; }
    }
}
