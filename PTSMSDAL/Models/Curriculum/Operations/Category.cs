using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.References;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSMSDAL.Models.Curriculum.Operations
{
    [Table("CATEGORY")]
    public class Category : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int CategoryId { get; set; }

        [Index("UK_Category", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "Category Type is required.")]
        [Display(Name = "Category Type")]
        [ForeignKey("CategoryType")]
        public int CategoryTypeId { get; set; }

        [Index("UK_Category", IsUnique = true, Order = 2)]
        [Required(ErrorMessage = "Category Name is required.")]
        [Display(Name = "Category Name")]
        [MaxLength(64)] 
        public string CategoryName { get; set; }
        
        //[Required(ErrorMessage = "Time Aircraft Dual is required.")]
        [Display(Name = "Time Aircraft Dual")]
        public float TimeAircraftDual { get; set; }

        //[Required(ErrorMessage = "Time Aircraft Solo is required.")]
        [Display(Name = "Time Aircraft Solo")]
        public float TimeAircraftSolo { get; set; }

        //[Required(ErrorMessage = "FTD Time is required.")]
        [Display(Name = "FTD Time")]
        public float FTDTime { get; set; }
        
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousCategory")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_Category", IsUnique = true, Order = 3)]        
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }
                
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Pilot Flying Time")]
        public float PilotFlying { get; set; }

        [Display(Name = "Pilot Monitoring Time")]
        public float PilotMonitoring { get; set; }

        public virtual Category PreviousCategory{ get; set; }

        public virtual CategoryType CategoryType { get; set; }
    }
}