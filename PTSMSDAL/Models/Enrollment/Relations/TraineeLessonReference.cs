using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEELESSONREFERENCE")]
    public class TraineeLessonReference : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeLessonReferenceId { get;set; }

        [ForeignKey("TraineeLesson")]
        [Index("UK_TraineeLessonReference", IsUnique = true, Order = 1)]
        public int TraineeLessonId { get; set; }

        [ForeignKey("Reference")]
        [Index("UK_TraineeLessonReference", IsUnique = true, Order = 2)]
        public int ReferenceId { get; set; }
        
        public virtual TraineeLesson TraineeLesson { get; set; }
        public virtual Reference Reference { get; set; }
    }
}