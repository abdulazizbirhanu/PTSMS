using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("DAY")]
    public class Day : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DayId { get; set; }

        [ForeignKey("DayTemplate")]
        [Index("UK_DAY", IsUnique = true, Order = 1)]
        public int DayTemplateId { get; set; }

        [Index("UK_DAY",IsUnique = true, Order = 2)]
        [Display(Name = "DayName")]
        [Required]
        [StringLength(20)]
        public string DayName { get; set; }

        public virtual DayTemplate DayTemplate { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousDay")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_DAY", IsUnique = true, Order = 3)]

        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual Day PreviousDay { get; set; }
    }
}