using PTSMSDAL.Models.Curriculum.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Curriculum.View
{
    public class ProgramHierarchy
    {
        public ProgramHierarchy()
        {
            this.CategoryList = new List<CategoryView>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get { return "ProgramHierarchy"; } }
        public List<CategoryView> CategoryList { get; set; }
    }

    public class CategoryView
    {
        public CategoryView()
        {
            this.LessonList = new List<LessonsView>();
            this.CourseList = new List<CourseView>();
        }
        public string Type { get { return "CategoryView"; } }
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryType { get; set; }        
        public List<CourseView> CourseList { get; set; }
        public List<LessonsView> LessonList { get; set; }
    }
    public class CourseView
    {
        public CourseView()
        {
            this.ModuleList = new List<ModuleView>();
            this.CourseExamList = new List<CourseExamView>();
            this.PrerequisiteList = new List<PrerequisiteView>();
        }
        public string Type { get { return "CourseView"; } }
        public int Id { get; set; }
        public string Name { get; set; }       
        public List<ModuleView> ModuleList { get; set; }
        public List<CourseExamView> CourseExamList { get; set; }
        public List<PrerequisiteView> PrerequisiteList { get; set; }
    }
    public class LessonsView
    {
        public LessonsView()
        {
            this.LossonExamList = new List<LessonExamView>();
            this.LessonEvaluationTemplateViewList = new List<LessonEvaluationTemplateView>();
        }
        public string Type { get { return "LessonView"; } }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<LessonExamView> LossonExamList { get; set; }
        public List<LessonEvaluationTemplateView> LessonEvaluationTemplateViewList { get; set; }
        //LessonEvaluationTemplateView
    }
    public class ModuleView
    {
        public ModuleView()
        {
            this.ModuleExamList = new List<ModuleExamView>();
            this.ModuleGroundLessonList = new List<ModuleGroundLessonView>(); 
        }
        public string Type { get { return "ModuleView"; } }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ModuleExamView> ModuleExamList { get; set; }
        public List<ModuleGroundLessonView> ModuleGroundLessonList { get; set; }
    }
    public class CourseExamView
    {
        public string Type { get { return "CourseExamView"; } }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class PrerequisiteView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get { return "PrerequisiteView"; } }
    }
    public class LessonExamView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get { return "LessonExamView"; } }
    }
    public class ModuleExamView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get { return "ModuleExamView"; } }
    }
    public class ModuleGroundLessonView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get { return "ModuleGroundLessonView"; } }
    }
    //LessonEvaluationTemplate
    public class LessonEvaluationTemplateView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get { return "LessonEvaluationTemplateView"; } }
    }
}
