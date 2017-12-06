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
    [Table("EQUIPMENT_STATUS")]
    public class EquipmentStatus : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentStatusId { get; set; } 

        [Required(ErrorMessage = "Equipment Status is required.")]
        [Display(Name = "Equipment Status")]
        public string EquipmentStatusName { get; set; } 


        [Display(Name = "Description")]
        public string Description { get; set; }    
    }
}
