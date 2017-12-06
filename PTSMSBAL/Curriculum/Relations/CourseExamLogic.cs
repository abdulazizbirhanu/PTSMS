using System;
using System.Collections.Generic;
using PTSMSDAL.Access.Curriculum.Relations;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSBAL.Logic.Curriculum.Relations
{
    public class CourseExamLogic
    {
        CourseExamAccess courseExamAccess = new CourseExamAccess();

        public object List()
        {
            return courseExamAccess.List();
        }

        public object Details(int id)
        {
            return courseExamAccess.Details(id);
        }

        public object Add(CourseExam courseExam)
        {
            return courseExamAccess.Add(courseExam);
        }

        public object Revise(CourseExam courseExam)
        {
            return courseExamAccess.Revise(courseExam);
        }

        public object Delete(int id)
        {
            return courseExamAccess.Delete(id);
        }

        public List<CourseExam> CourseExamList(int courseId)
        {
            return courseExamAccess.CourseExamList(courseId);
        }
    }
}