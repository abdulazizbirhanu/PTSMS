using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Scheduling.References
{
    [Table("EQUIPMENT")]
    public class Equipment : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "NameOrSerialNo is required.")]
        [Index("UK_EQUIPMENT", IsUnique = true, Order = 1)]
        [MaxLength(30)]
        [Display(Name = "NameOrSerialNo")]
        public string NameOrSerialNo { get; set; }

        [Required(ErrorMessage = "Equipment Model Id is required.")]
        [Index("UK_EQUIPMENT", IsUnique = true, Order = 2)]
        [ForeignKey("EquipmentModel")]
        [Display(Name = "Equipment Model Id")]
        public int EquipmentModelId { get; set; }

        [Required]
        [Display(Name = "Status")]
        [ForeignKey("EquipmentStatus")]
        public int EquipmentStatusId { get; set; }

        [Required]
        [Display(Name = "Working Hours")]
        public Decimal WorkingHours { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Room No")]
        public int? RoomNo { get; set; }

        [Display(Name = "Building")]
        public int? Building { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [Display(Name = "Location Id")]
        [ForeignKey("Location")]
        public int LocationId { get; set; }

        [Display(Name = "Total Hours")]
        public float TotalFlyingHours { get; set; }

        [Display(Name = "Estimated Hours")]
        public float EstimatedRemainingHours { get; set; }

        [Display(Name = "Actual Hours")]
        public float ActualRemainingHours { get; set; }

        [Display(Name = "Enable Auto forcast")]
        public bool EnableAutoForcast{get;set;}

        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual Location Location { get; set; }
        public virtual EquipmentStatus EquipmentStatus { get; set; }
        public virtual EquipmentModel EquipmentModel { get; set; }
    }
}
