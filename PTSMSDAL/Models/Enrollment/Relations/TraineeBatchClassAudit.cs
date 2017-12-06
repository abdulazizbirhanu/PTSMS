using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEEBATCHCLASS")]
    public class TraineeBatchClassAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchTraineeId {get;set;}

        [ForeignKey("Trainee")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeId { get; set; }

        [ForeignKey("BatchClass")]
        [Index(IsUnique = true, Order = 2)]
        public int BatchClassId { get; set; }

        public virtual Trainee Trainee { get; set; }
        public virtual BatchClass BatchClass { get; set; }
    }
}