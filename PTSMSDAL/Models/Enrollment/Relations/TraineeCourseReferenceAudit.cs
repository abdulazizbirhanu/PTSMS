using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEECOURSEREFERENCE")]
    public class TraineeCourseReferenceAudit : AuditAttribute
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