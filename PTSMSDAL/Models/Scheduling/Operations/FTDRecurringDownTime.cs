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
    [Table("FTD_RECURRING_DOWN_TIME")]
    public class FTDRecurringDownTime : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FTDRecurringDownTimeId { get; set; }


        [Required(ErrorMessage = "Break Name is required.")]
       // [Index("UK_FTD_LESSON_BREAK_TIME", IsUnique = true, Order = 1)]
        [Display(Name = "Break Name")]
        public string BreakName { get; set; }

        [Required(ErrorMessage = "Day is required.")]
        //[Index("UK_FTD_LESSON_BREAK_TIME", IsUnique = true, Order = 1)]
        [Display(Name = "Day")]
        public string Day { get; set; } 

        [Required]
        [Display(Name = "Start Date")]
        public TimeSpan StartingDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public TimeSpan EndingDate { get; set; }

    }
}
