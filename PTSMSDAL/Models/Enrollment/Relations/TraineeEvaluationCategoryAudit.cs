using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEEEVALUATIONCATEGORY")]
    public class TraineeEvaluationCategoryAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeEvaluationCategoryId { get;set; }

        [ForeignKey("TraineeEvaluationTemplate")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeEvaluationTemplateId { get; set; }

        [ForeignKey("EvaluationCategory")]
        [Index(IsUnique = true, Order = 2)]
        public int EvaluationCategoryId { get; set; }

        [Required(ErrorMessage = "Group Score is required.")]
        [Display(Name = "Group Score")]
        public float GroupScore { get; set; }

        public virtual TraineeEvaluationTemplate TraineeEvaluationTemplate { get; set; }
        public virtual EvaluationCategory EvaluationCategory { get; set; }
    }
}