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
    [Table("INSTRUMENT_APPROACH")]
    public class InstrumentApproach : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstrumentApproachId { get; set; }

        [StringLength(100)]
        [Index("UK_INSTRUMENT_APPROACH", IsUnique = true, Order = 1)]
        [Display(Name = "Check In Status Name")]
        public string InstrumentApproachName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("PreviousInstrumentApproach")]
        public int? RevisionGroupId { get; set; }

        [Index("UK_INSTRUMENT_APPROACH", IsUnique = true, Order = 2)]
        [Display(Name = "Revision Number")]
        public int RevisionNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual InstrumentApproach PreviousInstrumentApproach { get; set; }
    }
}
