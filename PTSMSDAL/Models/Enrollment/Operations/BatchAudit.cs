using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("BATCHAUIDIT")]
    public class BatchAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchId { get; set; }

        [ForeignKey("Program")]
        [Index("UK_BatchAudit", IsUnique = true, Order = 1)]
        public int ProgramId { get; set; }

        [Index("UK_BatchAudit", IsUnique = true, Order = 2)]
        [Required(ErrorMessage = "Batch Name is required.")]
        [Display(Name = "Batch Name")]
        [MaxLength(32)]
        public string BatchName { get; set; }

        [Required(ErrorMessage = "Batch Start Date is required.")]
        [Display(Name = "Batch Start Date")]
        public DateTime BatchStartDate { get; set; }

        [Required(ErrorMessage = "Estimated End Date is required.")]
        [Display(Name = "Estimated End Date")]
        public DateTime EstimatedEndDate { get; set; }
        
        public virtual Program Program { get; set; }
    }
}