using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSBAL.Curriculum.References;
using System.Collections.Generic;
using PTSMSDAL.Models.Curriculum.Operations;

namespace PTSMS.Controllers
{
    [PTSAuthorizeAttribute]
    public class PhasesController : Controller
    {
        PhaseLogic phaseLogic = new PhaseLogic();

        // GET: Phases
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(phaseLogic.List());
        }

        // GET: Phases/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phase phase = (Phase)phaseLogic.Details((int)id);
            if (phase == null)
            {
                return HttpNotFound();
            }
            return View(phase);
        }

        // GET: Phases/Create

        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Phases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(Phase phase)
        {
            if (ModelState.IsValid)
            {
                if ((bool)phaseLogic.Add(phase))
                    return RedirectToAction("Index");
            }
            return View(phase);
        }

        // GET: Phases/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phase phase = (Phase)phaseLogic.Details((int)id);
            if (phase == null)
            {
                return HttpNotFound();
            }
            return View(phase);
        }

        // POST: Phases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(Phase phase)
        {
            if (ModelState.IsValid)
            {
                if ((bool)phaseLogic.Revise(phase))
                    return RedirectToAction("Index");
            }
            return View(phase);
        }

        // GET: Phases/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phase phase = (Phase)phaseLogic.Details((int)id);
            if (phase == null)
            {
                return HttpNotFound();
            }
            return View(phase);
        }

        // POST: Phases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)phaseLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }
    }
}
