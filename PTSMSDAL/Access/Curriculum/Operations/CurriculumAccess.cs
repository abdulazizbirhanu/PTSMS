using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Enrollment.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class CurriculumAccess
    {
        BatchAccess batchAccess = new BatchAccess();

        /*
        public OperationResult AddCategory(int programId, int categoryId, int batchId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<string> message = new List<string>();

                var traineeSyllabusList = db.TraineeSyllabuses.Where(s => s.BatchId == batchId).ToList();

                Category category = db.Categories.Find(categoryId);
                if (category != null)
                {
                    int categoryRevisionGroupId = category.RevisionGroupId != null ? (int)category.RevisionGroupId : category.CategoryId;
                    foreach (var traineeSyllabus in traineeSyllabusList)
                    {
                        TraineeProgram traineeProgram = db.TraineePrograms.FirstOrDefault(p => p.TraineeSyllabusId == traineeSyllabus.TraineeSyllabusId && p.ProgramId == programId);
                        if (traineeProgram != null)
                        {
                            var result = batchAccess.AddCategory(traineeProgram.TraineeProgramId, categoryRevisionGroupId, (int)(traineeProgram.Program.RevisionGroupId == null ? traineeProgram.ProgramId : traineeProgram.Program.RevisionGroupId));
                            if (!result.IsSuccess)
                                message.Add(result.Message + " for trainee ID = " + traineeSyllabus.Trainee.Person.CompanyId);
                        }
                        else
                            message.Add("Trainee Program is not found. ");
                    }
                }
                else
                    message.Add("Category is not found. ");
                if (message.Count > 0)
                    return new OperationResult { IsSuccess = false, Message = string.Join(",", message) };
                return new OperationResult { IsSuccess = true, Message = "Change has successfully applied." };

            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, Message = ex.Message };
            }
        }
        public OperationResult AddCourse(int programCategoryId, int courseId, int batchId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<string> message = new List<string>();

                ProgramCategory programCategory = db.ProgramCategories.Find(programCategoryId);
                if (programCategory != null)
                {
                    var traineeSyllabusList = db.TraineeSyllabuses.Where(s => s.BatchId == batchId).ToList();

                    Course course = db.Courses.Find(courseId);
                    if (course != null)
                    {
                        int courseRevisionGroupId = course.RevisionGroupId != null ? (int)course.RevisionGroupId : course.CourseId;

                        //Apply change to all polulated trainee Syllabus
                        foreach (var traineeSyllabus in traineeSyllabusList)
                        {
                            TraineeProgram traineeProgram = db.TraineePrograms.FirstOrDefault(p => p.TraineeSyllabusId == traineeSyllabus.TraineeSyllabusId && p.ProgramId == programCategory.ProgramId);
                            if (traineeProgram != null)
                            {
                                TraineeProgramCategory traineeCategory = db.TraineeCategories.FirstOrDefault(c => c.TraineeProgramId == traineeProgram.TraineeProgramId && c.CategoryId == programCategory.CategoryId);
                                if (traineeCategory != null)
                                {
                                    var result = batchAccess.AddCourse(traineeCategory.TraineeCategoryId, courseRevisionGroupId, programCategory.CategoryId, (int)(traineeProgram.Program.RevisionGroupId == null ? traineeProgram.ProgramId : traineeProgram.Program.RevisionGroupId));
                                    if (!result.IsSuccess)
                                        message.Add(result.Message + " for trainee ID = " + traineeSyllabus.Trainee.Person.CompanyId);
                                }
                                else
                                    message.Add("Trainee Category is not found. ");
                            }
                            else
                                message.Add("Trainee Program is not found. ");
                        }
                    }
                    else
                        message.Add("Course is not found. ");
                }
                else
                    message.Add("Program Category is not found. ");
                if (message.Count > 0)
                    return new OperationResult { IsSuccess = false, Message = string.Join(",", message) };
                return new OperationResult { IsSuccess = true, Message = "Change has successfully applied." };
            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, Message = ex.Message };
            }
        }
        public OperationResult AddCourseExam(int courseCategoryId, int examId, int batchId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<string> message = new List<string>();
                CourseCategory courseCategory = db.CourseCategories.Find(courseCategoryId);

                if (courseCategory != null)
                {
                    ProgramCategory programCategory = db.ProgramCategories.Where(p => p.ProgramId == courseCategory.ProgramCategory.ProgramId && p.CategoryId == courseCategory.ProgramCategory.CategoryId).FirstOrDefault();

                    if (programCategory != null)
                    {
                        var traineeSyllabusList = db.TraineeSyllabuses.Where(s => s.BatchId == batchId).ToList();
                        Exam exam = db.Exams.Find(examId);
                        if (exam != null)
                        {
                            int examRevisionGroupId = exam.RevisionGroupId != null ? (int)exam.RevisionGroupId : exam.ExamId;
                            //Apply change to all polulated trainee Syllabus
                            foreach (var traineeSyllabus in traineeSyllabusList)
                            {
                                TraineeProgram traineeProgram = db.TraineePrograms.FirstOrDefault(p => p.TraineeSyllabusId == traineeSyllabus.TraineeSyllabusId && p.ProgramId == programCategory.ProgramId);
                                if (traineeProgram != null)
                                {
                                    TraineeProgramCategory traineeCategory = db.TraineeCategories.FirstOrDefault(c => c.TraineeProgramId == traineeProgram.TraineeProgramId && c.CategoryId == programCategory.CategoryId);
                                    if (traineeCategory != null)
                                    {
                                        BatchCourse traineeCourse = db.TraineeCourses.FirstOrDefault(c => c.TraineeCategoryId == traineeCategory.TraineeCategoryId && c.CourseId == courseCategory.CourseId);
                                        if (traineeCourse != null)
                                        {
                                            var result = batchAccess.AddCourseExam(traineeCourse.TraineeCourseId, examRevisionGroupId);
                                            if (!result.IsSuccess)
                                                message.Add(result.Message + " for trainee ID = " + traineeSyllabus.Trainee.Person.CompanyId);
                                        }
                                        else
                                            message.Add("Trainee Course is not found. ");
                                    }
                                    else
                                        message.Add("Trainee Category is not found. ");
                                }
                                else
                                    message.Add("Trainee Program is not found. ");
                            }
                        }
                        else
                            message.Add("Exam is not found. ");
                    }
                    else
                        message.Add("Program Category is not found. ");
                }
                else
                    message.Add("Course Category is not found. ");
                if (message.Count > 0)
                    return new OperationResult { IsSuccess = false, Message = string.Join(",", message) };
                return new OperationResult { IsSuccess = true, Message = "Change has successfully applied." };
            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, Message = "Error occured while appllying the change. " + ex.Message };
            }
        }
        public OperationResult AddCourseModule(int courseCategoryId, int moduleId, int batchId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<string> message = new List<string>();
                CourseCategory courseCategory = db.CourseCategories.Find(courseCategoryId);

                if (courseCategory != null)
                {
                    ProgramCategory programCategory = db.ProgramCategories.Where(p => p.ProgramId == courseCategory.ProgramCategory.ProgramId && p.CategoryId == courseCategory.ProgramCategory.CategoryId).FirstOrDefault();

                    if (programCategory != null)
                    {
                        var traineeSyllabusList = db.TraineeSyllabuses.Where(s => s.BatchId == batchId).ToList();
                        Module module = db.Modules.Find(moduleId);

                        if (module != null)
                        {
                            int moduleRevisionGroupId = module.RevisionGroupId != null ? (int)module.RevisionGroupId : module.ModuleId;

                            //Apply change to all polulated trainee Syllabus
                            foreach (var traineeSyllabus in traineeSyllabusList)
                            {
                                TraineeProgram traineeProgram = db.TraineePrograms.FirstOrDefault(p => p.TraineeSyllabusId == traineeSyllabus.TraineeSyllabusId && p.ProgramId == programCategory.ProgramId);
                                if (traineeProgram != null)
                                {
                                    TraineeProgramCategory traineeCategory = db.TraineeCategories.FirstOrDefault(c => c.TraineeProgramId == traineeProgram.TraineeProgramId && c.CategoryId == programCategory.CategoryId);
                                    if (traineeCategory != null)
                                    {
                                        BatchCourse traineeCourse = db.TraineeCourses.FirstOrDefault(c => c.TraineeCategoryId == traineeCategory.TraineeCategoryId && c.CourseId == courseCategory.CourseId);
                                        if (traineeCourse != null)
                                        {
                                            int programRevisionGroupId = (int)(traineeProgram.Program.RevisionGroupId == null ? traineeProgram.ProgramId : traineeProgram.Program.RevisionGroupId);
                                            int categoryRevisionGroupId = (int)(traineeCategory.Category.RevisionGroupId == null ? traineeCategory.CategoryId : traineeCategory.Category.RevisionGroupId);
                                            var result = batchAccess.AddModule(traineeCourse.TraineeCourseId, moduleRevisionGroupId, categoryRevisionGroupId, courseCategory.CourseId, programRevisionGroupId);

                                            if (!result.IsSuccess)
                                                message.Add(result.Message + " for trainee ID = " + traineeSyllabus.Trainee.Person.CompanyId);
                                        }
                                        else
                                            message.Add("Trainee Course is not found. ");
                                    }
                                    else
                                        message.Add("Trainee Category is not found. ");
                                }
                                else
                                    message.Add("Trainee Program is not found. ");
                            }
                        }
                        else
                            message.Add("Exam is not found. ");
                    }
                    else
                        message.Add("Program Category is not found. ");
                }
                else
                    message.Add("Course Category is not found. ");
                if (message.Count > 0)
                    return new OperationResult { IsSuccess = false, Message = string.Join(",", message) };
                return new OperationResult { IsSuccess = true, Message = "Change has successfully applied." };
            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, Message = "Error occured while appllying the change. " + ex.Message };
            }
        }
        public OperationResult AddModuleExam(int courseModuleId, int examId, int batchId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<string> message = new List<string>();
                CourseModule courseModule = db.CourseModules.Find(courseModuleId);
                if (courseModule != null)
                {
                    CourseCategory courseCategory = db.CourseCategories.Find(courseModule.CourseCategoryId);

                    if (courseCategory != null)
                    {
                        ProgramCategory programCategory = db.ProgramCategories.Where(p => p.ProgramId == courseCategory.ProgramCategory.ProgramId && p.CategoryId == courseCategory.ProgramCategory.CategoryId).FirstOrDefault();

                        if (programCategory != null)
                        {
                            var traineeSyllabusList = db.TraineeSyllabuses.Where(s => s.BatchId == batchId).ToList();
                            Exam exam = db.Exams.Find(examId);
                            if (exam != null)
                            {
                                int examRevisionGroupId = exam.RevisionGroupId != null ? (int)exam.RevisionGroupId : exam.ExamId;
                                //Apply change to all polulated trainee Syllabus
                                foreach (var traineeSyllabus in traineeSyllabusList)
                                {
                                    TraineeProgram traineeProgram = db.TraineePrograms.FirstOrDefault(p => p.TraineeSyllabusId == traineeSyllabus.TraineeSyllabusId && p.ProgramId == programCategory.ProgramId);
                                    if (traineeProgram != null)
                                    {
                                        TraineeProgramCategory traineeCategory = db.TraineeCategories.FirstOrDefault(c => c.TraineeProgramId == traineeProgram.TraineeProgramId && c.CategoryId == programCategory.CategoryId);
                                        if (traineeCategory != null)
                                        {
                                            BatchCourse traineeCourse = db.TraineeCourses.FirstOrDefault(c => c.TraineeCategoryId == traineeCategory.TraineeCategoryId && c.CourseId == courseCategory.CourseId);
                                            if (traineeCourse != null)
                                            {
                                                BatchModule traineeModule = db.TraineeModules.FirstOrDefault(c => c.TraineeCourseId == traineeCourse.TraineeCourseId && c.ModuleId == courseModule.ModuleId);
                                                if (traineeModule != null)
                                                {
                                                    var result = batchAccess.AddModuleExam(traineeModule.TraineeModuleId, examRevisionGroupId);
                                                    if (!result.IsSuccess)
                                                        message.Add(result.Message + " for trainee ID = " + traineeSyllabus.Trainee.Person.CompanyId);
                                                }
                                            }
                                            else
                                                message.Add("Trainee Course is not found. ");
                                        }
                                        else
                                            message.Add("Trainee Category is not found. ");
                                    }
                                    else
                                        message.Add("Trainee Program is not found. ");
                                }
                            }
                            else
                                message.Add("Exam is not found. ");
                        }
                        else
                            message.Add("Program Category is not found. ");
                    }
                    else
                        message.Add("Course Category is not found. ");
                }
                else
                    message.Add("Course Module is not found. ");
                if (message.Count > 0)
                    return new OperationResult { IsSuccess = false, Message = string.Join(",", message) };
                return new OperationResult { IsSuccess = true, Message = "Change has successfully applied." };
            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, Message = "Error occured while appllying the change. " + ex.Message };
            }
        }
        public OperationResult AddLesson(int programCategoryId, int lessonId, int batchId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<string> message = new List<string>();

                ProgramCategory programCategory = db.ProgramCategories.Find(programCategoryId);
                if (programCategory != null)
                {
                    var traineeSyllabusList = db.TraineeSyllabuses.Where(s => s.BatchId == batchId).ToList();

                    Lesson lesson = db.Lessons.Find(lessonId);
                    if (lesson != null)
                    {
                        int lessonRevisionGroupId = lesson.RevisionGroupId != null ? (int)lesson.RevisionGroupId : lesson.LessonId;

                        //Apply change to all polulated trainee Syllabus
                        foreach (var traineeSyllabus in traineeSyllabusList)
                        {
                            TraineeProgram traineeProgram = db.TraineePrograms.FirstOrDefault(p => p.TraineeSyllabusId == traineeSyllabus.TraineeSyllabusId && p.ProgramId == programCategory.ProgramId);
                            if (traineeProgram != null)
                            {
                                TraineeProgramCategory traineeCategory = db.TraineeCategories.FirstOrDefault(c => c.TraineeProgramId == traineeProgram.TraineeProgramId && c.CategoryId == programCategory.CategoryId);
                                if (traineeCategory != null)
                                {
                                    var result = batchAccess.AddLesson(traineeCategory.TraineeCategoryId, programCategory.CategoryId, lessonRevisionGroupId, (int)(traineeProgram.Program.RevisionGroupId == null ? traineeProgram.ProgramId : traineeProgram.Program.RevisionGroupId));
                                    if (!result.IsSuccess)
                                        message.Add(result.Message + " for trainee ID = " + traineeSyllabus.Trainee.Person.CompanyId);
                                }
                                else
                                    message.Add("Trainee Category is not found. ");
                            }
                            else
                                message.Add("Trainee Program is not found. ");
                        }
                    }
                    else
                        message.Add("Lesson is not found. ");
                }
                else
                    message.Add("Program Category is not found. ");
                if (message.Count > 0)
                    return new OperationResult { IsSuccess = false, Message = string.Join(",", message) };
                return new OperationResult { IsSuccess = true, Message = "Change has successfully applied." };
            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, Message = ex.Message };
            }
        }
        public OperationResult AddLessonEvaluationTemplate(int lessonCategoryId, int evaluationTemplateId, int batchId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<string> message = new List<string>();
                LessonCategory lessonCategory = db.LessonCategories.Find(lessonCategoryId);

                if (lessonCategory != null)
                {
                    ProgramCategory programCategory = db.ProgramCategories.Where(p => p.ProgramId == lessonCategory.ProgramCategory.ProgramId && p.CategoryId == lessonCategory.ProgramCategory.CategoryId).FirstOrDefault();

                    if (programCategory != null)
                    {
                        var traineeSyllabusList = db.TraineeSyllabuses.Where(s => s.BatchId == batchId).ToList();
                        EvaluationTemplate evaluationTemplate = db.EvaluationTemplates.Find(evaluationTemplateId);
                        if (evaluationTemplate != null)
                        {
                            int evaluationTemplateRevisionGroupId = evaluationTemplate.RevisionGroupId != null ? (int)evaluationTemplate.RevisionGroupId : evaluationTemplate.EvaluationTemplateId;
                            //Apply change to all polulated trainee Syllabus
                            foreach (var traineeSyllabus in traineeSyllabusList)
                            {
                                TraineeProgram traineeProgram = db.TraineePrograms.FirstOrDefault(p => p.TraineeSyllabusId == traineeSyllabus.TraineeSyllabusId && p.ProgramId == programCategory.ProgramId);
                                if (traineeProgram != null)
                                {
                                    TraineeProgramCategory traineeCategory = db.TraineeCategories.FirstOrDefault(c => c.TraineeProgramId == traineeProgram.TraineeProgramId && c.CategoryId == programCategory.CategoryId);
                                    if (traineeCategory != null)
                                    {
                                        BatchLesson traineelesson = db.TraineeLessons.FirstOrDefault(l => l.TraineeCategoryId == traineeCategory.TraineeCategoryId && l.LessonId == lessonCategory.LessonId);
                                        if (traineelesson != null)
                                        {
                                            var result = batchAccess.AddEvaluationTemplate(traineelesson.TraineeLessonId, evaluationTemplateRevisionGroupId);
                                            if (!result.IsSuccess)
                                                message.Add(result.Message + " for trainee ID = " + traineeSyllabus.Trainee.Person.CompanyId);
                                        }
                                        else
                                            message.Add("Trainee Course is not found. ");
                                    }
                                    else
                                        message.Add("Trainee Category is not found. ");
                                }
                                else
                                    message.Add("Trainee Program is not found. ");
                            }
                        }
                        else
                            message.Add("Exam is not found. ");
                    }
                    else
                        message.Add("Program Category is not found. ");
                }
                else
                    message.Add("Course Category is not found. ");
                if (message.Count > 0)
                    return new OperationResult { IsSuccess = false, Message = string.Join(",", message) };
                return new OperationResult { IsSuccess = true, Message = "Change has successfully applied." };

            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        */
    }
}
