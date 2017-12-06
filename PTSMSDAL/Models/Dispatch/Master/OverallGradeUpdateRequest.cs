using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Dispatch.Master
{


    [Table("OVERALL_GRADE_UPDATE_REQUEST")]
    public class OverallGradeUpdateRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OverallGradeUpdateRequestId { get; set; }

        [ForeignKey("FlyingFTDSchedule")]
        [Display(Name = "Flying FTD Schedule")]
        [Index("UK_OVERALL_GRADE_UPDATE_REQUEST", IsUnique = true, Order = 1)]
        public int FlyingFTDScheduleId { get; set; }

        [ForeignKey("NewOverallGrade")]
        [Display(Name = "New Overall Grade")]        
        [Index("UK_OVERALL_GRADE_UPDATE_REQUEST", IsUnique = true, Order = 2)]
        public int NewOverallGradeId { get; set; }

        [Required]
        [Display(Name = "Status")]       
        [StringLength(100)]
        [Index("UK_OVERALL_GRADE_UPDATE_REQUEST", IsUnique = true, Order = 3)]
        public string Status { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime ApprovedDate { get; set; }

        public virtual OverallGrade NewOverallGrade { get; set; }
        public virtual FlyingFTDSchedule FlyingFTDSchedule { get; set; }
    }

    public enum OverallGradeUpdateRequestStatus
    {
        Requested,
        Approved,
        Rejected
    }
}
