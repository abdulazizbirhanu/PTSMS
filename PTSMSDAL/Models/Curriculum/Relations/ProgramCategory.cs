using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Relations
{
    [Table("REL_PROGRAMCATEGORY")]
    public class ProgramCategory : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProgramCategoryId { get; set; }

        [Index("UK_ProgramCategory", IsUnique = true, Order = 1)]
        [ForeignKey("Program")]
        public int ProgramId { get; set; }
        
        [Index("UK_ProgramCategory", IsUnique = true, Order = 2)]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
                
        public virtual Program Program { get; set; }
        public virtual Category Category { get; set; }
    }
}