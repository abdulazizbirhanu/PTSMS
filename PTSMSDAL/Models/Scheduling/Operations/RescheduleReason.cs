using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("RESCHEDULE_REASON")]
    public class RescheduleReason: AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RescheduleReasonId { get; set; }

        [StringLength(100)]
        [Display(Name = "Reschedule Reason Name")]
        [Index("UK_RESCHEDULE_REASON", IsUnique = true, Order = 1)]
        public string RescheduleReasonName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousRescheduleReason")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_RESCHEDULE_REASON", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual RescheduleReason PreviousRescheduleReason { get; set; }
    }
}
