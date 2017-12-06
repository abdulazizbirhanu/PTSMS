using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEEEVALUATIONCATEGORY")]
    public class TraineeEvaluationCategory : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeEvaluationCategoryId { get; set; }

        [ForeignKey("TraineeLesson")]
        [Index("UK_TraineeEvaluationCategory", IsUnique = true, Order = 1)]
        public int TraineeLessonId { get; set; }

        [Index("UK_TraineeEvaluationCategory", IsUnique = true, Order = 2)]
        [StringLength(100)]
        public string EvaluationCategoryName { get; set; }

        [Required(ErrorMessage = "Group Score is required.")]
        [Display(Name = "Group Score")]
        public float GroupScore { get; set; }
        public int sequenceNo { get; set; }

        public virtual TraineeLesson TraineeLesson { get; set; }
    }
}