using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_BATCHMODULEEXAM")]
    public class BatchModuleExam : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchModuleExamId { get; set; }

        [ForeignKey("BatchModule")]
        [Index("UK_BatchModuleExam", IsUnique = true, Order = 1)]
        public int BatchModuleId { get; set; }

        [ForeignKey("Exam")]
        [Index("UK_BatchModuleExam", IsUnique = true, Order = 2)]
        public int ExamId { get; set; }

        public virtual BatchModule BatchModule { get; set; }
        public virtual Exam Exam { get; set; }
    }
}