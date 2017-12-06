using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSBAL.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMS.Controllers
{
    public class EvaluationItemsController : Controller
    {
        EvaluationItemLogic evaluationItemLogic = new EvaluationItemLogic();
        EvaluationCategoryLogic evaluationCategoryLogic = new EvaluationCategoryLogic();

        // GET: EvaluationItems
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(evaluationItemLogic.List());
        }

        // GET: EvaluationItems/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationItem evaluationItem = (EvaluationItem)evaluationItemLogic.Details((int)id);
            if (evaluationItem == null)
            {
                return HttpNotFound();
            }
            return View(evaluationItem);
        }

        // GET: EvaluationItems/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.EvaluationCategoryId = new SelectList(((List<EvaluationCategory>)evaluationCategoryLogic.List()).Select(s => new
            {
                EvaluationCategoryId = s.EvaluationCategoryId,
                EvaluationCategoryName = string.Format("{0} - {1}", s.EvaluationTemplate.EvaluationTemplateName, s.EvaluationCategoryName)
            }), "EvaluationCategoryId", "EvaluationCategoryName");
            return View();
        }

        // POST: EvaluationItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(EvaluationItem evaluationItem)
        {
            if (ModelState.IsValid)
            {
                if ((bool)evaluationItemLogic.Add(evaluationItem))
                    return RedirectToAction("Index");
            }

            ViewBag.EvaluationCategoryId = new SelectList(((List<EvaluationCategory>)evaluationCategoryLogic.List()).Select(s => new
            {
                EvaluationCategoryId = s.EvaluationCategoryId,
                EvaluationCategoryName = string.Format("{0} - {1}", s.EvaluationTemplate.EvaluationTemplateName, s.EvaluationCategoryName)
            }), "EvaluationCategoryId", "EvaluationCategoryName", evaluationItem.EvaluationCategoryId);
            return View(evaluationItem);
        }

        // GET: EvaluationItems/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationItem evaluationItem = (EvaluationItem)evaluationItemLogic.Details((int)id);
            if (evaluationItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvaluationCategoryId = new SelectList(((List<EvaluationCategory>)evaluationCategoryLogic.List()).Select(s => new
            {
                EvaluationCategoryId = s.EvaluationCategoryId,
                EvaluationCategoryName = string.Format("{0} - {1}", s.EvaluationTemplate.EvaluationTemplateName, s.EvaluationCategoryName)
            }), "EvaluationCategoryId", "EvaluationCategoryName", evaluationItem.EvaluationCategoryId);
            return View(evaluationItem);
        }

        // POST: EvaluationItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(EvaluationItem evaluationItem)
        {
            if (ModelState.IsValid)
            {
                if ((bool)evaluationItemLogic.Revise(evaluationItem))
                    return RedirectToAction("Index");
            }
            ViewBag.EvaluationCategoryId = new SelectList(((List<EvaluationCategory>)evaluationCategoryLogic.List()).Select(s => new
            {
                EvaluationCategoryId = s.EvaluationCategoryId,
                EvaluationCategoryName = string.Format("{0} - {1}", s.EvaluationTemplate.EvaluationTemplateName, s.EvaluationCategoryName)
            }), "EvaluationCategoryId", "EvaluationCategoryName", evaluationItem.EvaluationCategoryId);
            return View(evaluationItem);
        }

        // GET: EvaluationItems/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationItem evaluationItem = (EvaluationItem)evaluationItemLogic.Details((int)id);
            if (evaluationItem == null)
            {
                return HttpNotFound();
            }
            return View(evaluationItem);
        }

        // POST: EvaluationItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)evaluationItemLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }
}
