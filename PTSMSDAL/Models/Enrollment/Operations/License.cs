using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.References;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("LICENSE")]
    public class License : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LicenseId { get; set; }

        [ForeignKey("Person")]
        [Index("UK_License", IsUnique = true, Order = 1)]
        public int PersonId { get; set; }

        [ForeignKey("LicenseType")]
        [Index("UK_License", IsUnique = true, Order = 2)]
        public int LicenseTypeId { get; set; }

        [Required(ErrorMessage = "LicenseNo is required.")]
        [Display(Name = "LicenseNo")]
        public string LicenseNo { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "IssueDate is required.")]
        [Display(Name = "IssueDate")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Expiry Date is required.")]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "Threshold is required.")]
        [Display(Name = "Threshold")]
        public float Threshold { get; set; }

        [ForeignKey("PersonDocument")]
        public int? PersonDocumentId { get; set; }

        public virtual Person Person { get; set; }
        public virtual LicenseType LicenseType { get; set; }
        public virtual PersonDocument PersonDocument { get; set; }
    }

}