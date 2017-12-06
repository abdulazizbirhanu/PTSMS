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
    //[Table("PassFailExamResult")]
    public class PassFailExamResult : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PassFailExamResultId { get; set; }

        [Index("UK_PassFailExam", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Pass/Fail Exam Name is required.")]
        [Display(Name = "Pass/Fail Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Exam Passed is required.")]
        [Display(Name = "Exam Passed")]
        public bool ExamPassed { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
