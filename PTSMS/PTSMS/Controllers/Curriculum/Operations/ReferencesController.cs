using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;
using System.Collections.Generic;

namespace PTSMS.Controllers
{
    [PTSAuthorizeAttribute]
    public class ReferencesController : Controller
    {
        ReferenceLogic referenceLogic = new ReferenceLogic();

        // GET: References        
        public ActionResult Index()
        {
            return View(referenceLogic.List());
        }

        // GET: References/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reference reference = (Reference)referenceLogic.Details((int)id);
            if (reference == null)
            {
                return HttpNotFound();
            }
            return View(reference);
        }

        // GET: References/Create
        public ActionResult Create()
        {
            ViewBag.RevisionGroupId = new SelectList((List<Reference>)referenceLogic.List(), "ReferenceId", "ReferenceName");
            return View();
        }

        // POST: References/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reference reference)
        {
            if (ModelState.IsValid)
            {
                if ((bool)referenceLogic.Add(reference))
                    return RedirectToAction("Index");
            }

            ViewBag.RevisionGroupId = new SelectList((List<Reference>)referenceLogic.List(), "ReferenceId", "ReferenceName", reference.RevisionGroupId);
            return View(reference);
        }

        // GET: References/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reference reference = (Reference)referenceLogic.Details((int)id);
            if (reference == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevisionGroupId = new SelectList((List<Reference>)referenceLogic.List(), "ReferenceId", "ReferenceName", reference.RevisionGroupId);
            return View(reference);
        }

        // POST: References/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Reference reference)
        {
            if (ModelState.IsValid)
            {
                if ((bool)referenceLogic.Revise(reference))
                    return RedirectToAction("Index");
            }
            ViewBag.RevisionGroupId = new SelectList((List<Reference>)referenceLogic.List(), "ReferenceId", "ReferenceName", reference.RevisionGroupId);
            return View(reference);
        }

        // GET: References/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reference reference = (Reference)referenceLogic.Details((int)id);
            if (reference == null)
            {
                return HttpNotFound();
            }
            return View(reference);
        }

        // POST: References/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)referenceLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }        
    }
}
