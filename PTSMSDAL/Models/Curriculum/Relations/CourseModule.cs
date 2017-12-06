using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_COURSEMODULE")]
    public class CourseModule : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseModuleId { get; set; }

        [Index("UK_CourseModule", IsUnique = true, Order = 1)]
        [ForeignKey("CourseCategory")]
        public int CourseCategoryId { get; set; }

        [Index("UK_CourseModule", IsUnique = true, Order = 2)]
        [ForeignKey("Module")]
        public int ModuleId { get; set; }

        //public int Sequence { get; set; }

        [Index("UK_CourseModule", IsUnique = true, Order = 3)]
        [ForeignKey("Phase")]
        public int PhaseId { get; set; }
        public virtual CourseCategory CourseCategory { get; set; }
        public virtual Module Module { get; set; }
        public virtual Phase Phase { get; set; }
    }
}