using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEELESSON")]
    public class TraineeLessonAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeLessonId { get;set; }

        [ForeignKey("TraineeCategory")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeCategoryId { get; set; }

        [ForeignKey("Lesson")]
        [Index(IsUnique = true, Order = 2)]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Lesson Score is required.")]
        [Display(Name = "Lesson Score")]
        public float LessonScore { get; set; }

        public virtual TraineeCategory TraineeCategory { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}