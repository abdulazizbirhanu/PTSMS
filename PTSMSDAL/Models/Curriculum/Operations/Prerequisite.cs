using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Relations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("PREREQUISITE")]
    public class Prerequisite : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrerequisiteId { get; set; }

        [Index("UK_Prerequisite", IsUnique = true, Order = 1)]
        [ForeignKey("CourseCategory")]
        public int CourseCategoryId { get; set; }

        [Index("UK_Prerequisite", IsUnique = true, Order = 2)]
        [ForeignKey("PrerequisiteCourse")]
        public int PrerequisiteCourseId { get; set; }

        [Required(ErrorMessage = "Remark is required.")]
        [Display(Name = "Remark")]
        public string Remark { get; set; }
        
        public virtual CourseCategory CourseCategory { get; set; }
        public virtual Course PrerequisiteCourse { get; set; }
    }
}