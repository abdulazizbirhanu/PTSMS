using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Models.Others.Messaging;
using PTSMSDAL.Access.Others;
using System.Web.Mvc;

namespace PTSMSBAL.Others
{
    public class MessageLogic
    {
        MessageAccess messageAccess = new MessageAccess();


        public List<Message> List()
        {
            return messageAccess.List();
        }

        public Message Details(int id)
        {
            return messageAccess.Details(id);
        }

        public bool SaveAndSendMessage(Message message)
        {
            return messageAccess.SaveAndSendMessage(message);
        }

        public UserMessages GetUserMessages(string name)
        {
            return messageAccess.GetUserMessages(name);
        }

        public List<SelectListItem> AllRecipientPersons()
        {
            return messageAccess.AllRecipientPersons();
        }
    }
}
