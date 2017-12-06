using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using PTSMSBAL.Curriculum.Operations;
using PTSMSBAL.Logic.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSDAL.Generic;
using PTSMSBAL.Curriculum.References;
using PTSMSDAL.Models.Curriculum.References;

namespace PTSMS.Controllers.Scheduling
{
    public class InstructorSchedulesController : Controller
    {
        ModuleInstructorScheduleLogic moduleInstructorScheduleLogic = new ModuleInstructorScheduleLogic();
        CourseLogic courseLogic = new CourseLogic();
        ModuleLogic moduleLogic = new ModuleLogic();
        ProgramLogic programLogic = new ProgramLogic();
        InstructorLogic instructorLogic = new InstructorLogic();
        ModuleInstructorScheduleLogic moduleInstroctorLogic = new ModuleInstructorScheduleLogic();

        // GET: InstructorSchedules
        public ActionResult Index()
        {
            var moduleInstructorSchedule = moduleInstructorScheduleLogic.ListAll();//db.ModuleInstructorSchedules.Include(i => i.Instructor).Include(i => i.Module).Include(i => i.Instructor.Person);

            if (TempData["InstructorModuleScheduleMessage"] != null)
            {
                ViewBag.InstructorModuleScheduleMessage = TempData["InstructorModuleScheduleMessage"];
                TempData["InstructorModuleScheduleMessage"] = null;
            }
            return View(moduleInstructorSchedule);
        }

        // GET: InstructorSchedules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleInstructorSchedule moduleInstructorSchedule = moduleInstructorScheduleLogic.Details((int)id);
            if (moduleInstructorSchedule == null)
            {
                return HttpNotFound();
            }
            return View(moduleInstructorSchedule);
        }

        // GET: InstructorSchedules/Create
        public ActionResult Create()
        {
            ViewBag.InstructorId = new SelectList(moduleInstructorScheduleLogic.GetInstructorsByQualification("Ground"), "InstructorId", "Instructor.Person.FirstName");

            ViewBag.CourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode");
            ViewBag.ProgramId = new SelectList((List<Program>)programLogic.List(), "ProgramId", "ProgramName");

            return View();
        }

        [HttpGet]
        public ActionResult FilteredListModules(string CourseId)
        {
            int courseId = Convert.ToInt32(CourseId);

            ViewBag.InstructorId = new SelectList(instructorLogic.List(), "InstructorId", "Person.CompanyId");

            ViewBag.CourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseCode");

            var listModules = courseLogic.ListModulesByCourseId(courseId);

            ModuleModel objModuleModel = new ModuleModel();

            List<SelectListItem> modules = new List<SelectListItem>();
            foreach (var item in listModules)
            {
                modules.Add(new SelectListItem { Text = item.ModuleCode + "(" + item.ModuleTitle + ")", Value = item.ModuleId.ToString() });
            }

            objModuleModel.Module = modules;
            //return View(objModuleModel);
            return View("Create", objModuleModel);
        }

        [HttpPost]
        public ActionResult CreateModuleInstructorSchedule()
        {
            string modules = Request.Form["SelectedModules"];
            string instructorId = Request.Form["InstructorId"];

            //Save to Instructor and Instructor Qualification
            //  List<InstructorSchedule> instructorScheduleLst = new List<InstructorSchedule>(); 
            ModuleLogic moduleLogic = new ModuleLogic();
            //Assign Instructor Qualification
            var instrctId = Convert.ToInt32(instructorId);
            var moduleId = 0;
            string message = "";
            string[] moduleArray = modules.Split('~');
            foreach (var module in moduleArray)
            {
                if (string.IsNullOrEmpty(module))
                    continue;

                moduleId = Convert.ToInt32(module);
                Module objModule = (Module)moduleLogic.Details(moduleId);

                ModuleInstructorSchedule moduleInstructorSchedule = new ModuleInstructorSchedule();
                moduleInstructorSchedule.ModuleId = (int)(objModule.RevisionGroupId == null ? objModule.ModuleId : objModule.RevisionGroupId);
                moduleInstructorSchedule.InstructorId = instrctId;

                //save
                var result = moduleInstroctorLogic.ListModuleInstructors(instrctId, moduleId);

                if (result.Count() == 0)
                {
                    moduleInstroctorLogic.Add(moduleInstructorSchedule);

                }
                else
                {
                    message = message + result.FirstOrDefault().Instructor.Person.FirstName + " is already assigned to " + result.FirstOrDefault().Module.ModuleTitle + ". ";
                }
            }
            if (message == "")
                message = "Module has assigned to instructor successfully.";
            TempData["InstructorModuleScheduleMessage"] = message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetModuleByProgram(int programId, int instructorId)
        {
            List<SelectListItem> modules = new List<SelectListItem>();
            var moduleList = courseLogic.ListModulesByProgram(programId);
            var existingAssociation=moduleInstructorScheduleLogic.ListInstructorModuleAssociation(instructorId);

            foreach(var m in moduleList)
            {
                if (existingAssociation.Where(ea => ea.ModuleId.Equals(m.ModuleId)).Count() == 0)
                {
                    modules.Add(new SelectListItem { Text = m.ModuleCode + "(" + m.ModuleTitle + ")", Value = m.RevisionGroupId == null ? m.ModuleId.ToString() : m.RevisionGroupId.ToString() });
                }
            }

            return Json(new { resultData = modules, hasList = modules.Count() > 0 }, JsonRequestBehavior.AllowGet);
        }

        // GET: InstructorSchedules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleInstructorSchedule moduleInstructorSchedule = moduleInstroctorLogic.Details((int)id);
            if (moduleInstructorSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstructorId = new SelectList(instructorLogic.List(), "InstructorId", "InstructorId", moduleInstructorSchedule.ModuleInstructorScheduleId);
            ViewBag.ModuleId = new SelectList((List<Module>)moduleLogic.List(), "ModuleId", "ModuleCode", moduleInstructorSchedule.ModuleId);
            //ViewBag.PreviousRevisionId = new SelectList(db.InstructorSchedules, "InstructorScheduleId", "Status", moduleInstructorSchedule.PreviousRevisionId);
            return View(moduleInstructorSchedule);
        }

        // POST: InstructorSchedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModuleInstructorSchedule instructorSchedule)
        {

            if (ModelState.IsValid)
            {
                moduleInstroctorLogic.Revise(instructorSchedule);
                return RedirectToAction("Index");
            }
            ViewBag.InstructorId = new SelectList(instructorLogic.List(), "InstructorId", "CreatedBy", instructorSchedule.InstructorId);
            ViewBag.ModuleId = new SelectList((List<Module>)moduleLogic.List(), "ModuleId", "ModuleCode", instructorSchedule.ModuleId);
            //ViewBag.PreviousRevisionId = new SelectList(db.ModuleInstructorSchedules, "InstructorScheduleId", "Status");
            return View(instructorSchedule);
        }

        // GET: InstructorSchedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleInstructorSchedule moduleInstructorSchedule = (ModuleInstructorSchedule)moduleLogic.Details((int)id);
            if (moduleInstructorSchedule == null)
            {
                return HttpNotFound();
            }
            return View(moduleInstructorSchedule);
        }

        // POST: InstructorSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            moduleInstroctorLogic.Delete(id);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public class ModuleModel
        {
            public IList<SelectListItem> Module { get; set; }
        }
    }
}
