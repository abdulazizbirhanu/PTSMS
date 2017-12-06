using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("PERIODTEMPLATE")]
    public class PeriodTemplate : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PeriodTemplateId { get; set; }

        [Index("UK_PERIODTEMPLATE", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Period Template Name is required.")]
        [Display(Name = "Period Template Name")]
        [MaxLength(64)]
        public string PeriodTemplateName { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousPeriodTemplate")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_PERIODTEMPLATE", IsUnique = true, Order = 2)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        public decimal PeriodLength { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public PeriodTemplate PreviousPeriodTemplate { get; set; }
    }
}