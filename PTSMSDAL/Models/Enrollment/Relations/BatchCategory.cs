using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_BATCHCATEGORY")]
    public class BatchCategory : AuditAttribute
    {
        [Key]
        public int BatchCategoryId { get;set; }

        [ForeignKey("Batch")]
        [Index("UK_BatchCategory", IsUnique = true, Order = 1)]
        public int BatchId { get; set; }

        [ForeignKey("Category")]
        [Index("UK_BatchCategory", IsUnique = true, Order = 2)]
        public int CategoryId { get; set; }
        public int Sequence { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual Category Category { get; set; }
    }
}