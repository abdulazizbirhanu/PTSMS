using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_LESSONREFERENCE")]
    public class LessonReference : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int LessonReferenceId { get; set; }

        [ForeignKey("Lesson")]
        [Index("UK_LessonReference", IsUnique = true, Order = 1)]
        public int LessonId { get; set; }
       
        [Index("UK_LessonReference", IsUnique = true, Order = 2)]
        [DataType(DataType.Url)]
        [StringLength(200)]
        public string ReferenceURL { get; set; }

        [Index("UK_LessonReference", IsUnique = true, Order = 3)]
        [StringLength(200)]
        public string FileName { get; set; }

        [Index("UK_LessonReference", IsUnique = true, Order = 4)]
        [StringLength(50)]
        public string FileType { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}