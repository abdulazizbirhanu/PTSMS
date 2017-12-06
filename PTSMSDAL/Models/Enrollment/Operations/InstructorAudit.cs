using PTSMSDAL.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("INSTRUCTORAUDIT")]
    public class InstructorAudit : AuditAttribute
    {
        [Key]
        [ForeignKey("Person")]
        [Index(IsUnique = true, Order = 1)]
        public int InstructorId { get; set; }
        
        public virtual Person Person { get; set; }
    }
}