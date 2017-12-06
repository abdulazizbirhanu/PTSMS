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
    [Table("ACTIVITY_RAMP_OUT")]
    public class ActivityRampOut
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityRampOutId { get; set; }

        [Required]
        [ForeignKey("ActivityCheckIn")]
        [Display(Name = "Activity Check In")]
        [Index("UK_ACTIVITY_RAMP_OUT", IsUnique = true, Order = 1)]
        public int ActivityCheckinId { get; set; }

        [Required]
        [Display(Name = "Hobbs")]
        public int Hobbs { get; set; }

        //[Required]
        //[Display(Name = "Tach")]
        //public int Tach { get; set; }

        [Required]
        [Display(Name = "Adjusted Departure Time")]
        public DateTime AdjustedDepartureTime { get; set; }
                
        [ForeignKey("DepartureTimeReason")]
        [Display(Name = "Adjusted Reason")]
        public int? AdjustedReasonId { get; set; }

        [Display(Name = "Remark")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        public virtual ActivityCheckIn ActivityCheckIn { get; set; }
        public virtual DepartureTimeReason DepartureTimeReason { get; set; }
        
    }
}
