using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.Operations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_MODULETOOLSCHEDULE")]
    public class ModuleToolSchedule : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleToolScheduleId { get; set; }

        [Index("UK_REL_MODULETOOLSCHEDULE", IsUnique = true, Order = 1)]
        [ForeignKey("ModuleSchedule")]
        public int ModuleScheduleId { get; set; }

        [Index("UK_REL_MODULETOOLSCHEDULE", IsUnique = true, Order = 2)]
        [ForeignKey("Tool")]
        public int ToolId { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousModuleToolSchedule")]
        public int? PreviousRevisionId { get; set; }

        [Index(IsUnique = true, Order = 3)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public ModuleToolSchedule PreviousModuleToolSchedule { get; set; }
        public virtual ModuleSchedule ModuleSchedule { get; set; }
        public virtual Tool Tool { get; set; }
    }
}