using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("MODULE")]
    public class Module : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int ModuleId { get; set; }

        [Index("UK_Module", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Module Code is required.")]
        [Display(Name = "Module Code")]
        [MaxLength(64)]
        public string ModuleCode { get; set; }

        [Required(ErrorMessage = "Module Title is required.")]
        [Display(Name = "Module Title")]
        public string ModuleTitle { get; set; }

        [Display(Name = "Practical Duration")]
        public float PracticalDuration { get; set; }

        [Display(Name = "Theoretical Duration")]
        public float TheoreticalDuration { get; set; }
        

        [Display(Name = "Course")]
        [ForeignKey("Course")]
        public int?  CourseId { get; set; }

        //[Required(ErrorMessage = "Credit Hour is required.")]
        //[Display(Name = "Credit Hour")]
        //public int CreditHour { get; set; }

        //[Required(ErrorMessage = "External Chapter is required.")]
        [Display(Name = "Regulatory Ref. Chapter")]
        public string ExternalChapter { get; set; }

        //[Required(ErrorMessage = "External Reference is required.")]
        [Display(Name = "Regulatory Ref. Reference")]
        public string ExternalReference { get; set; }
        
        //[Required(ErrorMessage = "Has Exam is required.")]
        [Display(Name = "Has Exam")]
        public bool HasExam { get; set; }

        //[Required(ErrorMessage = "Module Passing Mark is required.")]
        //[Display(Name = "Module Passing Mark")]
        //public float ModulePassingMark { get; set; }

        //[Required(ErrorMessage = "Module Weight is required.")]
        [Display(Name = "Module Weight")]
        public float ModuleWeight { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousModule")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_Module", IsUnique = true, Order = 2)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }
                
        [Display(Name = "Status")]
        public string Status { get; set; }

        //NEWLY ADDED
        [Display(Name = "Syllabus Details and associated learning objective.")]       
        [DataType(DataType.MultilineText)]
        public string MuduleObjective { get; set; }

        [Display(Name = "Learning Teaching Method")]        
        [DataType(DataType.MultilineText)]
        public string LearningTeachingMethod { get; set; }

        [Display(Name = "Assessment Method")]       
        [DataType(DataType.MultilineText)]
        public string AssessmentMethod { get; set; }

        [Display(Name = "Reference Material")]       
        [DataType(DataType.MultilineText)]
        public string ReferenceMaterial { get; set; }

        //[Required(ErrorMessage = "Reference URL.")]
        //[Display(Name = "Reference URL")]
        //public string ReferenceLocation { get; set; }

        [Display(Name = "Remark")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }       
       
        //NEWLY ADDED
        public virtual Module PreviousModule { get; set; }
        public virtual Course Course { get; set; }       

        [NotMapped]
        public List<SelectListItem> DropDownFileLists { get; set; }
    }
}