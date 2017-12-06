using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PTSMSBAL.Grading;
using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Grading;

namespace PTSMS.Controllers.Grading
{
    public class GradingController : Controller
    {
        /*

        // GET: Grading
        GradingLogic gradingLogic = new GradingLogic();
        BatchClassAccess batchClass = new BatchClassAccess();
        ModuleAccess moduleAccess = new ModuleAccess();
        CourseAccess CourseAccess = new CourseAccess();

        GradingFilterBO gradingFilterBO = new GradingFilterBO();
        public ActionResult Index()
        {
            ViewBag.CategoryName = new SelectList(CategoryNames);
            var BatchClass = new SelectList((List<BatchClass>)batchClass.List(), "BatchClassId", "BatchClassName");
            ViewBag.BatchClass = BatchClass;
            ViewBag.SelectedList = new SelectList(new List<string> { });//
            ViewBag.SelectedExam = new SelectList(new List<string> { });

            gradingFilterBO.GradingFilterList = new List<GradingFilterList>() { };

            if (TempData["GradeMessage"] != null)
            {
                ViewBag.GradeMessage = TempData["GradeMessage"];
                TempData["GradeMessage"] = null;
            }
            return View(gradingFilterBO);
        }

        public ActionResult ViewGrade()
        {
            ViewBag.CategoryName = new SelectList(CategoryNames);
            ViewBag.BatchClass = new SelectList((List<BatchClass>)batchClass.List(), "BatchClassId", "BatchClassName");
            ViewBag.SelectedList = new SelectList(new List<string> { });

            gradingFilterBO.GradingFilterList = new List<GradingFilterList>() { };

            if (TempData["GradMessage"] != null)
            {
                ViewBag.GradeMessage = TempData["GradMessage"];
                TempData["GradMessage"] = null;
            }
            return View(gradingFilterBO);
        }

        [HttpPost]
        public ActionResult FindTrainne(GradingFilterBO gradingFilterBO)
        {
            ViewBag.CategoryName = new SelectList(CategoryNames);
            ViewBag.BatchClass = new SelectList((List<BatchClass>)batchClass.List(), "BatchClassId", "BatchClassName");
            if (gradingFilterBO.CategoryName == "Course")
            {

                ViewBag.SelectedList = new SelectList((List<Course>)CourseAccess.FilteredListCourse(gradingFilterBO.BatchClassId), "CourseId", "CourseTitle", gradingFilterBO.SelectedListID);
                ViewBag.SelectedExam = new SelectList((List<Exam>)CourseAccess.FilteredCourceExam(gradingFilterBO.SelectedListID), "ExamId", "Name", gradingFilterBO.SelectedExam);
            }
            else
            {
                ViewBag.SelectedList = new SelectList((List<Module>)moduleAccess.FilteredListModule(gradingFilterBO.BatchClassId), "ModuleId", "ModuleTitle", gradingFilterBO.SelectedListID);
                ViewBag.SelectedExam = new SelectList((List<Exam>)moduleAccess.FilteredModuleExam(gradingFilterBO.SelectedListID), "ExamId", "Name", gradingFilterBO.SelectedListID);
            }
            // ViewBag.SelectedList = new SelectList(new List<string> { });

            if (gradingFilterBO.CategoryName == "Course")
            {
                var result = gradingLogic.FindTrainneForCourse(gradingFilterBO);
                gradingFilterBO.GradingFilterList = (List<GradingFilterList>)result;
                gradingFilterBO.IsCourse = true;

                return View("Index", gradingFilterBO);
            }
            if (gradingFilterBO.CategoryName == "Module")
            {
                var result = gradingLogic.FindTrainneForModule(gradingFilterBO);
                gradingFilterBO.GradingFilterList = (List<GradingFilterList>)result;
                gradingFilterBO.IsModule = true;

                return View("Index", gradingFilterBO);
            }
            return null;
        }
        public ActionResult Find(GradingFilterBO gradingFilterBO, FormCollection formCollection)
        {
            ViewBag.CategoryName = new SelectList(CategoryNames);
            ViewBag.BatchClass = new SelectList((List<BatchClass>)batchClass.List(), "BatchClassId", "BatchClassName");
            if (gradingFilterBO.CategoryName == "Course")
            {
                ViewBag.SelectedList = new SelectList((List<Course>)CourseAccess.FilteredListCourse(gradingFilterBO.BatchClassId), "CourseId", "CourseTitle", gradingFilterBO.SelectedListID);
            }

            if (gradingFilterBO.CategoryName == "Course")
            {
                var result = gradingLogic.FindTrainneCourse(gradingFilterBO);
                gradingFilterBO.GradingFilterList = (List<GradingFilterList>)result;
                gradingFilterBO.IsCourse = true;

                return View("ViewGrade", gradingFilterBO);
            }

            return null;
        }
        public ActionResult Save()
        {
            bool gradingModeAll = Request.Form["GradingMode"] == "All";
            if (gradingModeAll)
            {
                foreach (string input in Request.Form.Keys)
                {
                    if (input != "GradingMode")
                    {
                        string[] val = input.Replace("GradeValue", "").Split('~');
                        string traineeCourseId = val[0],
                            traineeModuleId = val[1],
                            examId = val[2],
                            TraineeID = val[3];
                        float gradeValue = float.Parse(Request.Form[input]);
                        if (traineeCourseId != "0")
                        {
                            if (gradingLogic.SaveCourseExam(int.Parse(traineeCourseId), int.Parse(examId), gradeValue, 0))
                            {
                                //if (gradingLogic.CalculateCourseScore(TraineeID,traineeCourseId))
                                TempData["GradeMessage"] = "Trainee score successfully inserted.";
                                // else
                                //      TempData["GradeMessage"] = "Aggrigate trainee score is not assigned into course.";
                            }
                            else
                                TempData["GradeMessage"] = "Trainee Course score is not inserted.";
                        }
                        if (traineeModuleId != "0")
                        {
                            if (gradingLogic.SaveModuleExam(int.Parse(traineeModuleId), int.Parse(examId), gradeValue, 0))
                            {
                                //  if (gradingLogic.CalculateModuleScore(TraineeID))
                                TempData["GradeMessage"] = "Trainee score successfully inserted.";
                                //  else
                                //     TempData["GradeMessage"] = "Aggrigate trainee score is not assigned into course.";
                            }
                            else
                                TempData["GradeMessage"] = "Trainee module score is not inserted.";
                        }
                    }
                }
            }
            else
            {
                string[] val = Request.Form["GradingMode"].Split('~');
                string traineeCourseId = val[0],
                    traineeModuleId = val[1],
                    examId = val[2],
                    TraineeID = val[3]; ;
                string key = "GradeValue" + Request.Form["GradingMode"];
                float gradeValue = float.Parse(Request.Form[key]);

                if (traineeCourseId != "0")
                {
                    if (gradingLogic.SaveCourseExam(int.Parse(traineeCourseId), int.Parse(examId), gradeValue, 0))
                    {
                        //  if (gradingLogic.CalculateCourseScore(TraineeID, traineeCourseId))
                        TempData["GradeMessage"] = "Trainee score successfully inserted.";
                        //   else
                        //     TempData["GradeMessage"] = "Aggrigate trainee score is not assigned into course.";
                    }
                    else
                        TempData["GradeMessage"] = "Trainee Course score is not inserted.";
                }
                if (traineeModuleId != "0")
                {
                    if (gradingLogic.SaveModuleExam(int.Parse(traineeModuleId), int.Parse(examId), gradeValue, 0))
                    {
                        //  if (gradingLogic.CalculateModuleScore(TraineeID))
                        TempData["GradeMessage"] = "Trainee score successfully inserted.";
                        // else
                        //    TempData["GradeMessage"] = "Aggrigate trainee score is not assigned into course.";
                    }
                    else
                        TempData["GradeMessage"] = "Trainee module score is not inserted.";
                }


            }

            return RedirectToAction("Index");
        }

        */
        // public ActionResult SaveCource()
        // {
        //     bool gradingModeAll = Request.Form["Grading"] == "All";
        //     if (gradingModeAll)
        //     {
        //         foreach (string input in Request.Form.Keys)
        //         {
        //             if (!input.Contains("TotalValue"))
        //                 continue;

