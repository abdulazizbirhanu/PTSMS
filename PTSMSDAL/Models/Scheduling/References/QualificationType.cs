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
    [Table("QUALIFICATION_TYPE")]
    public class QualificationType : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QualificationTypeId { get; set; } 

        [Index(IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Qualification Type is required.")]
        [Display(Name = "Qualification Type")]
        [MaxLength(64)]
        public string Type { get; set; } 

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }    
    }
}
