using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PTSMSDAL.Models.Grading
{
   public class GradingFilterBO
    {
        public bool IsCourse { get; set; }
        public bool IsModule { get; set; }
        public int BatchClassId { get; set; }
        public string CategoryName { get; set; }
        public int SelectedListID { get; set; }
        public int SelectedExam { get; set; }
        public List<GradingFilterList> GradingFilterList { get; set; }
    }
    public class GradingFilterList
    {
        public string TraineeID { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public int Exam { get; set; }
        public double Grade { get; set;}
        public int ReExamCount { get; set; }
        ///////
        public int TraineeCourseId { get; set; }
        public int TraineeModuleId { get; set; }
        public int CourseId { get; set; }
        public int ModuleId { get; set; }        
        public int? ExamId { get; set; }
        public string ExamName { get; set; }
        public int TraineeCourseExamId { get; set; }
        public int TraineeModuleExamId { get; set; }
        public float? Weight { get; set; }
       
        public float? PassingMark { get; set; }
        public int TraineeCategoryId { get; set; }
        public double CourseWeight { get; set; }
        public double ModuleWeight { get; set; }
        public double CourseScore { get; set; }
        public double ModuleScore { get; set; }
        public double TotalScore { get; set; }
        public List<SelectListItem> ExamInfo { get; set; }
        public List<SelectListItem> PassFailExamResult { get; set; }
        public bool IsPassFailExam { get; set; }
        public int? PassFailExamResultId { get; set; }
    }

    public class ExamInfo
    {
        public int? ExamId { get; set; }
        public string ExamName { get; set; }

    }
}