        //             if (input != "Grading")
        //             {

        //                 string[] val = input.Replace("TotalValue", "").Split('~');
        //                 // (item.CourseId + "~" + item.TraineeCourseId + "~" + item.TraineeCategoryId)
        //                 int CourseId = int.Parse(val[0]),
        //                     TraineeCourseId = int.Parse(val[1]),
        //                     TraineeCategoryId = int.Parse(val[2]);

        //                 string record = input.Replace("TotalValue", "");

        //                 string key1 = "ModuleValue" + record, key2 = "CourceValue" + record;

        //                 var ModuleValue = Request.Form[key1];
        //                 var CourceValue = Request.Form[key2];
        //                 //  var TotalValue = Request.Form[input];

        //                 if (gradingLogic.SaveCource(CourseId, TraineeCourseId, TraineeCategoryId, float.Parse(ModuleValue), float.Parse(CourceValue)/*, float.Parse(TotalValue)*/))
        //                 {
        //                     TempData["GradMessage"] = "Trainee score successfully inserted.";
        //                     // else
        //                     //   TempData["GradeMessage"] = "Aggrigate trainee score is not assigned into course.";
        //                 }
        //                 else
        //                     TempData["GradMessage"] = "Trainee score score is not inserted.";

        //             }
        //         }
        //     }
        //     else
        //     {
        //         string[] val = Request.Form["Grading"].Split('~');
        //         string key = Request.Form["Grading"];


