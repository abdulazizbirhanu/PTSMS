using PTSMS.Models;
using PTSMSBAL.Curriculum.Operations;
using PTSMSBAL.Curriculum.References;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.References;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTSMS.Controllers.Curriculum.Operations
{

    public class CurriculumController : Controller
    {
        [HttpGet]
        [PTSAuthorizeAttribute]
        public ActionResult Index(int programId, string message)
        {
            ProgramLogic programLogic = new ProgramLogic();
            ViewBag.ProgramHierarchy = programLogic.GetProgramHierarchy(programId);
            ViewBag.ProgramId = programId;
            if (!(String.IsNullOrEmpty(message) && String.IsNullOrWhiteSpace(message)))
            {
                ViewBag.Message = message;
            }
            //string cookieToken = "";
            //string formToken = "";
            //System.Web.Helpers.AntiForgery.GetTokens(null, out cookieToken, out formToken);
            //string antiForgeryToken = cookieToken + ":" + formToken; 
            return View();
        }

        [HttpGet]
        public ActionResult AddCategory(int ProgramId, int CategoryId, int batchId)
        {
            CurriculumLogic curriculumLogic = new CurriculumLogic();
            //OperationResult result = curriculumLogic.AddCategory(ProgramId, CategoryId, batchId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCourse(int programCategoryId, int courseId, int batchId)
        {
            CurriculumLogic curriculumLogic = new CurriculumLogic();
            //OperationResult result = curriculumLogic.AddCourse(programCategoryId, courseId, batchId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCourseExam(int courseCategoryId, int examId, int batchId)
        {
            CurriculumLogic curriculumLogic = new CurriculumLogic();
            //OperationResult result = curriculumLogic.AddCourseExam(courseCategoryId, examId, batchId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCourseModule(int courseCategoryId, int moduleId, int batchId)
        {
            CurriculumLogic curriculumLogic = new CurriculumLogic();
            //OperationResult result = curriculumLogic.AddCourseModule(courseCategoryId, moduleId, batchId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddModuleExam(int courseModuleId, int examId, int batchId)
        {
            CurriculumLogic curriculumLogic = new CurriculumLogic();
            //OperationResult result = curriculumLogic.AddModuleExam(courseModuleId, examId, batchId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddLesson(int programCategoryId, int lessonId, int batchId)
        {
            CurriculumLogic curriculumLogic = new CurriculumLogic();
            //OperationResult result = curriculumLogic.AddLesson(programCategoryId, lessonId, batchId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddLessonEvaluationTemplate(int lessonCategoryId, int evaluationTemplateId, int batchId)
        {
            CurriculumLogic curriculumLogic = new CurriculumLogic();
            //OperationResult result = curriculumLogic.AddLessonEvaluationTemplate(lessonCategoryId, evaluationTemplateId, batchId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public ActionResult AddLessonEvaluationCategory(int evaluationTemplateId, int evaluationTemplateId, int batchId)
        //{
        //    CurriculumLogic curriculumLogic = new CurriculumLogic();
        //    OperationResult result = curriculumLogic.AddLessonEvaluationTemplate(lessonCategoryId, evaluationTemplateId, batchId);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }
}