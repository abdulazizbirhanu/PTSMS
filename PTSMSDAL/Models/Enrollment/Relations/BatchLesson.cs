using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Dispatch.Master;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_BATCHLESSON")]
    public class BatchLesson : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BatchLessonId { get; set; }

        [ForeignKey("BatchCategory")]
        [Index("UK_BatchCourse", IsUnique = true, Order = 1)]
        public int BatchCategoryId { get; set; }

        [ForeignKey("Lesson")]
        [Index("UK_BatchCourse", IsUnique = true, Order = 2)]
        public int LessonId { get; set; }

        [ForeignKey("EvaluationTemplate")]
        public int EvaluationTemplateId { get; set; }
        [ForeignKey("Phase")]
        public int PhaseId { get; set; }
        public int Sequence { get; set; }

        public virtual EvaluationTemplate EvaluationTemplate { get; set; }
        public virtual BatchCategory BatchCategory { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual Phase Phase { get; set; }
    }
}