        //         int CourseId = int.Parse(val[0]),
        //                    TraineeCourseId = int.Parse(val[1]),
        //                    TraineeCategoryId = int.Parse(val[2]);


        //         var ModuleValue = Request.Form["ModuleValue" + key];
        //         var CourceValue = Request.Form["CourceValue" + key];
        //         // var TotalValue = Request.Form["TotalValue" + key];


        //         if (gradingLogic.SaveCource(CourseId, TraineeCourseId, TraineeCategoryId, float.Parse(ModuleValue), float.Parse(CourceValue)/*, float.Parse(TotalValue)*/))
        //         {
        //             TempData["GradMessage"] = "Trainee score successfully inserted.";
        //             // else
        //             //   TempData["GradeMessage"] = "Aggrigate trainee score is not assigned into course.";
        //         }
        //         else
        //             TempData["GradMessage"] = "Trainee score score is not inserted.";
        //     }

        //     return RedirectToAction("ViewGrade");
        // }

        // // SET OF FUNCTIONS AND GLOBAL LISTS
        // List<string> CategoryNames = new List<string>
        // {
        //     "Course",
        //     "Module",

        // };

        // public JsonResult CourseExamList(int courseid)
        // {
        //     List<CourseExam> examList = gradingLogic.CourseExamList(courseid);

        //     return Json(new
        //     {
        //         examList = examList,
        //         hasList = examList.Count > 0
        //     }, JsonRequestBehavior.AllowGet);
        // }

        // public JsonResult ModuleExamList(int moduleid)
        // {
        //     List<ModuleExam> examList = gradingLogic.ModuleExamList(moduleid);

        //     return Json(new
        //     {
        //         examList = examList,
        //         hasList = examList.Count > 0
        //     }, JsonRequestBehavior.AllowGet);
        // }

        // public JsonResult ModuleStudentsList(int examID, int batchClassID, int moduleID)
        // {
        //     List<GradingFilterList> studentList = gradingLogic.ModuleStudentsList(examID, batchClassID, moduleID);

        //     ViewBag.StudentList = studentList;
        //     return Json(new
        //     {
        //         studentList = studentList,
        //         hasList = studentList.Count > 0
        //     }, JsonRequestBehavior.AllowGet);
        //     //return PartialView("_StudentGradeList", studentList);
        // }

        // public JsonResult CourseStudentsList(int examID, int batchClassID, int courseID)
        // {
        //     List<GradingFilterList> studentList = gradingLogic.CourseStudentsList(examID, batchClassID, courseID);
        //     ViewBag.StudentList = studentList;
        //     return Json(new
        //     {
        //         studentList = studentList,
        //         hasList = studentList.Count > 0
        //     }, JsonRequestBehavior.AllowGet);
        // }

        // [HttpPost]
        // public ActionResult ModuleStudentsListPartialView(int examID, int batchClassID, int moduleID)
        // {
        //     List<GradingFilterList> studentList = gradingLogic.ModuleStudentsList(examID, batchClassID, moduleID);

