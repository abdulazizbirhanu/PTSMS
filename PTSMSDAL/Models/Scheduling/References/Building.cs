using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.References
{
    [Table("REF_BUILDING")]
    public class Building : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BuildingId { get; set; }

        [Index("UK_REF_BUILDING", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Building Name is required.")]
        [Display(Name = "Building Name")]
        [MaxLength(64)]
        public string BuildingName { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousBuilding")]
        public int? PreviousRevisionId { get; set; }

        [Index("UK_REF_BUILDING", IsUnique = true, Order = 2)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public Building PreviousBuilding { get; set; }
    }
}