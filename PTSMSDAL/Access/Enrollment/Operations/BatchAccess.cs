using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.View;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Enrollment.View;
using PTSMSDAL.Models.Others.View;

namespace PTSMSDAL.Access.Enrollment.Operations
{
    public class BatchAccess
    {
        private PTSContext db = new PTSContext();
        TraineeAccess traineeAccess = new TraineeAccess();
        /*nice*/
        public List<resultSet> EnrollBatch(int batchId, int traineeId, string CompanyId, ref List<resultSet> resultSet)
        {

            try
            {
                var traineeObj = traineeAccess.Details(traineeId);
                if (traineeObj == null)
                {
                    try
                    {
                        Trainee trainee = new Trainee
                        {
                            TraineeId = traineeId,
                            StartDate = DateTime.Now,
                            EndDate = new DateTime(9999, 12, 31),
                            CreationDate = DateTime.Now,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                            RevisionDate = DateTime.Now,
                            RevisedBy = System.Web.HttpContext.Current.User.Identity.Name
                        };
                        db.Trainees.Add(trainee);
                        db.SaveChanges();


                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                /////Add Trainee to batch class by defualt
                TraineeBatchClass batchTrainee = new TraineeBatchClass();
                var batchClass = db.BatchClasses.Where(tr => tr.BatchId == batchId).ToList();
                if (batchClass.Count() > 0)
                {
                    batchTrainee.TraineeId = traineeId;
                    batchTrainee.BatchClassId = batchClass.FirstOrDefault().BatchClassId;
                    batchTrainee.StartDate = DateTime.Now;
                    batchTrainee.EndDate = Constants.EndDate;
                    batchTrainee.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    batchTrainee.CreationDate = DateTime.Now;
                    db.TraineeBatchClasses.Add(batchTrainee);
                    db.SaveChanges();
                }
                ////////////////
                if (resultSet != null)
                {
                    resultSet result = resultSet.Where(r => r.resultType == "BatchClassSuccess").FirstOrDefault();
                    if (result != null)
                        result.resultValue = result.resultValue + "," + CompanyId;
                    else
                    {
                        resultSet result1 = new resultSet();
                        result1.resultType = "BatchClassSuccess";
                        result1.resultValue = CompanyId;
                        resultSet.Add(result1);
                    }
                }
                else
                {
                    resultSet result = new resultSet();
                    result.resultType = "BatchClassSuccess";
                    result.resultValue = CompanyId;
                    resultSet.Add(result);
                }
                return resultSet; // Success
            }
            catch (System.Exception e)
            {
                PTSContext db1 = new PTSContext();
                var trainee = db1.Trainees.Include(p => p.Person).Where(tr => tr.TraineeId == traineeId).ToList();
                if (resultSet.Count > 0)
                {
                    resultSet result = resultSet.Where(r => r.resultType == "BatchClassFailed").FirstOrDefault();
                    if (result != null)
                        result.resultValue = result.resultValue + "," + CompanyId;
                    else
                    {
                        resultSet result1 = new resultSet();
                        result1.resultType = "BatchClassFailed";
                        result1.resultValue = CompanyId;
                        resultSet.Add(result1);
                    }
                }
                else
                {
                    resultSet result = new resultSet();
                    result.resultType = "BatchClassFailed";
                    result.resultValue = CompanyId;
                    resultSet.Add(result);
                }
                return resultSet; // Exception
            }
        }
        /*nice*/
        
        public object GenerateSyllabus(int batchId)
        {
            try
            {
                var batch = db.Batches.Find(batchId);
                var programCateogries = db.ProgramCategories.Include(c => c.Category).Where(p => p.ProgramId == batch.ProgramId && p.Category.Status=="Active").ToList();
                foreach (var cat in programCateogries)
                {
                    AddCategory(batchId, cat.CategoryId,cat.ProgramId);
                }

                return new { status = true, message = "Successfully generated" };
            }
            catch (Exception e)
            {
                return new { status = false, message = e.Message };
            }
        }
        
        public OperationResult AddCategory(int batchId, int categoryId, int programId)
        {
            try
            {
                Category category = db.Categories
                    .Single(c => (c.RevisionGroupId == categoryId || c.CategoryId == categoryId)
                    && c.Status.Equals("Active"));

                BatchCategory batchCategory = new BatchCategory();
                batchCategory.BatchId = batchId;
                batchCategory.CategoryId = category.CategoryId;
                batchCategory.StartDate = DateTime.Now;
                batchCategory.EndDate = new DateTime(9999, 12, 31);
                batchCategory.CreationDate = DateTime.Now;
                batchCategory.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchCategory.RevisionDate = DateTime.Now;
                batchCategory.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                db.BatchCategories.Add(batchCategory);
                db.SaveChanges();

                List<CourseCategory> courses = db.CourseCategories.Include(c => c.Course).
                    Where(c => c.ProgramCategory.CategoryId == categoryId && c.ProgramCategory.ProgramId == programId).ToList();
                courses.ForEach(c => AddCourse(batchCategory.BatchCategoryId, c.CourseId, categoryId, programId));

                List<LessonCategory> lessons = db.LessonCategories.Include(c => c.Lesson).
                    Where(l => l.ProgramCategory.CategoryId == categoryId && l.ProgramCategory.ProgramId == programId).ToList();
                lessons.ForEach(l => AddLesson(batchCategory.BatchCategoryId, categoryId, l.LessonId, programId,l.PhaseId));

                return new OperationResult { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResult { IsSuccess = false, Message = e.Message };
            }
        }

        public OperationResult AddCourse(int batchCategoryId, int courseRevisionGroupId, int categoryId, int programId)
        {
            try
            {
                Course course = db.Courses
                    .Single(c => (c.RevisionGroupId == courseRevisionGroupId || c.CourseId == courseRevisionGroupId)
                    && c.Status.Equals("Active"));

                BatchCourse batchCourse = new BatchCourse();
                batchCourse.BatchCategoryId = batchCategoryId;
                batchCourse.CourseId = course.CourseId;
                batchCourse.StartDate = DateTime.Now;
                batchCourse.EndDate = new DateTime(9999, 12, 31);
                batchCourse.CreationDate = DateTime.Now;
                batchCourse.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchCourse.RevisionDate = DateTime.Now;
                batchCourse.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.BatchCourses.Add(batchCourse);
                db.SaveChanges();

                List<CourseModule> modules = db.CourseModules.Include(c => c.Module).
                    Where(m => m.CourseCategory.ProgramCategory.ProgramId == programId && m.CourseCategory.ProgramCategory.CategoryId == categoryId && m.CourseCategory.CourseId == courseRevisionGroupId).ToList();
                modules.ForEach(c => AddModule(batchCourse.BatchCourseId, c.ModuleId, categoryId, courseRevisionGroupId, programId, c.PhaseId));

                List<CourseExam> courseExam = db.CourseExams.Include(c => c.Exam).
                    Where(m => m.CourseCategory.ProgramCategory.ProgramId == programId && m.CourseCategory.ProgramCategory.CategoryId == categoryId && m.CourseCategory.CourseId == courseRevisionGroupId).ToList();
                courseExam.ForEach(c => AddCourseExam(batchCourse.BatchCourseId, c.ExamId));

                //List<CourseReference> courseReference = db.CourseReferences.Include(c => c.Reference).
                //    Where(m => m.CourseCategory.ProgramCategory.ProgramId == programRevisionGroupId && m.CourseCategory.ProgramCategory.CategoryId == categoryRevisionGroupId && m.CourseCategory.CourseId == courseRevisionGroupId).ToList();
                //courseReference.ForEach(c => AddCourseReference(tCourse.TraineeCourseId,
                //    (int)(c.Reference.RevisionGroupId == null ? c.ReferenceId : c.Reference.RevisionGroupId)));

                List<Prerequisite> prerequisite = db.Prerequisites.
                    Where(m => m.CourseCategory.ProgramCategory.ProgramId == programId && m.CourseCategory.ProgramCategory.CategoryId == categoryId && m.CourseCategory.CourseId == courseRevisionGroupId).ToList();
                prerequisite.ForEach(c => AddPrerequisite(batchCourse.BatchCourseId, courseRevisionGroupId, c.PrerequisiteCourseId));

                return new OperationResult { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResult { IsSuccess = false, Message = e.Message };
            }
        }

        public OperationResult AddLesson(int batchCategoryId, int categoryId, int lessonRevisionGroupId, int programId, int phaseId)
        {
            try
            {
                Lesson lesson = db.Lessons
                    .Single(c => (c.RevisionGroupId == lessonRevisionGroupId || c.LessonId == lessonRevisionGroupId)
                    && c.Status.Equals("Active"));

                var lessonEvaluationTemplate = db.LessonEvaluationTemplates.Include(c => c.EvaluationTemplate).
                    Where(m => m.LessonCategory.ProgramCategory.ProgramId == programId && (m.LessonCategory.ProgramCategory.CategoryId.Equals(categoryId)) && m.LessonCategory.LessonId == lessonRevisionGroupId).SingleOrDefault();

                BatchLesson batchLesson = new BatchLesson();
                batchLesson.BatchCategoryId = batchCategoryId;
                batchLesson.EvaluationTemplateId = lessonEvaluationTemplate.EvaluationTemplateId;
                batchLesson.LessonId = lesson.LessonId;
                batchLesson.StartDate = DateTime.Now;
                batchLesson.EndDate = new DateTime(9999, 12, 31);
                batchLesson.CreationDate = DateTime.Now;
                batchLesson.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchLesson.RevisionDate = DateTime.Now;
                batchLesson.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchLesson.PhaseId = phaseId;
                batchLesson.Sequence = 1;
                db.BatchLessons.Add(batchLesson);
                db.SaveChanges();

               
                //AddEvaluationTemplate(batchLesson.BatchLessonId,(int)(lessonEvaluationTemplate.EvaluationTemplate.RevisionGroupId == null ? lessonEvaluationTemplate.EvaluationTemplate.EvaluationTemplateId : lessonEvaluationTemplate.EvaluationTemplate.RevisionGroupId));

                return new OperationResult { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResult { IsSuccess = false, Message = e.Message };
            }
        }
        /*nice*/
        public OperationResult AddModule(int batchCourseId, int moduleRevisionGroupId, int categoryId, int courseRevisionGroupId, int programId, int phaseId)
        {
            try
            {
                Module module = db.Modules
                    .Single(c => (c.RevisionGroupId == moduleRevisionGroupId || c.ModuleId == moduleRevisionGroupId)
                    && c.Status.Equals("Active"));

                BatchModule batchModule = new BatchModule();
                batchModule.BatchCourseId = batchCourseId;
                batchModule.ModuleId = module.ModuleId;
                batchModule.StartDate = DateTime.Now;
                batchModule.EndDate = new DateTime(9999, 12, 31);
                batchModule.CreationDate = DateTime.Now;
                batchModule.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchModule.RevisionDate = DateTime.Now;
                batchModule.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                batchModule.PhaseId = phaseId;
                batchModule.Sequence = 1;

                db.BatchModules.Add(batchModule);
                db.SaveChanges();

                List<ModuleExam> moduleExam = db.ModuleExams.Include(c => c.Exam).
                    Where(m => m.CourseModule.CourseCategory.ProgramCategory.ProgramId == programId && m.CourseModule.CourseCategory.ProgramCategory.CategoryId == categoryId && m.CourseModule.CourseCategory.CourseId == courseRevisionGroupId && m.CourseModule.ModuleId == moduleRevisionGroupId).ToList();
                moduleExam.ForEach(c => AddModuleExam(batchModule.BatchModuleId,
                    (int)(c.Exam.RevisionGroupId == null ? c.ExamId : c.Exam.RevisionGroupId)));

                return new OperationResult { IsSuccess = true }; // Success
            }
            catch (Exception e)
            {
                return new OperationResult { IsSuccess = false, Message = e.Message };
            }
        }

        public OperationResult AddCourseExam(int batchCourseId, int examRevisionGroupId)
        {
            try
            {
                Exam exam = db.Exams
                    .Single(c => (c.RevisionGroupId == examRevisionGroupId || c.ExamId == examRevisionGroupId)
                    && c.Status.Equals("Active"));

                BatchCourseExam batchCourseExam = new BatchCourseExam();
                batchCourseExam.BatchCourseId = batchCourseId;
                batchCourseExam.ExamId = exam.ExamId;
                batchCourseExam.StartDate = DateTime.Now;
                batchCourseExam.EndDate = new DateTime(9999, 12, 31);
                batchCourseExam.CreationDate = DateTime.Now;
                batchCourseExam.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchCourseExam.RevisionDate = DateTime.Now;
                batchCourseExam.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.BatchCourseExams.Add(batchCourseExam);
                db.SaveChanges();

                return new OperationResult { IsSuccess = true }; // Success
            }
            catch (Exception e)
            {
                return new OperationResult { IsSuccess = false, Message = e.Message };
            }
        }
        public OperationResult AddModuleExam(int batchModuleId, int examRevisionGroupId)
        {
            try
            {
                Exam exam = db.Exams
                    .Single(c => (c.RevisionGroupId == examRevisionGroupId || c.ExamId == examRevisionGroupId)
                    && c.Status.Equals("Active"));

                BatchModuleExam batchModuleExam = new BatchModuleExam();
                batchModuleExam.BatchModuleId = batchModuleId;
                batchModuleExam.ExamId = exam.ExamId;
                batchModuleExam.StartDate = DateTime.Now;
                batchModuleExam.EndDate = new DateTime(9999, 12, 31);
                batchModuleExam.CreationDate = DateTime.Now;
                batchModuleExam.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchModuleExam.RevisionDate = DateTime.Now;
                batchModuleExam.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.BatchModuleExams.Add(batchModuleExam);
                db.SaveChanges();

                return new OperationResult { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResult { IsSuccess = false, Message = e.Message };
            }
        }
        private object AddPrerequisite(int batchCourseId, int courseId, int prerequisiteId)
        {
            try
            {
                BatchCoursePrerequisite batchPrerequisite = new BatchCoursePrerequisite();
                batchPrerequisite.BatchCourseId = batchCourseId;
                batchPrerequisite.CourseId = courseId;
                batchPrerequisite.PrerequisiteId = prerequisiteId;
                batchPrerequisite.StartDate = DateTime.Now;
                batchPrerequisite.EndDate = new DateTime(9999, 12, 31);
                batchPrerequisite.CreationDate = DateTime.Now;
                batchPrerequisite.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                batchPrerequisite.RevisionDate = DateTime.Now;
                batchPrerequisite.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.BatchPrerequisites.Add(batchPrerequisite);
                db.SaveChanges();

                return new { status = true }; // Success
            }
            catch (Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object ListTrainee(int batchId)
        {
            var result = db.TraineeBatchClasses.Include(tr => tr.Trainee.Person).Where(tr => tr.BatchClass.BatchId == batchId && tr.EndDate > DateTime.Now).ToList();
            var batchTrainees = new List<BatchTraineeView>();
            result.ForEach(r => batchTrainees.Add(new BatchTraineeView {
                BatchId = r.BatchClass.BatchId,
                TraineeId = r.TraineeId,
                BatchTraineeId = r.TraineeBatchClassId,
                FirstName = r.Trainee.Person.FirstName,
                LastName = r.Trainee.Person.MiddleName,
                CompanyId = r.Trainee.Person.CompanyId,
                BatchClassId = r.BatchClassId
            }));
            
            return batchTrainees;
        }

        public object DeleteBatchTrainee(int id)
        {
            try
            {
                var traineeBatch = db.TraineeBatchClasses.Find(id);
                db.TraineeBatchClasses.Remove(traineeBatch);
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public List<Batch> List()
        {
            try
            {
                return db.Batches.Where(c => c.EndDate > DateTime.Now).ToList();
            }
            catch (Exception ex)
            {

                return new List<Batch>();
            }

        }

        public object Details(int id)
        {
            try
            {
                Batch batch = db.Batches.Find(id);
                if (batch == null)
                {
                    return false; // Not Found
                }
                return batch; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public object Add(Batch batch)
        {
            using (var dbContext = new PTSContext())
            {
                using (var dbContextTransaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        batch.StartDate = DateTime.Now;
                        batch.EndDate = Constants.EndDate;
                        batch.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        batch.CreationDate = DateTime.Now;
                        dbContext.Batches.Add(batch);
                        dbContext.SaveChanges();

                        //Add Defualt Batch Class
                        int batchId = Convert.ToInt32(dbContext.Database.SqlQuery<decimal>("Select IDENT_CURRENT ('BATCH')", new object[0]).FirstOrDefault());
                        BatchClass batchClass = new BatchClass();
                        batchClass.BatchId = batchId;
                        batchClass.BatchClassName = batch.BatchName;
                        batchClass.StartDate = DateTime.Now;
                        batchClass.EndDate = Constants.EndDate;
                        batchClass.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        batchClass.CreationDate = DateTime.Now;
                        dbContext.BatchClasses.Add(batchClass);
                        dbContext.SaveChanges();
                        dbContextTransaction.Commit();
                        return true; // Success
                    }
                    catch (System.Exception e)
                    {
                        dbContextTransaction.Rollback();
                        return false; // Exception
                    }
                }
            }
        }

        public object Revise(Batch batch)
        {
            try
            {
                batch.RevisionDate = DateTime.Now;
                batch.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(batch).State = EntityState.Modified;
                db.SaveChanges();

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
                Batch batch = db.Batches.Find(id);
                batch.EndDate = DateTime.Now;
                batch.BatchName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                db.Entry(batch).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        //Trainee Batch Class  Access
        public object AddTraineeToBatchClass(TraineeBatchClass bTrainee)
        {
            try
            {
                var batchTraineeResult = db.TraineeBatchClasses.Where(tr => tr.TraineeId == bTrainee.TraineeId).ToList();

                bTrainee.StartDate = DateTime.Now;
                bTrainee.EndDate = Constants.EndDate;
                bTrainee.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                bTrainee.CreationDate = DateTime.Now;
                if (batchTraineeResult.Count() > 0)
                {
                    TraineeBatchClass bt = batchTraineeResult.FirstOrDefault();
                    if (bt.BatchClassId == bTrainee.BatchClassId)
                    {
                        return new { status = false, message = "The Trainee has already been assigned in this batch class." };
                    }
                    bt.BatchClassId = batchTraineeResult.First().BatchClassId;
                    db.Entry(bt).State = EntityState.Modified;
                }
                else
                {
                    db.TraineeBatchClasses.Add(bTrainee);
                }
                db.SaveChanges();
                return new { status = true };
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message };
            }
        }
        public object ListTraineeToBatchClass(int batchClassId)
        {
            return db.TraineeBatchClasses.Include(tr => tr.Trainee.Person).Where(c => c.BatchClassId == batchClassId && c.EndDate > DateTime.Now).ToList();
        }

        public ProgramHierarchy GetTraineeHierarchy(int traineeId)
        {
            try
            {
                var trainee = db.Trainees.Include(testc => testc.Person).Single(t => t.TraineeId == traineeId && t.EndDate > DateTime.Now);
                var batchTrainee = db.TraineeBatchClasses.FirstOrDefault(p => p.TraineeId.Equals(trainee.TraineeId));

                ProgramHierarchy programHierarchy = new ProgramHierarchy();
                programHierarchy.Id = batchTrainee.BatchClass.Batch.ProgramId;
                programHierarchy.Name = "[Program] " + batchTrainee.BatchClass.Batch.Program.ProgramName;

                var batchCategories = db.BatchCategories.Include(c => c.Category).Where(cat => cat.BatchId == batchTrainee.BatchClass.BatchId).ToList();

                List<CategoryView> CategoryViewList = new List<CategoryView>();
                CategoryView categoryView = null;
                foreach (var cat in batchCategories)
                {
                    categoryView = new CategoryView();
                    categoryView.Id = cat.CategoryId;
                    categoryView.Name = "[Category] " + cat.Category.CategoryName + "-" + cat.Category.CategoryType.Description;
                    categoryView.CategoryType = cat.Category.CategoryName;


                    var traineeCourses = db.TraineeCourses.Include(c => c.Course).Where(cc => cc.TraineeId == trainee.TraineeId && cc.BatchCategory.Equals(cat.BatchCategoryId)).ToList();
                    List<CourseView> CourseViewList = new List<CourseView>();
                    CourseView courseView = null;
                    foreach (var course in traineeCourses)
                    {
                        courseView = new CourseView();

                        courseView.Name = "[Course] " + course.Course.CourseTitle;
                        courseView.Id = course.CourseId;

                        var traineeCourseExams = db.TraineeCourseExams.Include(ex => ex.Exam).Where(cx => cx.TraineeCourseId.Equals(course.TraineeCourseId)).ToList();
                        List<CourseExamView> CourseExamViewList = new List<CourseExamView>();
                        CourseExamView courseExamView = null;
                        foreach (var courceExam in traineeCourseExams)
                        {
                            courseExamView = new CourseExamView();

                            courseExamView.Name = "[CourseExam] " + courceExam.Exam.Name;
                            courseExamView.Id = courceExam.Exam.ExamId;
                            CourseExamViewList.Add(courseExamView);
                        }
                        var prerequisites = db.BatchPrerequisites.Include(p => p.PrerequisiteId).Where(pre => pre.PrerequisiteId.Equals(course.TraineeCourseId)).ToList();
                        List<PrerequisiteView> PrerequisiteViewList = new List<PrerequisiteView>();
                        PrerequisiteView prerequisiteView = null;
                        int xx = course.CourseId;
                        foreach (var prerequisite in prerequisites)
                        {
                            prerequisiteView = new PrerequisiteView();

                            prerequisiteView.Name = "[Prerequisite] " + prerequisite.Course.CourseTitle;
                            prerequisiteView.Id = prerequisite.PrerequisiteId;
                            PrerequisiteViewList.Add(prerequisiteView);
                        }
                        //
                        var traineeModules = db.TraineeModules.Include(m => m.Module).Where(cm => cm.TraineeCourseId.Equals(course.TraineeCourseId)).ToList();

                        List<ModuleView> ModuleViewList = new List<ModuleView>();
                        ModuleView moduleView = null;
                        foreach (var courseModule in traineeModules)
                        {
                            moduleView = new ModuleView();

                            moduleView.Name = "[CourseModule] " + courseModule.Module.ModuleTitle;
                            moduleView.Id = courseModule.ModuleId;
                            //
                            var traineeModuleExams = db.TraineeModuleExams.Where(mx => mx.TraineeModuleId.Equals(courseModule.TraineeModuleId)).ToList();
                            List<ModuleExamView> ModuleExamViewList = new List<ModuleExamView>();
                            ModuleExamView moduleExamView = null;
                            foreach (var modelExam in traineeModuleExams)
                            {
                                moduleExamView = new ModuleExamView();

                                moduleExamView.Name = "[ModelExams] " + modelExam.Exam.Name;
                                moduleExamView.Id = modelExam.ExamId;
                                ModuleExamViewList.Add(moduleExamView);
                            }
                            if (ModuleExamViewList.Count() > 0)
                                moduleView.ModuleExamList.AddRange(ModuleExamViewList);
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

                    var traineeLessons = db.TraineeLessons.Include(l => l.Lesson).Where(cl => cl.TraineeId == trainee.TraineeId && cl.BatchCategoryId.Equals(cat.BatchId)).ToList();
                    List<LessonsView> LessonViewList = new List<LessonsView>();
                    LessonsView lessonView = null;
                    foreach (var lesson in traineeLessons)
                    {
                        lessonView = new LessonsView();

                        lessonView.Name = "[Lesson]" + lesson.Lesson.LessonName;
                        lessonView.Id = lesson.LessonId;

                        var traineeEvaluationTemplate = db.EvaluationTemplates.Find(lesson.EvaluationTemplateId);
                        List<LessonEvaluationTemplateView> LessonEvaluationTemplateViewList = new List<LessonEvaluationTemplateView>();
                        LessonEvaluationTemplateView lessonEvaluationTemplateView = new LessonEvaluationTemplateView();
                        if (traineeEvaluationTemplate != null)
                        {
                            lessonEvaluationTemplateView.Name = "[LessonEvaluationTemplate] " + traineeEvaluationTemplate.EvaluationTemplateName;
                            lessonEvaluationTemplateView.Id = traineeEvaluationTemplate.EvaluationTemplateId;
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

                return programHierarchy;
            }
            catch (System.Exception)
            {
                return new ProgramHierarchy();
            }
        }

    }
}