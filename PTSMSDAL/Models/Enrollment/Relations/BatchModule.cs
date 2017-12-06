using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_BATCHMODULE")]
    public class BatchModule : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchModuleId { get;set; }
        
        [ForeignKey("BatchCourse")]
        [Index("UK_BatchModule", IsUnique = true, Order = 1)]
        public int BatchCourseId { get; set; }

        [ForeignKey("Module")]
        [Index("UK_BatchModule", IsUnique = true, Order = 2)]
        public int ModuleId { get; set; }
        [ForeignKey("Phase")]
        public int PhaseId { get; set; }
        public int Sequence { get; set; }
        public virtual BatchCourse BatchCourse { get; set; }
        public virtual Module Module { get; set; }
        public virtual Phase Phase { get; set; }
    }
}