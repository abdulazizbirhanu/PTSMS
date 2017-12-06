using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("GROUNDLESSON")]
    public class GroundLesson : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int GroundLessonId { get; set; }

        [Index("UK_GroundLesson", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Lesson Code is required.")]
        [Display(Name = "Lesson Code")]
        [MaxLength(64)]
        public string LessonCode { get; set; }

        [Required(ErrorMessage = "Lesson Name is required.")]
        [Display(Name = "Lesson Name")]
        [MaxLength(64)]
        public string LessonName { get; set; }
                
        [Required(ErrorMessage = "Duration is required.")]
        [Display(Name = "Duration")]
        public float Duration { get; set; }
                
        //[Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        
        [Display(Name = "Effective Date")]
        [Column(TypeName = "DateTime2")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousGroundLesson")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_GroundLesson", IsUnique = true, Order = 2)]        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }
                
        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual  GroundLesson PreviousGroundLesson { get; set; }              
    }
}