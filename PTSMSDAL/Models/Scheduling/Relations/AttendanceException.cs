using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Generic;


namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_ATTENDANCE_EXCEPTION")]
    public class AttendanceException : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceExceptionId { get; set; } 

        [ForeignKey("ActualModuleTaken")]
        [Index("UK_REL_ATTENDANCE_EXCEPTION", IsUnique = true, Order = 1)]
        public int ActualModuleTakenId { get; set; }

        [ForeignKey("Trainee")]
        [Index("UK_REL_ATTENDANCE_EXCEPTION", IsUnique = true, Order = 2)]
        public int TraineeId { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        [Required]
        [Display(Name = "Trainee Status")]
        public string TraineeStatus { get; set; }

        public virtual ActualModuleTaken ActualModuleTaken { get; set; }
        public virtual Trainee Trainee { get; set; }       

    }
    public enum TraineeAttendanceStatus
    {
        Absent,
        Present
    }
}
