using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("INSTRUCTOR_QUALIFICATION")]
    public class InstructorQualification : AuditAttribute
    {
        [Key]
        public int InstructorQualificationId { get; set; } 

        [Required]
        [ForeignKey("Instructor")]
        public int InstructorId { get; set; } 
        
        [Required]
        [ForeignKey("QualificationType")]
        public int QualificationTypeId { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual Instructor Instructor { get; set; }
        public virtual QualificationType QualificationType { get; set; } 
    }
}
