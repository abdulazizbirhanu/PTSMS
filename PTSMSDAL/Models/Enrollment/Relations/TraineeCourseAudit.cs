using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEECOURSEAUDIT")]
    public class TraineeCourseAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeCourseId { get;set; }
        
        [ForeignKey("TraineeProgram")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeProgramId { get; set; }

        [ForeignKey("Course")]
        [Index(IsUnique = true, Order = 2)]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Module Score is required.")]
        [Display(Name = "Module Score")]
        public float ModuleScore { get; set; }

        [Required(ErrorMessage = "Course Score is required.")]
        [Display(Name = "Course Score")]
        public float CourseScore { get; set; }

        [Required(ErrorMessage = "Total Score is required.")]
        [Display(Name = "Total Score")]
        public float TotalScore { get; set; }

        public virtual TraineeProgram TraineeProgram { get; set; }
        public virtual Course Course { get; set; }
    }
}