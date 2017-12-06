using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEEMODULEREFERENCE")]
    public class TraineeModuleReference : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeModuleReferenceId { get;set; }

        [ForeignKey("TraineeModule")]
        [Index("UK_TraineeModuleReference", IsUnique = true, Order = 1)]
        public int TraineeModuleId { get; set; }

        [ForeignKey("Reference")]
        [Index("UK_TraineeModuleReference", IsUnique = true, Order = 2)]
        public int ReferenceId { get; set; }
        
        public virtual TraineeModule TraineeModule { get; set; }
        public virtual Reference Reference { get; set; }
    }
}