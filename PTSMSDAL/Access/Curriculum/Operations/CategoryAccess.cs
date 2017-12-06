using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PTSMSDAL.Access.Curriculum.Operations
{
    public class CategoryAccess
    {
        private PTSContext db = new PTSContext();

        public object ListPhase()
        {
            return db.Phases.ToList();
        }
        public object ListStage()
        {
            return db.Stages.ToList();
        }

        public object ListCourse(int programCategoryId)
        {
            return db.CourseCategories.Include(c => c.Course).Include(c => c.Phase).Include(c => c.Stage).
                Where(p => p.ProgramCategoryId == programCategoryId && p.Course.Status == "Active").ToList();
        }
        public object AddCourse(List<string> courseIdList, int programCategoryId)
        {
            try
            {
                CourseCategory courseCategory = null;
                string message = "";
                CategoryAccess categoryAccess = new CategoryAccess();
                foreach (var courseId in courseIdList)
                {
                    if (!(String.IsNullOrEmpty(courseId) || String.IsNullOrWhiteSpace(courseId)))
                    {
                        int CourseId = Convert.ToInt32(courseId);
                        var result = ((List<CourseCategory>)categoryAccess.ListCourse(programCategoryId)).Where(c => c.CourseId.Equals(CourseId)).ToList();
                        if (result.Count() > 0)
                        {
                            message = message + result.FirstOrDefault().Course.CourseTitle + " is already exist in this category. ";
                        }
                        else
                        {
                            courseCategory = new CourseCategory();
                            courseCategory.CourseId = CourseId;
                            courseCategory.ProgramCategoryId = programCategoryId;
                            courseCategory.StartDate = DateTime.Now;
                            courseCategory.EndDate = new DateTime(9999, 12, 31);
                            courseCategory.CreationDate = DateTime.Now;
                            courseCategory.CreatedBy = HttpContext.Current.User.Identity.Name;
                            courseCategory.RevisionDate = DateTime.Now;
                            courseCategory.RevisedBy = HttpContext.Current.User.Identity.Name;
                            db.CourseCategories.Add(courseCategory);
                            db.SaveChanges();
                        }
                    }
                }
                if (message == "")
                    message = "Successfully added to the curriculum";
                return new { status = true, message = message }; // Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object RemoveCourse(int courseCategoryId)
        {
            try
            {
                CourseCategory courseCategory = db.CourseCategories.FirstOrDefault(p => p.CourseCategoryId == courseCategoryId);
                db.CourseCategories.Remove(courseCategory);
                db.SaveChanges();
                return new { status = true }; // Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }

        public object ListLesson(int programCategoryId)
        {
            return db.LessonCategories.Include(c => c.Lesson).
                Where(p => p.ProgramCategoryId == programCategoryId && p.Lesson.Status.Equals("Active")).ToList();
        }
        public object AddLesson(List<string> lessonIdList, int programCategoryId, int phaseId, int stageId)
        {
            try
            {
                CategoryAccess categoryAccess = new CategoryAccess();
                string message = "";
                foreach (var lessonid in lessonIdList)
                {
                    if (!(String.IsNullOrEmpty(lessonid) || String.IsNullOrWhiteSpace(lessonid)))
                    {
                        int LessonId = Convert.ToInt32(lessonid);
                        var result = ((List<LessonCategory>)categoryAccess.ListLesson(programCategoryId)).Where(c => c.LessonId.Equals(LessonId)).ToList();
                        if (result.Count() > 0)
                        {
                            message = message + result.FirstOrDefault().Lesson.LessonName + " is already exist in the selected category. ";
                        }
                        else
                        {
                            LessonCategory lessonCategory = new LessonCategory();
                            lessonCategory.LessonId = LessonId;
                            lessonCategory.ProgramCategoryId = programCategoryId;
                            lessonCategory.PhaseId = phaseId;
                            lessonCategory.StageId = stageId;

                            lessonCategory.StartDate = DateTime.Now;
                            lessonCategory.EndDate = new DateTime(9999, 12, 31);
                            lessonCategory.CreationDate = DateTime.Now;
                            lessonCategory.CreatedBy = HttpContext.Current.User.Identity.Name;
                            lessonCategory.RevisionDate = DateTime.Now;
                            lessonCategory.RevisedBy = HttpContext.Current.User.Identity.Name;

                            db.LessonCategories.Add(lessonCategory);
                            db.SaveChanges();
                        }
                    }
                }
                if (message == "")
                    message = "Successfully added to the curriculum.";
                return new { status = true, message = message }; // Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }
        public object RemoveLessonCategory(int lessonCategoryId)
        {
            try
            {
                LessonCategory lessonCategory = db.LessonCategories.FirstOrDefault(p => p.LessonCategoryId == lessonCategoryId);
                db.LessonCategories.Remove(lessonCategory);
                db.SaveChanges();
                return new { status = true };// Success
            }
            catch (System.Exception e)
            {
                return new { status = false, message = e.Message }; // Exception
            }
        }


        public List<Category> List()
        {
            return db.Categories.Include(c => c.CategoryType).Include(c => c.PreviousCategory)
                .Where(c => c.Status == "Active" && c.EndDate > DateTime.Now).ToList();
        }

        public Category Details(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return null; // Not Found
                }
                return category; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public Category CategoryDetailPartialView(int programCategoryId)
        {
            try
            {
                ProgramCategory programCategory = db.ProgramCategories.Find(programCategoryId);
                if (programCategory != null)
                {
                    var category = db.Categories.Where(c => ((c.RevisionGroupId == null && c.CategoryId == programCategory.CategoryId)
                        || (c.RevisionGroupId != null && c.RevisionGroupId == programCategory.CategoryId)) && c.Status == "Active").ToList();
                    if (category.Count > 0)
                        return category.FirstOrDefault();                    
                }
                return new Category();
            }
            catch (System.Exception e)
            {
                return new Category(); // Exception
            }
        }



        public Category CategoryDetail(int programCategoryId)
        {
            try
            {
                ProgramCategory pc = db.ProgramCategories.Find(programCategoryId);
                if (pc != null)
                {
                    Category category = db.Categories.Find(pc.CategoryId);
                    return category;
                }
                return new Category();
            }
            catch (Exception ex)
            {
                return new Category();
            }
        }


        public bool Add(Category category)
        {
            try
            {
                category.StartDate = DateTime.Now;
                category.EndDate = Constants.EndDate;
                category.CreatedBy = HttpContext.Current.User.Identity.Name;
                category.CreationDate = DateTime.Now;

                db.Categories.Add(category);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

        public bool Revise(Category category)
        {
            try
            {
                category.RevisionDate = DateTime.Now;
                category.RevisedBy = HttpContext.Current.User.Identity.Name;

                db.Entry(category).State = EntityState.Modified;
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
                Category category = db.Categories.Find(id);
                category.EndDate = DateTime.Now;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
    }
}