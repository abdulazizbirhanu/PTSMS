using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Scheduling.References;

namespace PTSMSDAL.Models.Enrollment.Operations
{
    [Table("PERSON_LEAVE")]
    public class PersonLeave : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int PersonLeaveId { get; set; } 

        [Required(ErrorMessage = "Leave Type is required.")]
        [ForeignKey("LeaveType")]
        [Index("UK_PERSON_LEAVE", IsUnique = true, Order = 1)]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }


        [Required(ErrorMessage = "Person Id is required.")]
        [ForeignKey("Person")]
        [Index("UK_PERSON_LEAVE", IsUnique = true, Order = 2)]
        [Display(Name = "Person")]
        public int PersonId { get; set; }

        [Index("UK_PERSON_LEAVE", IsUnique = true, Order = 3)]
        [Required(ErrorMessage = "From Date is required.")]
        [Display(Name = "Start Date")]
        [Column(TypeName = "DateTime2")]
        public DateTime FromDate { get; set; }

        [Index("UK_PERSON_LEAVE", IsUnique = true, Order = 4)]
        [Required(ErrorMessage = "To Date is required.")]
        [Column(TypeName = "DateTime2")]
        [Display(Name = "End Date")]
        public DateTime ToDate { get; set; }


        [Display(Name = "Description")]
        public string Description { get; set; } 


        public virtual Person Person { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        
    }
}
