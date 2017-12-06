using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_INSTRUCTORSCHEDULE")]
    public class InstructorSchedule : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstructorScheduleId { get; set; }

        [Index("UK_REL_INSTRUCTORSCHEDULE", IsUnique = true, Order = 1)]
        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }

        [Index("UK_REL_INSTRUCTORSCHEDULE", IsUnique = true, Order = 2)]
        [ForeignKey("Module")]
        public int ModuleId { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousInstructorSchedule")]
        public int? PreviousRevisionId { get; set; }

        [Index(IsUnique = true, Order = 3)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public InstructorSchedule PreviousInstructorSchedule { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual Module Module { get; set; }
    }
}