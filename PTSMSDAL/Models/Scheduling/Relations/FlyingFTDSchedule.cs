using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("FLYING_FTD_SCHEDULE")]
    public class FlyingFTDSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FlyingFTDScheduleId { get; set; }


        [Required(ErrorMessage = "Instructor is required.")]
        [Index("UK_FLYING_SCHEDULE", IsUnique = true, Order = 1)]
        [ForeignKey("Instructor")]
        [Display(Name = "Instructor Id")]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Trainee Id is required.")]
        [Index("UK_FLYING_SCHEDULE", IsUnique = true, Order = 2)]
        [ForeignKey("Trainee")]
        [Display(Name = "Trainee Id")]
        public int TraineeId { get; set; }

        [Required(ErrorMessage = "Lesson Id is required.")]
        [Display(Name = "Lesson Id")]
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Equipment Id is required.")]
        [Index("UK_FLYING_SCHEDULE", IsUnique = true, Order = 3)]
        [Display(Name = "Equipment Id")]
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; }

        [Required]
        [Display(Name = "Schedule Start Time")]
        [Index("UK_FLYING_SCHEDULE", IsUnique = true, Order = 4)]
        public DateTime ScheduleStartTime { get; set; }

        [Required]
        [Display(Name = "Schedule End Time")]
        [Index("UK_FLYING_SCHEDULE", IsUnique = true, Order = 5)]
        public DateTime ScheduleEndTime { get; set; }

        [Index("UK_FLYING_SCHEDULE", IsUnique = true, Order = 6)]
        public int Sequence { get; set; }

        [ForeignKey("RescheduleReason")]
        [Display(Name = "Reschedule Reason")]
        public int? RescheduleReasonId { get; set; }

        [StringLength(50)]
        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status is required.")]
        [Index("UK_FLYING_SCHEDULE", IsUnique = true, Order = 7)]// Used to OVERRIDE canceled and new Schedule
        public string Status { get; set; }

        [Display(Name = "Reason")]
        public string Reason { get; set; }

        [Display(Name = "Is Notified?")]
        public bool IsNotified { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public virtual Trainee Trainee { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual RescheduleReason RescheduleReason { get; set; }
        
    }

    public enum FlyingFTDScheduleStatus
    {
        New,
        Canceled,
        CheckedIn,
        CheckedInAuthorized,
        CheckedInRejected,
        RampOut,
        RampIn,
        Evaluated,
        Unaccepted,
        Completed,
        Unattended
    }
}
