using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEECATEGORY")]
    public class TraineeCategoryAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeCategoryId { get;set; }

        [ForeignKey("TraineeProgram")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeProgramId { get; set; }

        [ForeignKey("Category")]
        [Index(IsUnique = true, Order = 2)]
        public int CategoryId { get; set; }
        
        public virtual TraineeProgram TraineeProgram { get; set; }
        public virtual Category Category { get; set; }
    }
}