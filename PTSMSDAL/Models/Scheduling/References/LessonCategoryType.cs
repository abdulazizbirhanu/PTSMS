using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.References
{
    [Table("LESSON_CATEGORY_TYPE")]
    public class LessonCategoryType : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonCategoryTypeId { get; set; }

        [Required]
        [Display(Name = "Lesson Type Name")]
        public string LessonCategoryTypName { get; set; }
    }
}
