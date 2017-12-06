using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Dispatch.Master;
using PTSMSDAL.Models.Enrollment.Operations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Enrollment.Relations
{
    [Table("REL_TRAINEELESSON")]
    public class TraineeLesson : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TraineeLessonId { get; set; }
        [ForeignKey("Trainee")]
        [Index("UK_TraineeLesson", IsUnique = true, Order = 1)]
        public int TraineeId { get; set; }

        [ForeignKey("BatchCategory")]
        [Index("UK_TraineeLesson", IsUnique = true, Order = 2)]
        public int BatchCategoryId { get; set; }

        [ForeignKey("Lesson")]
        [Index("UK_TraineeLesson", IsUnique = true, Order = 3)]
        public int LessonId { get; set; }

        [Index("UK_TraineeLesson", IsUnique = true, Order = 4)]
        public int Sequence { get; set; }

        [ForeignKey("OverallGradeBO")]
        [Display(Name = "Lesson Score")]       
        public int? OverallGradeId { get; set; }

        [ForeignKey("EvaluationTemplate")]
        public int EvaluationTemplateId { get; set; }

        public string Remark { get; set; }
        public DateTime? EvaluationDate { get; set; }

        [StringLength(100)]
        public string Status { get; set; }
        public string TimeIN { get; set; }
        public string TimeOut { get; set; }
        public string FlightTime { get; set; }
        public string FlightDate { get; set; }
        
        public DateTime? AgreedDate { get; set; }

        public virtual Trainee Trainee { get; set; }
        public virtual BatchCategory BatchCategory { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual OverallGrade OverallGradeBO { get; set; }
        public virtual EvaluationTemplate EvaluationTemplate { get; set; }
    }
}