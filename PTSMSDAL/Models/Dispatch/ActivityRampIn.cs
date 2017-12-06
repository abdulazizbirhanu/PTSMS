using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Dispatch
{
    [Table("ACTIVITY_RAMP_IN")]
    public class ActivityRampIn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityRampInId { get; set; }

        [Required]
        [ForeignKey("ActivityRampOut")]
        [Display(Name = "Activity Ramp Out")]
        [Index("UK_ACTIVITY_RAMP_IN", IsUnique = true, Order = 1)]
        public int ActivityRampOutId { get; set; }

        [Required]
        [Display(Name = "Hobbs")]
        public int Hobbs { get; set; }

        //[Display(Name = "Tach")]
        //public int Tach { get; set; }

        [Required]
        [Display(Name = "Adjusted Arrival Time")]
        public DateTime AdjustedArrivalTime { get; set; }

        [ForeignKey("ArrivalTimeReason")]
        [Display(Name = "Arrival Time Reason")]
        public int? ArrivalTimeReasonId { get; set; }

        [Display(Name = "Remark")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        public virtual ActivityRampOut ActivityRampOut { get; set; }
        public virtual ArrivalTimeReason ArrivalTimeReason { get; set; }
        
    }
}
