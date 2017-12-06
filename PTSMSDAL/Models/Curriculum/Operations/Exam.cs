using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("EXAM")]
    public class Exam : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamId { get; set; }

        [Index("UK_Exam", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Exam Name is required.")]
        [Display(Name = "Exam Name")]
        [MaxLength(200)]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Weight is required.")]
        [Display(Name = "Weight")]
        public float? Weight { get; set; }

        [Display(Name ="Is Pass or Fail ?")]
        public bool IsPassFailExam { get;set;}

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        //[Required(ErrorMessage = "Passing Mark is required.")]
        [Display(Name = "Passing Mark")]
        public float? PassingMark { get; set; }
        
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousExam")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_Exam", IsUnique = true, Order = 2)]        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public Exam PreviousExam { get; set; }
    }
}