using System.Web.Mvc;

namespace PTSMS.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult ServiceUnavailable()
        {
            return View();
        }
        public ActionResult Unauthorized()
        {
            return View();
        }
        public ActionResult BadRequest()
        {
            return View();
        }
        public ActionResult NoOfficerProfile()
        {
            return View();
        }
    }
}