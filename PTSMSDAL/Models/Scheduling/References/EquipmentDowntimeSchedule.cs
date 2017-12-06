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
    [Table("EQUIPMENT_DOWNTIME_SCHEDULE")]
    public class EquipmentDowntimeSchedule 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentDowntimeScheduleId { get; set; } 

        
        [Required(ErrorMessage = "Equipment Id is required.")]
        [Index("UK_EQUIPMENT_DOWNTIME_SCHEDULE", IsUnique = true, Order = 1)]
        [Display(Name = "Equipment Id")]
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Downtime Reason is required.")]
        [Index("UK_EQUIPMENT_DOWNTIME_SCHEDULE", IsUnique = true, Order = 2)]
        [Display(Name = "Downtime Reason")]
        [ForeignKey("DowntimeReason")]
        public int DowntimeReasonId { get; set; } 

        [Required]
        [Index("UK_EQUIPMENT_DOWNTIME_SCHEDULE", IsUnique = true, Order = 3)]
        [Display(Name = "Schedule Start Date")]
        public DateTime ScheduleStartDate { get; set; }

        [Required]
        [Index(IsUnique = true, Order = 3)]
        [Index("UK_EQUIPMENT_DOWNTIME_SCHEDULE", IsUnique = true, Order = 4)]
        [Display(Name = "Schedule End Date")]
        public DateTime ScheduleEndDate { get; set; }

        [ForeignKey("EquipmentStatus")]
        public int EquipmentStatusId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }


        public virtual EquipmentStatus EquipmentStatus { get; set; }
        public virtual DowntimeReason DowntimeReason { get; set; }
        public virtual Equipment Equipment { get; set; }
    }
}
