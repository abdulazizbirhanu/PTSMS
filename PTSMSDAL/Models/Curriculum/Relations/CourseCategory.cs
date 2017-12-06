using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_COURSECATEGORY")]
    public class CourseCategory : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseCategoryId { get; set; }
        
        [Index("UK_CourseCategory", IsUnique = true, Order = 1)]
        [ForeignKey("ProgramCategory")]
        public int ProgramCategoryId { get; set; }

        [Index("UK_CourseCategory", IsUnique = true, Order = 2)]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [ForeignKey("Phase")]
        public int? PhaseId { get; set; }

        [ForeignKey("Stage")]
        public int? StageId { get; set; }

        public virtual ProgramCategory ProgramCategory { get; set; }
        public virtual Course Course { get; set; }
        public virtual Phase Phase { get; set; }
        public virtual Stage Stage { get; set; }
    }
}