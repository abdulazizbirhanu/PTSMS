using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEEEVALUATIONITEM")]
    public class TraineeEvaluationItemAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeEvaluationItemId { get;set; }

        [ForeignKey("TraineeEvaluationCategory")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeEvaluationCategoryId { get; set; }

        [ForeignKey("EvaluationItem")]
        [Index(IsUnique = true, Order = 2)]
        public int EvaluationItemId { get; set; }

        [Required(ErrorMessage = "Evaluation Item is required.")]
        [Display(Name = "Evaluation Item Score")]
        public float EvaluationItemScore { get; set; }

        public virtual TraineeEvaluationCategory TraineeEvaluationCategory { get; set; }
        public virtual EvaluationItem EvaluationItem { get; set; }
    }
}