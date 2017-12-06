using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Dispatch.Master
{

    
    [Table("OVERALL_GRADE")]
    public class OverallGrade : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OverallGradeId { get; set; }

        [Display(Name = "Overall Grade Name")]
        [StringLength(100)]
        [Index("UK_OVERALL_GRADE", IsUnique = true, Order = 1)]
        public string OverallGradeName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousOverallGrade")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_OVERALL_GRADE", IsUnique = true, Order = 2)]

        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual OverallGrade PreviousOverallGrade { get; set; }
    }
}

