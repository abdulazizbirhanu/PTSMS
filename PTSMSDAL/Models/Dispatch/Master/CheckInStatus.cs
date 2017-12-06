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
    [Table("CHECK_IN_STATUS")]
    public class CheckInStatus : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheckInStatusId { get; set; }

        [StringLength(100)]
        [Display(Name = "Check In Status Name")]
        [Index("UK_CHECK_IN_STATUS", IsUnique = true, Order = 1)]
        public string CheckInStatusName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousCheckInStatus")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_CHECK_IN_STATUS", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual CheckInStatus PreviousCheckInStatus { get; set; }
    }
}
