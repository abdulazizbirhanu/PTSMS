using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEEBATCHCLASS")]
    public class TraineeBatchClass : AuditAttribute
    {
        [Key]
        public int TraineeBatchClassId { get;set; }

        [ForeignKey("Trainee")]
        [Index("UK_TraineeBatchClass", IsUnique = true, Order = 1)]
        public int TraineeId { get; set; }

        [ForeignKey("BatchClass")]
        [Index("UK_TraineeBatchClass", IsUnique = true, Order = 2)]
        public int BatchClassId { get; set; }

        public virtual Trainee Trainee { get; set; }
        public virtual BatchClass BatchClass { get; set; }
    }
}