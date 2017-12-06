using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.References;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("TOOL")]
    public class Tool : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ToolId { get; set; }

        [Index("UK_TOOL", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Tool Name is required.")]
        [Display(Name = "Tool Name")]
        [MaxLength(64)]
        public string ToolName { get; set; }

        [Required(ErrorMessage = "Tool Description is required.")]
        [Display(Name = "Tool Description")]
        public string ToolDescription { get; set; }

        [Required(ErrorMessage = "Tool Status is required.")]
        [Display(Name = "Tool Status")]
        public string ToolStatus { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }


        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousTool")]
        public int? PreviousRevisionId { get; set; }

        [Index("UK_TOOL", IsUnique = true, Order = 2)]
        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        
        [Display(Name = "Status")]
        public string Status { get; set; }

        public Tool PreviousTool { get; set; }
        public virtual Location Location { get; set; }
    }
}