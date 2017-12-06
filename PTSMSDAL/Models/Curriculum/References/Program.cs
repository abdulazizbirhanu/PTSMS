using PTSMSDAL.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.References
{
    [Table("REF_PROGRAM")]
    public class Program : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int ProgramId { get; set; }

        [Index("UK_Program", IsUnique = true, Order = 1)]
        [MaxLength(64)]
        [Required(ErrorMessage = "Program Name is required.")]
        [Display(Name = "Program Name")]
        public string ProgramName { get; set; }
        
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }
        
        [ForeignKey("PreviousProgram")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_Program", IsUnique = true, Order = 2)]        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }
                
        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual Program PreviousProgram { get; set; }
    }
}