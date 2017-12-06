using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("EQUIPMENT_SCHEDULE_BRIEFING_AND_DEBRIEFING")]
    public class EquipmentScheduleBriefingDebriefing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentScheduleBriefingDebriefingId { get; set; }

        [Required]
        [ForeignKey("FlyingFTDSchedule")]
        [Index("UK_EQUIPMENT_SCHEDULE_BRIEFING_AND_DEBRIEFING", IsUnique = true, Order = 1)]
        public int FlyingFTDScheduleId { get; set; }

        [Required]
        [ForeignKey("BriefingAndDebriefing")]
        [Index("UK_EQUIPMENT_SCHEDULE_BRIEFING_AND_DEBRIEFING", IsUnique = true, Order = 2)]
        public int BriefingAndDebriefingId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public string Status { get; set; }      

        public virtual FlyingFTDSchedule FlyingFTDSchedule { get; set; }

        public virtual BriefingAndDebriefing BriefingAndDebriefing { get; set; }
    }
}
