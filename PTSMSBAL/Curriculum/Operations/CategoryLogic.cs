using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;

namespace PTSMSBAL.Curriculum.Operations
{
    public class CategoryLogic
    {
        CategoryAccess categoryAccess = new CategoryAccess();

        public object ListPhase()
        {
            return categoryAccess.ListPhase();
        }
        public object ListStage()
        {
            return categoryAccess.ListStage();
        }

        public object ListCourse(int categoryId)
        {
            return categoryAccess.ListCourse(categoryId);
        }

        public object AddCourse(List<string> courseIdList, int categoryId)
        {
            return categoryAccess.AddCourse(courseIdList, categoryId);
        }

        public object RemoveCourse(int courseCategoryId)
        {
            return categoryAccess.RemoveCourse(courseCategoryId);
        }


        public object ListLesson(int courseId)
        {
            return categoryAccess.ListLesson(courseId);
        }

        public object AddLesson(List<string> lessonIdList, int categoryId, int phaseId, int stageId)
        {
            return categoryAccess.AddLesson(lessonIdList, categoryId, phaseId, stageId);
        }

        public object RemoveLessonCategory(int lessonCategoryId)
        {
            return categoryAccess.RemoveLessonCategory(lessonCategoryId);
        }

        

        public List<Category> List()
        {
            return categoryAccess.List();
        }

        public Category Details(int id)
        {
            return categoryAccess.Details(id);
        }

        public Category CategoryDetailPartialView(int ProgramcategoryId)
        {
            return categoryAccess.CategoryDetailPartialView(ProgramcategoryId);
        }
        public Category CategoryDetail(int ProgramCategoryDetail)
        {
            return categoryAccess.CategoryDetail(ProgramCategoryDetail);
        }

        public bool Add(Category category)
        {
            category.Status = "Active";
            category.RevisionNo = 1;
            return categoryAccess.Add(category);
        }

        public bool Revise(Category category)
        {
            Category cat = (Category)categoryAccess.Details(category.CategoryId);
            return categoryAccess.Revise(cat);
        }

        public bool Delete(int id)
        {
            return categoryAccess.Delete(id);
        }
    }
}