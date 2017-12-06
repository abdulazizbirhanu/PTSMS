using PTSMSDAL.Models.Dispatch;
using PTSMSDAL.Models.Dispatch.Master;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.Relations
{
    [Table("FLIGHT_LOG")]
    public class FlightLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FlightLogId { get; set; }

        [Required]
        [ForeignKey("ActivityCheckIn")]
        [Index("UK_FLIGHT_LOG", IsUnique = true, Order = 1)]
        public int ActivityCheckInId { get; set; }

        [Required]
        [Display(Name = "Day Take-Off")]
        public int DayTakeOff { get; set; }

        [Required]
        [Display(Name = "Night Take-Off")]
        public int NightTakeOff { get; set; }

        [Required]
        [Display(Name = "Day Landing")]
        public int DayLanding { get; set; }

        [Required]
        [Display(Name = "Night Landing")]
        public int NightLanding { get; set; }

        [ForeignKey("InstrumentApproach")]
        [Display(Name = "Instrument Approach")]
        public int? InstrumentApproachId { get; set; }

        [Display(Name = "Remark")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        public virtual ActivityCheckIn ActivityCheckIn { get; set; }
        public virtual InstrumentApproach InstrumentApproach { get; set; }        
    }
}
