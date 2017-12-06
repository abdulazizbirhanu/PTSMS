using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("MODULE_ACTIVITY_LOG")]
    public class ModuleActivityLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleActivityLogId { get; set; }

        [Required]
        [ForeignKey("ModuleSchedule")]
        [Index("UK_MODULE_ACTIVITY_LOG", IsUnique = true, Order = 1)]
        public int ModuleScheduleId { get; set; }

        [Required]
        [ForeignKey("ModuleActivity")]
        [Index("UK_MODULE_ACTIVITY_LOG", IsUnique = true, Order = 2)]
        public int ModuleActivityId { get; set; }

        [Display(Name = "Module Activity Name")]
        public string ModuleActivityName { get; set; }

        [Display(Name = "Estimated Duration")]
        public double EstimatedDuration { get; set; }
        public virtual ModuleSchedule ModuleSchedule { get; set; }
        public virtual ModuleActivity ModuleActivity { get; set; }
        
    }
}
