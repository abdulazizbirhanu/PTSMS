using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace PTSMSBAL.Curriculum.Operations
{
    public class LessonLogic
    {
        LessonAccess lessonAccess = new LessonAccess();
        
        // relation methods
        public object ListExam(int lessonId)
        {
            return lessonAccess.ListExam(lessonId);
        }

        public object AddExam(List<string> examIdList, int lessonId)
        {
            return lessonAccess.AddExam(examIdList, lessonId);
        }

        public object RemoveLessonEvaluationTemplate(int lessonEvaluationTemplateId)
        {
            return lessonAccess.RemoveLessonEvaluationTemplate(lessonEvaluationTemplateId);
        }


        public object List()
        {
            return lessonAccess.List();
        }
        public List<Lesson> ListLessonByCategoryTypeId(int programCategoryId)
        {
            return lessonAccess.ListLessonByCategoryTypeId(programCategoryId);
        }

        

        public object Details(int id)
        {
            Lesson lesson = (Lesson)lessonAccess.Details(id);

            PTSContext db = new PTSContext();

            var lessonReferences = db.LessonReferences.Where(lr=>lr.LessonId == lesson.LessonId).ToList();

            List<SelectListItem> fileNameList = new List<SelectListItem>();

            foreach (var reference in lessonReferences)
            {
                fileNameList.Add(new SelectListItem { Text = reference.FileName, Value = reference.Lesson.LessonId.ToString() });
            }
            lesson.LessonReferenceFiles = fileNameList;
            return lesson;
        }

        public Lesson LessonDetail(int lessoncategoryId)
        {
            Lesson lesson = (Lesson)lessonAccess.LessonDetail(lessoncategoryId);

            PTSContext db = new PTSContext();

            var lessonReferences = db.LessonReferences.Where(lr => lr.LessonId == lesson.LessonId).ToList();

            List<SelectListItem> fileNameList = new List<SelectListItem>();

            foreach (var reference in lessonReferences)
            {
                fileNameList.Add(new SelectListItem { Text = reference.FileName, Value = reference.Lesson.LessonId.ToString() });
            }
            lesson.LessonReferenceFiles = fileNameList;
            return lesson;
        }
        
        public object Add(Lesson lesson,IEnumerable<HttpPostedFileBase> LessonReferenceFiles)
        {
            lesson.Status = "Active";
            lesson.RevisionNo = 1;
            return lessonAccess.Add(lesson, LessonReferenceFiles, "");
        }

        public object Revise(Lesson lesson, IEnumerable<HttpPostedFileBase> LessonReferenceFiles)
        {
            Lesson less = (Lesson)lessonAccess.Details(lesson.LessonId);

            string lessonName = less.LessonName;


            less.Status = "Replaced";

            lesson.RevisionNo = less.RevisionNo + 1;
            lesson.Status = "Active";

            if (less.RevisionGroupId == null)
                lesson.RevisionGroupId = lesson.LessonId;
            else
                lesson.RevisionGroupId = less.RevisionGroupId;

            lessonAccess.Revise(less);

            return lessonAccess.Add(lesson, LessonReferenceFiles, lessonName);
        }

        public object Delete(int id)
        {
            return lessonAccess.Delete(id);
        }
        public void FilesView(string lessonName, string FileName, ref string filePath)
        {
            var targetPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/");
            string targetPath2 = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Data/LessonReference/" + lessonName + "/");

            if (System.IO.Directory.Exists(targetPath2))
            {
                filePath = targetPath2 + FileName;
            }
            else
            {
                filePath = targetPath + FileName;
            }
        }

    }
}