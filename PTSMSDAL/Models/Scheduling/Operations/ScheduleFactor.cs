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
    [Table("SCHEDULE_FACTOR")]
    public class ScheduleFactor : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FTDScheduleFactorId { get; set; }


        [Required(ErrorMessage = "Factor Name is required.")]
        [Index(IsUnique = true, Order = 1)]
        [Display(Name = "Factor Name")]
        [MaxLength(30)]
        public string FactorName { get; set; }

        [Required(ErrorMessage = "Factor Value required.")]
        [Display(Name = "Factor Value")]
        public string FactorValue { get; set; }

        [Required(ErrorMessage = "Factor Type required.")]
        [Display(Name = "Factor Type")]
        public string FactorType { get; set; }
    }
}
