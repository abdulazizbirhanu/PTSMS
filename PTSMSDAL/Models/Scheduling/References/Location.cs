using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.References
{
    [Table("REF_LOCATION")]
    public class Location : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationId { get; set; }

        [Index(IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Location Name is required.")]
        [Display(Name = "Location Name")]
        [MaxLength(128)]
        public string LocationName { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousLocation")]
        public int? PreviousRevisionId { get; set; }

        [Index(IsUnique = true, Order = 3)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public Location PreviousLocation { get; set; }
    }
}