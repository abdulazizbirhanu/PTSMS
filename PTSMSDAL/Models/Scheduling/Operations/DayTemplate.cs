using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("DAYTEMPLATE")]
    public class DayTemplate : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DayTemplateId { get; set; }

        [Index("UK_DAYTEMPLATE", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Day Template Name is required.")]
        [Display(Name = "Day Template Name")]
        [MaxLength(64)]
        public string DayTemplateName { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousDayTemplate")]
        public int? RevisionGroupId { get; set; } 

        [Index("UK_DAYTEMPLATE", IsUnique = true, Order = 2)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public DayTemplate PreviousDayTemplate { get; set; }
    }
}