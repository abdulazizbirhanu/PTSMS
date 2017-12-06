using PTSMSDAL.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("BATCHCLASSAUDIT")]
    public class BatchClassAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchClassId { get; set; }

        [ForeignKey("Batch")]
        [Index("UK_BatchClassAudit", IsUnique = true, Order = 1)]
        public int BatchId { get; set; }

        [Index("UK_BatchClassAudit", IsUnique = true, Order = 2)]
        [Required(ErrorMessage = "Batch Class Name is required.")]
        [Display(Name = "Batch Class Name")]
        [MaxLength(32)]
        public string BatchClassName { get; set; }
                
        public virtual Batch Batch { get; set; }
    }
}