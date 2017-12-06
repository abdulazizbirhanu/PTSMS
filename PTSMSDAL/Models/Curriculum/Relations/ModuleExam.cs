using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_MODULEEXAM")]
    public class ModuleExam : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleExamId { get; set; }

        [Index("UK_ModuleExam", IsUnique = true, Order = 1)]
        [ForeignKey("CourseModule")]
        public int CourseModuleId { get; set; }        
        
        [Index("UK_ModuleExam", IsUnique = true, Order = 2)]
        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        
        public virtual CourseModule CourseModule { get; set; }
        public virtual Exam Exam { get; set; }
    }
}