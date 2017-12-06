using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Access.Utility;
using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class LessonAccess
    {
        private PTSContext db = new PTSContext();
        static LessonReferenceAccess lessonReferenceAccess = new LessonReferenceAccess();

        public object ListExam(int lessonId)
        {
            return db.LessonEvaluationTemplates.Include(c => c.EvaluationTemplate).
                Where(p => p.LessonCategoryId == lessonId).ToList();
        }
        public object AddExam(List<string> examIdList, int lessonId)
        {
            try
            {
                string message = "";
                foreach (var examIdvar in examIdList)
                {
                    if (!(String.IsNullOrEmpty(examIdvar) || String.IsNullOrWhiteSpace(examIdvar)))
                    {
                        int examId = Convert.ToInt32(examIdvar);

                        var result = ((List<LessonEvaluationTemplate>)ListExam(lessonId)).Where(c => c.EvaluationTemplateId.Equals(examId)).ToList();
                        if (result.Count() > 0)
                        {
                            message = message + result.FirstOrDefault().EvaluationTemplate.EvaluationTemplateName + " is already exist in the selected Lesson. ";
                        }
                        else
                        {
                            LessonEvaluationTemplate lessonEvaluationTemplate = new LessonEvaluationTemplate();
                            lessonEvaluationTemplate.EvaluationTemplateId = examId;
                            lessonEvaluationTemplate.LessonCategoryId = lessonId;

                            lessonEvaluationTemplate.StartDate = DateTime.Now;
                            lessonEvaluationTemplate.EndDate = new DateTime(9999, 12, 31);
                            lessonEvaluationTemplate.CreationDate = DateTime.Now;
                            lessonEvaluationTemplate.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            lessonEvaluationTemplate.RevisionDate = DateTime.Now;
                            lessonEvaluationTemplate.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                            db.LessonEvaluationTemplates.Add(lessonEvaluationTemplate);
                            db.SaveChanges();
                        }
                    }
                }
                if (message == "")
                    message = "Lesson Evaluation Template is successfully added to the selected Lesson.";
                return new { status = true, message = message };
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object RemoveLessonEvaluationTemplate(int lessonEvaluationTemplateId)
        {
            try
            {
                LessonEvaluationTemplate lessonEvaluationTemplate = db.LessonEvaluationTemplates.FirstOrDefault(p => p.LessonEvaluationTemplateId == lessonEvaluationTemplateId);
                db.LessonEvaluationTemplates.Remove(lessonEvaluationTemplate);
                db.SaveChanges();
                return new { status = true }; // Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }

        public object List()
        {
            return db.Lessons.Include(l => l.PreviousLesson).Where(l => l.Status == "Active" && l.EndDate > DateTime.Now).ToList();
        }
        public List<Lesson> ListLessonByCategoryTypeId(int programCategoryId)
        {
            var programCategory = db.ProgramCategories.Find(programCategoryId);
            var categoryList = db.Categories.Where(c => ((c.RevisionGroupId == null && c.CategoryId == programCategory.CategoryId) ||
                       (c.RevisionGroupId != null && c.RevisionGroupId == programCategory.CategoryId))
            && c.Status == "Active").ToList();
            var category = categoryList.FirstOrDefault();
            return db.Lessons.Include(l => l.PreviousLesson).Where(l => l.Status == "Active" && l.EndDate > DateTime.Now &&
                        l.CategoryTypeId == category.CategoryTypeId && l.EquipmentType.EquipmentTypeName.ToLower() == category.CategoryName.ToLower()
                     ).ToList();
        }

        public object Details(int id)
        {
            try
            {
                Lesson lesson = db.Lessons.Find(id);
                if (lesson == null)
                {
                    return false; // Not Found
                }
                return lesson; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public Lesson DetailsLesson(int id)
        {
            try
            {
                Lesson lesson = db.Lessons.Find(id);
                if (lesson == null)
                {
                    return lesson; // Not Found
                }
                return lesson; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }
        public Lesson LessonDetail(int lessonCategoryId)
        {
            try
            {
                LessonCategory lessonCategory = db.LessonCategories.Find(lessonCategoryId);
                if (lessonCategory != null)
                {
                    var lesson = db.Lessons.Where(c => ((c.RevisionGroupId == null && c.LessonId == lessonCategory.LessonId)
                       || (c.RevisionGroupId != null && c.RevisionGroupId == lessonCategory.LessonId)) && c.Status == "Active").ToList();
                    if (lesson.Count > 0)
                        return lesson.FirstOrDefault();
                }
                return new Lesson();
            }
            catch (System.Exception e)
            {
                return new Lesson();
            }
        }


        //public object Add(Lesson lesson, IEnumerable<HttpPostedFileBase> LessonReferenceFiles, string lessonCode)
        //{
        //    try
        //    {
        //        lesson.StartDate = DateTime.Now;
        //        lesson.EndDate =  DateTime.MaxValue;
        //        lesson.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;

        //        lesson.CreationDate = DateTime.Now;
        //        lesson.EffectiveDate = DateTime.Now;
        //        lesson.RevisionDate = DateTime.Now;                

        //        db.Lessons.Add(lesson);
        //        db.SaveChanges();
        //        return true; // Success
        //    }
        //    catch (System.Exception e)
        //    {
        //        return false; // Exception
        //    }
        //}

        public bool Add(Lesson lesson, IEnumerable<HttpPostedFileBase> LessonReferenceFiles, string oldLessonName)
        {
            try
            {
                ModuleReferenceAccess moduleReferenceAccess = new ModuleReferenceAccess();

                int oldLessonId = lesson.LessonId;
                string newLessonFileDirectoryName = lesson.LessonName;

                lesson.StartDate = DateTime.Now;
                lesson.EndDate = DateTime.MaxValue;
                lesson.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;

                lesson.CreationDate = DateTime.Now;
                lesson.EffectiveDate = DateTime.Now;
                lesson.RevisionDate = DateTime.Now;

                db.Lessons.Add(lesson);
                if (db.SaveChanges() > 0)
                {
                    UtilityClass utilityClass = new UtilityClass();
                    int latestLessonId = utilityClass.GetLatestIdNumber("LESSON");

                    //string newFileName = "No File";
                    string fileType = "No File";

                    //string fileTypeCollection = String.Empty;
                    if (LessonReferenceFiles != null)
                    {
                        if ((LessonReferenceFiles.Count() > 0 && LessonReferenceFiles.First() != null))
                        {
                            if (LessonReferenceFiles.First() != null)
                            {
                                int revisionNo = lesson.RevisionNo;
                                if (lesson.RevisionNo == 0)
                                    revisionNo = 1;

                                //Replace a special character / by a character - because file name doesnot support specail character / 
                                newLessonFileDirectoryName = newLessonFileDirectoryName.Replace(@"/", "-");
                                newLessonFileDirectoryName = newLessonFileDirectoryName.Replace(@"\", "-");
                                newLessonFileDirectoryName = newLessonFileDirectoryName + "_R" + revisionNo + "_L" + latestLessonId;

                                //Delete the previous file directory
                                string targetPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName);
                                if (!Directory.Exists(targetPath))
                                {
                                    Directory.CreateDirectory(targetPath);
                                }

                                //Create Folder and save files on it
                                foreach (HttpPostedFileBase file in LessonReferenceFiles)
                                {
                                    var orginalFileName = Path.GetFileName(file.FileName);

                                    fileType = Path.GetExtension(System.IO.Path.GetFileName(file.FileName));

                                    fileType = utilityClass.GetFileType(fileType);

                                    // store the file inside ~/App_Data/uploads folder
                                    var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName), orginalFileName);
                                    file.SaveAs(path);

                                    //Save Module Reference
                                    LessonReferenceAccess lessonReferenceAccess = new LessonReferenceAccess();
                                    LessonReference lessonReference = new LessonReference();
                                    lessonReference.LessonId = latestLessonId;
                                    lessonReference.ReferenceURL = path;
                                    lessonReference.FileName = orginalFileName;
                                    lessonReference.FileType = fileType;

                                    lessonReferenceAccess.Add(lessonReference);
                                }

                                //Copy lesson reference if there is any
                                if (oldLessonId > 0)
                                {
                                    string oldLessonFileDirectoryName = "";
                                    var objModule = (Lesson)Details(oldLessonId);
                                    int oldRevisionNo = oldLessonId;
                                    if (objModule.RevisionNo > 0)
                                        oldRevisionNo = objModule.RevisionNo;
                                    oldLessonName = oldLessonName.Replace(@"/", "-");
                                    oldLessonName = oldLessonName.Replace(@"\", "-");
                                    oldLessonFileDirectoryName = oldLessonName + "_R" + oldRevisionNo + "_L" + oldLessonId;

                                    string fromPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + oldLessonFileDirectoryName);
                                    string toPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName);
                                    if (Directory.Exists(fromPath))
                                    {
                                        CopyFilesFromOldToNewDirectory(fromPath, toPath, newLessonFileDirectoryName, oldLessonId, latestLessonId);
                                    }
                                }
                            }
                        }
                    }

                    if (LessonReferenceFiles != null)
                    {
                        if ((LessonReferenceFiles.Count() == 0 || LessonReferenceFiles.First() == null))
                        {
                            if (oldLessonId > 0)
                            {
                                int revisionNo = lesson.RevisionNo;
                                if (lesson.RevisionNo == 0)
                                    revisionNo = 1;

                                //Replace a special character / by a character - because file name doesnot support specail character / 
                                newLessonFileDirectoryName = newLessonFileDirectoryName.Replace(@"/", "-");
                                newLessonFileDirectoryName = newLessonFileDirectoryName.Replace(@"\", "-");
                                newLessonFileDirectoryName = newLessonFileDirectoryName + "_R" + revisionNo + "_L" + latestLessonId;

                                string oldLessonFileDirectoryName = "";
                                var objLesson = db.Lessons.Find(oldLessonId);
                                int oldRevisionNo = oldLessonId;
                                if (objLesson.RevisionNo > 0)
                                    oldRevisionNo = objLesson.RevisionNo;
                                oldLessonName = oldLessonName.Replace(@"/", "-");
                                oldLessonName = oldLessonName.Replace(@"\", "-");
                                oldLessonFileDirectoryName = oldLessonName + "_R" + oldRevisionNo + "_L" + oldLessonId;

                                string fromPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + oldLessonFileDirectoryName);
                                string toPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + newLessonFileDirectoryName);

                                if (Directory.Exists(fromPath))
                                {
                                    if (!Directory.Exists(toPath))
                                    {
                                        Directory.CreateDirectory(toPath);
                                    }
                                    CopyFilesFromOldToNewDirectory(fromPath, toPath, newLessonFileDirectoryName, oldLessonId, latestLessonId);
                                }
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }


        public static void CopyFilesFromOldToNewDirectory(string fromDirectory, string toDirectory, string directoryName, int oldLessonId, int latestLessonId)
        {
            string[] files = Directory.GetFiles(fromDirectory);
            string[] dirs = Directory.GetDirectories(fromDirectory);

            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);
                string fileType = Path.GetExtension(filename);

                if (fileType != ".html")
                {
                    //Copy file if it doesn't exist
                    var OldFile = Path.Combine(fromDirectory, filename);
                    var NewFile = Path.Combine(toDirectory, filename);
                    if (!File.Exists(NewFile))
                    {
                        //Copy File
                        File.Copy(OldFile, NewFile);
                        //update Module reference Module ID by the new module id
                        LessonReference lessonReference = lessonReferenceAccess.GetLessonReferencesByLessonIdANdFileName(oldLessonId, filename);
                        if (lessonReference != null)
                        {
                            lessonReference.LessonId = latestLessonId;
                            lessonReference.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            lessonReference.RevisionDate = DateTime.Now;
                            lessonReference.ReferenceURL = toDirectory + filename;
                            lessonReference.FileName = filename;
                            lessonReferenceAccess.Revise(lessonReference);
                        }
                    }
                }
            }
            foreach (string dir in dirs)
            {
                //Call recursively,
                CopyFilesFromOldToNewDirectory(fromDirectory, toDirectory, directoryName, oldLessonId, latestLessonId);
            }
        }



        public object Revise(Lesson lesson)
        {
            try
            {
                lesson.RevisionDate = DateTime.Now;
                lesson.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public object Delete(int id)
        {
            try
            {
                Lesson lesson = db.Lessons.Find(id);
                lesson.EndDate = DateTime.Now;
                lesson.LessonName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }



        //Unused method
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                //Call recursively
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
        public static void RenameFile(string target_dir, string fileLessonCode, int latestLessonId, int lessonId)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                //File.SetAttributes(file, FileAttributes.Normal);
                //File.Delete(file);
                // store the file inside ~/App_Data/uploads folder
                string filename = Path.GetFileName(file);
                string fileType = Path.GetExtension(filename);
                if (fileType != ".html")
                {
                    string[] filenameArray = filename.Split('~');
                    var path = Path.Combine(target_dir, filename);

                    string newFileName = fileLessonCode + "~" + filenameArray[1];

                    //For Renaming the file                    
                    var newFilePath = Path.Combine(target_dir, newFileName);
                    System.IO.File.Move(path, newFilePath);

                    //update Module reference Module ID by the new module id
                    List<LessonReference> lessonReferenceList = lessonReferenceAccess.GetLessonReferencesByLessonId(lessonId);
                    foreach (LessonReference lessonReference in lessonReferenceList)
                    {
                        lessonReference.LessonId = latestLessonId;
                        lessonReference.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        lessonReference.RevisionDate = DateTime.Now;
                        lessonReference.ReferenceURL = newFilePath;
                        lessonReference.FileName = newFileName;
                        lessonReferenceAccess.Revise(lessonReference);
                    }
                }
            }

            foreach (string dir in dirs)
            {
                //Call recursively
                RenameFile(dir, fileLessonCode, latestLessonId, lessonId);
            }
        }
    }
}