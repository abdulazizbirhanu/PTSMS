using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Models.Others.Messaging;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace PTSMSDAL.Access.Others
{
    public class MessageAccess
    {
        PTSContext db = new PTSContext();


        public List<SelectListItem> AllRecipientPersons()
        {
            var personsList = db.Persons.Select(item => item.CompanyId).Distinct().ToList();
            List<SelectListItem> selectListItem = new List<SelectListItem>();
            foreach (var item in personsList)
            {
                selectListItem.Add(new SelectListItem { Text = item, Value = item });
            }
            return selectListItem;
        }

        public List<Message> List()
        {
            string RecipientName = HttpContext.Current.User.Identity.Name;
            return db.Messages.Where(item => item.RecipientName == RecipientName).OrderBy(item => item.MessageState).ThenByDescending(item => item.MessageTime).ToList();
        }

        public Message Details(int id)
        {
            Message message = db.Messages.Find(id);
            message.MessageState = MessageState.READ;
            message.ReadDate = DateTime.Now;

            db.Entry(message).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return message;
        }

        public bool SaveAndSendMessage(Message message)
        {
            try
            {
                List<Instructor> instructorsList = new List<Instructor>();
                List<Trainee> traineesList = new List<Trainee>();
                List<Person> personsList = new List<Person>();
                var recipientNameArr = message.RecipientName.Split(',');
                foreach (var item in recipientNameArr)
                {

                    if (item == "1")//All Instructors
                        instructorsList = db.Instructors.ToList();
                    else if (item == "2")//All Students
                        traineesList = db.Trainees.ToList();
                    else if (item == "3")//All System Users
                        personsList = db.Persons.ToList();
                    else
                        personsList.Add(db.Persons.FirstOrDefault(x => x.CompanyId == item));


                    foreach (var inst in instructorsList)
                    {
                        personsList.Add(inst.Person);
                    }
                    foreach (var traainee in traineesList)
                    {
                        personsList.Add(traainee.Person);
                    }
                    bool isThereRecipient = false;
                    foreach (var persons in personsList)
                    {
                        isThereRecipient = true;
                        //message.RecipientName = persons.CompanyId;
                        //message.MessageTime = DateTime.Now;
                        //message.SenderName = HttpContext.Current.User.Identity.Name;
                        db.Messages.Add(new Message
                        {
                            RecipientName = persons.CompanyId,
                            MessageTime = DateTime.Now,
                            SenderName = HttpContext.Current.User.Identity.Name,
                            Body = message.Body,
                            MessageState = message.MessageState,
                            ReadDate = message.ReadDate,
                            SeenDate = message.SeenDate,
                            Subject = message.Subject
                        });
                    }
                    if (isThereRecipient)
                    {
                        if (db.SaveChanges() > 0)
                            return true;
                        else
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public UserMessages GetUserMessages(string recipientId)
        {
            try
            {
                UserMessages userMessages = new UserMessages();

                List<UserMessageView> userMessageList = new List<UserMessageView>();

                var userMessagesList = db.Messages.Where(item => item.RecipientName == recipientId).OrderBy(item => item.MessageState).ThenByDescending(item => item.MessageTime).ToList();
                int unreadNotification = userMessagesList.Where(n => n.MessageState == MessageState.UNREAD).ToList().Count();
                int seenNotification = userMessagesList.Where(n => n.MessageState == MessageState.SEEN).ToList().Count();
               
                foreach (var message in userMessagesList)
                {
                    userMessageList.Add(new UserMessageView
                    {
                        //ObjectURL = message.ObjectURL,
                        TimeSent = message.MessageTime,
                        Body = message.Body,
                        Subject = message.Subject,
                        SenderName = message.SenderName,
                        RecipientName = message.RecipientName,
                        ReadDate = message.ReadDate,
                        SeenDate = message.SeenDate,
                        MessageId = message.MessageId,
                        MessageState = message.MessageState
                    });
                }
                userMessages.UserMessageViewList = userMessageList;
                userMessages.SeenMessages = seenNotification;
                userMessages.UnreadMessages = unreadNotification;
                return userMessages;
            }
            catch (Exception)
            {
                return new UserMessages();
            }

        }
    }
}
