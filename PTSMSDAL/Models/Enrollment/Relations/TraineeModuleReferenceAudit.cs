using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEEMODULEREFERENCE")]
    public class TraineeModuleReferenceAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeModuleReferenceId { get;set; }

        [ForeignKey("TraineeModule")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeModuleId { get; set; }

        [ForeignKey("Reference")]
        [Index(IsUnique = true, Order = 2)]
        public int ReferenceId { get; set; }
        
        public virtual TraineeModule TraineeModule { get; set; }
        public virtual Reference Reference { get; set; }
    }
}