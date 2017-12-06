using PTSMSBAL.Curriculum.Operations;
using PTSMSBAL.Enrollment.Operations;
using PTSMSBAL.InstructorProfile;
using PTSMSBAL.Logic.Curriculum.Relations;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSBAL.Scheduling.Relations;
using PTSMSDAL.Access.Enrollment.Relations;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.View;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Others.View;
using PTSMSDAL.Models.Scheduling.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.InstructorProfile
{
    public class InstructorProfileController : Controller
    {
        PersonLogic personLogic = new PersonLogic();
        InstructorProfileLogic instructorProfileLogic = new InstructorProfileLogic();
        /// <summary>
        /// This method is used to display LIST OF LESSON assigned for a specific instructor
        /// </summary>
        /// <returns></returns>

        [PTSAuthorizeAttribute]
        public ActionResult InstructorLessons()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    List<Lesson> InstructorLessonList = instructorProfileLogic.InstructorLessons(person.PersonId);
                    return View(InstructorLessonList);
                }
                TempData["GlobalMessage"] = "Failed to find instructor details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        /// <summary>
        /// Used to display specific LESSON DETAIL of an instructor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult LessonDetails(int? id)
        {
            LessonLogic lessonLogic = new LessonLogic();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = (Lesson)lessonLogic.Details((int)id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        /// <summary>
        /// Used to Display lesson evalaution template for a particular lesson
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult LessonEvaluationTemplate(int lessonId)
        {
            EvaluationTemplateLogic evaluationTemplateLogic = new EvaluationTemplateLogic();

            if (HttpContext.User.Identity.Name != null)
            {
                EvaluationTemplateView evaluationTemplate = evaluationTemplateLogic.GetLessonEvaluationTemplate(lessonId);
                return PartialView("LessonEvaluationTemplate", evaluationTemplate);
            }
            else
            {
                ViewBag.GlobalMessage = "Failed to find user detail from the session.";
                return PartialView("LessonEvaluationTemplate", new TraineeEvaluationTemplateView());
            }
        }

        /// <summary>
        /// User to display COURSE LIST that a specific instructor can teach
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult InstructorCourses()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    List<Course> InstructorCourseList = instructorProfileLogic.InstructorCourses(person.PersonId);
                    return View(InstructorCourseList);
                }
                TempData["GlobalMessage"] = "Failed to find instructor details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        /// <summary>
        /// Used to display specific COURSE DETAIL filtered by Course ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [PTSAuthorizeAttribute]
        public ActionResult CourseDetails(int? id)
        {
            CourseLogic courseLogic = new CourseLogic();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = (Course)courseLogic.Details((int)id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        /// <summary>
        /// Used to display MODULE LIST of a specific course for a particular instructor
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult ModuleList(int courseId)
        {
            ModuleLogic moduleLogic = new ModuleLogic();
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    List<Module> InstructorModuleList = instructorProfileLogic.ModuleList(courseId, person.PersonId);
                    return PartialView(InstructorModuleList);
                }
                ViewBag.GlobalMessage = "Failed to find instructor details.";
                return PartialView(new List<Module>());
            }
            else
            {
                ViewBag.GlobalMessage = "Failed to find user detail from the session.";
                return PartialView(new List<Module>());
            }
        }

        /// <summary>
        /// Used to Display instructor MODULE DETAIL filtered by Module Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult ModuleDetails(int moduleId)
        {
            ModuleLogic moduleLogic = new ModuleLogic();
            Module module = (Module)moduleLogic.Details(moduleId);
           
            return PartialView(module);
        }


        /// <summary>
        /// Used to display EXAM LIST of a specific module for a particular instructor
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult ModuleExamList(int moduleId)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    ModuleLogic moduleLogic = new ModuleLogic();
                    ViewBag.CourseId = ((Module)moduleLogic.Details(moduleId)).CourseId;
                    List<ModuleExam> ModuleExamList = instructorProfileLogic.ModuleExamList(moduleId, person.PersonId);
                    return PartialView(ModuleExamList);
                }
                ViewBag.GlobalMessage = "Failed to find instructor details.";
                return PartialView(new List<ModuleExam>());
            }
            else
            {
                ViewBag.GlobalMessage = "Failed to find user detail from the session.";
                return PartialView(new List<ModuleExam>());
            }
        }

        /// <summary>
        /// Used to Display EXAM DETAIL filtered by Module Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult ModuleExamDetails(int moduleExamId)
        {
            ModuleExamLogic moduleExamLogic = new ModuleExamLogic();
            ModuleExam moduleExam = (ModuleExam)moduleExamLogic.Details(moduleExamId);
            return PartialView(moduleExam);
        }
        /// <summary>
        /// Used to display EXAM LIST of a specific COURSE for a particular instructor
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult CourseExamList(int courseId)
        {
            ViewBag.CourseId = courseId;
            if (HttpContext.User.Identity.Name != null)
            {
                CourseExamLogic courseExamLogic = new CourseExamLogic();
                List<CourseExam> CourseExamList = courseExamLogic.CourseExamList(courseId);
                return PartialView(CourseExamList);
            }
            else
            {
                ViewBag.GlobalMessage = "Failed to find user detail from the session.";
                return PartialView(new List<CourseExam>());
            }
        }
        /// <summary>
        /// Used to Display instructor MODULE DETAIL filtered by Module Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult CourseExamDetails(int courseExamId)
        {
            CourseExamLogic courseExamLogic = new CourseExamLogic();
            CourseExam courseExam = (CourseExam)courseExamLogic.Details(courseExamId);
            return PartialView(courseExam);
        }

        /// <summary>
        /// This method is used to display LICENSE for a specific instructor
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult InstructorLicenseList()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    LicenseLogic licenseLogic = new LicenseLogic();
                    var instructorLicense = licenseLogic.GetLicense(person.PersonId);
                    return View(instructorLicense);
                }
                TempData["GlobalMessage"] = "Failed to find instructor details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        /// <summary>
        /// Used to Display LICENSE DETAIL for a particular instructor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [PTSAuthorizeAttribute]
        public PartialViewResult LicenseDetail(int licenceId)
        {
            LicenseLogic licenseLogic = new LicenseLogic();
            var instructorLicense = licenseLogic.Details(licenceId);
            return PartialView(instructorLicense);
        }

        /// <summary>
        /// This method is used to display LEAVE for a specific instructor
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult InstructorLeaveList()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    PersonLeaveLogic personLeaveLogic = new PersonLeaveLogic();
                    var instructorLeave = personLeaveLogic.GetLeave(person.PersonId);
                    return View(instructorLeave);
                }
                TempData["GlobalMessage"] = "Failed to find instructor details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }



        /// <summary>
        /// This method is used to display batch and batch class for which an instructor is assigned to teach
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult BatchList()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    List<Batch> batchList = instructorProfileLogic.GetBatches(person.PersonId);
                    return View(batchList);
                }
                TempData["GlobalMessage"] = "Failed to find instructor details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        /// <summary>
        /// This method is used to display batch detail for which an instructor is assigned to teach
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult BatchDetails(int? id)
        {
            BatchLogic batchLogic = new BatchLogic();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = (Batch)batchLogic.Details((int)id);
            if (batch == null)
            {
                return HttpNotFound();
            }
            BatchClassLogic batchClassLogic = new BatchClassLogic();
            ViewBag.BatchClassList = batchClassLogic.List((int)id);
            ViewBag.TraineeList = batchLogic.ListTrainee((int)id);
            ViewBag.BatchClassSelectList = new SelectList((List<BatchClass>)batchClassLogic.List((int)id), "BatchClassId", "BatchClassName");
            return View(batch);
        }

        /// <summary>
        /// This method is used to display batch class detail for which an instructor is assigned to teach
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult BatchClassDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BatchLogic batchLogic = new BatchLogic();
            BatchClassLogic batchClassLogic = new BatchClassLogic();
            ViewBag.TraineeList = batchLogic.ListTrainee((int)id);
           BatchClass batchClass = (BatchClass)batchClassLogic.Details((int)id);
            if (batchClass == null)
            {
                return HttpNotFound();
            }
            return View(batchClass);
        }


        /// <summary>
        /// This method is used to display SYLLABUS for a specific trainee for which an instructor is assigned to teach his/her batrch
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult TraineeSyllabus(int id, string TraineeFullName, string CompanyId, string BatchClass)
        {
                BatchLogic batchLogic = new BatchLogic();
               
                ViewBag.TraineeProgramHierarchy = batchLogic.GetTraineeHierarchy(id);

                ViewBag.TraineeFullName = TraineeFullName;
                ViewBag.CompanyId = CompanyId;
                ViewBag.BatchClass = BatchClass;
                return View();           
        }


        public ActionResult Profile()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                InstructorProfileView instructorProfile = instructorProfileLogic.Profile(companyId);
                if (TempData["GlobalMessage"] != null)
                {
                    ViewBag.GlobalMessage = TempData["GlobalMessage"];
                    TempData["GlobalMessage"] = null;
                }
                return View(instructorProfile);
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        [HttpPost]
        public ActionResult UpdateProfile(InstructorProfileView InstructorProfile)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;
                string phoneNumber = InstructorProfile.Phone, email = InstructorProfile.Email;
                if (instructorProfileLogic.UpdateProfile(companyId, phoneNumber, email))
                {
                    TempData["GlobalMessage"] = "Your contact information has been successfully updated";
                }
                else
                {
                    TempData["GlobalMessage"] = "Failed to update your contact information.";
                }
                return RedirectToAction("Profile");
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }
    }
}