        //     //List<SelectListItem> selectListItem = gradingLogic.PassFailExamResult();
        //     //ViewBag.PassFailExamResult = selectListItem;
        //     //ViewBag.StudentList = studentList;
        //     return PartialView("_StudentGradeList", studentList);
        // }

        // [HttpPost]
        // public ActionResult CourseStudentsListPartialView(int examID, int batchClassID, int courseID)
        // {
        //     List<GradingFilterList> studentList = gradingLogic.CourseStudentsList(examID, batchClassID, courseID);
        //     ViewBag.StudentList = studentList;
        //     return PartialView("_StudentGradeList", studentList);
        // }

        // [HttpPost]
        // public ActionResult ReModuleStudentsListPartialView(int examID, int batchClassID, int moduleID)
        // {
        //     List<GradingFilterList> studentList = gradingLogic.ReModuleStudentsList(examID, batchClassID, moduleID);

        //     ViewBag.StudentList = studentList;
        //     return PartialView("_ReStudentGradeList", studentList);
        // }

        // [HttpPost]
        // public ActionResult ReCourseStudentsListPartialView(int examID, int batchClassID, int courseID)
        // {
        //     List<GradingFilterList> studentList = gradingLogic.ReCourseStudentsList(examID, batchClassID, courseID);


        //     ViewBag.StudentList = studentList;
        //     return PartialView("_ReStudentGradeList", studentList);
        // }


        // [HttpPost]
        // public JsonResult SaveStudentResult(int traineeCourseId, int traineeModuleId, int traineeId, int examId, float? grade, int? passFailExamResult, bool IsPassFailExam)
        // {
        //     string message = "";
        //     bool isSuccessfull = false;

        //     if ((grade >= 0 && grade <= 100) || IsPassFailExam)
        //     {
        //         if (traineeCourseId != 0)
        //         {
        //             isSuccessfull = gradingLogic.SaveCourseExam(traineeCourseId, examId, grade, passFailExamResult);
        //             if (isSuccessfull)
        //             {
        //                 message = "Trainee score successfully inserted.";
        //             }
        //             else
        //                 message = "Trainee Course score is not inserted.";
        //         }
        //         if (traineeModuleId != 0)
        //         {
        //             isSuccessfull = gradingLogic.SaveModuleExam(traineeModuleId, examId, grade, passFailExamResult);
        //             if (isSuccessfull)
        //             {
        //                 message = "Trainee score successfully inserted.";
        //             }
        //             else
        //                 message = "Trainee module score is not inserted.";
        //         }
        //     }
        //     else
        //         message = "Grade result is out of range";


        //     return Json(new
        //     {
        //         message = message,
        //         isSuccessfull = isSuccessfull
        //     }, JsonRequestBehavior.AllowGet);
        // }
        // [HttpPost]
        // public JsonResult SaveAllStudentResult(string traineeIdList)
        // {
        //     bool isSuccessfull = false;

        //     string[] traineeIdListArray = traineeIdList.Split('~');


        //     List<resultSet> resultMessage = new List<resultSet>();
        //     foreach (string gradingFilterList in traineeIdListArray)
        //     {
        //         string[] traineeIdArray = gradingFilterList.Split('-');
        //         if ((Convert.ToInt32(traineeIdArray[5]) >= 0 && Convert.ToInt32(traineeIdArray[5]) <= 100) || Convert.ToBoolean(traineeIdArray[4]))
        //         {
        //             if (Convert.ToInt32(traineeIdArray[0]) != 0)
        //             {
        //                 isSuccessfull = gradingLogic.SaveCourseExam(Convert.ToInt32(traineeIdArray[0]), Convert.ToInt32(traineeIdArray[3]), Convert.ToInt32(traineeIdArray[5]), Convert.ToInt32(traineeIdArray[6]));
        //                 if (isSuccessfull)
        //                 {
        //                     resultSet tempMessage = new resultSet();
        //                     tempMessage.resultType = "Successfull score inserted IDs :- ";
        //                     tempMessage.resultValue = traineeIdArray[2];

