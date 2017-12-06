using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_MODULESCHEDULE")]
    public class ModuleSchedule : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleScheduleId { get; set; }

        [Required]
        [Index("UK_REL_MODULESCHEDULE", IsUnique = true, Order = 1)]
        [ForeignKey("PhaseSchedule")]
        public int PhaseScheduleId { get; set; }

        [Required]
        [Index("UK_REL_MODULESCHEDULE", IsUnique = true, Order = 2)]
        [ForeignKey("Module")]
        public int ModuleId { get; set; }

        [Required]
        [Index("UK_REL_MODULESCHEDULE", IsUnique = true, Order = 3)]
        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }

        [Required]
        [Index("UK_REL_MODULESCHEDULE", IsUnique = true, Order = 4)]
        [ForeignKey("ClassRoom")]
        public int ClassRoomId { get; set; }

        [Index("UK_REL_MODULESCHEDULE", IsUnique = true, Order = 5)]
        [Required(ErrorMessage = "Date is required.")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Index("UK_REL_MODULESCHEDULE", IsUnique = true, Order = 6)]
        [ForeignKey("Period")]
        public int PeriodId { get; set; }

        [ForeignKey("ParentModuleSchedule")]
        public int? ParentScheduleId { get; set; }


        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Reason")]
        public string Reason { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public virtual PhaseSchedule PhaseSchedule { get; set; }
        public virtual Module Module { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual ClassRoom ClassRoom { get; set; }
        public virtual Period Period { get; set; }
        public virtual ModuleSchedule ParentModuleSchedule { get; set; }
    }

    public enum ModuleScheduleStatus
    {
        New = 0,
        Canceled = 1,
        Evaluated = 2,
        Unaccepted = 3,
        Completed = 4,
        Unattended = 5
    }
}