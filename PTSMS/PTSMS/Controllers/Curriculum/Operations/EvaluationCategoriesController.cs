using System.Data.Entity;
using System.Net;
using System.Linq;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;
using System.Collections.Generic;

namespace PTSMS.Controllers
{
    public class EvaluationCategoriesController : Controller
    {
        EvaluationCategoryLogic evaluationCategoryLogic = new EvaluationCategoryLogic();
        EvaluationTemplateLogic evaluationTemplateLogic = new EvaluationTemplateLogic();

        // GET: EvaluationCategories
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(evaluationCategoryLogic.List());
        }

        // GET: EvaluationCategories/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationCategory evaluationCategory = (EvaluationCategory)evaluationCategoryLogic.Details((int)id);
            if (evaluationCategory == null)
            {
                return HttpNotFound();
            }
            return View(evaluationCategory);
        }

        // GET: EvaluationCategories/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.EvaluationTemplateId = new SelectList(((List<EvaluationTemplate>)evaluationTemplateLogic.List()).Select(item => new
            {
                EvaluationTemplateId = item.RevisionGroupId == null ? item.EvaluationTemplateId : item.RevisionGroupId,
                EvaluationTemplateName = item.EvaluationTemplateName
            }), "EvaluationTemplateId", "EvaluationTemplateName");
            return View();
        }

        // POST: EvaluationCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(EvaluationCategory evaluationCategory)
        {
            if (ModelState.IsValid)
            {
                if ((bool)evaluationCategoryLogic.Add(evaluationCategory))
                    return RedirectToAction("Index");
            }

            ViewBag.EvaluationTemplateId = new SelectList(((List<EvaluationTemplate>)evaluationTemplateLogic.List()).Select(item => new
            {
                EvaluationTemplateId = item.RevisionGroupId == null ? item.EvaluationTemplateId : item.RevisionGroupId,
                EvaluationTemplateName = item.EvaluationTemplateName
            }), "EvaluationTemplateId", "EvaluationTemplateName");
            return View(evaluationCategory);
        }

        // GET: EvaluationCategories/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationCategory evaluationCategory = (EvaluationCategory)evaluationCategoryLogic.Details((int)id);
            if (evaluationCategory == null)
            {
                return HttpNotFound();
            }

            ViewBag.EvaluationTemplateId = new SelectList(((List<EvaluationTemplate>)evaluationTemplateLogic.List()).Select(item => new
            {
                EvaluationTemplateId = item.RevisionGroupId == null ? item.EvaluationTemplateId : item.RevisionGroupId,
                EvaluationTemplateName = item.EvaluationTemplateName
            }), "EvaluationTemplateId", "EvaluationTemplateName", evaluationCategory.EvaluationTemplateId);
            return View(evaluationCategory);
        }

        // POST: EvaluationCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(EvaluationCategory evaluationCategory)
        {
            if (ModelState.IsValid)
            {
                if ((bool)evaluationCategoryLogic.Revise(evaluationCategory))
                    return RedirectToAction("Index");
            }
            ViewBag.EvaluationTemplateId = new SelectList(((List<EvaluationTemplate>)evaluationTemplateLogic.List()).Select(item => new
            {
                EvaluationTemplateId = item.RevisionGroupId == null ? item.EvaluationTemplateId : item.RevisionGroupId,
                EvaluationTemplateName = item.EvaluationTemplateName
            }), "EvaluationTemplateId", "EvaluationTemplateName", evaluationCategory.EvaluationCategoryId);
            return View(evaluationCategory);
        }

        // GET: EvaluationCategories/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationCategory evaluationCategory = (EvaluationCategory)evaluationCategoryLogic.Details((int)id);
            if (evaluationCategory == null)
            {
                return HttpNotFound();
            }
            return View(evaluationCategory);
        }

        // POST: EvaluationCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)evaluationCategoryLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }
}
