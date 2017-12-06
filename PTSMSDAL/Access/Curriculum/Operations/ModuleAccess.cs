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
    public class ModuleAccess
    {
        static private ModuleReferenceAccess moduleReferenceAccess = new ModuleReferenceAccess();

        private PTSContext db = new PTSContext();

        public object ListExam(int moduleId)
        {
            return db.ModuleExams.Include(c => c.Exam).
                Where(p => p.CourseModuleId == moduleId).ToList();
        }
        public object AddExam(List<string> examIdList, int moduleId)
        {
            try
            {
                string message = "";
                foreach (var examIdvar in examIdList)
                {
                    if (!(String.IsNullOrEmpty(examIdvar) || String.IsNullOrWhiteSpace(examIdvar)))
                    {
                        int examId = Convert.ToInt32(examIdvar);

                        var result = ((List<ModuleExam>)ListExam(moduleId)).Where(c => c.ExamId.Equals(examId)).ToList();
                        if (result.Count() > 0)
                        {
                            message = message + result.FirstOrDefault().Exam.Name + " is already exist in the selected Module. ";
                        }
                        else
                        {
                            ModuleExam moduleExam = new ModuleExam();
                            moduleExam.ExamId = examId;
                            moduleExam.CourseModuleId = moduleId;

                            moduleExam.StartDate = DateTime.Now;
                            moduleExam.EndDate = new DateTime(9999, 12, 31);
                            moduleExam.CreationDate = DateTime.Now;
                            moduleExam.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            moduleExam.RevisionDate = DateTime.Now;
                            moduleExam.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                            db.ModuleExams.Add(moduleExam);
                            db.SaveChanges();
                        }
                    }
                }
                if (message == "")
                    message = "Exam is successfully added to the selected module.";
                return new { status = true, message = message };
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }

        public object RemoveModuleExam(int moduleExamId)
        {
            try
            {
                ModuleExam moduleExam = db.ModuleExams.FirstOrDefault(p => p.ModuleExamId == moduleExamId);
                db.ModuleExams.Remove(moduleExam);
                db.SaveChanges();
                return new { status = true };// Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }

        public object RemoveModuleGroundLesson(int moduleGroundLessonId)
        {
            try
            {
                ModuleGroundLesson moduleGroundLesson = db.ModuleGroundLessons.FirstOrDefault(g => g.ModuleGroundLessonId == moduleGroundLessonId);
                db.ModuleGroundLessons.Remove(moduleGroundLesson);
                db.SaveChanges();
                return new { status = true };// Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        //
        public object ListGroundLesson(int courseModuleId)
        {
            return db.ModuleGroundLessons.Where(p => p.CourseModuleId == courseModuleId).ToList();
        }
        public object AddGroundLesson(List<string> moduleGroundLessonIdList, int courseModuleId)
        {
            try
            {
                string message = "";
                foreach (var moduleGroundLessonIdvar in moduleGroundLessonIdList)
                {
                    if (!(String.IsNullOrEmpty(moduleGroundLessonIdvar) || String.IsNullOrWhiteSpace(moduleGroundLessonIdvar)))
                    {
                        int moduleGroundLessonId = Convert.ToInt32(moduleGroundLessonIdvar);

                        var result = ((List<ModuleGroundLesson>)ListGroundLesson(courseModuleId)).Where(c => c.ModuleGroundLessonId.Equals(moduleGroundLessonId)).ToList();
                        if (result.Count() > 0)
                        {
                            message = message + result.FirstOrDefault().GroundLesson.Description + " is already exist in the selected Module. ";
                        }
                        else
                        {
                            ModuleGroundLesson moduleGroundLesson = new ModuleGroundLesson();
                            moduleGroundLesson.GroundLessonId = moduleGroundLessonId;
                            moduleGroundLesson.CourseModuleId = courseModuleId;

                            moduleGroundLesson.StartDate = DateTime.Now;
                            moduleGroundLesson.EndDate = new DateTime(9999, 12, 31);
                            moduleGroundLesson.CreationDate = DateTime.Now;
                            moduleGroundLesson.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            moduleGroundLesson.RevisionDate = DateTime.Now;
                            moduleGroundLesson.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                            db.ModuleGroundLessons.Add(moduleGroundLesson);
                            db.SaveChanges();
                        }
                    }
                }
                if (message == "")
                    message = "Ground Lesson is successfully added to the selected module.";
                return new { status = true, message = message };
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object RemoveGroundLesson(int groundLessonId, int courseModuleId)
        {
            try
            {
                ModuleGroundLesson moduleGroundLesson = db.ModuleGroundLessons.
                    FirstOrDefault(p => p.ModuleGroundLessonId == groundLessonId && p.CourseModuleId == courseModuleId);
                db.ModuleGroundLessons.Remove(moduleGroundLesson);
                db.SaveChanges();
                return new { status = true };// Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }

        //
        public object List()
        {
            return db.Modules.Include(m => m.PreviousModule).Where(m => m.Status == "Active" && m.EndDate > DateTime.Now).ToList();
        }

        public object List(int courseCategoryId)
        {
            var courseCategory = db.CourseCategories.Find(courseCategoryId);
            if (courseCategory != null)
            {
                int revisionGroupId = courseCategory.CourseId;
                return db.Modules.Include(m => m.PreviousModule).Where(m => m.Status == "Active" && m.EndDate > DateTime.Now && ((m.Course.RevisionGroupId != null && m.Course.RevisionGroupId == revisionGroupId) || (m.Course.RevisionGroupId == null && m.CourseId == revisionGroupId))).ToList();
            }
            return new List<Module>();
        }

        public object Details(int id)
        {
            try
            {
                Module module = db.Modules.Find(id);
                
                if (module == null)
                {
                    return false; // Not Found
                }
                return module; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public Module ModuleDetail(int courseModuleId)
        {
            try
            {
                CourseModule courseModule = db.CourseModules.Find(courseModuleId);
                if (courseModule != null)
                {
                    var module = db.Modules.Where(c => ((c.RevisionGroupId == null && c.ModuleId == courseModule.ModuleId)
                       || (c.RevisionGroupId != null && c.RevisionGroupId == courseModule.ModuleId)) && c.Status == "Active").ToList();
                    if (module.Count > 0)
                        return module.FirstOrDefault();
                }
                return new Module();
            }
            catch (Exception ex)
            {
                return new Module();
            }
        }



        public bool Add(Module module, IEnumerable<HttpPostedFileBase> ModuleReferenceFiles, string oldModuleCode)
        {
            try
            {
                ModuleReferenceAccess moduleReferenceAccess = new ModuleReferenceAccess();

                int oldModuleId = module.ModuleId;
                string newModuleFileDirectoryName = module.ModuleCode;

                module.StartDate = DateTime.Now;
                module.EndDate = Constants.EndDate;
                module.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                module.CreationDate = DateTime.Now;

                db.Modules.Add(module);
                if (db.SaveChanges() > 0)
                {
                    UtilityClass utilityClass = new UtilityClass();
                    int latestModuleId = utilityClass.GetLatestIdNumber("MODULE");

                    //string newFileName = "No File";
                    string fileType = "No File";
                    //bool isModuleReferenceFileNull)
                    //string fileTypeCollection = String.Empty;

                    if (ModuleReferenceFiles != null)
                    {
                        if ((ModuleReferenceFiles.Count() == 0))
                        {
                            //newFileName = "No File";
                            fileType = "No File";
                        }
                        else
                        {
                            if (ModuleReferenceFiles.First() != null)
                            {

                                int revisionNo = module.RevisionNo;
                                if (module.RevisionNo == 0)
                                    revisionNo = 1;

                                //Replace a special character / by a character - because file name doesnot support specail character / 
                                newModuleFileDirectoryName = newModuleFileDirectoryName.Replace(@"/", "-");
                                newModuleFileDirectoryName = newModuleFileDirectoryName.Replace(@"\", "-");
                                newModuleFileDirectoryName = newModuleFileDirectoryName + "_R" + revisionNo + "_L" + latestModuleId;
                                //Delete the previous file directory

                                string targetPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName);
                                if (!Directory.Exists(targetPath))
                                {
                                    //Create Folder and save files on it
                                    Directory.CreateDirectory(targetPath);
                                }

                                foreach (HttpPostedFileBase file in ModuleReferenceFiles)
                                {
                                    var orginalFileName = Path.GetFileName(file.FileName);

                                    fileType = Path.GetExtension(System.IO.Path.GetFileName(file.FileName));
                                    //Get file type
                                    fileType = utilityClass.GetFileType(fileType);

                                    // store the file inside ~/App_Data/uploads folder
                                    var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName), orginalFileName);
                                    file.SaveAs(path);

                                    //Save Module Reference
                                    ModuleReference moduleReference = new ModuleReference();
                                    moduleReference.ModuleId = latestModuleId;
                                    moduleReference.ReferenceURL = path;
                                    moduleReference.FileName = orginalFileName;
                                    moduleReference.FileType = fileType;

                                    moduleReferenceAccess.Add(moduleReference);
                                }

                                //Copy Module reference if there is any that doesn't belongs to the new one
                                if (oldModuleId > 0)
                                {
                                    string oldModuleFileDirectoryName = "";
                                    var objModule = db.Modules.Find(oldModuleId);
                                    int oldRevisionNo = oldModuleId;
                                    if (objModule.RevisionNo > 0)
                                        oldRevisionNo = objModule.RevisionNo;
                                    oldModuleCode = oldModuleCode.Replace(@"/", "-");
                                    oldModuleCode = oldModuleCode.Replace(@"\", "-");
                                    oldModuleFileDirectoryName = oldModuleCode + "_R" + oldRevisionNo + "_L" + oldModuleId;

                                    string fromPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + oldModuleFileDirectoryName);
                                    string toPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName);
                                    if (Directory.Exists(fromPath))
                                    {
                                        CopyFilesFromOldToNewDirectory(fromPath, toPath, newModuleFileDirectoryName, oldModuleId, latestModuleId);
                                    }
                                }
                            }
                        }
                    }
                    //Copy Module reference from the old into the new revised one.
                    if (ModuleReferenceFiles != null)
                    {
                        if ((ModuleReferenceFiles.Count() == 0 || ModuleReferenceFiles.First() == null))
                        {
                            if (oldModuleId > 0)
                            {
                                int revisionNo = module.RevisionNo;
                                if (module.RevisionNo == 0)
                                    revisionNo = 1;

                                //Replace a special character / by a character - because file name doesnot support specail character / 
                                newModuleFileDirectoryName = newModuleFileDirectoryName.Replace(@"/", "-");
                                newModuleFileDirectoryName = newModuleFileDirectoryName.Replace(@"\", "-");
                                newModuleFileDirectoryName = newModuleFileDirectoryName + "_R" + revisionNo + "_L" + latestModuleId;

                                string oldModuleFileDirectoryName = "";
                                var objModule = db.Modules.Find(oldModuleId);
                                int oldRevisionNo = oldModuleId;
                                if (objModule.RevisionNo > 0)
                                    oldRevisionNo = objModule.RevisionNo;
                                oldModuleCode = oldModuleCode.Replace(@"/", "-");
                                oldModuleCode = oldModuleCode.Replace(@"\", "-");
                                oldModuleFileDirectoryName = oldModuleCode + "_R" + oldRevisionNo + "_L" + oldModuleId;

                                string fromPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + oldModuleFileDirectoryName);
                                string toPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/ModuleReference/" + newModuleFileDirectoryName);

                                if (Directory.Exists(fromPath))
                                {
                                    if (!Directory.Exists(toPath))
                                    {
                                        Directory.CreateDirectory(toPath);
                                    }
                                    CopyFilesFromOldToNewDirectory(fromPath, toPath, newModuleFileDirectoryName, oldModuleId, latestModuleId);
                                }
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false; // Exception
            }
        }

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

        public static void CopyFilesFromOldToNewDirectory(string fromDirectory, string toDirectory, string directoryName, int oldModuleId, int latestMduleId)
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
                        ModuleReference moduleReference = moduleReferenceAccess.GetModuleReferencesByMuduleIdANdFileName(oldModuleId, filename);
                        if (moduleReference != null)
                        {
                            moduleReference.ModuleId = latestMduleId;
                            moduleReference.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            moduleReference.RevisionDate = DateTime.Now;
                            moduleReference.ReferenceURL = toDirectory + filename;
                            moduleReference.FileName = filename;
                            moduleReferenceAccess.Revise(moduleReference);
                        }
                    }
                }
            }
            foreach (string dir in dirs)
            {
                //Call recursively,
                CopyFilesFromOldToNewDirectory(fromDirectory, toDirectory, directoryName, oldModuleId, latestMduleId);
            }
        }

        public static void RenameFile(string target_dir, string fileModuleCode, int latestModuleId, int moduleId)
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

                    string newFileName = fileModuleCode + "~" + filenameArray[1];

                    //For Renaming the file                    
                    var newFilePath = Path.Combine(target_dir, newFileName);
                    System.IO.File.Move(path, newFilePath);

                    //update Module reference Module ID by the new module id
                    List<ModuleReference> moduleReferenceList = moduleReferenceAccess.GetModuleReferencesByMuduleId(moduleId);
                    foreach (ModuleReference moduleReference in moduleReferenceList)
                    {
                        moduleReference.ModuleId = latestModuleId;
                        moduleReference.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        moduleReference.RevisionDate = DateTime.Now;
                        moduleReference.ReferenceURL = newFilePath;
                        moduleReference.FileName = newFileName;
                        moduleReferenceAccess.Revise(moduleReference);
                    }
                }
            }

            foreach (string dir in dirs)
            {
                //Call recursively
                RenameFile(dir, fileModuleCode, latestModuleId, moduleId);
            }
        }

        public object Revise(Module module)
        {
            try
            {
                module.RevisionDate = DateTime.Now;
                module.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(module).State = EntityState.Modified;
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
                Module module = db.Modules.Find(id);
                module.EndDate = DateTime.Now;
                module.ModuleCode += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(module).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }



        /*Nice*/
        public object FilteredModuleExam(int eCourseId)
        {

            /* var query =
             from cc in db.CourseCategories
             join rc in db.CourseExams on cc.CourseCategoryId equals rc.CourseCategoryId
             where cc.CourseId == courceId
             join cexa in db.Exams on rc.ExamId equals (cexa.RevisionGroupId == null ? cexa.ExamId : cexa.RevisionGroupId)
             where (cexa.Status.Equals("Active"))*/

            var query =
        from rc in db.Courses
        join c in db.CourseCategories on rc.CourseId equals c.CourseId
        join r in db.CourseModules on c.CourseCategoryId equals r.CourseCategoryId
        where r.ModuleId == eCourseId
        join mo in db.Modules on r.ModuleId equals mo.ModuleId
        join cmod in db.ModuleExams on r.CourseModuleId equals cmod.CourseModuleId
        join cexa in db.Exams on cmod.ExamId equals (cexa.RevisionGroupId == null ? cexa.ExamId : cexa.RevisionGroupId)
        where (cexa.Status.Equals("Active"))
        select new
        {
            cexa.Name,
            cmod.ExamId

        };

            List<Exam> result = new List<Exam>();
            foreach (var item in query)
            {
                Exam exam = new Exam();
                exam.ExamId = item.ExamId;
                exam.Name = item.Name;

                result.Add(exam);
            }

            return result;
        }
        public object FilteredListModule(int batchClassId)
        {


            /* var query =
            from rc in db.BatchClasses
            join c in db.Batches on rc.BatchId equals c.BatchId
            join r in db.ProgramCategories on (c.Program.RevisionGroupId == null ? c.ProgramId : c.Program.RevisionGroupId) equals r.ProgramId
             join cat in db.CourseCategories on r.CategoryId equals cat.ProgramCategory.CategoryId
            join course in db.Courses on cat.CourseId equals (course.RevisionGroupId == null ? course.CourseId : course.RevisionGroupId)
            where (rc.BatchClassId == BatchClassId && course.Status.Equals("Active"))*/

            var query =
            from rc in db.BatchClasses
            join c in db.Batches on rc.BatchId equals c.BatchId
            join r in db.ProgramCategories on (c.Program.RevisionGroupId == null ? c.ProgramId : c.Program.RevisionGroupId) equals r.ProgramId
            join cat in db.CourseCategories on r.CategoryId equals cat.ProgramCategory.CategoryId
            join cmod in db.CourseModules on cat.CourseCategoryId equals cmod.CourseCategoryId
            join mod in db.Modules on cmod.ModuleId equals (mod.RevisionGroupId == null ? mod.ModuleId : mod.RevisionGroupId)
            where rc.BatchClassId == batchClassId && mod.Status.Equals("Active")
            select new
            {
                cmod.ModuleId,
                mod.ModuleTitle
            };

            List<Module> moduleList = new List<Module>();
            foreach (var item in query)
            {
                Module mod = new Module();
                mod.ModuleId = item.ModuleId;
                mod.ModuleTitle = item.ModuleTitle;
                moduleList.Add(mod);
            }
            return moduleList;
        }
        /*Nice*/
    }
}