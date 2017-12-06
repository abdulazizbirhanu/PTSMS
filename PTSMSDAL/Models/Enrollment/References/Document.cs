using PTSMSDAL.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.References
{
    [Table("REF_DOCUMENT")]
    public class Document : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "Document Type is required.")]
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}