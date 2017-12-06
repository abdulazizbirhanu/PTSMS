using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_LESSONEVALUATIONTEMPLATE")]
    public class LessonEvaluationTemplate : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonEvaluationTemplateId { get; set; }

        [ForeignKey("LessonCategory")]
        [Index("UK_LessonEvaluationTemplate", IsUnique = true, Order = 1)]
        public int LessonCategoryId { get; set; }
        
        [ForeignKey("EvaluationTemplate")]
        [Index("UK_LessonEvaluationTemplate", IsUnique = true, Order = 2)]
        public int EvaluationTemplateId { get; set; }
        
        public virtual LessonCategory LessonCategory { get; set; }
        public virtual EvaluationTemplate EvaluationTemplate { get; set; }
    }
}