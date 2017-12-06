using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEEPREREQUISITE")]
    public class TraineePrerequisiteAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineePrerequisiteId { get;set; }

        [ForeignKey("Course")]
        [Index(IsUnique = true, Order = 1)]
        public int CourseId { get; set; }

        [ForeignKey("Prerequisite")]
        [Index(IsUnique = true, Order = 2)]
        public int PrerequisiteId { get; set; }
        
        public virtual Course Course { get; set; }
        public virtual Prerequisite Prerequisite { get; set; }
    }
}