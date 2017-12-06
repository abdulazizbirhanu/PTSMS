using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("AUDIT_REL_TRAINEEPROGRAM")]
    public class TraineeProgramAudit : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeProgramId { get;set; }

        [ForeignKey("TraineeSyllabus")]
        [Index(IsUnique = true, Order = 1)]
        public int TraineeSyllabusId { get; set; }

        [ForeignKey("Program")]
        [Index(IsUnique = true, Order = 2)]
        public int ProgramId { get; set; }
        
        public virtual TraineeSyllabus TraineeSyllabus { get; set; }
        public virtual Program Program { get; set; }
    }
}