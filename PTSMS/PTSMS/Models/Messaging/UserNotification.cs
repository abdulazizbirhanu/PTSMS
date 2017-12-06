using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PTSMS.Models.Messaging
{
    [Table("USER_NOTIFICATION")]
    public class UserNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserNotificationId { get; set; }

        [Required]
        [ForeignKey("User")]
        [Display(Name = "Recipient")]
        public string RecipientId { get; set; }

        [Display(Name = "Notification State")]
        public NotificationState NotificationState { get; set; } //This can be an ENUM class, specifying whether the notification is ‘READ’, ‘SEEN’ or ‘UNREAD’.)

        [Display(Name = "Read Date")]
        public DateTime? ReadDate { get; set; }

        [Display(Name = "Seen Date")]
        public DateTime? SeenDate { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}