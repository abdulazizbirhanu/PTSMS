using PTSMSDAL.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.References
{
    [Table("REF_PHASE")]
    public class Phase : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int PhaseId { get; set; }

        [Index(IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Phase Name is required.")]
        [Display(Name = "Phase Name")]
        [MaxLength(64)]
        public string Name { get; set; }

        public int PhaseSequence { get; set; }

        [Required(ErrorMessage = "Phase Description is required.")]
        [Display(Name = "Phase Description")]
        public string Description { get; set; }

        
    }
}