using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Dispatch
{
    [Table("ACTIVITY_AUTHORIZATION")]
    public class ActivityAuthorization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityAuthorizationId { get; set; }

        [Required]
        [ForeignKey("ActivityCheckIn")]        
        [Display(Name = "Activity Check In")]
        [Index("UK_ACTIVITY_AUTHORIZATION", IsUnique = true, Order = 1)]
        public int ActivityCheckinId { get; set; }

        [Display(Name = "Remark")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual ActivityCheckIn ActivityCheckIn { get; set; }
    }

    public enum ActivityAuthorizationStatus
    {
        Authorized,
        Rejected
    }
}
