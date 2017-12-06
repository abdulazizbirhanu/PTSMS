using System.Web.Mvc;

namespace PTSMS.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Welcome()
        {
            if (TempData["GlobalMessage"] != null)
            {
                ViewBag.GlobalMessage = TempData["GlobalMessage"];
                TempData["GlobalMessage"] = null;
            }
            if (Session["SchoolName"] == null)
            {
                Session["SchoolName"] = "PILOT TRAINING SCHOOL";
            }
            return View();
        }
    }
}