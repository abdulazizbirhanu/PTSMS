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
    [Table("OPERATION_AREA")]
    public class OperationArea : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Operation Area")]
        public int OperationAreaId { get; set; }

        [Display(Name = "Operation Area Name")]
        [Index("UK_OPERATION_AREA", IsUnique = true, Order = 1)]
        [StringLength(100)]
        public string OperationAreaName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousOperationArea")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_OPERATION_AREA", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual OperationArea PreviousOperationArea { get; set; }
    }
}
