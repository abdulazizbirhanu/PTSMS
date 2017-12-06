using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Generic;

namespace PTSMSDAL.Models.Scheduling.References
{
    [Table("LEAVE_TYPE")]
    public class LeaveType : AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaveTypeId { get; set; } 

        [Required(ErrorMessage = "Leave Type is required.")]
        [Display(Name = "Leave Type")]
        public string Type { get; set; }

        
        [Display(Name = "Description")]
        public string Description { get; set; }    

    }
}
