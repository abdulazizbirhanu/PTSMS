using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEECOURSEEXAM")]
    public class TraineeCourseExam : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeCourseExamId { get;set; }
        
        [ForeignKey("TraineeCourse")]
        [Index("UK_TraineeCourseExam", IsUnique = true, Order = 1)]
        public int TraineeCourseId { get; set; }

        [ForeignKey("Exam")]
        [Index("UK_TraineeCourseExam", IsUnique = true, Order = 2)]
        public int ExamId { get; set; }
        
        [Required(ErrorMessage = "Exam Score is required.")]
        [Display(Name = "Exam Score")]
        public float ExamScore { get; set; }

        [Index("UK_TraineeCourseExam", IsUnique = true, Order = 3)]
        public int ReExamCount { get; set; }

        [ForeignKey("PassFailExamResult")]
        public int? PassFailExamResultId { get; set; }

        public virtual TraineeCourse TraineeCourse { get; set; }
        public virtual Exam Exam { get; set; }
        public virtual PassFailExamResult PassFailExamResult { get; set; }
    }
}