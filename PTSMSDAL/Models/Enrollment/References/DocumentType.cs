using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.References
{
    [Table("REF_DOCUMENT")]
    public class DocumentType : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        [Index(IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Document Type Name is required.")]
        [Display(Name = "Document Type Name")]
        [MaxLength(64)]
        public string DocumentTypeName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousDocument")]
        public int? PreviousRevisionId { get; set; }

        [Index(IsUnique = true, Order = 2)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public DocumentType PreviousDocument { get; set; }
    }
}