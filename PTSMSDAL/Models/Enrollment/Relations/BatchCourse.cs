using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_BATCHCOURSE")]
    public class BatchCourse : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchCourseId { get;set; }
        
        [ForeignKey("BatchCategory")]
        [Index("UK_BatchCourse", IsUnique = true, Order = 1)]
        public int BatchCategoryId { get; set; }

        [ForeignKey("Course")]
        [Index("UK_BatchCourse", IsUnique = true, Order = 2)]
        public int CourseId { get; set; }
        public int Sequence { get; set; }

        public virtual BatchCategory BatchCategory { get; set; }
        public virtual Course Course { get; set; }

    }
}