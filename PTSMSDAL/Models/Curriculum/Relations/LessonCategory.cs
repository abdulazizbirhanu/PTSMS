using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_LESSONCATEGORY")]
    public class LessonCategory : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonCategoryId { get; set; }
        
        [Index("UK_LessonCategory", IsUnique = true, Order = 1)]
        [ForeignKey("ProgramCategory")]
        public int ProgramCategoryId { get; set; }

        [Index("UK_LessonCategory", IsUnique = true, Order = 2)]
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Lesson Sequence is required.")]
        [Display(Name = "Lesson Sequence")]
        public int LessonSequence { get; set; }

        [ForeignKey("Phase")]
        public int PhaseId { get; set; }

        [ForeignKey("Stage")]
        public int? StageId { get; set; }

        public virtual ProgramCategory ProgramCategory { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual Phase Phase { get; set; }
        public virtual Stage Stage { get; set; }
    }
}