using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_COURSEREFERENCE")]
    public class CourseReference : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int CourseReferenceId { get; set; }

        [ForeignKey("CourseCategory")]
        [Index("UK_CourseReference", IsUnique = true, Order = 1)]
        public int CourseCategoryId { get; set; }

        [ForeignKey("Reference")]
        [Index("UK_CourseReference", IsUnique = true, Order = 2)]
        public int ReferenceId { get; set; }

        [Required(ErrorMessage = "Is Syllabus Resource is required.")]
        [Display(Name = "Is Syllabus Resource")]
        public bool IsSyllabusResource { get; set; }
                
        public virtual CourseCategory CourseCategory { get; set; }
        public virtual Reference Reference { get; set; }
    }
}