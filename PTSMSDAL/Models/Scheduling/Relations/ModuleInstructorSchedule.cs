using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_MODULE_INSTRUCTOR_SCHEDULE")]
    public class ModuleInstructorSchedule : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleInstructorScheduleId { get; set; }

        [Required]
        [Index("MODULE_INSTRUCTOR", IsUnique = true, Order = 1)]
        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }

        [Index("MODULE_INSTRUCTOR", IsUnique = true, Order = 2)]
        [Required]
        public int ModuleId { get; set; }

        public virtual Instructor Instructor { get; set; }
        public virtual Module Module { get; set; }

    }
}
