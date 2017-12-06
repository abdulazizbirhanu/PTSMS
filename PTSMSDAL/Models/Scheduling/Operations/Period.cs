using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("PERIOD")]
    public class Period : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PeriodId { get; set; }

        [Index("UK_PERIOD", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Period Name is required.")]
        [Display(Name = "Period Name")]
        [MaxLength(64)]
        public string PeriodName { get; set; }

        [ForeignKey("PeriodTemplate")]
        [Index("UK_PERIOD", IsUnique = true, Order = 2)]
        public int PeriodTemplateId { get; set; }
        
        [Required(ErrorMessage = "Start Time is required.")]
        [Display(Name = "Start Time")]
        public string StartTime { get; set; }

        //[Required(ErrorMessage = "End Time is required.")]
        [Display(Name = "End Time")]
        public string EndTime { get; set; }

        //[Required(ErrorMessage = "AM or PM is required.")]
        //[Display(Name = "AM or PM")]
        //public string AMorPM { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousPeriod")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_PERIOD", IsUnique = true, Order = 3)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public Period PreviousPeriod { get; set; }
        public virtual PeriodTemplate PeriodTemplate { get; set; }
    }
}