using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEEPROGRAM")]
    public class TraineeProgram : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeProgramId { get;set; }

        [ForeignKey("TraineeSyllabus")]
        [Index("UK_TraineeProgram", IsUnique = true, Order = 1)]
        public int TraineeSyllabusId { get; set; }

        [ForeignKey("Program")]
        [Index("UK_TraineeProgram", IsUnique = true, Order = 2)]
        public int ProgramId { get; set; }
        
        public virtual TraineeSyllabus TraineeSyllabus { get; set; }
        public virtual Program Program { get; set; }
    }
}