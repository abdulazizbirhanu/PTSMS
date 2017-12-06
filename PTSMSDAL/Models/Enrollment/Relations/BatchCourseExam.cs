using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_BATCHCOURSEEXAM")]
    public class BatchCourseExam : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchCourseExamId { get;set; }

        [ForeignKey("BatchCourse")]
        [Index("UK_BatchCourse", IsUnique = true, Order = 1)]
        public int BatchCourseId { get; set; }

        [ForeignKey("Exam")]
        [Index("UK_BatchCourse", IsUnique = true, Order = 2)]
        public int ExamId { get; set; }

        public virtual BatchCourse BatchCourse { get; set; }
        public virtual Exam Exam { get; set; }
    }
}