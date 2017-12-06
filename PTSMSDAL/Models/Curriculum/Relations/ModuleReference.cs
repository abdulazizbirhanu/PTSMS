using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_MODULEREFERENCE")]
    public class ModuleReference : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleReferenceId { get; set; }

        [ForeignKey("Module")]
        [Index("UK_ModuleReference", IsUnique = true, Order = 1)]
        public int ModuleId { get; set; }

        
        [Index("UK_ModuleReference", IsUnique = true, Order = 2)]       
        [DataType(DataType.Url)]
        [StringLength(200)]
        public string ReferenceURL { get; set; }

        [Index("UK_ModuleReference", IsUnique = true, Order = 3)]
        [StringLength(200)]
        public string FileName { get; set; }

        [StringLength(50)]
        [Index("UK_ModuleReference", IsUnique = true, Order = 4)]

        public string FileType { get; set; }
        public virtual Module Module { get; set; }
        
    }
}