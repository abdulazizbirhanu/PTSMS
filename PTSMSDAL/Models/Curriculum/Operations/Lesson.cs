using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("LESSON")]
    public class Lesson : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Equipment Type is required.")]
        [Index("UK_Lesson", IsUnique = true, Order = 1)]
        [Display(Name = "Equipment Type")]
        [ForeignKey("EquipmentType")]
        public int EquipmentTypeId { get; set; }

        [Index("UK_Lesson", IsUnique = true, Order = 2)]
        [Required(ErrorMessage = "Lesson Category Type is required.")]
        [Display(Name = "Lesson Category Type")]
        [ForeignKey("CategoryType")]
        public int CategoryTypeId { get; set; }

        [Index("UK_Lesson", IsUnique = true, Order = 3)]
        [Required(ErrorMessage = "Lesson Name is required.")]
        [Display(Name = "Lesson Name")]
        [MaxLength(300)]
        public string LessonName { get; set; }

        //[Required(ErrorMessage = "Time Aircraft Dual is required.")]
        [Display(Name = "TimeAircraftDual")]
        public float TimeAircraftDual { get; set; }

        //[Required(ErrorMessage = "Time Aircraft Solo is required.")]
        [Display(Name = "Time Aircraft Solo")]
        public float TimeAircraftSolo { get; set; }

        //[Required(ErrorMessage = "FTD Time is required.")]
        [Display(Name = "FTD Time")]
        public float FTDTime { get; set; }

        //[Required(ErrorMessage = "Is Check Ride is required.")]
        [Display(Name = "Is Check Ride")]
        public bool IsCheckRide { get; set; }

        //[Required(ErrorMessage = "Check Ride Passing Mark is required.")]
        [Display(Name = "Check Ride Passing Mark")]
        public float CheckRidePassingMark { get; set; }

        //[Required(ErrorMessage = "Lesson Passing Mark is required.")]
        [Display(Name = "Lesson Passing Mark")]
        public string LessonPassingMark { get; set; }


        [Display(Name = "IsPreSolo")]
        public bool IsPreSolo { get; set; }

        [Display(Name = "Retaken Count")]
        public int RetakenCount { get; set; }

        [Display(Name = "Effective Date")]
        [Column(TypeName = "DateTime2")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousLesson")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_Lesson", IsUnique = true, Order = 4)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public Lesson PreviousLesson { get; set; }

        //NEWLY ADDED
        [Display(Name = "Topic")]
        [DataType(DataType.MultilineText)]       
        public string Topic { get; set; }

        [Display(Name = "Objective")]       
        [DataType(DataType.MultilineText)]
        public string Objective { get; set; }

        [Display(Name = "Reference")]        
        [DataType(DataType.MultilineText)]
        public string Reference { get; set; }


        [Display(Name = "Required Instruction during Flight")]       
        [DataType(DataType.MultilineText)]
        public string RequiredInstructionDuringFlight { get; set; }

        [Display(Name = "Trainee Competency Criteria")]       
        [DataType(DataType.MultilineText)]
        public string TraineeCompetencyCriteria { get; set; }

        [Display(Name = "Instructor Pre-brief")]      
        [DataType(DataType.MultilineText)]
        public string InstructorPrebrief { get; set; }

        [Display(Name = "Lesson Plan")]      
        [DataType(DataType.MultilineText)]
        public string LessonPlan { get; set; }

        [Display(Name = "Is Type Rating")]     
        public bool IsTypeRating { get; set; }
         
        [Display(Name = "Pilot Flying Time")]
        public float PilotFlying { get; set; }
         
        [Display(Name = "Pilot Monitoring Time")]
        public float PilotMonitoring { get; set; }

        //NEWLY ADDED

        public virtual CategoryType CategoryType { get; set; }//Multi-Engine(ME), NIGHT FLYING()NTF, Cross-Country
        public virtual EquipmentType EquipmentType { get; set; } //FTD, Flying       

        [NotMapped]
        public List<SelectListItem> LessonReferenceFiles { get; set; }
    }
}