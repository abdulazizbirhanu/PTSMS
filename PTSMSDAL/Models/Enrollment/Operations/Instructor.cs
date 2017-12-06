using PTSMSDAL.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("INSTRUCTOR")]
    public class Instructor : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstructorId { get; set; }

        [Required]
     //   [Index("UK_INSTRUCTOR", IsUnique = true, Order = 1)]
        [ForeignKey("Person")]
        public int PersonId { get; set; } 
        
        public virtual Person Person { get; set; }
    }
}