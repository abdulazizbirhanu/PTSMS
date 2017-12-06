using PTSMSDAL.Access.Grading;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Grading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PTSMSBAL.Grading
{
    public class GradingLogic
    {
        GradingAccess gradingAccess = new GradingAccess();
        public object FindTrainneForCourse(GradingFilterBO gradingFilterBO)
        {
            return gradingAccess.FindTrainneForCourse(gradingFilterBO);
        }
        public object FindTrainneCourse(GradingFilterBO gradingFilterBO)
        {
            return gradingAccess.FindTrainneCourse(gradingFilterBO);
        }
        public object FindTrainneForModule(GradingFilterBO gradingFilterBO)
        {
            return gradingAccess.FindTrainneForModule(gradingFilterBO);
        }


        public bool CalculateCourseScore(string traineeId,string traineeCourseId)
        {
            try
            {

                var result = gradingAccess.CalculateCourseScore(traineeId, int.Parse(traineeCourseId));
                var value = 0.0;
                int examcount = 0;
                bool isSuccess = false;
                foreach (var item in result)
                {
                    if (item.Grade >= item.PassingMark)
                    {
                        examcount = examcount + 1;
                        //value = value + ((item.Grade / item.Weight) * 100);
                        item.CourseScore = (value / examcount) * (item.CourseWeight) / 100;
                        if (item.CourseScore > item.CourseWeight)
                        {
                            item.CourseScore = -1;
                        }
                        double totalScore = item.CourseScore + item.ModuleScore;
                        isSuccess= gradingAccess.insertCourseScore(item.CourseId, item.TraineeCourseId, item.TraineeCategoryId, item.CourseScore, totalScore);
                    }
                }
                return isSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CalculateModuleScore(string traineeId)
        {
            try
            {
                var result = gradingAccess.CalculateModuleScore(traineeId);
                var value=0.0;
                int examcount = 0;
                bool isSuccess = false;
                foreach (var item in result)
                {
                    if (item.Grade <= item.PassingMark)
                    {
                        examcount = examcount + 1;
                        //value = value + ((item.Grade / item.Weight) * 100);
                        item.ModuleScore = (value / examcount) * (item.ModuleWeight) / 100;
                        if (item.ModuleScore > item.ModuleWeight)
                        {
                            item.ModuleScore = -1;
                        }
                        double totalScore = item.CourseScore + item.ModuleScore;
                        isSuccess = gradingAccess.insertModuleScore(item.CourseId, item.TraineeCourseId, item.TraineeCategoryId, item.ModuleScore, totalScore);
                    }
                }
                return isSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SaveCource(int traineeCourseId, int examId, int value, float trainee, float exam)
        {
            return gradingAccess.SaveCource(traineeCourseId, examId, value, trainee, exam);
        }





        public bool SaveModuleExam(int traineeModuleId, int examId, float? value,int? passFailExamResult)
        {
            return gradingAccess.SaveModuleExam(traineeModuleId, examId, value, passFailExamResult);
        }

        public bool SaveCourseExam(int traineeCourseId, int examId, float? value,int? passFailExamResult)
        {
            return gradingAccess.SaveCourseExam(traineeCourseId, examId, value, passFailExamResult);
        }

        public List<CourseExam> CourseExamList(int courseid)
        {
            return gradingAccess.CourseExamList(courseid);
            //throw new NotImplementedException();
        }

        public List<ModuleExam> ModuleExamList(int moduleid)
        {
            return gradingAccess.ModuleExamList(moduleid);
        }

        public List<GradingFilterList> CourseStudentsList(int examID, int batchClassID, int courseID)
        {
            return gradingAccess.CourseStudentsList(examID, batchClassID, courseID);
        }

        public List<GradingFilterList> ModuleStudentsList(int examID, int batchClassID, int moduleID)
        {
            return gradingAccess.ModuleStudentsList(examID, batchClassID, moduleID);
        }

        public List<GradingFilterList> ReModuleStudentsList(int examID, int batchClassID, int moduleID)
        {
            return gradingAccess.ReModuleStudentsList(examID, batchClassID, moduleID);
        }

        public List<GradingFilterList> ReCourseStudentsList(int examID, int batchClassID, int courseID)
        {
            return gradingAccess.ReCourseStudentsList(examID, batchClassID, courseID);
        }

        public bool ReSaveCourseExam(int traineeCourseId, int examId, float? grade,int ReExamCount,int? RePassFailExamResult)
        {
            return gradingAccess.ReSaveCourseExam(traineeCourseId, examId, grade, ReExamCount, RePassFailExamResult);
        }

        public bool ReSaveModuleExam(int traineeModuleId, int examId, float? grade,int ReExamCount,int? RePassFailExamResult)
        {
            return gradingAccess.ReSaveModuleExam(traineeModuleId, examId, grade, ReExamCount, RePassFailExamResult);
        }

        //public List<SelectListItem> PassFailExamResult()
        //{
        //    return gradingAccess.PassFailExamResult();
        //}
    }
}
