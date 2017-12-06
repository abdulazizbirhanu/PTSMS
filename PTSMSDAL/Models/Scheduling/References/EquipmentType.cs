using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.References
{
    [Table("EQUIPMENT_TYPE")]
    public class EquipmentType : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentTypeId { get; set; }

        [Required]
        [Display(Name = "Equipment Type Name")]
        public string EquipmentTypeName { get; set; }
    }
}
