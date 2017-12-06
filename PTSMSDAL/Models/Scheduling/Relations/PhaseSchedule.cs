using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_PHASESCHEDULE")]
    public class PhaseSchedule
    {
        [Key]//PhaseSchedule 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PhaseScheduleId { get; set; }

        [Index("UK_TraineeBatchId", IsUnique = true, Order = 1)]
        [ForeignKey("Batch")]
        public int BatchId { get; set; }

        [Index("UK_TraineeBatchId", IsUnique = true, Order = 2)]
        [ForeignKey("Phase")]
        public int PhaseId { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [ForeignKey("Location")]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [Display(Name = "Start Date")]
        public DateTime StartingDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required]
        [Index("UK_TraineeBatchId", IsUnique = true, Order = 3)]
        [Display(Name = "Type")]
        [ForeignKey("LessonCategoryType")]
        public int LessonCategoryTypeId { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual Phase Phase { get; set; }
        public virtual Location Location { get; set; }
        public virtual LessonCategoryType LessonCategoryType { get; set; }
    }
}