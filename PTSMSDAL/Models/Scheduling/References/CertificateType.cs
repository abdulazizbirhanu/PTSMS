using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Scheduling.References
{
    [Table("CERTIFICATE_TYPE")]
    public class CertificateType : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CertificateTypeId { get; set; }

        [Required(ErrorMessage = "Certificate Type is required.")]
        [Display(Name = "Certificate Type")]
        public string Type { get; set; }


        [Display(Name = "Description")]
        public string Description { get; set; }   
    }
}
