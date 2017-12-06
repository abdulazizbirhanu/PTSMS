using PTSMSDAL.Access.Curriculum.References;
using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class CourseAccess
    {
        private PTSContext db = new PTSContext();
        private ProgramAccess programAccess = new ProgramAccess();
        public object ListModule(int courseId)
        {
            return db.CourseModules.Include(c => c.Module).
                Where(p => p.CourseCategoryId == courseId && p.Module.Status.Equals("Active")).ToList();
        }
        public List<Module> ListModulesByCourseId(int courseId)
        {
            var query =
            from courCat in db.CourseCategories
            join courMod in db.CourseModules on courCat.CourseCategoryId equals courMod.CourseCategoryId
            join mod in db.Modules on courMod.ModuleId equals mod.ModuleId/**/
            where courCat.CourseId == courseId
            select new
            {
                ModuleCode = mod.ModuleCode,
                ModuleId = mod.ModuleId,
                ModuleTitle = mod.ModuleTitle,
            };
            List<Module> list = new List<Module>();
            foreach (var item in query)
            {
                Module module = new Module();
                module.ModuleCode = item.ModuleCode;
                module.ModuleId = item.ModuleId;
                module.ModuleTitle = item.ModuleTitle;
                list.Add(module);
            }
            return list;
        }

        //for which instructor doesn't have qualifications
        public List<Module> ListModulesByProgram(int programId)
        {
            var prog = (Program)programAccess.Details(programId);
            int programRevisionGroupId = prog.RevisionGroupId == null ? prog.ProgramId : (int)prog.RevisionGroupId;

            var query =
            from courCat in db.CourseCategories
            join courMod in db.CourseModules on courCat.CourseCategoryId equals courMod.CourseCategoryId
            join mod in db.Modules on courMod.ModuleId equals (mod.RevisionGroupId == null ? mod.ModuleId : mod.RevisionGroupId)/**/
            where courCat.ProgramCategory.ProgramId == programRevisionGroupId && mod.Status.Equals("Active")
            select new
            {
                ModuleCode = mod.ModuleCode,
                ModuleId = mod.ModuleId,
                ModuleTitle = mod.ModuleTitle,
            };
            List<Module> list = new List<Module>();
            foreach (var item in query)
            {
                Module module = new Module();
                module.ModuleCode = item.ModuleCode;
                module.ModuleId = item.ModuleId;
                module.ModuleTitle = item.ModuleTitle;
                list.Add(module);
            }
            return list;
        }

        public object AddModule(List<string> moduleIdList, int courseId, int phaseId)
        {
            try
            {
                string message = "";
                foreach (var moduleIdvar in moduleIdList)
                {
                    if (!(String.IsNullOrEmpty(moduleIdvar) || String.IsNullOrWhiteSpace(moduleIdvar)))
                    {
                        int moduleId = Convert.ToInt32(moduleIdvar);

                        var result = ((List<CourseModule>)ListModule(courseId)).Where(c => c.ModuleId.Equals(moduleId)).ToList();
                        if (result.Count() > 0)
                        {
                            message = message + result.FirstOrDefault().Module.ModuleTitle + " is already exist in the selected Course. ";
                        }
                        else
                        {
                            CourseModule courseModule = new CourseModule();
                            courseModule.ModuleId = moduleId;
                            courseModule.CourseCategoryId = courseId;
                            courseModule.PhaseId = phaseId;

                            courseModule.StartDate = DateTime.Now;
                            courseModule.EndDate = new DateTime(9999, 12, 31);
                            courseModule.CreationDate = DateTime.Now;
                            courseModule.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            courseModule.RevisionDate = DateTime.Now;
                            courseModule.RevisedBy = HttpContext.Current.User.Identity.Name;

                            db.CourseModules.Add(courseModule);
                            db.SaveChanges();
                        }
                    }
                }
                if (message == "")
                    message = "Successfully added to the curriculum.";
                return new { status = true, message = message };
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object RemoveCourseModule(int courseModuleId)
        {
            try
            {
                CourseModule courseModule = db.CourseModules.FirstOrDefault(p => p.CourseModuleId == courseModuleId);
                db.CourseModules.Remove(courseModule);
                db.SaveChanges();
                return new { status = true };// Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }

        public object ListExam(int courseId)
        {
            return db.CourseExams.Include(c => c.Exam).
                Where(p => p.CourseCategoryId == courseId && p.Exam.Status.Equals("Active")).ToList();
        }
        public object AddExam(List<string> examIdList, int courseId)
        {
            try
            {
                string message = "";
                foreach (var examIdvar in examIdList)
                {
                    if (!(String.IsNullOrEmpty(examIdvar) || String.IsNullOrWhiteSpace(examIdvar)))
                    {
                        int examId = Convert.ToInt32(examIdvar);

                        var result = ((List<CourseExam>)ListExam(courseId)).Where(c => c.ExamId.Equals(examId)).ToList();
                        if (result.Count() > 0)
                        {
                            message = message + result.FirstOrDefault().Exam.Name + " is already exist in the selected Course. ";
                        }
                        else
                        {
                            CourseExam courseExam = new CourseExam();
                            courseExam.ExamId = examId;
                            courseExam.CourseCategoryId = courseId;

                            courseExam.StartDate = DateTime.Now;
                            courseExam.EndDate = new DateTime(9999, 12, 31);
                            courseExam.CreationDate = DateTime.Now;
                            courseExam.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            courseExam.RevisionDate = DateTime.Now;
                            courseExam.RevisedBy = HttpContext.Current.User.Identity.Name;

                            db.CourseExams.Add(courseExam);
                            db.SaveChanges();
                        }
                    }
                }
                if (message == "")
                    message = "Successfully added to the curriculum.";
                return new { status = true, message = message };
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object RemoveCourseExam(int courseExamId)
        {
            try
            {
                CourseExam courseExam = db.CourseExams.FirstOrDefault(p => p.CourseExamId == courseExamId);
                db.CourseExams.Remove(courseExam);
                db.SaveChanges();
                return new { status = true }; // Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object ListPrerequisite(int courseId)
        {
            return db.Prerequisites.Include(p => p.CourseCategory).Where(p => p.CourseCategoryId == courseId && p.PrerequisiteCourse.Status.Equals("Active")).ToList();
        }
        public object AddPrerequisite(int CourseId, List<string> PrerequisiteIdList)
        {
            try
            {
                string message = "";
                foreach (var prerequisiteIdvar in PrerequisiteIdList)
                {
                    if (!(String.IsNullOrEmpty(prerequisiteIdvar) || String.IsNullOrWhiteSpace(prerequisiteIdvar)))
                    {
                        int prerequisiteId = Convert.ToInt32(prerequisiteIdvar);

                        var result = ((List<Prerequisite>)ListPrerequisite(CourseId)).Where(c => c.PrerequisiteId.Equals(prerequisiteId)).ToList();
                        if (result.Count() > 0)
                        {
                            message = message + result.FirstOrDefault().PrerequisiteCourse.CourseTitle + " is already exist in the selected Course. ";
                        }
                        else
                        {
                            Prerequisite prerequisite = new Prerequisite();
                            prerequisite.PrerequisiteCourseId = prerequisiteId;
                            prerequisite.CourseCategoryId = CourseId;

                            prerequisite.StartDate = DateTime.Now;
                            prerequisite.EndDate = new DateTime(9999, 12, 31);
                            prerequisite.CreationDate = DateTime.Now;
                            prerequisite.CreatedBy = "Test";
                            prerequisite.RevisionDate = DateTime.Now;
                            prerequisite.RevisedBy = "Test2";
                            prerequisite.Remark = "Test2ghgh";

                            db.Prerequisites.Add(prerequisite);
                            db.SaveChanges();
                        }
                    }
                }
                if (message == "")
                    message = "Successfully added to the curriculum.";
                return new { status = true, message = message };
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object RemovePrerequisite(int prerequisiteId)
        {
            try
            {
                Prerequisite prerequisite = db.Prerequisites.FirstOrDefault(p => p.PrerequisiteId == prerequisiteId);
                db.Prerequisites.Remove(prerequisite);
                db.SaveChanges();
                return new { status = true }; // Success
            }
            catch (Exception e)
            {
                return new { status = false, message = e.InnerException.InnerException.Message }; // Exception
            }
        }

        public object List()
        {
            return db.Courses.Include(c => c.PreviousCourse).Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public object Details(int id)
        {
            try
            {
                Course course = db.Courses.Find(id);
                if (course == null)
                {
                    return false; // Not Found
                }
                return course; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public Course CourseDetail(int courseCategoryId)
        {
            try
            {
                CourseCategory courseCategory = db.CourseCategories.Find(courseCategoryId);
                if (courseCategory != null)
                {
                    var course = db.Courses.Where(c => ((c.RevisionGroupId == null && c.CourseId == courseCategory.CourseId)
                        || (c.RevisionGroupId != null && c.RevisionGroupId == courseCategory.CourseId)) && c.Status == "Active").ToList();
                    if (course.Count > 0)
                        return course.FirstOrDefault();
                }
                return new Course();
            }
            catch (System.Exception e)
            {
                return new Course();
            }
        }


        public object Add(Course course)
        {
            try
            {
                course.StartDate = DateTime.Now;
                course.EndDate = Constants.EndDate;
                course.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                course.CreationDate = DateTime.Now;

                db.Courses.Add(course);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Revise(Course course)
        {
            try
            {
                course.RevisionDate = DateTime.Now;
                course.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(course).State = EntityState.Modified;
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
                Course course = db.Courses.Find(id);
                course.EndDate = DateTime.Now;
                course.RevisionDate = DateTime.Now;
                
                course.CourseCode += "_" + DateTime.Now.ToString("HHmmss");
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            //catch (System.Exception dbEx)
            //{                          
            //    return false;
            //}
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}",validationError.PropertyName,validationError.ErrorMessage);
                    }
                }
                return 0;
            }
        }
        /*Nice*/
        public object FilteredCourceExam(int courceId)
        {//            join r in db.ProgramCategories on (c.Program.RevisionGroupId==null ? c.ProgramId : c.Program.RevisionGroupId) equals r.ProgramId
            var query =
            from cc in db.CourseCategories
            join rc in db.CourseExams on cc.CourseCategoryId equals rc.CourseCategoryId
            where cc.CourseId == courceId
            join cexa in db.Exams on rc.ExamId equals (cexa.RevisionGroupId == null ? cexa.ExamId : cexa.RevisionGroupId)
            where (cexa.Status.Equals("Active"))
            select new
            {
                cexa.Name,
                cexa.RevisionGroupId
            };

            List<Exam> result = new List<Exam>();
            foreach (var item in query)
            {
                Exam exam = new Exam();
                exam.ExamId = (int)item.RevisionGroupId;
                exam.Name = item.Name;

                result.Add(exam);
            }

            return result;
        }
        public object FilteredListCourse(int BatchClassId)
        {
            var query =
            from rc in db.BatchClasses
            join c in db.Batches on rc.BatchId equals c.BatchId
            join r in db.ProgramCategories on (c.Program.RevisionGroupId == null ? c.ProgramId : c.Program.RevisionGroupId) equals r.ProgramId //&& r.Program.RevisionGroupId == r.Program.ProgramId 
            join cat in db.CourseCategories on r.CategoryId equals cat.ProgramCategory.CategoryId
            join course in db.Courses on cat.CourseId equals (course.RevisionGroupId == null ? course.CourseId : course.RevisionGroupId)
            where (rc.BatchClassId == BatchClassId && course.Status.Equals("Active"))
            select new
            {
                cat.CourseId,
                course.CourseTitle,
                course.Status
            };
            //  var active =query.Where(cc => cc.Status.ToLower() == "active");
            List<Course> result = new List<Course>();
            foreach (var item in query)
            {
                Course course = new Course();
                course.CourseId = (int)item.CourseId;
                course.CourseTitle = item.CourseTitle;

                result.Add(course);
            }

            return result;
        }
        /*Nice*/

    }
}