using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Others.View
{
    public class CurriculumChangeView
    {
        public int CurriculumChangeId { get; set; }
        public int currentElementId { get; set; }// Actual Id,      
        public int ParentId { get; set; }//Hold actual relation id 
        public string Type { get; set; } //Program, Programcategory, CourseCategory, LessonCategory
        public string Name { get; set; }// Dispalyed Name
        public string Operation { get; set; }//Added, Delered, Revised - enum
        public int RevisionNo { get; set; }      
    }
    public enum CurriculumChangeOperation
    {
        Added,
        Revised,
        Deleted
    }

    public enum CurriculumChangeType
    {
        Program,
        Category,
        Course,
        Module,
        ModuleExam,
        CourseExam,
        Prerequisite,
        Lesson,
        EvaluationTemplate,
        EvaluationTemplateCategory,
        EvaluationCategoryItem
    }
}
