using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Dispatch.Master
{
    [Table("ARRIVAL_TIME_REASON")]
    public class ArrivalTimeReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Arrival Time Reason")]
        public int ArrivalTimeReasonId { get; set; }

        [StringLength(100)]
        [Display(Name = "Arrival Time Reason Name")]
        [Index("UK_ARRIVAL_TIME_REASON", IsUnique = true, Order = 1)]
        public string ArrivalTimeReasonName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousArrivalTimeReason")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_ARRIVAL_TIME_REASON", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual ArrivalTimeReason PreviousArrivalTimeReason { get; set; }
    }
}
