using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PTSMSDAL.Access.Curriculum.References
{
    public class ProgramAccess
    {
        private PTSContext db = new PTSContext();


        public object ListCategory(int ProgramId)
        {
            return db.ProgramCategories.Include(c => c.Category).
                Where(p => p.ProgramId == ProgramId && p.Category.Status.Equals("Active")).ToList();
        }
        public object AddCategory(int programId, List<string> categoryIdList)
        {
            try
            {
                string message = "";

                Program program = db.Programs.Single(p => p.ProgramId.Equals(programId));
                int? programRevisionGroupId = program.RevisionGroupId == null ? program.ProgramId : program.RevisionGroupId;

                foreach (var categoryId in categoryIdList)
                {
                    ProgramAccess programAccess = new ProgramAccess();
                    var result = ((List<ProgramCategory>)programAccess.ListCategory((int)programRevisionGroupId)).Where(c => c.CategoryId.Equals(Convert.ToInt32(categoryId))).ToList();
                    if (result.Count() > 0)
                    {
                        message = message + result.FirstOrDefault().Category.CategoryName + "-" + result.FirstOrDefault().Category.CategoryType.Description + " is already exist in the selected program. ";
                    }
                    else
                    {
                        ProgramCategory programCategory = new ProgramCategory();
                        programCategory.ProgramId = (int)programRevisionGroupId;
                        programCategory.CategoryId = Convert.ToInt32(categoryId);

                        programCategory.StartDate = DateTime.Now;
                        programCategory.EndDate = new DateTime(9999, 12, 31);
                        programCategory.CreationDate = DateTime.Now;
                        programCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        programCategory.RevisionDate = DateTime.Now;
                        programCategory.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                        db.ProgramCategories.Add(programCategory);
                        db.SaveChanges();
                    }
                }
                return new { status = true, message = message }; // Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object RemoveCategory(int programCategoryId)
        {
            try
            {
                ProgramCategory programCategory = db.ProgramCategories.FirstOrDefault(p => p.ProgramCategoryId == programCategoryId);
                db.ProgramCategories.Remove(programCategory);
                db.SaveChanges();
                return new { status = true }; // Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }


        public List<Program> List()
        {
            return db.Programs.Include(p => p.PreviousProgram).Where(p => p.Status == "Active" && p.EndDate > DateTime.Now).ToList();
        }

        public Program Details(int id)
        {
            try
            {
                Program program = db.Programs.Find(id);
                if (program == null)
                {
                    return null; // Not Found
                }
                return program; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(Program program)
        {
            try
            {
                program.StartDate = DateTime.Now;
                program.EndDate = Constants.EndDate;
                program.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                program.CreationDate = DateTime.Now;

                db.Programs.Add(program);
                db.SaveChanges();               
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(Program program)
        {
            try
            {
                program.RevisionDate = DateTime.Now;
                program.RevisedBy=System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(program).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Delete(int id)
        {
            try
            {
                Program program = db.Programs.Find(id);
                program.EndDate = DateTime.Now;
                program.Status = "Deleted";
                program.ProgramName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(program).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public ProgramHierarchy GetProgramHierarchy(int programId)
        {
            try
            {
                ProgramHierarchy programHierarchy = new ProgramHierarchy();
                //
                var programs = db.Programs.ToList();
                var prog = programs.Single(pr => pr.ProgramId == programId && pr.EndDate > DateTime.Now);
                if (programs != null)
                {
                    Program program = prog;
                    int? programRevisionGroupId = program.RevisionGroupId == null ? program.ProgramId : program.RevisionGroupId;
                    //program has revised, take active program
                    if (program.RevisionGroupId == null && program.Status.Equals("Active"))
                    {
                        programRevisionGroupId = program.ProgramId;
                    }
                    else
                    {
                        program = programs.Single(p => p.RevisionGroupId == programRevisionGroupId && p.Status.Equals("Active"));
                    }

                    programHierarchy.Name = "[Program] " + program.ProgramName;
                    programHierarchy.Id = program.ProgramId;

                    //filter by programId from ProgramCategory table
                    //var categories = program.Program.Categories;
                    var programCategories = db.ProgramCategories.Include(cat => cat.Category.CategoryType)
                        .Where(cat => cat.ProgramId == programRevisionGroupId).ToList();

                    List<CategoryView> CategoryViewList = new List<CategoryView>();
                    CategoryView categoryView = null;
                    foreach (var cat in programCategories)
                    {
                        Category category = cat.Category;
                        int? categoryRevisionGroupId = category.RevisionGroupId == null ? category.CategoryId : category.RevisionGroupId;

                        if (category.RevisionGroupId == null && category.Status.Equals("Active"))
                        {
                            categoryRevisionGroupId = category.CategoryId;
                        }
                        else
                        {
                            category = db.Categories.Single(c => c.RevisionGroupId == categoryRevisionGroupId && c.Status.Equals("Active"));
                        }

                        categoryView = new CategoryView();
                        categoryView.Id = cat.ProgramCategoryId;
                        categoryView.Name = "[Category] " + category.CategoryName + "-" + category.CategoryType.Description;
                        categoryView.CategoryType = category.CategoryName;


                        var courses = db.CourseCategories.Where(cc => cc.ProgramCategoryId.Equals(cat.ProgramCategoryId)).ToList();
                        List<CourseView> CourseViewList = new List<CourseView>();
                        CourseView courseView = null;
                        foreach (var courseItem in courses)
                        {
                            Course course = courseItem.Course;
                            int? courseRevisionGroupId = course.RevisionGroupId == null ? course.CourseId : course.RevisionGroupId;

                            if (course.RevisionGroupId == null && course.Status.Equals("Active"))
                            {
                                courseRevisionGroupId = course.CourseId;
                            }
                            else
                            {
                                course = db.Courses.Single(c => c.RevisionGroupId == courseRevisionGroupId && c.Status.Equals("Active"));
                            }

                            courseView = new CourseView();

                            courseView.Name = "[Course] " + course.CourseTitle;
                            courseView.Id = courseItem.CourseCategoryId;

                            var courceExams = db.CourseExams.Where(cx => cx.CourseCategoryId.Equals(courseItem.CourseCategoryId)).ToList();
                            List<CourseExamView> CourseExamViewList = new List<CourseExamView>();
                            CourseExamView courseExamView = null;
                            foreach (var courceExam in courceExams)
                            {
                                Exam exam = courceExam.Exam;
                                int? examRevisionGroupId = exam.RevisionGroupId == null ? exam.ExamId : exam.RevisionGroupId;

                                if (exam.RevisionGroupId == null && exam.Status.Equals("Active"))
                                {
                                    examRevisionGroupId = exam.ExamId;
                                }
                                else
                                {
                                    exam = db.Exams.Single(e => (e.RevisionGroupId == null? e.ExamId == examRevisionGroupId : e.RevisionGroupId == examRevisionGroupId) && e.Status.Equals("Active"));
                                }

                                courseExamView = new CourseExamView();

                                courseExamView.Name = "[CourseExam] " + exam.Name;
                                courseExamView.Id = courceExam.CourseExamId;
                                CourseExamViewList.Add(courseExamView);
                            }
                            var prerequisites = db.Prerequisites.Where(pre => pre.CourseCategoryId.Equals(courseItem.CourseCategoryId)).ToList();
                            List<PrerequisiteView> PrerequisiteViewList = new List<PrerequisiteView>();
                            PrerequisiteView prerequisiteView = null;
                            //int xx = course.CourseId;
                            foreach (var prerequisiteItem in prerequisites)
                            {
                                Course prerequisite = prerequisiteItem.PrerequisiteCourse;
                                int? prerequisiteRevisionGroupId = prerequisite.RevisionGroupId == null ? prerequisite.CourseId : prerequisite.RevisionGroupId;

                                if (prerequisite.RevisionGroupId == null && prerequisite.Status.Equals("Active"))
                                {
                                    prerequisiteRevisionGroupId = prerequisite.CourseId;
                                }
                                else
                                {
                                    prerequisite = db.Courses.Single(c => c.RevisionGroupId == prerequisiteRevisionGroupId && c.Status.Equals("Active"));
                                }

                                prerequisiteView = new PrerequisiteView();

                                prerequisiteView.Name = "[Prerequisite] " + prerequisite.CourseTitle;
                                prerequisiteView.Id = prerequisiteItem.PrerequisiteId;
                                PrerequisiteViewList.Add(prerequisiteView);
                            }
                            //
                            var courseModules = db.CourseModules.Where(cm => cm.CourseCategoryId.Equals(courseItem.CourseCategoryId)).ToList();

                            List<ModuleView> ModuleViewList = new List<ModuleView>();
                            ModuleView moduleView = null;
                            foreach (var courseModule in courseModules)
                            {
                                Module module = courseModule.Module;
                                int? moduleRevisionGroupId = module.RevisionGroupId == null ? module.ModuleId : module.RevisionGroupId;

                                if (module.RevisionGroupId == null && module.Status.Equals("Active"))
                                {
                                    moduleRevisionGroupId = module.ModuleId;
                                }
                                else
                                {
                                    module = db.Modules.Single(m => m.RevisionGroupId == moduleRevisionGroupId && m.Status.Equals("Active"));
                                }

                                moduleView = new ModuleView();

                                moduleView.Name = "[CourseModule] " + module.ModuleTitle;
                                moduleView.Id = courseModule.CourseModuleId;
                                //
                                var moduleExams = db.ModuleExams.Where(mx => mx.CourseModuleId.Equals(courseModule.CourseModuleId)).ToList();
                                List<ModuleExamView> ModuleExamViewList = new List<ModuleExamView>();
                                ModuleExamView moduleExamView = null;
                                foreach (var modelExam in moduleExams)
                                {
                                    Exam exam = modelExam.Exam;
                                    int? examRevisionGroupId = exam.RevisionGroupId == null ? exam.ExamId : exam.RevisionGroupId;

                                    if (exam.RevisionGroupId == null && exam.Status.Equals("Active"))
                                    {
                                        examRevisionGroupId = exam.ExamId;
                                    }
                                    else
                                    {
                                        exam = db.Exams.Single(m => m.RevisionGroupId == examRevisionGroupId && m.Status.Equals("Active"));
                                    }

                                    moduleExamView = new ModuleExamView();

                                    moduleExamView.Name = "[ModelExams] " + exam.Name;
                                    moduleExamView.Id = modelExam.ModuleExamId;
                                    ModuleExamViewList.Add(moduleExamView);
                                }
                                //
                                var moduleGroundLessons = db.ModuleGroundLessons.Where(mgl => mgl.CourseModuleId.Equals(courseModule.CourseModuleId)).ToList();
                                List<ModuleGroundLessonView> ModuleGroundLessonViewList = new List<ModuleGroundLessonView>();
                                ModuleGroundLessonView moduleGroundLessonView = null;
                                foreach (var moduleGroundLesson in moduleGroundLessons)
                                {
                                    GroundLesson groundLesson = moduleGroundLesson.GroundLesson;
                                    int? groundLessonRevisionGroupId = groundLesson.RevisionGroupId == null ? groundLesson.GroundLessonId : groundLesson.RevisionGroupId;

                                    if (groundLesson.RevisionGroupId == null && groundLesson.Status.Equals("Active"))
                                    {
                                        groundLessonRevisionGroupId = groundLesson.GroundLessonId;
                                    }
                                    else
                                    {
                                        groundLesson = db.GroundLessons.Single(m => m.RevisionGroupId == groundLessonRevisionGroupId && m.Status.Equals("Active"));
                                    }

                                    moduleGroundLessonView = new ModuleGroundLessonView();

                                    moduleGroundLessonView.Name = "[ModuleGroundLesson] " + groundLesson.LessonName;
                                    moduleGroundLessonView.Id = moduleGroundLesson.ModuleGroundLessonId;
                                    ModuleGroundLessonViewList.Add(moduleGroundLessonView);
                                }
                                if (ModuleExamViewList.Count() > 0)
                                    moduleView.ModuleExamList.AddRange(ModuleExamViewList);
                                if (ModuleGroundLessonViewList.Count() > 0)
                                    moduleView.ModuleGroundLessonList.AddRange(ModuleGroundLessonViewList);
                                ModuleViewList.Add(moduleView);
                            }

                            if (CourseExamViewList.Count() > 0)
                                courseView.CourseExamList.AddRange(CourseExamViewList);
                            if (PrerequisiteViewList.Count() > 0)
                                courseView.PrerequisiteList.AddRange(PrerequisiteViewList);
                            if (ModuleViewList.Count() > 0)
                                courseView.ModuleList.AddRange(ModuleViewList);
                            CourseViewList.Add(courseView);
                        }

                        var categoryLessons = db.LessonCategories.Where(cl => cl.ProgramCategoryId.Equals(cat.ProgramCategoryId)).ToList();
                        List<LessonsView> LessonViewList = new List<LessonsView>();
                        LessonsView lessonView = null;
                        foreach (var categoryLesson in categoryLessons)
                        {
                            Lesson lesson = categoryLesson.Lesson;
                            int? lessonRevisionGroupId = lesson.RevisionGroupId == null ? lesson.LessonId : lesson.RevisionGroupId;

                            if (lesson.RevisionGroupId == null && lesson.Status.Equals("Active"))
                            {
                                lessonRevisionGroupId = lesson.LessonId;
                            }
                            else
                            {
                                lesson = db.Lessons.Single(m => m.RevisionGroupId == lessonRevisionGroupId && m.Status.Equals("Active"));
                            }

                            lessonView = new LessonsView();

                            lessonView.Name = "[Lesson]" + lesson.LessonName;
                            lessonView.Id = categoryLesson.LessonCategoryId;

                            var lessonEvalutions
                                = db.LessonEvaluationTemplates.Where(le => le.LessonCategoryId.Equals(categoryLesson.LessonCategoryId)).ToList();
                            List<LessonEvaluationTemplateView> LessonEvaluationTemplateViewList = new List<LessonEvaluationTemplateView>();
                            LessonEvaluationTemplateView lessonEvaluationTemplateView = null;

                            foreach (var lessonEvaluation in lessonEvalutions)
                            {
                                EvaluationTemplate evaluationTemplate = lessonEvaluation.EvaluationTemplate;
                                int? evaluationTemplateRevisionGroupId = evaluationTemplate.RevisionGroupId == null ? evaluationTemplate.EvaluationTemplateId : evaluationTemplate.RevisionGroupId;

                                if (evaluationTemplate.RevisionGroupId == null && evaluationTemplate.Status.Equals("Active"))
                                {
                                    evaluationTemplateRevisionGroupId = evaluationTemplate.EvaluationTemplateId;
                                }
                                else
                                {
                                    evaluationTemplate = db.EvaluationTemplates.Single(m => m.RevisionGroupId == evaluationTemplateRevisionGroupId && m.Status.Equals("Active"));
                                }

                                lessonEvaluationTemplateView = new LessonEvaluationTemplateView();

                                lessonEvaluationTemplateView.Name = "[LessonEvaluationTemplate]" + evaluationTemplate.EvaluationTemplateName;
                                lessonEvaluationTemplateView.Id = lessonEvaluation.LessonEvaluationTemplateId;
                                LessonEvaluationTemplateViewList.Add(lessonEvaluationTemplateView);
                            }

                            if (LessonEvaluationTemplateViewList.Count() > 0)
                                lessonView.LessonEvaluationTemplateViewList.AddRange(LessonEvaluationTemplateViewList);
                            LessonViewList.Add(lessonView);
                        }
                        if (LessonViewList.Count() > 0)
                            categoryView.LessonList.AddRange(LessonViewList);
                        if (CourseViewList.Count() > 0)
                            categoryView.CourseList.AddRange(CourseViewList);
                        CategoryViewList.Add(categoryView);
                    }
                    if (CategoryViewList.Count() > 0)
                        programHierarchy.CategoryList.AddRange(CategoryViewList);
                }
                return programHierarchy;
            }
            catch (System.Exception ex)
            {
                return new ProgramHierarchy();
            }
        }
    }
}