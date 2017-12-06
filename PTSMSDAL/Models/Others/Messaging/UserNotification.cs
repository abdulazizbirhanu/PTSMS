using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PTSMSDAL.Models.Others.Messaging
{
    [Table("USER_NOTIFICATION")]
    public class UserNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserNotificationId { get; set; }

        [Required]
        //[ForeignKey("User")]
        [Display(Name = "Recipient")]
        public string RecipientId { get; set; }

        [ForeignKey("Notification")]
        [Display(Name = "Notification")]
        public int NotificationId { get; set; }

        [Display(Name = "Notification State")]
        public NotificationState NotificationState { get; set; } //This can be an ENUM class, specifying whether the notification is ‘READ’, ‘SEEN’ or ‘UNREAD’.)

        [Display(Name = "Read Date")]
        public DateTime? ReadDate { get; set; }

        [Display(Name = "Seen Date")]
        public DateTime? SeenDate { get; set; }

        public virtual Notification Notification { get; set; }
        //public virtual ApplicationUser User { get; set; }
    }

    public class UserNotifications
    {
        public UserNotifications()
        {
            this.UserNotificationList = new List<UserNotificationView>();
        }
        public List<UserNotificationView> UserNotificationList { get; set; }
        public int UnreadNotifications { get; set; }
        public int SeenNotifications { get; set; }
    }

    public class UserNotificationView
    {
        public int UserNotificationId { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Description { get; set; }
        public string ObjectURL { get; set; }
        public DateTime TimeSent { get; set; }
        public NotificationState NotificationState { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime? SeenDate { get; set; }
    }
}