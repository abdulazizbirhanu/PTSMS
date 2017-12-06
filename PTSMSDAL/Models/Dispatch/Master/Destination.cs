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
    [Table("DESTINATION")]
    public class Destination : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DestinationId { get; set; }

        [StringLength(100)]       
        [Index("UK_DESTINATION", IsUnique = true, Order = 1)]
        [Display(Name = "Destination Name")]
        public string DestinationName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousDestination")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_DESTINATION", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual Destination PreviousDestination { get; set; }
    }
}
