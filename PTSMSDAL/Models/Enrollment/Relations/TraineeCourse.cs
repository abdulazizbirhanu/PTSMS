using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEECOURSE")]
    public class TraineeCourse : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeCourseId { get;set; }
        
        [ForeignKey("BatchCategory")]
        [Index("UK_TraineeCourse", IsUnique = true, Order = 1)]
        public int BatchCategoryId { get; set; }
        [ForeignKey("Trainee")]
        public int TraineeId { get; set; }

        [ForeignKey("Course")]
        [Index("UK_TraineeCourse", IsUnique = true, Order = 2)]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Module Score is required.")]
        [Display(Name = "Module Score")]
        public double ModuleScore { get; set; }

        [Required(ErrorMessage = "Course Score is required.")]
        [Display(Name = "Course Score")]
        public double CourseScore { get; set; }

        [Required(ErrorMessage = "Total Score is required.")]
        [Display(Name = "Total Score")]
        public double TotalScore { get; set; }

        [Required(ErrorMessage = "Module Weight is required.")]
        [Display(Name = "Module Weight")]
        public double ModuleWeight { get; set; }

        [Required(ErrorMessage = "Course Weight is required.")]
        [Display(Name = "Course Weight")]
        public double CourseWeight { get; set; }

        public virtual Trainee Trainee { get; set; }
        public virtual BatchCategory BatchCategory { get; set; }
        public virtual Course Course { get; set; }
    }
}