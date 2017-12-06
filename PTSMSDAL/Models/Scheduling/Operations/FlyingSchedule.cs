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

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("FLYING_SCHEDULE")]
    public class FlyingSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FlyingScheduleId { get; set; }


        [Required(ErrorMessage = "Instructor is required.")]
        [Index("FLYING_SCHEDULE", IsUnique = true, Order = 1)]
        [ForeignKey("Instructor")]
        [Display(Name = "Instructor Id")]
        public string InstructorId { get; set; }

        [Required(ErrorMessage = "Trainee Id is required.")]
        [Index("FLYING_SCHEDULE", IsUnique = true, Order = 2)]
        [ForeignKey("Module")]
        [Display(Name = "Trainee Id")]
        public string TraineeId { get; set; }

        [Required(ErrorMessage = "Lesson Id is required.")]
        [Display(Name = "Lesson Id")]
        public string LessonId { get; set; }

        [Required(ErrorMessage = "Equipment Id is required.")]
        [Index(IsUnique = true, Order = 3)]
        [ForeignKey("Equipment")]
        [Display(Name = "Equipment Id")]
        public int EquipmentId { get; set; } 


        [Required]
        [Display(Name = "Schedule Start Time")]
        public DateTime ScheduleStartTime { get; set; }



        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public string Status { get; set; } 

        public virtual Module Module { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual Equipment Equipment { get; set; }

    }
}
