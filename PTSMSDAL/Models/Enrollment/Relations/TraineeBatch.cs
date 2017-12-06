using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEEBATCH")]
    public class TraineeBatch : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchTraineeId {get;set;}

        [ForeignKey("Trainee")]
        public int TraineeId { get; set; }

        [ForeignKey("Batch")]
        public int BatchId { get; set; }

        public virtual Trainee Trainee { get; set; }
        public virtual Batch Batch { get; set; }
    }
}