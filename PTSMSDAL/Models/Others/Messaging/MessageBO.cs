using PTSMSDAL.Models.Enrollment.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Others.Messaging
{
    [Table("USER_MESSAGE")]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Body")]
        public string Body { get; set; }

        [Display(Name = "Sender Name")]
        public string SenderName { get; set; }

        [Display(Name = "Sent Time")]
        public DateTime MessageTime { get; set; }

        [Display(Name = "Recipient Name")]
        public string RecipientName { get; set; }

        [Display(Name = "Message Status")]
        public MessageState MessageState { get; set; }

        [Display(Name = "Read Date")]
        public DateTime? ReadDate { get; set; }

        [Display(Name = "Seen Date")]
        public DateTime? SeenDate { get; set; }

       /* [ForeignKey("SenderName")]
        virtual public Person SenderPerson { get; set; }

        [ForeignKey("RecipientName")]
        virtual public Person RecipientPerson { get; set; }*/

    }



    public class UserMessages
    {
        public UserMessages()
        {
            this.UserMessageViewList = new List<UserMessageView>();
        }
        public List<UserMessageView> UserMessageViewList { get; set; }
        public int UnreadMessages { get; set; }
        public int SeenMessages { get; set; }
    }

    public class UserMessageView
    {
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public DateTime TimeSent { get; set; }
        public MessageState MessageState { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime? SeenDate { get; set; }
        public string ObjectURL { get; set; }
        public string Description { get; set; }
    }

    public enum MessageState
    {
        UNREAD,
        SEEN,
        READ
    }
}
