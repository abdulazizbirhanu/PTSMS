using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PTSMS.Models.Messaging
{
    [Table("NOTIFICATION")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        [ForeignKey("User")]
        [Display(Name = "Sender")]
        public string SenderId { get; set; }

        [Display(Name = "Notification Type")]
        public NotificationType NotificationType { get; set; }//'Schedule Change', 'Curriculum Change', etc

        [Display(Name = "Notification Description")]
        public string Description { get; set; }

        [Display(Name = "Object URL")]
        public string ObjectURL { get; set; }

        [Display(Name = "Time Sent")]
        public DateTime TimeSent { get; set; }
        public virtual ApplicationUser User { get; set; }
    }


    public enum NotificationType
    {
        ScheduleChange,
        CurriculumChange,
        NewSchedule,
        LessonEvaluation
    }

    public enum NotificationState
    {
        READ,
        SEEN,
        UNREAD
    }
}