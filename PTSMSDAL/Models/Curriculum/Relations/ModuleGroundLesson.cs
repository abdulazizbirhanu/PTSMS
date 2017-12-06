using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_MODULEGROUNDLESSON")]
    public class ModuleGroundLesson : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleGroundLessonId { get; set; }

        [Index("UK_ModuleGroundLesson", IsUnique = true, Order = 1)]
        [ForeignKey("CourseModule")]
        public int CourseModuleId { get; set; }        
        
        [Index("UK_ModuleGroundLesson", IsUnique = true, Order = 2)]
        [ForeignKey("GroundLesson")]
        public int GroundLessonId { get; set; }
        
        public virtual CourseModule CourseModule { get; set; }
        public virtual GroundLesson GroundLesson { get; set; }
    }
}