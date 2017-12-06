using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;

namespace PTSMSBAL.Curriculum.Operations
{
    public class CourseLogic
    {
        CourseAccess courseAccess = new CourseAccess();

        public object ListModule(int courseId)
        {
            return courseAccess.ListModule(courseId);
        }

        public List<Module> ListModulesByCourseId(int courseId)
        {
            return courseAccess.ListModulesByCourseId(courseId);
        }

        public List<Module> ListModulesByProgram(int programId)
        {
            return courseAccess.ListModulesByProgram(programId);
        }

        public object AddModule(List<string> moduleIdList, int courseId, int phaseId)
        {
            return courseAccess.AddModule(moduleIdList, courseId, phaseId);
        }

        public object RemoveCourseModule(int courseModuleId)
        {
            return courseAccess.RemoveCourseModule(courseModuleId);
        }

        public object ListExam(int courseId)
        {
            return courseAccess.ListExam(courseId);
        }

        public object AddExam(List<string> examIdList, int courseId)
        {
            return courseAccess.AddExam(examIdList, courseId);
        }

        public object RemoveCourseExam(int courseExamId)
        {
            return courseAccess.RemoveCourseExam(courseExamId);
        }

        public object ListPrerequisite(int courseId)
        {
            return courseAccess.ListPrerequisite(courseId);
        }

        public object AddPrerequisite(int CourseId, List<string> PrerequisiteIdList)
        { 
            return courseAccess.AddPrerequisite(CourseId, PrerequisiteIdList);
        }

        public object RemovePrerequisite(int prerequisiteId)
        {
            return courseAccess.RemovePrerequisite(prerequisiteId);
        }



        public object List()
        {
            return courseAccess.List();
        }

        public object Details(int id)
        {
            return courseAccess.Details(id);
        }
        public Course CourseDetail(int courseCategoryId)
        {
            return courseAccess.CourseDetail(courseCategoryId);
        }

        public object Add(Course course)
        {
            course.Status = "Active";
            course.RevisionNo = 1;
            return courseAccess.Add(course);
        }

        public object Revise(Course course)
        {
            Course cour = (Course)courseAccess.Details(course.CourseId);
            cour.Status = "Replaced";
            cour.CourseCode = cour.CourseCode + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss");

            course.RevisionNo = cour.RevisionNo + 1;
            course.Status = "Active";

            if (cour.RevisionGroupId == null)
                course.RevisionGroupId = course.CourseId;
            else
                course.RevisionGroupId = cour.RevisionGroupId;

            courseAccess.Revise(cour);

            return courseAccess.Add(course);
        }

        public object Delete(int id)
        {
            return courseAccess.Delete(id);
        }

        /*Nice*/
        public object FilteredCourceExam(int CourseId)
        {
            return courseAccess.FilteredCourceExam(CourseId);
        }

        public object FilteredListCourse(int BatchClassId)
        {
            return courseAccess.FilteredListCourse(BatchClassId);
        }
        /*Nice*/
    }
}