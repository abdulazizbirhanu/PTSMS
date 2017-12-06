using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("BRIEFING_AND_DEBRIEFING")]
    public class BriefingAndDebriefing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BriefingAndDebriefingId { get; set; }

        [Required]
        [Display(Name = "Starting Time")]
        public DateTime StartingTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndingTime { get; set; }

        [Display(Name = "End Time")]
        public bool IsBriefing { get; set; }

        [Display(Name = "End Time")]
        public bool IsDebriefing { get; set; }

    }
}
