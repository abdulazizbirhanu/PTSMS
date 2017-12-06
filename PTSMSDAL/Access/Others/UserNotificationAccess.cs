using PTSMSDAL.Context;
using PTSMSDAL.Models.Others.Messaging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Others
{
    public class UserNotificationAccess
    {
        private PTSContext db = new PTSContext();

        public List<UserNotification> GetNotificationListBySenderId(string recipientId)
        {
            return db.UserNotifications.Where(n => n.RecipientId == recipientId).ToList();
        }

        public List<UserNotification> GetUserNotification(string recipientId)
        {
            try
            {
                return db.UserNotifications.Where(UN => UN.RecipientId == recipientId && UN.NotificationState == NotificationState.UNREAD).ToList();
            }
            catch (Exception ex)
            {
                return new List<UserNotification>();
            }
        }

        public UserNotification Details(int userNotificationId)
        {
            try
            {
                return db.UserNotifications.Find(userNotificationId);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool Add(UserNotification userNotification)
        {
            try
            {
                db.UserNotifications.Add(userNotification);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Revise(UserNotification userNotification)
        {
            try
            {
                db.Entry(userNotification).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                UserNotification userNotification = db.UserNotifications.Find(id);
                db.UserNotifications.Remove(userNotification);
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool NotificationHasSeen(int userNotificationId)
        {
            try
            {
                var userNotification = db.UserNotifications.Find(userNotificationId);
                userNotification.NotificationState = NotificationState.SEEN;
                db.Entry(userNotification).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
