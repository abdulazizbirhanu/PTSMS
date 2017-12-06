using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEEMODULE")]
    public class TraineeModuleAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeModuleId { get; set; }

        [ForeignKey("TraineeCourse")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeCourseId { get; set; }

        [ForeignKey("Module")]
        [Index(IsUnique = true, Order = 2)]
        public int ModuleId { get; set; }

        [Required(ErrorMessage = "Module Score is required.")]
        [Display(Name = "Module Score")]
        public float ModuleScore { get; set; }

        public virtual TraineeCourse TraineeCourse { get; set; }
        public virtual Module Module { get; set; }
    }
}