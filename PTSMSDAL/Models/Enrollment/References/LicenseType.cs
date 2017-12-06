using PTSMSDAL.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.References
{
    [Table("REF_LICENSETYPE")]
    public class LicenseType : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LicenseTypeId { get; set; }

        [Index(IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "License Type is required.")]
        [Display(Name = "License Type")]
        [MaxLength(64)]
        public string Type { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }        
    }
}