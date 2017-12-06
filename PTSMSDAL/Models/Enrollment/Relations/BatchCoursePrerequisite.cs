using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_BATCHCOURSEPREREQUISITE")]
    public class BatchCoursePrerequisite : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchCoursePrerequisiteId { get;set; }

        [ForeignKey("BatchCourse")]
        [Index("UK_BatchCoursePrerequisite", IsUnique = true, Order = 1)]
        public int BatchCourseId { get; set; }

        [ForeignKey("Course")]
        [Index("UK_BatchCoursePrerequisite", IsUnique = true, Order = 2)]
        public int CourseId { get; set; }

        [ForeignKey("CoursePrerequisite")]
        [Index("UK_BatchCoursePrerequisite", IsUnique = true, Order = 3)]
        public int PrerequisiteId { get; set; }

        public virtual BatchCourse BatchCourse { get; set; }
        public virtual Course Course { get; set; }
        public virtual Course CoursePrerequisite { get; set; }
    }
}