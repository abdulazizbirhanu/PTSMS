using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.APIModels.Models;
using PTSMSDAL.Context;

namespace PTSMS.Controllers.APIController
{
    public class TraineeInfoBOESController : Controller
    {
        private EAA_API_Context db = new EAA_API_Context();

        // GET: TraineeInfoBOES
        public ActionResult Index()
        {
            return View(db.TraineeInfobo.ToList());
        }

        // GET: TraineeInfoBOES/Details/5
        public bool Details(int? id)
        {
            if (id == null)
            {
                return false;
            }
            TraineeInfoBO traineeInfoBO = db.TraineeInfobo.Find(id);
            if (traineeInfoBO == null)
            {
                return false;
            }
            return true;
        }

        // GET: TraineeInfoBOES/Create
        public bool Create()
        {
            return true;
        }

        // POST: TraineeInfoBOES/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public bool Create([Bind(Include = "id,Salutation,FirstName,MiddleName,LastName,Gender,Email,CellPhone,HomePhone,City,Country,EducationalLevel,ApplyingForProgram,CertificateType,Category,registrationDate")] TraineeInfoBO traineeInfoBO)
        {
            if (ModelState.IsValid)
            {
                db.TraineeInfobo.Add(traineeInfoBO);
                db.SaveChanges();
                return true;
            }

            return true;
        }

        // GET: TraineeInfoBOES/Edit/5
        public bool Edit(int? id)
        {
            if (id == null)
            {
                return false;
            }
            TraineeInfoBO traineeInfoBO = db.TraineeInfobo.Find(id);
            if (traineeInfoBO == null)
            {
                return false;
            }
            return true;
        }

        // POST: TraineeInfoBOES/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Salutation,FirstName,MiddleName,LastName,Gender,Email,CellPhone,HomePhone,City,Country,EducationalLevel,ApplyingForProgram,CertificateType,Category")] TraineeInfoBO traineeInfoBO)
        {
            if (ModelState.IsValid)
            {
                traineeInfoBO.registrationDate = traineeInfoBO.registrationDate.Date;
                db.Entry(traineeInfoBO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(traineeInfoBO);
        }

        // GET: TraineeInfoBOES/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TraineeInfoBO traineeInfoBO = db.TraineeInfobo.Find(id);
            if (traineeInfoBO == null)
            {
                return HttpNotFound();
            }
            return View(traineeInfoBO);
        }

        // POST: TraineeInfoBOES/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TraineeInfoBO traineeInfoBO = db.TraineeInfobo.Find(id);
            db.TraineeInfobo.Remove(traineeInfoBO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
