using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("FTD_SCHEDULE_FACTOR")]
    public class FTDScheduleFactor : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FTDScheduleFactorId { get; set; }


        [Required(ErrorMessage = "Factor Name is required.")]
        [Index(IsUnique = true, Order = 1)]
        [Display(Name = "Factor Name")]
        public string FactorName { get; set; }

        [Required(ErrorMessage = "Factor Value required.")]
        [Display(Name = "Factor Value")]
        public string FactorValue { get; set; }
    }
}
