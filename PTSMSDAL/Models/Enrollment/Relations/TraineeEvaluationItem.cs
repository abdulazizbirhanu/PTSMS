using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEEEVALUATIONITEM")]
    public class TraineeEvaluationItem : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeEvaluationItemId { get; set; }

        [ForeignKey("TraineeEvaluationCategory")]
        [Index("UK_TraineeEvaluationItem", IsUnique = true, Order = 1)]
        public int TraineeEvaluationCategoryId { get; set; }

        [Index("UK_TraineeEvaluationItem", IsUnique = true, Order = 2)]
        [StringLength(200)]
        public string EvaluationItemName { get; set; }

        [Display(Name = "Lesson Score")]
        [ForeignKey("LessonScore")]
        public int? LessonScoreId { get; set; }
        public int sequenceNo { get; set; }
        public virtual TraineeEvaluationCategory TraineeEvaluationCategory { get; set; }
        public virtual LessonScore LessonScore { get; set; }
    }
}