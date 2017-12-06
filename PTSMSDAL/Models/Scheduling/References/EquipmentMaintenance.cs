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
    [Table("EQUIPMENT_MAINTENANCE")]
    public class EquipmentMaintenance : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentMaintenanceId { get; set; }

        [Required(ErrorMessage = "Equipment Id is required.")]
        [Index("UK_EquipmentMaintenance", Order = 1, IsUnique = true)]
        [Display(Name = "Equipment Id")]
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; }

        [Index("UK_EquipmentMaintenance", Order = 2, IsUnique = true)]
        [StringLength(400)]
        [Required(ErrorMessage = "Name of Maintenance is required.")]
        [Display(Name = "Name")]
        public string MaintenanceName { get; set; }

        [Index("UK_EquipmentMaintenance", Order = 3, IsUnique = true)]
        [Required(ErrorMessage = "Event Type is required.")]
        [Display(Name = "Type")]
        public EventTypes EventType { get; set; }

        [Required]
        [Display(Name = "Parameter")]
        [Index("UK_EquipmentMaintenance", Order = 4, IsUnique = true)]
        public ParameterTypes ParameterType { get; set; }

        [Index("UK_EquipmentMaintenance", Order = 5, IsUnique = true)]
        [Display(Name = "Scheduled Start Date")]
        public DateTime? ScheduledCalanderStartDate { get; set; }

        [Display(Name = "Scheduled End Date")]
        public DateTime? ScheduledCalanderEndDate { get; set; }

        [Index("UK_EquipmentMaintenance", Order = 6, IsUnique = true)]
        [Display(Name = "Scheduled Maint' Hour")]
        public float? ScheduledMaintenanceHour { get; set; }

        [Display(Name = "Actual StartDate")]
        public DateTime? ActualCalanderStartDate { get; set; }

        [Display(Name = "Actual EndDate")]
        public DateTime? ActualCalanderEndDate { get; set; }

        [Display(Name = "Actual Maint' Hour")]
        public float? ActualMaintenanceHour { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Required]
        public StatusType Status { get; set; }

        public virtual Equipment Equipment { get; set; }

    }

    public enum EventTypes
    {
        Defect, Planned
    }
    public enum ParameterTypes
    {
        Hour, Calendar
    }
    public enum StatusType
    {
        Pending, Progressing, Completed, Canceled
    }
}
