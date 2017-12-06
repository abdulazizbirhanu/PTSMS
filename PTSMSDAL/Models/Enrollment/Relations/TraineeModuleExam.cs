using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEEMODULEEXAM")]
    public class TraineeModuleExam : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeModuleExamId { get; set; }

        [ForeignKey("TraineeModule")]
        [Index("UK_TraineeModuleExam", IsUnique = true, Order = 1)]
        public int TraineeModuleId { get; set; }

        [ForeignKey("Exam")]
        [Index("UK_TraineeModuleExam", IsUnique = true, Order = 2)]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "Exam Score is required.")]
        [Display(Name = "Exam Score")]
        public float ExamScore { get; set; }

        //[NotMapped]
        [Index("UK_TraineeModuleExam", IsUnique = true, Order = 3)]
        public int ReExamCount { get; set; }

        [ForeignKey("PassFailExamResult")]
        public int? PassFailExamResultId { get; set; }
        public virtual PassFailExamResult PassFailExamResult { get; set; }
        public virtual TraineeModule TraineeModule { get; set; }
        public virtual Exam Exam { get; set; }
    }
}