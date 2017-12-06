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
    [Table("EQUIPMENT_MODEL")] 
    public class EquipmentModel : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentModelId { get; set; }

        [ForeignKey("EquipmentType")]
        //[Index("UK_EQUIPMENT_MODEL", IsUnique = true, Order = 1)]        
        [Display(Name = "Equipment Type")]
        public int EquipmentTypeId { get; set; }
        
        //[Index("UK_EQUIPMENT_MODEL", IsUnique = true, Order = 2)]
        [Required(ErrorMessage = "Equipment Model Name is required.")] 
        [Display(Name = "Equipment Model Name")]
        public string EquipmentModelName { get; set; }

        [Required]
        [Display(Name = "IsMultiEngine")]
        public bool IsMultiEngine { get; set; }
   
       
        [Display(Name = "Description")]
        public string Description { get; set; }   
        public virtual EquipmentType EquipmentType { get; set; }        
    }
}
