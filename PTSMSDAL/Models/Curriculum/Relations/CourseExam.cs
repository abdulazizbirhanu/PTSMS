using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_COURSEEXAM")]
    public class CourseExam : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseExamId { get; set; }

        [Index("UK_CourseExam", IsUnique = true, Order = 1)]
        [ForeignKey("CourseCategory")]
        public int CourseCategoryId { get; set; }
        
        [Index("UK_CourseExam", IsUnique = true, Order = 2)]
        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        
        public virtual CourseCategory CourseCategory { get; set; }
        public virtual Exam Exam { get; set; }
    }
}