        //                     resultSet currentMessage = resultMessage.Where(r => r.resultType == tempMessage.resultType).FirstOrDefault();
        //                     if (currentMessage != null)
        //                         currentMessage.resultValue = currentMessage.resultValue + "," + traineeIdArray[2];
        //                     else
        //                         resultMessage.Add(tempMessage);
        //                 }
        //                 else
        //                 {
        //                     resultSet tempMessage = new resultSet();
        //                     tempMessage.resultType = "Failed IDs :- ";
        //                     tempMessage.resultValue = traineeIdArray[2];
        //                     resultSet currentMessage = resultMessage.Where(r => r.resultType == tempMessage.resultType).FirstOrDefault();
        //                     if (currentMessage != null)
        //                         currentMessage.resultValue = currentMessage.resultValue + "," + traineeIdArray[2];
        //                     else
        //                         resultMessage.Add(tempMessage);
        //                 }
        //             }
        //             if (Convert.ToInt32(traineeIdArray[1]) != 0)
        //             {
        //                 isSuccessfull = gradingLogic.SaveModuleExam(Convert.ToInt32(traineeIdArray[1]), Convert.ToInt32(traineeIdArray[3]), Convert.ToInt32(traineeIdArray[5]), Convert.ToInt32(traineeIdArray[6]));
        //                 if (isSuccessfull)
        //                 {
        //                     resultSet tempMessage = new resultSet();
        //                     tempMessage.resultType = "Score successfully inserted to IDs :- ";
        //                     tempMessage.resultValue = traineeIdArray[2];
        //                     resultSet currentMessage = resultMessage.Where(r => r.resultType == tempMessage.resultType).FirstOrDefault();
        //                     if (currentMessage != null)
        //                         currentMessage.resultValue = currentMessage.resultValue + "," + traineeIdArray[2];
        //                     else
        //                         resultMessage.Add(tempMessage);
        //                 }
        //                 else
        //                 {
        //                     resultSet tempMessage = new resultSet();
        //                     tempMessage.resultType = "Failed IDs :- ";
        //                     tempMessage.resultValue = traineeIdArray[2];
        //                     resultSet currentMessage = resultMessage.Where(r => r.resultType == tempMessage.resultType).FirstOrDefault();
        //                     if (currentMessage != null)
        //                         currentMessage.resultValue = currentMessage.resultValue + "," + traineeIdArray[2];
        //                     else
        //                         resultMessage.Add(tempMessage);
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             resultSet tempMessage = new resultSet();
        //             tempMessage.resultType = "error";
        //             tempMessage.resultValue = "Grade result is out of range";
        //         }

        //     }
        //     return Json(new
        //     {
        //         message = resultMessage,
        //         isSuccessfull = isSuccessfull
        //     }, JsonRequestBehavior.AllowGet);
        // }
        // [HttpPost]
        // public JsonResult ReSaveStudentResult(int traineeCourseId, int traineeModuleId, int traineeId, int examId, float? grade, int ReExamCount, int? RePassFailExamResult, bool IsPassFailExam)
        // {
        //     string message = "";
        //     bool isSuccessfull = false;
        //     if ((grade >= 0 && grade <= 100) || IsPassFailExam)
        //     {
        //         if (traineeCourseId != 0)
        //         {
        //             isSuccessfull = gradingLogic.ReSaveCourseExam(traineeCourseId, examId, grade, ReExamCount, RePassFailExamResult);
        //             if (isSuccessfull)
        //             {
        //                 message = "Trainee score successfully inserted.";
        //             }
        //             else
        //                 message = "Trainee Course score is not inserted.";
        //         }
        //         if (traineeModuleId != 0)
        //         {
        //             isSuccessfull = gradingLogic.ReSaveModuleExam(traineeModuleId, examId, grade, ReExamCount, RePassFailExamResult);
        //             if (isSuccessfull)
        //             {
        //                 message = "Trainee score successfully inserted.";
        //             }
        //             else
        //                 message = "Trainee module score is not inserted.";
        //         }
        //     }
        //     else
        //         message = "Grade result is out of range";


        //     return Json(new
        //     {
        //         message = message,
        //         isSuccessfull = isSuccessfull
        //     }, JsonRequestBehavior.AllowGet);
        // }

        //*/

    }
}