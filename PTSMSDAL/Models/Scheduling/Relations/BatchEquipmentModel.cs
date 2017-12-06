using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("BATCH_EQUIPMENT_MODEL")]
    public class BatchEquipmentModel : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchEquipmentModelId { get; set; }

        [Required(ErrorMessage = "Equipment is required.")]
        [Index("UK_BATCH_EQUIPMENT_MODEL", IsUnique = true, Order = 1)]
        [Display(Name = "Equipment Mode Id")]
        [ForeignKey("EquipmentModel")]
        public int EquipmentModelId { get; set; }

        [Required(ErrorMessage = "Batch Id is required.")]
        [Index("UK_BATCH_EQUIPMENT_MODEL", IsUnique = true, Order = 2)]
        [Display(Name = "BatchId Id")]
        [ForeignKey("Batch")]
        public int BatchId { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual EquipmentModel EquipmentModel { get; set; }
    }
}
