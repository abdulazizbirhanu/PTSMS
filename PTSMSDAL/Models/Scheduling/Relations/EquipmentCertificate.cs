using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("EQUIPMENT_CERTIFICATE")]
    public class EquipmentCertificate : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentLicenseId { get; set; } 


        [Required(ErrorMessage = "Equipment Id is required.")]
        [Index("UK_EQUIPMENT_CERTIFICATE", IsUnique = true, Order = 1)]
        [ForeignKey("Equipment")]
        [Display(Name = "Equipment Id")]
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Certificate Type is required.")]
        [Index("UK_EQUIPMENT_CERTIFICATE", IsUnique = true, Order = 2)]
        [ForeignKey("CertificateType")]
        [Display(Name = "Certificate Type")]
        public int CertificateTypeId { get; set; } 

        [Required]
        [Display(Name = "Starting Date")]
        public DateTime StartingDate { get; set; }

        [Required]
        [Display(Name = "Ending Date")]
        public DateTime EndingDate { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual CertificateType CertificateType { get; set; } 
        public virtual Equipment Equipment { get; set; }
    }
}
