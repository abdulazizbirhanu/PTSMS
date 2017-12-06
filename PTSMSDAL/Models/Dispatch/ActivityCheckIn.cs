using PTSMSDAL.Models.Dispatch.Master;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Dispatch
{
    [Table("ACTIVITY_CHECKIN")]
    public class ActivityCheckIn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityCheckInId { get; set; }

        [Required]
        [ForeignKey("FlyingFTDSchedule")]
        [Display(Name = "Flying FTD Schedule")]
        [Index("UK_ACTIVITY_CHECKIN", IsUnique = true, Order = 1)]
        public int FlyingFTDScheduleId { get; set; }

        [Required]
        [ForeignKey("Equipment")]
        [Display(Name = "Equipment")]
        public int EquipmentId { get; set; }

        [Required]
        [ForeignKey("Instructor")]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        [ForeignKey("Observer")]
        [Display(Name = "Observer")]
        public int? ObserverId { get; set; }

        [Required]
        [Display(Name = "Sequence")]
        [Index("UK_ACTIVITY_CHECKIN", IsUnique = true, Order = 2)]
        public int Sequence { get; set; }

        //[Required]
        //[Display(Name = "Start Time")]
        //public DateTime StartTime { get; set; }

        //[Display(Name = "End Time")]
        //public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Check-In Time")]
        public DateTime CheckInTime { get; set; }
        

        [ForeignKey("Destination")]
        [Display(Name = "Destination")]
        public int? DestinationId { get; set; }

        [ForeignKey("ParkingSpot")]
        [Display(Name = "Parking Spot")]
        public int? ParkingSpotId { get; set; }

        [ForeignKey("DepartureAirport")]
        [Display(Name = "Departure Airport")]
        public int? DepartureAirportId { get; set; }

        [ForeignKey("ArrivalAirport")]
        [Display(Name = "Arrival Airport")]
        public int? ArrivalAirportId { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [ForeignKey("CheckInStatus")]
        [Display(Name = "Check In Status")]
        public int? CheckInStatusId { get; set; }
               

        [ForeignKey("OperationAreas")]
        [Display(Name = "Operation Area")]
        public int? OperationAreaId { get; set; }
        
        public virtual FlyingFTDSchedule FlyingFTDSchedule { get; set; }
        public virtual Airport DepartureAirport { get; set; }
        public virtual Airport ArrivalAirport { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual Instructor Observer { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual Destination Destination { get; set; }
        public virtual ParkingSpot ParkingSpot { get; set; }
        public virtual CheckInStatus CheckInStatus { get; set; }
        public virtual OperationArea OperationAreas { get; set; }
       

    }
}
