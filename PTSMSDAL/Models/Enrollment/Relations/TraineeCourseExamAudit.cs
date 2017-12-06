using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEECOURSEEXAM")]
    public class TraineeCourseExamAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeCourseExamId { get;set; }
        
        [ForeignKey("TraineeCourse")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeCourseId { get; set; }

        [ForeignKey("Exam")]
        [Index(IsUnique = true, Order = 2)]
        public int ExamId { get; set; }
        
        [Required(ErrorMessage = "Exam Score is required.")]
        [Display(Name = "Exam Score")]
        public float ExamScore { get; set; }

        public virtual TraineeCourse TraineeCourse { get; set; }
        public virtual Exam Exam { get; set; }
    }
}