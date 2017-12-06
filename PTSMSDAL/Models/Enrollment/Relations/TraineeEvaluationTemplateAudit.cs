using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEEEVALUATIONTEMPLATE")]
    public class TraineeEvaluationTemplateAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeEvaluationTemplateId { get;set; }

        [ForeignKey("TraineeLesson")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeLessonId { get; set; }

        [ForeignKey("EvaluationTemplate")]
        [Index(IsUnique = true, Order = 2)]
        public int EvaluationTemplateId { get; set; }

        [Required(ErrorMessage = "Aggregated Score is required.")]
        [Display(Name = "Aggregated Score")]
        public float AggregatedScore { get; set; }

        public virtual TraineeLesson TraineeLesson { get; set; }
        public virtual EvaluationTemplate EvaluationTemplate { get; set; }
    }
}