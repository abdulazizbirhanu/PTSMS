using System.Linq;
using System.Net;
using System.Web.Mvc;
using PTSMSDAL.Models.Curriculum.Operations;
using System.Collections.Generic;
using PTSMSBAL.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Web;
using System.IO;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using PTSMS.Controllers.Others;

namespace PTSMS.Controllers
{
    [SessionExpire]
    public class ModulesController : Controller
    {
        ModuleLogic moduleLogic = new ModuleLogic();
        CourseLogic courseLogic = new CourseLogic();
        public JsonResult ListModule()
        {
            List<Module> result = (List<Module>)moduleLogic.List();
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.ModuleId,
                    Id = item.RevisionGroupId == null ? item.ModuleId : item.RevisionGroupId,

                    Name = item.ModuleTitle
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListCourseModule(int CourseCategoryId)
        {
            List<Module> result = (List<Module>)moduleLogic.List(CourseCategoryId);
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    //Id = item.ModuleId,
                    Id = item.RevisionGroupId == null ? item.ModuleId : item.RevisionGroupId,
                    Name = item.ModuleCode + " - "+ item.ModuleTitle
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExam(int ModuleId)
        {
            List<ModuleExam> result = (List<ModuleExam>)moduleLogic.ListExam(ModuleId);
            return Json(new
            {
                items = result.Select(item => new
                {
                    //examId = item.ExamId,
                    examId = item.Exam.RevisionGroupId == null ? item.ExamId : item.Exam.RevisionGroupId,

                    examName = item.Exam.Name,
                })
            });
        }

        public JsonResult AddExam(string ExamId, int ModuleId)
        {
            if (!(String.IsNullOrEmpty(ExamId) || String.IsNullOrWhiteSpace(ExamId)))
            {
                string[] ExamIdArray = ExamId.Split(',');
                object result = moduleLogic.AddExam(ExamIdArray.ToList(), ModuleId);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Invalid input." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveModuleExam(int moduleExamId)
        {
            object result = moduleLogic.RemoveModuleExam(moduleExamId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        //AddGroundLesson(int moduleGroundLessonId, int courseModuleId)      

        public JsonResult AddGroundLesson(string moduleGroundLessonId, int courseModuleId)
        {
            if (!(String.IsNullOrEmpty(moduleGroundLessonId) || String.IsNullOrWhiteSpace(moduleGroundLessonId)))
            {
                string[] moduleGroundLessonIdArray = moduleGroundLessonId.Split(',');
                object result = moduleLogic.AddGroundLesson(moduleGroundLessonIdArray.ToList(), courseModuleId);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Invalid input." }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveModuleGroundLesson(int moduleGroundLessonId)
        {
            object result = moduleLogic.RemoveModuleGroundLesson(moduleGroundLessonId);
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult ModuleDetailPartialView(int courseModuleId)
        {
            Module module = moduleLogic.ModuleDetail(courseModuleId);
            return PartialView("ModuleDetailPartialView", module);
        }

        // GET: Modules
        [PTSAuthorizeAttribute]
        public ActionResult Index()
        {
            if (TempData["ModuleMessage"] != null)
            {
                ViewBag.ModuleMessage = TempData["ModuleMessage"];
                TempData["ModuleMessage"] = null;
            }
            return View(moduleLogic.ListModule());
        }

        // GET: Modules/Details/5
        [PTSAuthorizeAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = (Module)moduleLogic.Details((int)id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // GET: Modules/Create
        [PTSAuthorizeAttribute]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(((List<Course>)courseLogic.List()).Select(item => new
                {
                    CourseId = item.RevisionGroupId != null ? item.RevisionGroupId : item.CourseId,
                    CourseTitle = item.CourseCode + " - " + item.CourseTitle
                })
            , "CourseId", "CourseTitle");

            ViewBag.RevisionGroupId = new SelectList((List<Module>)moduleLogic.List(), "ModuleId", "ModuleCode");
            ViewBag.ReferenceId = new SelectList((List<Module>)moduleLogic.List(), "ReferenceId", "ReferenceName");
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> ModuleReferenceFiles, Module module)
        {
            if (ModelState.IsValid)
            {
                if ((bool)moduleLogic.Add(module, ModuleReferenceFiles))
                    return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(((List<Course>)courseLogic.List()).Select(item => new
            {
                CourseId = item.RevisionGroupId != null ? item.RevisionGroupId : item.CourseId,
                CourseTitle = item.CourseCode + " - " + item.CourseTitle
            })
            , "CourseId", "CourseTitle");
            ViewBag.RevisionGroupId = new SelectList((List<Module>)moduleLogic.List(), "ModuleId", "ModuleCode", module.RevisionGroupId);
            return View(module);
        }

        // GET: Modules/Edit/5
        [PTSAuthorizeAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = (Module)moduleLogic.Details((int)id);
            if (module == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseTitle");
            ViewBag.RevisionGroupId = new SelectList((List<Module>)moduleLogic.List(), "ModuleId", "ModuleCode", module.RevisionGroupId);
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PTSAuthorizeAttribute]
        public ActionResult Edit(IEnumerable<HttpPostedFileBase> ModuleReferenceFiles, Module module)
        {
            if (ModelState.IsValid)
            {
                if ((bool)moduleLogic.Revise(ModuleReferenceFiles, module))
                    return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList((List<Course>)courseLogic.List(), "CourseId", "CourseTitle");
            ViewBag.RevisionGroupId = new SelectList((List<Module>)moduleLogic.List(), "ModuleId", "ModuleCode", module.RevisionGroupId);
            return View(module);
        }

        // GET: Modules/Delete/5
        [PTSAuthorizeAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = (Module)moduleLogic.Details((int)id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)moduleLogic.Delete(id))
                return RedirectToAction("Index");
            else
                return RedirectToAction("Delete", id);
        }

        /*Nice*/
        public JsonResult FilteredListModule(string BatchClassId)
        {
            int batchClassId = Convert.ToInt32(BatchClassId);

            List<Module> result = (List<Module>)moduleLogic.FilteredListModule(batchClassId);
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    Id = item.ModuleId,
                    Name = item.ModuleTitle
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FilteredModuleExam(string CourseId)
        {
            int EModuleId = Convert.ToInt32(CourseId);
            List<Exam> result = (List<Exam>)moduleLogic.FilteredModuleExam(EModuleId);
            return Json(new
            {
                resultData = result.Select(item => new
                {
                    Id = item.ExamId,
                    Name = item.Name
                }),
                hasList = result.Count() > 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void FilesView(string fileName, int moduleId)
        {

            string fileType = String.Empty, filePath = string.Empty;

            Module module = (Module)moduleLogic.Details(moduleId);
            int revisionNo = module.RevisionNo;
            if (module.RevisionNo == 0)
                revisionNo = 1;
            string newModuleFileDirectoryName = module.ModuleCode;
            newModuleFileDirectoryName = newModuleFileDirectoryName.Replace(@"/", "-");
            newModuleFileDirectoryName = newModuleFileDirectoryName.Replace(@"\", "-");
            newModuleFileDirectoryName = newModuleFileDirectoryName + "_R" + revisionNo + "_L" + module.ModuleId;




            moduleLogic.FilesView(newModuleFileDirectoryName, fileName, ref filePath);
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
                    filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName), fileName);
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
                    byte[] byteArray = System.IO.File.ReadAllBytes(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName), fileName));
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

                            System.IO.File.WriteAllText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName), "frontendsample.html"), html.ToStringNewLineOnAttributes());

                            System.Web.HttpContext.Current.Response.ContentType = "text/html";
                            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "inline; filename=" + "frontendsample.html");
                            filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName), "frontendsample.html");

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
                    inputFilePath = Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName + "/" + fileName);
                    outputFilePath = Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName + "/" + fileName + guid + ".pdf");
                    FinalPath = Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName + "/" + fileName + guid + ".pdf");

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
        /*Nice*/
    }
}
