using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("EVALUATIONCATEGORY")]
    public class EvaluationCategory : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EvaluationCategoryId { get; set; }

        [Index("UK_EvaluationCategory", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Evaluation Category Name is required.")]
        [Display(Name = "Evaluation Category Name")]
        [MaxLength(64)]
        public string EvaluationCategoryName { get; set; }

        [ForeignKey("EvaluationTemplate")]
        [Index("UK_EvaluationCategory", IsUnique = true, Order = 2)]
        public int EvaluationTemplateId { get; set; }
        public int sequenceNo { get; set; }
        public virtual EvaluationTemplate EvaluationTemplate { get; set; }
        public virtual ICollection<EvaluationItem> EvaluationItems { get; set; }
    }
}