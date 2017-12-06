using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEEPREREQUISITE")]
    public class TraineePrerequisite : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineePrerequisiteId { get;set; }

        [ForeignKey("Course")]
        [Index("UK_TRAINEEPREREQUISITE", IsUnique = true, Order = 1)]
        public int CourseId { get; set; }

        [ForeignKey("Prerequisite")]
        [Index("UK_TRAINEEPREREQUISITE", IsUnique = true, Order = 2)]
        public int PrerequisiteId { get; set; }

        [ForeignKey("TraineeCourse")]
        [Index("UK_TRAINEEPREREQUISITE", IsUnique = true, Order = 3)]
        public int TraineeCourseId { get; set; }

        public virtual TraineeCourse TraineeCourse { get; set; }
        public virtual Course Course { get; set; }
        public virtual Prerequisite Prerequisite { get; set; }
    }
}