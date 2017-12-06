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
    [Table("EQUIPMENT_MAINTENANCE_SCHEDULE")]
    public class EquipmentMaintenanceSchedule : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentMaintenanceScheduleId { get; set; } 

        
        [Required(ErrorMessage = "Equipment Id is required.")]
        [Index(IsUnique = true, Order = 1)]
        [Display(Name = "Equipment Id")]
        public string EquipmentId { get; set; }

        [Required(ErrorMessage = "Downtime Reason is required.")]
        [Display(Name = "Downtime Reason")]
        public string DowntimeReasonId { get; set; } 

        [Required]
        [Index(IsUnique = true, Order = 2)]
        [Display(Name = "Schedule Start Date")]
        public DateTime ScheduleStartDate { get; set; }

        [Required]
        [Index(IsUnique = true, Order = 3)]
        [Display(Name = "Schedule End Date")]
        public DateTime ScheduleEndDate { get; set; } 

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual DowntimeReason DowntimeReason { get; set; }
        public virtual Equipment Equipment { get; set; }
    }
}
