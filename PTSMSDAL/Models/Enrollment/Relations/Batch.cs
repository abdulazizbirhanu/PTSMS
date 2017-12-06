using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Scheduling.Operations;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("BATCH")]
    public class Batch : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchId { get; set; }

        [ForeignKey("Program")]
        [Index("UK_Batch", IsUnique = true, Order = 1)]
        public int ProgramId { get; set; }

        [Index("UK_Batch", IsUnique = true, Order = 2)]
        [Required(ErrorMessage = "Batch Name is required.")]
        [Display(Name = "Batch Name")]
        [MaxLength(32)]
        public string BatchName { get; set; }
        public bool isGenerated { get; set; }

        [ForeignKey("DayTemplate")]
        public int DayTemplateId { get; set; }

        [ForeignKey("PeriodTemplate")]
        public int PeriodTemplateId { get; set; }

        [Required(ErrorMessage = "Batch Start Date is required.")]
        [Display(Name = "Batch Start Date")]
        public DateTime BatchStartDate { get; set; }

        [Required(ErrorMessage = "Estimated End Date is required.")]
        [Display(Name = "Estimated End Date")]
        public DateTime EstimatedEndDate { get; set; }

        public virtual PeriodTemplate PeriodTemplate { get; set; }
        public virtual DayTemplate DayTemplate { get; set; }
        public virtual Program Program { get; set; }
    }
    public class resultSet
    {

        public string resultType { get; set; }
        public string resultValue { get; set; }
    }
}