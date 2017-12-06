using PTSMSBAL.Others;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Others.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.Others
{
    public class MessageController : Controller
    {
        MessageLogic messageLogic = new MessageLogic();

        // GET: Message
        public ActionResult Index()
        {            
            return View(messageLogic.List());
        }
        // GET: Message/Details/id
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = messageLogic.Details(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        public ActionResult Create()
        {
            List<SelectListItem> selectListItem = new List<SelectListItem> {
                 new SelectListItem {Text = "All Instractors", Value = "1"},
                        new SelectListItem {Text = "All Students", Value = "2"},
                        new SelectListItem {Text = "All System Users", Value = "3"}
            };
            selectListItem.AddRange(messageLogic.AllRecipientPersons());
            var selectRecipients = new SelectList(selectListItem, "Value", "Text");
            ViewBag.SelectRecipients = selectRecipients;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RecipientName,Subject,Body")] Message Message)
        {
            if (ModelState.IsValid)
            {
                var recipientName = Request.Form["RecipientName"];
                if (!String.IsNullOrEmpty(recipientName) || !String.IsNullOrWhiteSpace(recipientName))
                {
                    Message.RecipientName = recipientName;
                    messageLogic.SaveAndSendMessage(Message);
                    return RedirectToAction("Index");
                }
            }

            List<SelectListItem> selectListItem = new List<SelectListItem> {
                 new SelectListItem {Text = "All Instractors", Value = "1"},
                        new SelectListItem {Text = "All Students", Value = "2"},
                        new SelectListItem {Text = "All System Users", Value = "3"}
            };

            selectListItem.AddRange(messageLogic.AllRecipientPersons());
            var selectRecipients = new SelectList(selectListItem, "Value", "Text");
            ViewBag.SelectRecipients = selectRecipients;

            return View(Message);
        }
    }
}