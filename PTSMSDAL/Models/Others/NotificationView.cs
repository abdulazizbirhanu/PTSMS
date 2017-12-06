using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Others
{
    public class NotificationView
    {
        public NotificationView()
        {
            this.ConcernedBodyList = new List<ConcernedBodies>();
        }
        public int NotificationId { get; set; }
        public string Name { get; set; }
        public string EmailMessage { get; set; }
        public string SMSMessage { get; set; }
        public List<ConcernedBodies> ConcernedBodyList { get; set; }
    }

    public class ConcernedBodies
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public bool HasEmailAddress { get; set; }
        public bool HasMobileNumber { get; set; }
        public string LastName { get; set; }
        public int NotificationId { get; set; }
        public int PersonId { get; set; }
        public string PhoneNumber { get; set; }
        public int ScheduleId { get; set; }
    }
}
