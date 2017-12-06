using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSBAL.Curriculum.Operations;
using System.Collections.Generic;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSBAL.Curriculum.References;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSBAL.Scheduling.References;
using System.Web;
using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System.Xml.Linq;

namespace PTSMS.Controllers
{
    public class LessonsController : Controller
    {
        LessonLogic lessonLogic = new LessonLogic();

        public JsonResult ListLesson()
        {
            List<Lesson> result = (List<Lesson>)lessonLogic.List();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.LessonId,
                    Id = item.RevisionGroupId == null ? item.LessonId : item.RevisionGroupId,

                    Name = item.LessonName
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListLessonByCategoryTypeId(int programCategoryId)
        {
            CategoryLogic categoryLogic = new CategoryLogic();
            //get Lesson
            List<Lesson> result = (List<Lesson>)lessonLogic.ListLessonByCategoryTypeId(programCategoryId);
            var LessonList = result.Select(item => new
            {
                //Id = item.LessonId,
                Id = item.RevisionGroupId == null ? item.LessonId : item.RevisionGroupId,

                Name = item.LessonName
            }).ToList();
            //Get Phase
            List<Phase> phaseResult = (List<Phase>)categoryLogic.ListPhase();
            var phaseList = phaseResult.Select(item => new { Id = item.PhaseId, Name = item.Name }).ToList();
            //Get Stage

            List<Stage> stageResult = (List<Stage>)categoryLogic.ListStage();
            var stageList = stageResult.Select(item => new { Id = item.StageId, Name = item.Name }).ToList();

            return Json(new
            {
                LessonList = LessonList,
                StageList = stageList,
                PhaseList = phaseList,
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListExam(int LessonId)
        {
            List<LessonEvaluationTemplate> result = (List<LessonEvaluationTemplate>)lessonLogic.ListExam(LessonId);
            return Json(new
            {
                items = result.Select(item => new
                {
                    //evaluationTemplateId = item.EvaluationTemplateId,
                    evaluationTemplateId = item.EvaluationTemplate.RevisionGroupId == null ? item.EvaluationTemplateId : item.EvaluationTemplate.RevisionGroupId,

                    evaluationTemplateName = item.EvaluationTemplate.EvaluationTemplateName,
                })
            });
        }

        public JsonResult AddExam(string ExamId, int LessonId)
        {
            if (!(string.IsNullOrEmpty(ExamId) || string.IsNullOrWhiteSpace(ExamId)))
            {
                string[] ExamIdArray = ExamId.Split(',');
                object result = lessonLogic.AddExam(ExamIdArray.ToList(), LessonId);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Invalid input." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveLessonEvaluationTemplate(int lessonEvaluationTemplateId)
        {
            object result = lessonLogic.RemoveLessonEvaluationTemplate(lessonEvaluationTemplateId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public PartialViewResult LessonDetailPartialView(int lessonCategoryId)
        {
            Lesson lesson = lessonLogic.LessonDetail(lessonCategoryId);
            return PartialView("LessonDetailPartialView", lesson);
        }


        // GET: Lessons
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            return View(lessonLogic.List());
        }

        // GET: Lessons/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
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

        // GET: Lessons/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            CategoryTypeLogic categoryTypeLogic = new CategoryTypeLogic();
            EquipmentTypeLogic equipmentTypeLogic = new EquipmentTypeLogic();
            ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.CategoryTypeList()).Where(CT => !CT.IsTypeRating && !CT.Type.ToLower().Contains("ground")).ToList(), "CategoryTypeId", "Description");
            ViewBag.EquipmentTypeId = new SelectList((List<EquipmentType>)equipmentTypeLogic.List(), "EquipmentTypeId", "EquipmentTypeName");
            ViewBag.RevisionGroupId = new SelectList((List<Lesson>)lessonLogic.List(), "LessonId", "LessonName");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> LessonReferenceFiles, Lesson lesson)
        {
            //if (ModelState.IsValid)
            // {
            if ((bool)lessonLogic.Add(lesson, LessonReferenceFiles))
                return RedirectToAction("Index");
            else
                ViewBag.ErrorMessage = "";
            //   }
            ViewBag.ErrorMessage = "Invalid Input";
            CategoryTypeLogic categoryTypeLogic = new CategoryTypeLogic();
            EquipmentTypeLogic equipmentTypeLogic = new EquipmentTypeLogic();
            ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.CategoryTypeList()).Where(CT => !CT.IsTypeRating && !CT.Type.ToLower().Contains("ground")).ToList(), "CategoryTypeId", "Description");
            ViewBag.EquipmentTypeId = new SelectList((List<EquipmentType>)equipmentTypeLogic.List(), "EquipmentTypeId", "EquipmentTypeName");
            ViewBag.RevisionGroupId = new SelectList((List<Lesson>)lessonLogic.List(), "LessonId", "LessonName");
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = (Lesson)lessonLogic.Details((int)id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            CategoryTypeLogic categoryTypeLogic = new CategoryTypeLogic();
            EquipmentTypeLogic equipmentTypeLogic = new EquipmentTypeLogic();
            if (lesson.IsTypeRating)
                ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.CategoryTypeList()).Where(CT => CT.IsTypeRating && !CT.Type.ToLower().Contains("ground")).ToList(), "CategoryTypeId", "Description");
            else
                ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.CategoryTypeList()).Where(CT => !CT.IsTypeRating && !CT.Type.ToLower().Contains("ground")).ToList(), "CategoryTypeId", "Description");
            ViewBag.EquipmentTypeId = new SelectList((List<EquipmentType>)equipmentTypeLogic.List(), "EquipmentTypeId", "EquipmentTypeName");
            ViewBag.RevisionGroupId = new SelectList((List<Lesson>)lessonLogic.List(), "LessonId", "LessonName"); return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [PTSAuthorizeAttribute]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IEnumerable<HttpPostedFileBase> LessonReferenceFiles, Lesson lesson)
        {
            //if (ModelState.IsValid)
            /// {
            if ((bool)lessonLogic.Revise(lesson, LessonReferenceFiles))
                return RedirectToAction("Index");
            // }
            CategoryTypeLogic categoryTypeLogic = new CategoryTypeLogic();
            EquipmentTypeLogic equipmentTypeLogic = new EquipmentTypeLogic();
            if (lesson.IsTypeRating)
                ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.CategoryTypeList()).Where(CT => CT.IsTypeRating && !CT.Type.ToLower().Contains("ground")).ToList(), "CategoryTypeId", "Description");
            else
                ViewBag.CategoryTypeId = new SelectList(((List<CategoryType>)categoryTypeLogic.CategoryTypeList()).Where(CT => !CT.IsTypeRating && !CT.Type.ToLower().Contains("ground")).ToList(), "CategoryTypeId", "Description");
            ViewBag.EquipmentTypeId = new SelectList((List<EquipmentType>)equipmentTypeLogic.List(), "EquipmentTypeId", "EquipmentTypeName");
            ViewBag.RevisionGroupId = new SelectList((List<Lesson>)lessonLogic.List(), "LessonId", "LessonName");
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
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

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)lessonLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }

        [HttpGet]
        public void FilesView(string fileName, int lessonId)
        {

            string fileType = String.Empty, filePath = string.Empty;

            Lesson lesson = (Lesson)lessonLogic.Details(lessonId);
            int revisionNo = lesson.RevisionNo;
            if (lesson.RevisionNo == 0)
                revisionNo = 1;
            string newLessonFileDirectoryName = lesson.LessonName;
            newLessonFileDirectoryName = newLessonFileDirectoryName.Replace(@"/", "-");
            newLessonFileDirectoryName = newLessonFileDirectoryName.Replace(@"\", "-");
            newLessonFileDirectoryName = newLessonFileDirectoryName + "_R" + revisionNo + "_L" + lesson.LessonId;


            lessonLogic.FilesView(newLessonFileDirectoryName, fileName, ref filePath);
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(filePath);
            string strFileName = filePath;

            fileType = Path.GetExtension(filePath).ToLower();
            string contentType;

            {
                if (fileType == ".jpg" || fileType == ".png" || fileType == ".gif" || fileType == ".jpeg")
                {
                    System.Web.HttpContext.Current.Response.ContentType = fileType;
                    System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);
                    filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName), fileName);
                    System.Web.HttpContext.Current.Response.TransmitFile(filePath);
                }
                else if (fileType == ".doc" || fileType == ".docx")
                {
                    string css = @"
                    p.PtNormal
                        {margin-bottom:50.0pt;
                        margin-bottom:50.0pt;
                        font-size:11.0pt;
                        font-family:""Times"";}
                    h1.PtHeading1
                        {margin-top:24.0pt;
                        font-size:14.0pt;
                        font-family:""Helvetica"";
                        color:blue;}
                    h2.PtHeading2
                        {margin-top:10.0pt;
                        font-size:13.0pt;
                        font-family:""Helvetica"";
                        color:blue;}";
                    byte[] byteArray = System.IO.File.ReadAllBytes(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName), fileName));
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        memoryStream.Write(byteArray, 0, byteArray.Length);
                        using (WordprocessingDocument doc =
                            WordprocessingDocument.Open(memoryStream, true))
                        {
                            HtmlConverterSettings settings = new HtmlConverterSettings()
                            {
                                PageTitle = "Module File",
                                AdditionalCss = css
                            };
                            XElement html = HtmlConverter.ConvertToHtml(doc, settings);

                            //Note: the XHTML returned by ConvertToHtmlTransform contains objects of type
                            //XEntity. PtOpenXmlUtil.cs defines the XEntity class. See                         
                            //for detailed explanation.

                            //If you further transform the XML tree returned by ConvertToHtmlTransform, you
                            //must do it correctly, or entities do not serialize properly.

                            System.IO.File.WriteAllText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName), "frontendsample.html"), html.ToStringNewLineOnAttributes());

                            System.Web.HttpContext.Current.Response.ContentType = "text/html";
                            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "inline; filename=" + "frontendsample.html");
                            filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName), "frontendsample.html");

                            System.Web.HttpContext.Current.Response.TransmitFile(filePath);
                        }//end of using
                    }
                }//end of else if
                else if (fileType == ".xls" || fileType == ".xlsx")
                {
                    string guid = Guid.NewGuid().ToString();
                    string FinalPath = "";
                    fileName = Path.GetFileName(filePath);             // extract only the fielname
                    var ext = Path.GetExtension(filePath.ToLower());            //extract only the extension of filename and then convert it into lower case.


                    string inputFilePath = ""; string outputFilePath = "";
                    inputFilePath = Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName + "/" + fileName);
                    outputFilePath = Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName + "/" + fileName + guid + ".pdf");
                    FinalPath = Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName + "/" + fileName + guid + ".pdf");

                    contentType = "Application/pdf";
                    Response.ContentType = contentType;
                    Response.AppendHeader("content-disposition", "inline;filename=" + fileName);
                    Response.BinaryWrite(User.DownloadData(outputFilePath));
                    Response.End();
                }
                else
                {
                    switch (fileType)
                    {
                        case ".dwf":
                            contentType = "Application/x-dwf";
                            break;
                        case ".pdf":
                            contentType = "Application/pdf";
                            break;
                        case ".ppt":
                        case ".pptx":
                        case ".pps":
                            contentType = "Application/vnd.ms-powerpoint";
                            break;
                        case ".txt":
                            contentType = "text/plain";
                            break;
                        case ".html":
                        case ".htm":
                            contentType = "text/HTML";
                            break;
                        default:
                            contentType = "Application/octet-stream";
                            break;
                    }
                    Response.ContentType = contentType;
                    Response.AppendHeader("content-disposition", "inline;filename=" + fileName + fileType);
                    Response.BinaryWrite(FileBuffer);
                    Response.End();
                }
            }

        }

    }
}
