using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.Operations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEESYLLABUS")]
    public class TraineeSyllabus : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeSyllabusId { get;set; }

        [ForeignKey("Batch")]
        [Index("UK_TraineeSyllabus", IsUnique = true, Order = 1)]
        public int BatchId { get; set; }

        [ForeignKey("Trainee")]
        [Index("UK_TraineeSyllabus", IsUnique = true, Order = 2)]
        public int TraineeId { get; set; }

        [Required(ErrorMessage = "Syllabus Generated Date is required.")]
        [Display(Name = "Syllabus Generated Date")]
        public DateTime SyllabusGeneratedDate { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual Trainee Trainee { get; set; }
    }
}