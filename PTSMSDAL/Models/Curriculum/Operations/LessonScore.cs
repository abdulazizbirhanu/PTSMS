using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("LESSON_SCORE")]
    public class LessonScore : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonScoreId { get; set; }

        //[Index("UK_LessonScore", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Score Letter  is required.")]
        [Display(Name = "Score Letter")]
        public string ScoreLetter { get; set; }

        //[Index("UK_LessonScore", IsUnique = true, Order = 2)]
        [Required(ErrorMessage = "Score Letter Value  is required.")]
        [Display(Name = "Score Letter Value")]
        public float ScoreLetterValue { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

    }
}
