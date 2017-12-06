using PTSMSBAL.Curriculum.Operations;
using PTSMSBAL.Enrollment.Operations;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSBAL.Scheduling.Relations;
using PTSMSBAL.TraineeProfile;
using PTSMSDAL.Access.Enrollment.Relations;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Scheduling.View;
using PTSMSDAL.Models.Others.View;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace PTSMS.Controllers.TraineeProfile
{
    public class TraineeProfileController : Controller
    {
        BatchLogic batchLogic = new BatchLogic();
        PersonLogic personLogic = new PersonLogic();
        TraineeProfileLogic traineeProfileLogic = new TraineeProfileLogic();
        TraineeBatchClassAccess traineeBatchClassAccess = new TraineeBatchClassAccess();

        /// <summary>
        /// This method is used to get SYLLABUS for a specific trainee
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult TraineeSyllabus()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    ViewBag.TraineeProgramHierarchy = batchLogic.GetTraineeHierarchy(person.PersonId);

                    ViewBag.TraineeFullName = person.FirstName + " " + person.MiddleName + " " + person.LastName;
                    ViewBag.CompanyId = person.CompanyId;
                    ViewBag.BatchClass = traineeBatchClassAccess.BatchClassDetails(person.PersonId).BatchClass.BatchClassName;
                    return View();
                }
                TempData["GlobalMessage"] = "Failed to find trainee details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        /// <summary>
        /// This method is used to display LIST OF LESSON for a specific trainee
        /// </summary>
        /// <returns></returns>

        [PTSAuthorizeAttribute]
        public ActionResult TraineeLessons()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    List<Lesson> TraineeLessonList = traineeProfileLogic.TraineeLessons(person.PersonId);
                    return View(TraineeLessonList);
                }
                TempData["GlobalMessage"] = "Failed to find trainee details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        /// <summary>
        /// Used to display specific LESSON DETAIL of a trainee filtered by Lesson ID
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
        /// User to display COURSE LIST of a specific trainee
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult TraineeCourses()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    List<Course> TraineeCourseList = traineeProfileLogic.TraineeCourses(person.PersonId);
                    return View(TraineeCourseList);
                }
                TempData["GlobalMessage"] = "Failed to find trainee details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        /// <summary>
        /// Used to display specific COURSE DETAIL of a trainee filtered by Course ID
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
        /// Used to display MODULE LIST of a specific course for a particular trainee
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
                    List<Module> TraineeModuleList = traineeProfileLogic.TraineeModuleList(courseId, person.PersonId);
                    return PartialView(TraineeModuleList);
                }
                ViewBag.GlobalMessage = "Failed to find trainee details.";
                return PartialView(new List<Module>());
            }
            else
            {
                ViewBag.GlobalMessage = "Failed to find user detail from the session.";
                return PartialView(new List<Module>());
            }
        }

        /// <summary>
        /// Used to Display trainee MODULE DETAIL filtered by Module Id
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
        /// Used to Display lesson evalaution template of a specific trainee for a particular lesson
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult TraineeEvaluationTemplate(int lessonId)
        {
            TraineeEvaluationTemplateLogic traineeEvaluationTemplateLogic = new TraineeEvaluationTemplateLogic();

            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    //How to determine Schedule sequence when there more than one schedule for the same trainee and lesson Id ??
                    int sequence = 1;
                    List<TraineeEvaluationTemplateView> traineeEvaluationTemplate = traineeEvaluationTemplateLogic.EvaluationTemplateList(person.PersonId, lessonId, sequence);

                    return PartialView("TraineeEvaluationTemplate", traineeEvaluationTemplate);
                }
                ViewBag.GlobalMessage = "Failed to find trainee details.";
                return PartialView("TraineeEvaluationTemplate", new TraineeEvaluationTemplateView());
            }
            else
            {
                ViewBag.GlobalMessage = "Failed to find user detail from the session.";
                return PartialView("TraineeEvaluationTemplate", new TraineeEvaluationTemplateView());
            }
        }



        /// <summary>
        /// Used to display EXAM LIST of a specific module for a particular trainee
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
                    List<TraineeModuleExam> TraineeModuleExamList = traineeProfileLogic.TraineeModuleExamList(moduleId, person.PersonId);
                    return PartialView(TraineeModuleExamList);
                }
                ViewBag.GlobalMessage = "Failed to find trainee details.";
                return PartialView(new List<TraineeModuleExam>());
            }
            else
            {
                ViewBag.GlobalMessage = "Failed to find user detail from the session.";
                return PartialView(new List<TraineeModuleExam>());
            }
        }

        /// <summary>
        /// Used to Display trainee EXAM DETAIL filtered by Module Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult ModuleExamDetails(int traineeModuleExamId)
        {
            TraineeModuleExam traineeModuleExam = (TraineeModuleExam)traineeProfileLogic.ModuleExamDetails(traineeModuleExamId);
            return PartialView(traineeModuleExam);
        }

        /// <summary>
        /// Used to display EXAM LIST of a specific COURSE for a particular TRAINEE
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult TraineeCourseExamList(int courseId)
        {
            ViewBag.CourseId = courseId;
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    List<TraineeCourseExam> TraineeCourseExamList = traineeProfileLogic.TraineeCourseExamList(courseId, person.PersonId);
                    return PartialView(TraineeCourseExamList);
                }
                ViewBag.GlobalMessage = "Failed to find trainee details.";
                return PartialView(new List<TraineeModuleExam>());
            }
            else
            {
                ViewBag.GlobalMessage = "Failed to find user detail from the session.";
                return PartialView(new List<TraineeModuleExam>());
            }
        }

        /// <summary>
        /// Used to Display trainee MODULE DETAIL filtered by Module Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public PartialViewResult CourseExamDetails(int traineeCourseExamId)
        {
            TraineeCourseExam traineeCourseExam = traineeProfileLogic.CourseExamDetails(traineeCourseExamId);
            return PartialView(traineeCourseExam);
        }

        /// <summary>
        /// This method is used to display LICENSE for a specific trainee
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult TraineeLicenseList()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    LicenseLogic licenseLogic = new LicenseLogic();
                    var traineeLicense = licenseLogic.GetLicense(person.PersonId);
                    return View(traineeLicense);
                }
                TempData["GlobalMessage"] = "Failed to find trainee details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }
        /// <summary>
        /// Used to Display LICENSE DETAIL for a particular trainee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [PTSAuthorizeAttribute]
        public PartialViewResult TraineelicenseDetail(int licenceId)
        {
            LicenseLogic licenseLogic = new LicenseLogic();
            var traineeLicense = licenseLogic.Details(licenceId);
            return PartialView(traineeLicense);
        }

        /// <summary>
        /// This method is used to display LEAVE for a specific trainee
        /// </summary>
        /// <returns></returns>
        [PTSAuthorizeAttribute]
        public ActionResult TraineeLeaveList()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                Person person = (Person)personLogic.PersonDetail(companyId);
                if (person != null)
                {
                    PersonLeaveLogic personLeaveLogic = new PersonLeaveLogic();
                    var traineeLeave = personLeaveLogic.GetLeave(person.PersonId);
                    return View(traineeLeave);
                }
                TempData["GlobalMessage"] = "Failed to find trainee details.";
                return RedirectToAction("Welcome", "Home", new { });
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }

        public ActionResult Profile()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;

                TraineeProfileView traineeProfile = traineeProfileLogic.Profile(companyId);
                if (TempData["GlobalMessage"] != null)
                {
                    ViewBag.GlobalMessage = TempData["GlobalMessage"];
                    TempData["GlobalMessage"] = null;
                }
                return View(traineeProfile);
            }
            else
            {
                TempData["GlobalMessage"] = "Failed to find user detail from the session.";
                return RedirectToAction("Welcome", "Home", new { });
            }
        }
        [HttpPost]
        public ActionResult UpdateProfile(TraineeProfileView TraineeProfile)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string companyId = HttpContext.User.Identity.Name;
                string phoneNumber = TraineeProfile.Phone, email = TraineeProfile.Email;
                if (traineeProfileLogic.UpdateProfile(companyId, phoneNumber, email))
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