using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.View;

namespace PTSMS.Controllers
{
    public class EvaluationTemplatesController : Controller
    {
        EvaluationTemplateLogic evaluationTemplateLogic = new EvaluationTemplateLogic();

        public JsonResult ListEvaluationTemplate()
        {
            List<EvaluationTemplate> result = (List<EvaluationTemplate>)evaluationTemplateLogic.List();

            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.EvaluationTemplateId,
                    Id = item.RevisionGroupId == null ? item.EvaluationTemplateId : item.RevisionGroupId,

                    Name = item.EvaluationTemplateName
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult EvaluationTemplateDetailPartialView(int lessonEvaluationTemplateId)
        {
            EvaluationTemplateView evaluationTemplate = evaluationTemplateLogic.EvaluationTemplateDetail(lessonEvaluationTemplateId);
            return PartialView("EvaluationTemplateDetailPartialView", evaluationTemplate);
        }

        // GET: EvaluationTemplates
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(evaluationTemplateLogic.List());
        }

        // GET: EvaluationTemplates/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationTemplateView evaluationTemplateView = (EvaluationTemplateView)evaluationTemplateLogic.Details((int)id);
            if (evaluationTemplateView == null)
            {
                return HttpNotFound();
            }
            return View(evaluationTemplateView);
        }

        // GET: EvaluationTemplates/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList((List<EvaluationTemplate>)evaluationTemplateLogic.List(), "EvaluationTemplateId", "EvaluationTemplateName");
            return View();
        }

        // POST: EvaluationTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(EvaluationTemplate evaluationTemplate)
        {
            if (ModelState.IsValid)
            {
                if ((bool)evaluationTemplateLogic.Add(evaluationTemplate))
                    return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList((List<EvaluationTemplate>)evaluationTemplateLogic.List(), "EvaluationTemplateId", "EvaluationTemplateName", evaluationTemplate.RevisionGroupId);
            return View(evaluationTemplate);
        }

        // GET: EvaluationTemplates/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationTemplate evaluationTemplate = (EvaluationTemplate)evaluationTemplateLogic.Details((int)id);
            if (evaluationTemplate == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList((List<EvaluationTemplate>)evaluationTemplateLogic.List(), "EvaluationTemplateId", "EvaluationTemplateName", evaluationTemplate.RevisionGroupId);
            return View(evaluationTemplate);
        }

        // POST: EvaluationTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(EvaluationTemplate evaluationTemplate)
        {
            if (ModelState.IsValid)
            {
                if ((bool)evaluationTemplateLogic.Revise(evaluationTemplate))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList((List<EvaluationTemplate>)evaluationTemplateLogic.List(), "EvaluationTemplateId", "EvaluationTemplateName", evaluationTemplate.RevisionGroupId);
            return View(evaluationTemplate);
        }

        // GET: EvaluationTemplates/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationTemplate evaluationTemplate = (EvaluationTemplate)evaluationTemplateLogic.Details((int)id);
            if (evaluationTemplate == null)
            {
                return HttpNotFound();
            }
            return View(evaluationTemplate);
        }

        // POST: EvaluationTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)evaluationTemplateLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }
}
