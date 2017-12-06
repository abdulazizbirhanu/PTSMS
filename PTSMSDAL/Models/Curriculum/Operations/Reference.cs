using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("REFERENCE")]
    public class Reference : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int ReferenceId { get; set; }

        [Index("UK_Reference", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Reference Name is required.")]
        [Display(Name = "Reference Name")]
        [MaxLength(64)]
        public string ReferenceName { get; set; }

        [Required(ErrorMessage = "Reference Location is required.")]
        [Display(Name = "Reference Location")]
        public string ReferenceLocation { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousReference")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_Reference", IsUnique = true, Order = 2)]        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }
        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual Reference PreviousReference { get; set; }
    }
}