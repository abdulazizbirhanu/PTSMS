using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_CATEGORYLESSON")]
    public class CategoryLesson : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryLessonId { get; set; }
        
        [Index(IsUnique = true, Order = 1)]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Index(IsUnique = true, Order = 2)]
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}