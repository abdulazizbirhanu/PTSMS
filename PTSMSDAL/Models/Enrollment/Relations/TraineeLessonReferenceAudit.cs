using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEELESSONREFERENCEAUDIT")]
    public class TraineeLessonReferenceAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeCourseReferenceId { get;set; }

        [ForeignKey("TraineeCourse")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeCourseId { get; set; }

        [ForeignKey("Reference")]
        [Index(IsUnique = true, Order = 2)]
        public int ReferenceId { get; set; }
        
        public virtual TraineeCourse TraineeCourse { get; set; }
        public virtual Reference Reference { get; set; }
    }
}