using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Relations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("PREREQUISITE")]
    public class LessonPrerequisite : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonPrerequisiteId { get; set; }

        [ForeignKey("LessonCategory")]
        [Index("UK_LessonReference", IsUnique = true, Order = 1)]
        public int LessonCategoryId { get; set; }

        [Index("UK_Prerequisite", IsUnique = true, Order = 2)]
        [ForeignKey("Prerequisite")]
        public int PrerequisiteId { get; set; }

        [Required(ErrorMessage = "Remark is required.")]
        [Display(Name = "Remark")]
        public string Remark { get; set; }
        
        public virtual LessonCategory LessonCategory { get; set; }
        public virtual Course Prerequisite { get; set; }
    }
}