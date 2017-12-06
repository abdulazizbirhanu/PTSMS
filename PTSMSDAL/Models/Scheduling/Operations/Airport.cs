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
    [Table("AIRPORT")]
    public class Airport: AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AirportId { get; set; }

        [Required]
        [Display(Name = "Airport Name")]
        [Index("UK_AIRPORT", IsUnique = true, Order = 1)]
        [StringLength(200)]
        public string AirportName { get; set; }

        [Required]
        [Display(Name = "Airport Code")]
        [Index("UK_AIRPORT", IsUnique = true, Order = 2)]
        [StringLength(100)]
        public string AirportCode { get; set; }
        public string Status { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousAirport")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_AIRPORT", IsUnique = true, Order = 3)]

        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        public virtual Airport PreviousAirport { get; set; }
    }
}
