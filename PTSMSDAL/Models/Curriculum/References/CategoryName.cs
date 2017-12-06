using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.References
{
    [Table("REF_CATEGORYNAME")]
    public class CategoryName : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int CategoryNameId { get; set; }

        [Index("UK_CategoryName", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Category Name is required.")]
        [Display(Name = "Category Name")]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category Name Description is required.")]
        [Display(Name = "Category Name Description")]
        public string Description { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousCategoryName")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_CategoryName", IsUnique = true, Order = 2)]        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual CategoryName PreviousCategoryName { get; set; }
    }
}