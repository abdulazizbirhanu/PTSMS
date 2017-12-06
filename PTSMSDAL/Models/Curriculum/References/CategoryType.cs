using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.References
{
    [Table("REF_CATEGORYTYPE")]
    public class CategoryType : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int CategoryTypeId { get; set; }

        [Index("UK_CategoryType", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Category Type is required.")]
        [Display(Name = "Category Type")]
        [MaxLength(64)]
        public string Type { get; set; }

        [Required(ErrorMessage = "Category Type Description is required.")]
        [Display(Name = "Category Type Description")]
        public string Description { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousCategoryType")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_CategoryType", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Is Type Rating")]
        public bool IsTypeRating { get; set; }

        public virtual CategoryType PreviousCategoryType { get; set; }
    }
}