using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_BATCH_LESSON_SEQUENCE")]
    public class BatchLessonSequence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchLessonSequenceId { get; set; }

        [Display(Name = "Phase Schedule Id")]
        [Index("UK_REL_BATCH_LESSON_SEQUENCE", IsUnique = true, Order = 1)]
        [ForeignKey("PhaseSchedule")]
        public int PhaseScheduleId { get; set; }

        [Display(Name = "Module Id")]
        [Index("UK_REL_BATCH_LESSON_SEQUENCE", IsUnique = true, Order = 2)]
        [ForeignKey("Lessons")]
        public int LessonId { get; set; }

        [Required]
        [Index("UK_REL_BATCH_LESSON_SEQUENCE", IsUnique = true, Order = 3)]
        [Display(Name = "Sequence")]
        public double Sequence { get; set; }

        public virtual Lesson Lessons { get; set; }
        public virtual PhaseSchedule PhaseSchedule { get; set; }
    }
}
