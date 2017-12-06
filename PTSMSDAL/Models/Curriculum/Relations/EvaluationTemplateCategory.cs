using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_EVALUATIONTEMPLATECATEGORY")]
    public class EvaluationTemplateCategory : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EvaluationTemplateCategoryId { get; set; }

        [ForeignKey("LessonEvaluationTemplate")]
        [Index("UK_EvaluationTemplateCategory", IsUnique = true, Order = 1)]
        public int LessonEvaluationTemplateId { get; set; }
        
        [ForeignKey("EvaluationCategory")]
        [Index("UK_EvaluationTemplateCategory", IsUnique = true, Order = 2)]
        public int EvaluationCategoryId { get; set; }

        public virtual LessonEvaluationTemplate LessonEvaluationTemplate { get; set; }
        public virtual EvaluationCategory EvaluationCategory { get; set; }
    }
}