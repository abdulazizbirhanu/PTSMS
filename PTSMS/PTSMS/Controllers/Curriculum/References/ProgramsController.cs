using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSBAL.Curriculum.References;
using System.Collections.Generic;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMS.Controllers
{
    public class ProgramsController : Controller
    {
        ProgramLogic programLogic = new ProgramLogic();

        [HttpGet]
        public ActionResult ProgramList()
        {
            ViewBag.ProgramList = programLogic.List();
            return View();
        }

        [HttpPost]
        public ActionResult ProgramList(FormCollection fc)
        {
            string programId = Request.Form["dpdProgram"];
            return RedirectToAction("Index", "Curriculum", new { programId = programId });
        }
        // relation methods
        public JsonResult ListCategory(int ProgramId)
        {
            List<Category> result = (List<Category>)programLogic.ListCategory(ProgramId);
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.CategoryId,
                    Id = item.RevisionGroupId == null ? item.CategoryId : item.RevisionGroupId,
                    Name = item.CategoryName
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCategory(int ProgramId, string CategoryId)
        {
            if (!(string.IsNullOrEmpty(CategoryId) && string.IsNullOrWhiteSpace(CategoryId)))
            {
                string[] categoryIdArray = CategoryId.Split(',');
                object result = programLogic.AddCategory(ProgramId, categoryIdArray.ToList());
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = new { status = false, message = "Invalid Input" } }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveCategory(int CategoryId)
        {
            object result = programLogic.RemoveCategory(CategoryId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveProgram(int ProgramId)
        {
            bool result = (bool)programLogic.Delete(ProgramId);
            return Json(new { Result = new { status = result, message = result ? null : "" } }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult ProgramDetailPartialView(int programId)
        {            
            Program program = (Program)programLogic.Details(programId);
            return PartialView("ProgramDetailPartialView", program);
        }
        


        // GET: Programs
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(programLogic.List());
        }

        // GET: Programs/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = (Program)programLogic.Details((int)id);
            if (program == null)
            {
                return HttpNotFound();
            }
            return View(program);
        }

        // GET: Programs/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName");
            return View();
        }

        // POST: Programs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(Program program)
        {
            if (ModelState.IsValid)
            {
                if ((bool)programLogic.Add(program))
                    return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName", program.RevisionGroupId);
            return View(program);
        }

        // GET: Programs/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = (Program)programLogic.Details((int)id);
            if (program == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName", program.RevisionGroupId);
            return View(program);
        }

        // POST: Programs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(Program program)
        {
            if (ModelState.IsValid)
            {
                if ((bool)programLogic.Revise(program))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName", program.RevisionGroupId);
            return View(program);
        }

        // GET: Programs/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = (Program)programLogic.Details((int)id);
            if (program == null)
            {
                return HttpNotFound();
            }
            return View(program);
        }

        // POST: Programs/Delete/5
        [PTSAuthorizeAttribute]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)programLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }
}
