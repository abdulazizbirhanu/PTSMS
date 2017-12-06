using PTSMSDAL.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.References
{
    [Table("REF_STAGE")]
    public class Stage : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int StageId { get; set; }

        [Index(IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Stage Name is required.")]
        [Display(Name = "Stage Name")]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Stage Description is required.")]
        [Display(Name = "Stage Description")]
        public string Description { get; set; }        
    }
}