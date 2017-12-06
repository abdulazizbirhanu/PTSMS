using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Utility
{
    public class EmailLogic
    {
        public bool SendSMS(string text, string to)
        {
            try
            {
                string userNameSMSGateWay = ConfigurationManager.AppSettings["userNameSMSGateWay"].ToString();
                string passwordSMSGateWay = ConfigurationManager.AppSettings["passwordSMSGateWay"].ToString();


                string SMSgateWayURL = "http://10.0.231.93:13013/cgi-bin/sendsms?username=";
                string url = SMSgateWayURL + userNameSMSGateWay + "&password=" + passwordSMSGateWay + "&to=" + to + "&text=" + text;

                HttpWebRequest webReqSMSGateway = (HttpWebRequest)WebRequest.Create(string.Format(url));
                webReqSMSGateway.Method = "GET";
                webReqSMSGateway.Proxy = null;

                HttpWebResponse webResponse = (HttpWebResponse)webReqSMSGateway.GetResponse();

                Stream answer = webResponse.GetResponseStream();
                StreamReader _recivedAnswer = new StreamReader(answer);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SendEmail(string message, string toWhom, string subject, string attachmentPath)
        {
            string sentFrom = ConfigurationManager.AppSettings["FromEmail"].ToString();
            string password = ConfigurationManager.AppSettings["password"].ToString();
            string DomainProxy = ConfigurationManager.AppSettings["DomainProxy"].ToString();
            if (!String.IsNullOrEmpty(toWhom))
            {
                string[] reciever = toWhom.Split('~');
                reciever = reciever.Where(email => !String.IsNullOrEmpty(email)).ToArray();

                string sentTo = toWhom;
                using (MailMessage mail = new MailMessage())
                {
                    try
                    {
                        string server = ConfigurationManager.AppSettings["host"].ToString();
                        System.Net.Mail.Attachment attachment;
                        MailAddress from = new MailAddress(sentFrom, sentFrom.Substring(0, sentFrom.IndexOf('@')));
                        //MailAddress to = new MailAddress(supportBO.supporters);
                        MailMessage objMail = new MailMessage();
                        objMail.From = from;
                        for (int i = 0; i < reciever.Length; i++)
                        {
                            if (reciever[i].Contains('@') && reciever[i].Contains('.'))
                            {
                                MailAddress towhom = new MailAddress(reciever[i]);
                                objMail.To.Add(towhom);
                            }
                        }
                        objMail.Subject = subject;
                        objMail.DeliveryNotificationOptions = DeliveryNotificationOptions.Never;
                        objMail.Body = message;
                        objMail.IsBodyHtml = true;
                        SmtpClient smtpClient = new SmtpClient(server, 25);
                        //smtpClient.UseDefaultCredentials = false;  
                        smtpClient.Credentials = new System.Net.NetworkCredential(from.User, password);
                        //smtpClient.EnableSsl = true;
                        /**/
                        if (!String.IsNullOrEmpty(attachmentPath))
                        {
                            attachment = new System.Net.Mail.Attachment(attachmentPath);
                            objMail.Attachments.Add(attachment);
                        }
                        /**/
                        smtpClient.Send(objMail);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
