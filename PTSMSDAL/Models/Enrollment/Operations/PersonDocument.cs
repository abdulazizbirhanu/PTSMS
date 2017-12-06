using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("PERSONDOCUMENT")]
    public class PersonDocument : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonDocumentId { get; set; }

        [ForeignKey("DocumentType")]
        [Index("UK_PersonDocument", IsUnique = true, Order = 1)]
        public int DocumentTypeId { get; set; }

        [ForeignKey("Person")]
        [Index("UK_PersonDocument", IsUnique = true, Order = 2)]
        public int PersonId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Document URL is required.")]
        [Display(Name = "Document URL")]
        [DataType(DataType.ImageUrl)]
        public string DocumentURL { get; set; }

        public virtual DocumentType DocumentType { get; set; }
        public virtual Person Person { get; set; }
    }
}