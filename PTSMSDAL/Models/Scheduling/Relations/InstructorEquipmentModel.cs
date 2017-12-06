using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("INSTRUCTOR_EQUIPMENT_MODEL")]
    public class InstructorEquipmentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstructorEquipmentModelId { get; set; }

        [Required(ErrorMessage = "Equipment is required.")]
        [Index("UK_INSTRUCTOR_EQUIPMENT_MODEL", IsUnique = true, Order = 1)]
        [Display(Name = "Equipment Mode Id")]
        [ForeignKey("EquipmentModel")]
        public int EquipmentModelId { get; set; }

        [Required(ErrorMessage = "Batch Id is required.")]
        [Index("UK_INSTRUCTOR_EQUIPMENT_MODEL", IsUnique = true, Order = 2)]
        [Display(Name = "Instructor Id")]
        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }

        public virtual EquipmentModel EquipmentModel { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}
