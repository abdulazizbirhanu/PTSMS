using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using PTSMSBAL.Curriculum.References;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSBAL.Scheduling.Others;

namespace PTSMS.Controllers.Scheduling
{
    [PTSAuthorizeAttribute]
    public class InstructorsController : Controller
    {
        //private PTSContext db = new PTSContext();
        InstructorLogic instructorLogic = new InstructorLogic();
        PersonLogic personLogic = new PersonLogic();
        ProgramLogic programLogic = new ProgramLogic();

        // GET: Instructors
        public ActionResult Index()
        {
            var instructors = instructorLogic.List();
            return View(instructors);
        }

        // GET: Instructors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = instructorLogic.Details((Int32)id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructors/Create
        public ActionResult Create()
        {
            var listQualificationType = instructorLogic.GetQulaificationType();

            ViewBag.PersonId = new SelectList((List<Person>)personLogic.List(), "PersonId", "CompanyId");

            QualificationModel objQualificationModel = new QualificationModel();

            List<SelectListItem> types = new List<SelectListItem>();
            foreach (var item in listQualificationType)
            {
                types.Add(new SelectListItem { Text = item.Type, Value = item.QualificationTypeId.ToString() });
            }

            objQualificationModel.QualificationTypes = types;

            return View(objQualificationModel);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection)
        {
            string QualTypes = Request.Form["QualificationTypes"];
            string[] QualificationTypes = QualTypes.Split(',');
            string personIds = Request.Form["PersonId"];
            string[] personIdArray = personIds.Split(',');

            foreach (var personId in personIdArray)
            {
                //Save to Instructor and Instructor Qualification
                Instructor instructor = new Instructor();
                List<InstructorQualification> instructorQualificationLst = new List<InstructorQualification>();

                //Assign Instructor Qualification
                foreach (var item in QualificationTypes)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        InstructorQualification instructorQualification = new InstructorQualification();
                        instructorQualification.QualificationTypeId = Convert.ToInt32(item);
                        instructorQualificationLst.Add(instructorQualification);
                    }
                }
                //Assign Instructor
                instructor.PersonId = Convert.ToInt32(personId);
                //call Save Function
                instructorLogic.Add(instructor, instructorQualificationLst);
            }
            return RedirectToAction("Index");
        }


        // GET: Instructors/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = instructorLogic.Details(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonId = new SelectList((List<Person>)personLogic.List(), "PersonId", "CompanyId", instructor.PersonId);
            ViewBag.ShortName = instructor.Person.ShortName;
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstructorId,PersonId,StartDate,EndDate,CreationDate,CreatedBy,RevisionDate,RevisedBy")] Instructor instructor)
        {
            Instructor inst = instructorLogic.Details(instructor.InstructorId);
            if (ModelState.IsValid)
            {
                Person person = inst.Person;
                person.ShortName = Request.Form["ShortName"].ToString();
                personLogic.Revise(person);
                instructorLogic.Revise(inst);
                return RedirectToAction("Index");
            }
            ViewBag.PersonId = new SelectList((List<Person>)personLogic.List(), "PersonId", "CompanyId", inst.PersonId);
            ViewBag.ShortName = inst.Person.ShortName;
            return View(instructor);
        }
        
        // GET: Instructors/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = instructorLogic.Details(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            instructorLogic.Delete(id);
            return RedirectToAction("Index");
        }

        public class QualificationModel
        {
            public IList<SelectListItem> QualificationTypes { get; set; }
        }

    }


}
