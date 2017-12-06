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
    [Table("PARKING_SPOT")]
    public class ParkingSpot: AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParkingSpotId { get; set; }

        [StringLength(100)]
        [Index("UK_PARKING_SPOT", IsUnique = true, Order = 1)]
        [Display(Name = "Parking Spot Name")]
        public string ParkingSpotName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousParkingSpot")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_PARKING_SPOT", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual ParkingSpot PreviousParkingSpot { get; set; }
    }
}
