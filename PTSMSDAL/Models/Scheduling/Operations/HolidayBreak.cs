using PTSMSDAL.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.Operations
{
    [Table("HOLIDAYS")]
    public class Holiday: AuditAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HolidayId { get; set; }

        [Index("HolidayName", IsUnique = true)]
        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name")]
        [MaxLength(32)]
        public string HolidayName { get; set; }       
        public DateTime StartDateTime { get; set; }      
        public DateTime EndDateTime { get; set; }        
        public string Description { get; set; }
    }
}
