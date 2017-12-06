using System;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace PTSMS
{
    public class MvcApplication : System.Web.HttpApplication
    { 
        protected void Application_Start()
        { 
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles); 
        }

        /////////////////////////// To enable Session in WebAPI ///////////////////
        protected void Application_PostAuthorizeRequest()
        {
            //if (IsWebApiRequest())
            //{
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            //}
        } 
        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }

        //public void OnThreadEnter()
        //{
        //    //this._savedContext = HttpContextWrapper.SwitchContext(this._context);
        //    //this._context.Impersonation.Start(false, true);
        //    //HttpRuntime.RequestTimeoutManager.Add(this._context);
        //    //this.SetPrincipalOnThread(this._context.User);
        //    //this.SetCulture(false);
        //}

        //public void SetPrincipalOnThread(IPrincipal principal)
        //{
        //    //if (!this._restorePrincipal)
        //    //{
        //    //    this._restorePrincipal = true;
        //    //    this._savedPrincipal = Thread.CurrentPrincipal;
        //    //}
        //    //Thread.CurrentPrincipal = principal;
        //}
    }
}
