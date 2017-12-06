using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("EVALUATIONTEMPLATE")]
    public class EvaluationTemplate : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EvaluationTemplateId { get; set; }

        [Index("UK_EvaluationTemplate", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Evaluation Template Name is required.")]
        [Display(Name = "EvaluationTemplate Name")]
        [MaxLength(64)]
        public string EvaluationTemplateName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousEvaluationTemplate")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_EvaluationTemplate", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public EvaluationTemplate PreviousEvaluationTemplate { get; set; }

        public virtual Category Category { get; set; }
    }
}