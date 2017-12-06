using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("CLASSROOM")]
    public class ClassRoom : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassRoomId { get; set; }

        [Index("UK_CLASSROOM", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Room Number is required.")]
        [Display(Name = "Room Number")]
        [MaxLength(32)]
        public string RoomNo { get; set; }

        [ForeignKey("Building")]
        [Index("UK_CLASSROOM", IsUnique = true, Order = 2)]
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Display(Name = "Capacity")]
        public string Capacity { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousClassRoom")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_CLASSROOM", IsUnique = true, Order = 3)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public ClassRoom PreviousClassRoom { get; set; }
        public virtual Building Building { get; set;}
        public virtual Location Location { get; set; }
    }
}