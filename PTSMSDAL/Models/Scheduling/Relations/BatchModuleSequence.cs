using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("REL_BATCH_MODULE_SEQUENCE")]
    public class BatchModuleSequence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchModuleSequenceId { get; set; }

        [Display(Name = "Phase Schedule Id")]
        [Index("UK_REL_BATCH_MODULE_SEQUENCE", IsUnique = true, Order = 1)]
        [ForeignKey("PhaseSchedule")]
        public int PhaseScheduleId { get; set; }

        [Display(Name = "Module Id")]
        [Index("UK_REL_BATCH_MODULE_SEQUENCE", IsUnique = true, Order = 2)]
        [ForeignKey("Module")]
        public int ModuleId { get; set; }

        [Required]
        //[Index("UK_REL_BATCH_MODULE_SEQUENCE", IsUnique = true, Order = 3)]
        [Display(Name = "Sequence")]
        public double Sequence { get; set; }

        public virtual Module Module { get; set; }
        public virtual PhaseSchedule PhaseSchedule { get; set; }
    }
}
