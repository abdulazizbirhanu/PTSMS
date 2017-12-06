using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Generic;



namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_ACTUAL_MODULE_TAKEN")]
    public class ActualModuleTaken : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActualModuleTakenId { get; set; }
                
        [ForeignKey("ModuleSchedule")]
        [Index("UK_REL_ACTUAL_MODULE_TAKEN", IsUnique = true, Order = 1)]
        public int ModuleScheduleId { get; set; }


        [ForeignKey("Instructor")]
        public int InstructorId { get; set; } 

        [ForeignKey("ClassRoom")]
        public int ClassRoomId { get; set; }
        
        [Display(Name = "Taken date")]
        public DateTime TakenDate { get; set; } 
        
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual ClassRoom ClassRoom { get; set; }
        public virtual ModuleSchedule ModuleSchedule { get; set; }

    }
}
