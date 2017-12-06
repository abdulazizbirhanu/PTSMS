using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.Others
{
    public class AATMSMethodFilter : ActionFilterAttribute
    {
        private readonly NameValueCollection formData;
        public NameValueCollection FormData { get { return formData; } }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //HttpContext.Current =  ;
            if (filterContext.ActionDescriptor.ActionName == "Login")
            {
                //string schoolCode = formData["SchoolCode"];
                //if (!string.IsNullOrEmpty(schoolCode))
                //{
                //    //schoolLogic.ChangeConnectionStringInWebConfig(Int16.Parse(schoolId));
                //    System.Web.HttpContext.Current.Session["SchoolCode"] = schoolCode;
                //}
            }
            else
            {
                // do something else
            }
        }
    }
}