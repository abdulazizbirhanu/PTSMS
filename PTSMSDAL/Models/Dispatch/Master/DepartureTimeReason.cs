using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Dispatch.Master
{
    [Table("DEPARTURE_TIME_REASON")]
    public class DepartureTimeReason : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartureTimeReasonId { get; set; }

        [StringLength(100)]
        [Display(Name = "Departure Time Reason")]
        [Index("UK_DEPARTURE_TIME_REASON", IsUnique = true, Order = 1)]
        public string DepartureTimeReasonName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousDepartureTimeReason")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_DEPARTURE_TIME_REASON", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual DepartureTimeReason PreviousDepartureTimeReason { get; set; }
    }
}
