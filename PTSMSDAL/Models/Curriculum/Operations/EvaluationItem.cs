using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("EVALUATIONITEM")]
    public class EvaluationItem : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EvaluationItemId { get; set; }

        [Index("UK_EvaluationCategoryItem", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Evaluation Item Name is required.")]
        [Display(Name = "Evaluation Item Name")]
        [MaxLength(64)]
        public string EvaluationItemName { get; set; }

        [ForeignKey("EvaluationCategory")]
        [Index("UK_EvaluationCategoryItem", IsUnique = true, Order = 2)]
        public int EvaluationCategoryId { get; set; }
        public int sequenceNo { get; set; }

        public virtual EvaluationCategory EvaluationCategory { get; set; }
    }
}