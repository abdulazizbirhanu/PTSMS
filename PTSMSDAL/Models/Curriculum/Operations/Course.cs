using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("COURSE")]
    public class Course : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int CourseId { get; set; }
        
        [Index("UK_Course", IsUnique = true, Order = 1)]
        [MaxLength(32)]
        [Required(ErrorMessage = "Course Code is required.")]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Course Title is required.")]
        [Display(Name = "Course Title")]
        public string CourseTitle { get; set; }

        [Display(Name = "Practical Duration")]
        public float PracticalDuration { get; set; }

        [Display(Name = "Theoretical Duration")]
        public float TheoreticalDuration { get; set; }

        [Required(ErrorMessage = "Credit Hour is required.")]
        [Display(Name = "Credit Hour")]
        public int CreditHour { get; set; }

        //[Required(ErrorMessage = "External Time is required.")]
        [Display(Name = "Regulatory Ref. Time")]
        public string ExternalTime { get; set; }

        //[Required(ErrorMessage = "External Chapter is required.")]
        [Display(Name = "Regulatory Ref. Chapter")]
        public string ExternalChapter { get; set; }

        //[Required(ErrorMessage = "External Reference is required.")]
        [Display(Name = "Regulatory Ref. Reference")]
        public string ExternalReference { get; set; }

        [Required(ErrorMessage = "Course Passing Mark is required.")]
        [Display(Name = "Course Passing Mark")]
        public float CoursePassingMark { get; set; }
        
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousCourse")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_Course", IsUnique = true, Order = 2)]        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }
                
        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual Course PreviousCourse { get; set; }        
    }
}