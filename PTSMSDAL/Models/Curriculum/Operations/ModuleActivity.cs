using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("MODULE_ACTIVITY")]
    public class ModuleActivity : AuditAttribute
    {
        [Key]
        [Display(Name = "Module Activity")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleActivityId { get; set; }

        [StringLength(100)]
        [Display(Name = "Module Activity Name")]
        [Index("UK_MODULE_ACTIVITY", IsUnique = true, Order = 1)]
        public string ModuleActivityName { get; set; }

        [Display(Name = "Estimated Duration")]
        public double EstimatedDuration { get; set; }

        [Required]
        [ForeignKey("Module")]
        [Index("UK_MODULE_ACTIVITY", IsUnique = true, Order = 2)]
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
