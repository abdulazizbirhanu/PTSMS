using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Scheduling.References
{
    [Table("DOWNTIME_REASON")]
    public class DowntimeReason : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DowntimeReasonId { get; set; }

        [Required(ErrorMessage = "Downtime Reason is required.")]
        [Display(Name = "Downtime Reason")]
        public string DowntimeReasonName { get; set; } 


        [Display(Name = "Description")]
        public string Description { get; set; }  
    }
}
