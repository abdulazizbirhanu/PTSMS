using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Access.Grading;
using PTSMSDAL.Models.Grading;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Relations;
using System.Web.Mvc;
using System.Data.Entity;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSDAL.Access.Grading
{
    public class GradingAccess
    {
        private PTSContext db = new PTSContext();

        public List<GradingFilterList> FindTrainneForModule(GradingFilterBO gradingFilterBO)
        {

            var query =
           from rc in db.BatchClasses
           join c in db.TraineeBatchClasses on rc.BatchClassId equals c.BatchClassId /**/
                                                                                     // where rc.BatchClassId == gradingFilterBO.BatchClassId
           join r in db.Trainees on c.TraineeId equals r.TraineeId
           join pers in db.Persons on r.TraineeId equals pers.PersonId
           join sy in db.TraineeBatchClasses on r.TraineeId equals sy.TraineeId
           join trC in db.BatchCategories on rc.BatchId equals trC.BatchId
           join trCours in db.TraineeCourses on trC.BatchCategoryId equals trCours.BatchCategoryId
           // join course in db.Courses on trCours.CourseId equals course.CourseId

           //join Cmodul in db.CourseModules on course.CourseId equals Cmodul.CourseCategory.CourseId /**/

           join trM in db.TraineeModules on trCours.TraineeCourseId equals trM.TraineeCourseId
           //join modul in db.Modules on trM.ModuleId equals modul.ModuleId
           //join Cmodul in db.CourseModules on modul.ModuleId equals Cmodul.ModuleId /**/
           // where Cmodul.ModuleId == gradingFilterBO.SelectedListID
           join tMEx in db.TraineeModuleExams on trM.TraineeModuleId equals tMEx.TraineeModuleId into trainexam
           from tMEx in trainexam.DefaultIfEmpty()
           join Ex in db.Exams on tMEx.ExamId equals Ex.ExamId into exam
           from Ex in exam.DefaultIfEmpty()
               // where rc.BatchClassId == gradingFilterBO.BatchClassId && modul.ModuleId == gradingFilterBO.SelectedListID && Ex.ExamId == gradingFilterBO.SelectedExam
           where rc.BatchClassId == gradingFilterBO.BatchClassId && trM.ModuleId == gradingFilterBO.SelectedListID &&
             (Ex.RevisionGroupId == null ? Ex.ExamId : Ex.RevisionGroupId) == gradingFilterBO.SelectedExam

           select new GradingFilterList
           {
               TraineeID = pers.CompanyId,
               FullName = pers.FirstName + " " + pers.MiddleName + " " + pers.LastName,

               TraineeModuleId = trM.TraineeModuleId,
               Code = trM.Module.ModuleCode + "-" + trM.Module.ModuleTitle,

               ModuleId = trM.Module.ModuleId,
               Grade = tMEx.ExamScore,
               ExamId = tMEx.ExamId,
               ExamName = Ex.Name,
           };

            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var item in query)
            {
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = item.TraineeID;
                gradingFilterList.FullName = item.FullName;
                gradingFilterList.TraineeModuleId = item.TraineeModuleId;
                gradingFilterList.Code = item.Code;
                gradingFilterList.ModuleId = item.ModuleId;
                gradingFilterList.Grade = item.Grade;
                gradingFilterList.ExamId = item.ExamId;
                gradingFilterList.ExamName = item.ExamName;

                list.Add(gradingFilterList);
            }

            return list;

        }

        public bool insertModuleScore(int CourseId, int TraineeCourseId, int batchCategoryId, double ModuleScore, double totalScore)
        {
            try
            {
                TraineeCourse result = db.TraineeCourses.Where(x => x.CourseId == CourseId && x.TraineeCourseId == TraineeCourseId && x.BatchCategoryId == batchCategoryId).Single();
                result.ModuleScore = ModuleScore;
                result.TotalScore = totalScore;
                return db.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool insertCourseScore(int CourseId, int TraineeCourseId, int batchCategoryId, double CourseScore, double totalScore)
        {
            try
            {
                TraineeCourse result = db.TraineeCourses.Where(x => x.CourseId == CourseId && x.TraineeCourseId == TraineeCourseId && x.BatchCategoryId == batchCategoryId).Single();
                result.CourseScore = CourseScore;
                result.TotalScore = totalScore;
                return db.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public List<GradingFilterList> CalculateCourseScore(string traineeId, int traineeCourseId)
        {
            var query =
                from per in db.Persons
                join trainee in db.Trainees on per.PersonId equals trainee.TraineeId
                where per.CompanyId == traineeId
                join tcource in db.TraineeCourses on traineeCourseId equals tcource.TraineeCourseId
                join cau in db.Courses on tcource.CourseId equals cau.CourseId
                join tcexam in db.TraineeCourseExams on tcource.TraineeCourseId equals tcexam.TraineeCourseId
                where tcource.TraineeCourseId == traineeCourseId
                join exam in db.Exams on tcexam.ExamId equals exam.ExamId

                select new
                {
                    per.CompanyId,
                    cau.CourseId,
                    tcource.TraineeCourseId,
                    tcource.CourseWeight,
                    tcexam.TraineeCourseExamId,
                    tcource.BatchCategoryId,
                    exam.ExamId,
                    tcexam.ExamScore,
                    exam.Weight,
                    exam.PassingMark,
                    tcource.CourseScore,
                    tcource.ModuleScore



                };
            //var grouped=  query.GroupBy(g => g.CompanyId);
            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var item in query)
            {
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = item.CompanyId;
                gradingFilterList.TraineeCategoryId = item.BatchCategoryId;
                gradingFilterList.TraineeCourseExamId = item.TraineeCourseExamId;
                gradingFilterList.TraineeCourseId = item.TraineeCourseId;
                gradingFilterList.Weight = item.Weight;
                gradingFilterList.Grade = item.ExamScore;
                gradingFilterList.ExamId = item.ExamId;
                gradingFilterList.CourseId = item.CourseId;
                gradingFilterList.CourseWeight = item.CourseWeight;
                gradingFilterList.PassingMark = item.PassingMark;
                gradingFilterList.ModuleScore = item.ModuleScore;
                gradingFilterList.CourseScore = item.CourseScore;



                list.Add(gradingFilterList);
            }

            return list;
        }


        public List<GradingFilterList> CalculateModuleScore(string traineeId)
        {
            var query =
                from per in db.Persons
                join trainee in db.Trainees on per.PersonId equals trainee.TraineeId
                where per.CompanyId == traineeId
                join tbat in db.TraineeBatchClasses on trainee.TraineeId equals tbat.TraineeId
                join tcat in db.BatchCategories on tbat.BatchClass.BatchId equals tcat.BatchId
                join tcource in db.TraineeCourses on tcat.BatchCategoryId equals tcource.BatchCategoryId
                join cau in db.Courses on tcource.CourseId equals cau.CourseId
                join tmodule in db.TraineeModules on tcource.TraineeCourseId equals tmodule.TraineeCourseId
                join module in db.Modules on tmodule.ModuleId equals module.ModuleId
                join tmexam in db.TraineeModuleExams on tmodule.TraineeModuleId equals tmexam.TraineeModuleId
                join exam in db.Exams on tmexam.ExamId equals exam.ExamId

                select new
                {
                    per.CompanyId,
                    cau.CourseId,
                    tcource.TraineeCourseId,
                    tmexam.TraineeModuleExamId,
                    tcat.BatchCategoryId,
                    exam.ExamId,
                    tcource.ModuleWeight,
                    module.ModuleId,
                    tmexam.ExamScore,
                    exam.Weight,
                    exam.PassingMark,
                    tcource.CourseScore,
                    tcource.ModuleScore



                };
            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var item in query)
            {
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = item.CompanyId;
                gradingFilterList.CourseId = item.CourseId;
                gradingFilterList.TraineeModuleExamId = item.TraineeModuleExamId;
                gradingFilterList.TraineeCategoryId = item.BatchCategoryId;
                gradingFilterList.ModuleId = item.ModuleId;
                gradingFilterList.TraineeCourseId = item.TraineeCourseId;
                gradingFilterList.Weight = item.Weight;
                gradingFilterList.Grade = item.ExamScore;
                gradingFilterList.ExamId = item.ExamId;
                gradingFilterList.PassingMark = item.PassingMark;
                gradingFilterList.ModuleWeight = item.ModuleWeight;
                gradingFilterList.ModuleScore = item.ModuleScore;
                gradingFilterList.CourseScore = item.CourseScore;

                list.Add(gradingFilterList);
            }

            return list;
        }

        public bool SaveCource(int CourseId, int TraineeCourseId, int batchCategoryId, float ModuleScoure, float courceScoure)
        {
            try
            {
                TraineeCourse result = db.TraineeCourses.Where(x => x.CourseId == CourseId && x.TraineeCourseId == TraineeCourseId && x.BatchCategoryId == batchCategoryId).Single();
                result.CourseScore = courceScoure;
                result.ModuleScore = ModuleScoure;
                result.TotalScore = courceScoure + ModuleScoure;
                if (db.SaveChanges() > 0)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }


        public List<GradingFilterList> FindTrainneForCourse(GradingFilterBO gradingFilterBO)
        {
            var query =
            from rc in db.BatchClasses
            join c in db.TraineeBatchClasses on rc.BatchClassId equals c.BatchClassId
            join r in db.Trainees on c.TraineeId equals r.TraineeId
            join tCat in db.BatchCategories on rc.BatchId  equals tCat.BatchId
            join trCours in db.TraineeCourses on tCat.BatchCategoryId equals trCours.BatchCategoryId
            join course in db.Courses on trCours.CourseId equals course.CourseId
            join trEx in db.TraineeCourseExams on trCours.TraineeCourseId equals trEx.TraineeCourseId/*for left outer join*/ into trainexam
            from trEx in trainexam.DefaultIfEmpty()
            join Ex in db.Exams on trEx.ExamId equals Ex.ExamId /*for left outer join*/ into exam
            from Ex in exam.DefaultIfEmpty()
            where
            rc.BatchClassId == gradingFilterBO.BatchClassId && (course.RevisionGroupId == null ? course.CourseId :
            course.RevisionGroupId) == gradingFilterBO.SelectedListID &&
            (Ex.RevisionGroupId == null ? Ex.ExamId : Ex.RevisionGroupId) == gradingFilterBO.SelectedExam
            select new GradingFilterList
            {
                TraineeID = r.Person.CompanyId,
                FullName = r.Person.FirstName + " " + r.Person.MiddleName + " " + r.Person.LastName,

                TraineeCourseId = trCours.TraineeCourseId,
                Code = course.CourseCode + "-" + course.CourseTitle,

                CourseId = course.CourseId,
                Grade = trEx.ExamScore,
                ExamId = trEx.ExamId,
                ExamName = Ex.Name,
            };

            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var item in query)
            {
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = item.TraineeID;
                gradingFilterList.FullName = item.FullName;
                gradingFilterList.TraineeCourseId = item.TraineeCourseId;
                gradingFilterList.Code = item.Code;
                gradingFilterList.CourseId = item.CourseId;
                gradingFilterList.ExamId = item.ExamId;
                gradingFilterList.ExamName = item.ExamName;
                gradingFilterList.Grade = item.Grade;

                list.Add(gradingFilterList);
            }

            return list;
        }
        public List<GradingFilterList> FindTrainneCourse(GradingFilterBO gradingFilterBO)
        {
            var query =
            from rc in db.BatchClasses
            join c in db.TraineeBatchClasses on rc.BatchClassId equals c.BatchClassId
            join r in db.Trainees on c.TraineeId equals r.TraineeId
            join pers in db.Persons on r.TraineeId equals pers.PersonId
            join tCat in db.BatchCategories on rc.BatchId equals tCat.BatchId
            join trCours in db.TraineeCourses on tCat.BatchCategoryId equals trCours.BatchCategoryId
            join course in db.Courses on trCours.CourseId equals course.CourseId
            join trEx in db.TraineeCourseExams on trCours.TraineeCourseId equals trEx.TraineeCourseId/*for left outer join*/ into trainexam
            from trEx in trainexam.DefaultIfEmpty()
            join Ex in db.Exams on trEx.ExamId equals Ex.ExamId /*for left outer join*/ into exam
            from Ex in exam.DefaultIfEmpty()
            where rc.BatchClassId == gradingFilterBO.BatchClassId && (course.RevisionGroupId == null ? course.CourseId : course.RevisionGroupId) == gradingFilterBO.SelectedListID

            select new GradingFilterList
            {
                TraineeID = pers.CompanyId,
                TraineeCourseId = trCours.TraineeCourseId,
                FullName = pers.FirstName + " " + pers.MiddleName + " " + pers.LastName,
                Code = course.CourseCode + "-" + course.CourseTitle,
                CourseId = course.CourseId,
                CourseScore = trCours.CourseScore,
                ModuleScore = trCours.ModuleScore,
                TotalScore = trCours.TotalScore,
                TraineeCategoryId = tCat.BatchCategoryId

            };

            var groupedQuery = query.GroupBy(x => new { x.TraineeID }).Select(grp => grp.ToList()).ToList();

            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var grade in groupedQuery)
            {
                GradingFilterList grading = grade.FirstOrDefault();
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = grading.TraineeID;
                gradingFilterList.FullName = grading.FullName;

                gradingFilterList.Code = grading.Code;
                gradingFilterList.CourseId = grading.CourseId;
                gradingFilterList.CourseScore = grading.CourseScore;
                gradingFilterList.ModuleScore = grading.ModuleScore;
                gradingFilterList.TotalScore = grading.TotalScore;
                gradingFilterList.TraineeCourseId = grading.TraineeCourseId;
                gradingFilterList.TraineeCategoryId = grading.TraineeCategoryId;

                list.Add(gradingFilterList);
            }

            return list;
        }
        public List<ModuleExam> ModuleExamList(int moduleid)
        {
            var examList = db.ModuleExams.Where(item => item.CourseModule.ModuleId == moduleid).Distinct();
            return examList.ToList();
        }
        public List<CourseExam> CourseExamList(int courseid)
        {
            var examList = db.CourseExams.Where(item => item.CourseCategory.CourseId == courseid).Distinct();
            return examList.ToList();
        }

        public bool SaveCourseExam(int traineeCourseID, int examID, float? value, int? passFailExamResult)
        {
            try
            {
                TraineeCourseExam result = db.TraineeCourseExams.Where(x => x.TraineeCourseId == traineeCourseID && x.ExamId == examID).Single();
                if (result.Exam.IsPassFailExam)
                {
                    result.ExamScore = 0;
                    result.PassFailExamResultId = passFailExamResult;
                }
                else
                {
                    result.ExamScore = (float)value;
                    result.PassFailExamResultId = null;
                }
                if (result != null)
                {
                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SaveModuleExam(int traineeModuleId, int examId, float? value, int? passFailExamResult)
        {
            try
            {
                //TraineeModuleExam result = db.TraineeModuleExams.Where(x => x.TraineeModuleId == traineeModuleId && x.ExamId == examId && x.ReExamCount == 0).Single();
                var resultList = db.TraineeModuleExams.Where(x => x.TraineeModuleId == traineeModuleId && x.ExamId == examId).ToList();
                TraineeModuleExam result = resultList.Where(x => x.ReExamCount == resultList.Max(y => y.ReExamCount)).FirstOrDefault();
                //result.ReExamCount += 1;

                if (result.Exam.IsPassFailExam)
                {
                    result.ExamScore = 0;
                    result.PassFailExamResultId = passFailExamResult;
                }
                else
                {
                    result.ExamScore = (float)value;
                    result.PassFailExamResultId = null;
                }

                if (result != null)
                {
                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool ReSaveModuleExam(int traineeModuleId, int examId, float? grade, int ReExamCount, int? RePassFailExamResult)
        {
            try
            {
                //TraineeModuleExam result = db.TraineeModuleExams.Where(x => x.TraineeModuleId == traineeModuleId && x.ExamId == examId && x.ReExamCount == 0).Single();
                var resultList = db.TraineeModuleExams.Where(x => x.TraineeModuleId == traineeModuleId && x.ExamId == examId).ToList();
                TraineeModuleExam result = resultList.Where(x => x.ReExamCount == resultList.Max(y => y.ReExamCount)).FirstOrDefault();
                result.ReExamCount += 1;

                if (result.Exam.IsPassFailExam)
                {
                    result.ExamScore = 0;
                    result.PassFailExamResultId = RePassFailExamResult;
                }
                else
                {
                    result.ExamScore = (float)grade;
                    result.PassFailExamResultId = null;
                }

                if (result != null)
                {
                    db.TraineeModuleExams.Add(result);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ReSaveCourseExam(int traineeCourseId, int examId, float? grade, int ReExamCount, int? RePassFailExamResult)
        {
            try
            {
                //TraineeCourseExam result = db.TraineeCourseExams.Where(x => x.TraineeCourseId == traineeCourseId && x.ExamId == examId).Single();
                //result.ReExamCount = ReExamCount + 1;
                var resultList = db.TraineeCourseExams.Where(x => x.TraineeCourseId == traineeCourseId && x.ExamId == examId).ToList();
                TraineeCourseExam result = resultList.Where(x => x.ReExamCount == resultList.Max(y => y.ReExamCount)).FirstOrDefault();
                result.ReExamCount += 1;

                if (result.Exam.IsPassFailExam)
                {
                    result.ExamScore = 0;
                    result.PassFailExamResultId = RePassFailExamResult;
                }
                else
                {
                    result.ExamScore = (float)grade;
                    result.PassFailExamResultId = null;
                }

                if (result != null)
                {
                    db.TraineeCourseExams.Add(result);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<GradingFilterList> ModuleStudentsList(int examID, int batchClassID, int moduleID)
        {

            var query =
           from rc in db.BatchClasses
           join c in db.TraineeBatchClasses on rc.BatchClassId equals c.BatchClassId
           join r in db.Trainees on c.TraineeId equals r.TraineeId
           join pers in db.Persons on r.TraineeId equals pers.PersonId
           join tCat in db.BatchCategories on c.BatchClass.BatchId equals tCat.BatchId
           join trCours in db.TraineeCourses on tCat.BatchCategoryId equals trCours.BatchCategoryId
           join trM in db.TraineeModules on trCours.TraineeCourseId equals trM.TraineeCourseId
           join tMEx in db.TraineeModuleExams on trM.TraineeModuleId equals tMEx.TraineeModuleId into trainexam
           from tMEx in trainexam.DefaultIfEmpty()

           join mark in db.PassFailExamResults on tMEx.PassFailExamResultId equals mark.PassFailExamResultId into markExam
           from mark in markExam.DefaultIfEmpty()

           join Ex in db.Exams on tMEx.ExamId equals Ex.ExamId into exam
           from Ex in exam.DefaultIfEmpty()
           where rc.BatchClassId == batchClassID && trM.ModuleId == moduleID &&
             (Ex.RevisionGroupId == null ? Ex.ExamId : Ex.RevisionGroupId) == examID

           select new GradingFilterList
           {
               TraineeID = pers.CompanyId,
               FullName = pers.FirstName + " " + pers.MiddleName + " " + pers.LastName,

               TraineeModuleId = trM.TraineeModuleId,
               Code = trM.Module.ModuleCode + "-" + trM.Module.ModuleTitle,
               ReExamCount = tMEx.ReExamCount,
               ModuleId = trM.Module.ModuleId,
               Grade = tMEx.ExamScore,
               ExamId = tMEx.ExamId,
               ExamName = Ex.Name,
               IsPassFailExam = Ex.IsPassFailExam,
               PassFailExamResultId = mark.PassFailExamResultId
           };

            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var item in query)
            {
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = item.TraineeID;
                gradingFilterList.FullName = item.FullName;
                gradingFilterList.TraineeModuleId = item.TraineeModuleId;
                gradingFilterList.Code = item.Code;
                gradingFilterList.ModuleId = item.ModuleId;
                gradingFilterList.ExamId = item.ExamId;
                gradingFilterList.ExamName = item.ExamName;
                gradingFilterList.ReExamCount = item.ReExamCount;
                gradingFilterList.IsPassFailExam = item.IsPassFailExam;

                if (!item.IsPassFailExam)
                {
                    gradingFilterList.Grade = item.Grade;
                    gradingFilterList.PassFailExamResult = new List<SelectListItem>()
                    { new SelectListItem() { Text="Select",Value=null} };
                }
                else
                {
                    gradingFilterList.Grade = 0;
                    gradingFilterList.PassFailExamResultId = item.PassFailExamResultId;
                    gradingFilterList.PassFailExamResult = PassFailExamResult(item.PassFailExamResultId);
                }

                list.Add(gradingFilterList);
            }

            var listGp2 = list.GroupBy(item => item.TraineeModuleId);
            list = new List<GradingFilterList>();
            foreach (var item in listGp2)
            {
                var listItems = item.ToList();
                var selected2 = listItems.Where(x => x.ReExamCount == item.Max(y => y.ReExamCount)).First();
                list.Add(selected2);
            }

            return list;
        }
        public List<GradingFilterList> CourseStudentsList(int examID, int batchClassID, int courseID)
        {
            var query =
            from rc in db.BatchClasses
            join c in db.TraineeBatchClasses on rc.BatchClassId equals c.BatchClassId
            join r in db.Trainees on c.TraineeId equals r.TraineeId
            join pers in db.Persons on r.TraineeId equals pers.PersonId
            join tCat in db.BatchCategories on rc.BatchId equals tCat.BatchId
            join trCours in db.TraineeCourses on tCat.BatchCategoryId equals trCours.BatchCategoryId
            join course in db.Courses on trCours.CourseId equals course.CourseId
            join trEx in db.TraineeCourseExams on trCours.TraineeCourseId equals trEx.TraineeCourseId/*for left outer join*/ into trainexam
            from trEx in trainexam.DefaultIfEmpty()
            join Ex in db.Exams on trEx.ExamId equals Ex.ExamId /*for left outer join*/ into exam
            from Ex in exam.DefaultIfEmpty()


            join mark in db.PassFailExamResults on trEx.PassFailExamResultId equals mark.PassFailExamResultId into markExam
            from mark in markExam.DefaultIfEmpty()

            where
            rc.BatchClassId == batchClassID && (course.RevisionGroupId == null ? course.CourseId :
            course.RevisionGroupId) == courseID &&
            (Ex.RevisionGroupId == null ? Ex.ExamId : Ex.RevisionGroupId) == examID
            select new GradingFilterList
            {
                TraineeID = pers.CompanyId,
                FullName = pers.FirstName + " " + pers.MiddleName + " " + pers.LastName,

                TraineeCourseId = trCours.TraineeCourseId,
                Code = course.CourseCode + "-" + course.CourseTitle,
                ReExamCount = trEx.ReExamCount,

                CourseId = course.CourseId,
                Grade = trEx.ExamScore,
                ExamId = trEx.ExamId,
                ExamName = Ex.Name,
                IsPassFailExam = Ex.IsPassFailExam,
                PassFailExamResultId = trEx.PassFailExamResultId
            };

            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var item in query)
            {
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = item.TraineeID;
                gradingFilterList.FullName = item.FullName;
                gradingFilterList.TraineeCourseId = item.TraineeCourseId;
                gradingFilterList.Code = item.Code;
                gradingFilterList.CourseId = item.CourseId;
                gradingFilterList.ExamId = item.ExamId;
                gradingFilterList.ExamName = item.ExamName;
                gradingFilterList.ReExamCount = item.ReExamCount;
                gradingFilterList.IsPassFailExam = item.IsPassFailExam;
                //gradingFilterList.PassFailExamResultId = item.PassFailExamResultId;

                if (!item.IsPassFailExam)
                {
                    gradingFilterList.Grade = item.Grade;
                    gradingFilterList.PassFailExamResult = new List<SelectListItem>()
                    { new SelectListItem() { Text="Select",Value=null} };
                }
                else
                {
                    gradingFilterList.Grade = 0;
                    gradingFilterList.PassFailExamResultId = item.PassFailExamResultId;
                    gradingFilterList.PassFailExamResult = PassFailExamResult(item.PassFailExamResultId);
                }


                list.Add(gradingFilterList);
            }


            var listGp2 = list.GroupBy(item => item.TraineeCourseId);
            list = new List<GradingFilterList>();
            foreach (var item in listGp2)
            {
                var listItems = item.ToList();
                var selected2 = listItems.Where(x => x.ReExamCount == item.Max(y => y.ReExamCount)).First();
                list.Add(selected2);
            }

            return list;
        }


        public List<GradingFilterList> ReModuleStudentsList(int examID, int batchClassID, int moduleID)
        {


            var query =
           from rc in db.BatchClasses
           join c in db.TraineeBatchClasses on rc.BatchClassId equals c.BatchClassId /**/
                                                                                     // where rc.BatchClassId == gradingFilterBO.BatchClassId
           join r in db.Trainees on c.TraineeId equals r.TraineeId
           join pers in db.Persons on r.TraineeId equals pers.PersonId
           join tCat in db.BatchCategories on rc.BatchId equals tCat.BatchId
           join trCours in db.TraineeCourses on tCat.BatchCategoryId equals trCours.BatchCategoryId
           // join course in db.Courses on trCours.CourseId equals course.CourseId

           //join Cmodul in db.CourseModules on course.CourseId equals Cmodul.CourseCategory.CourseId /**/

           join trM in db.TraineeModules on trCours.TraineeCourseId equals trM.TraineeCourseId
           //join modul in db.Modules on trM.ModuleId equals modul.ModuleId
           //join Cmodul in db.CourseModules on modul.ModuleId equals Cmodul.ModuleId /**/
           // where Cmodul.ModuleId == gradingFilterBO.SelectedListID
           join tMEx in db.TraineeModuleExams on trM.TraineeModuleId equals tMEx.TraineeModuleId into trainexam
           from tMEx in trainexam.DefaultIfEmpty()

           join mark in db.PassFailExamResults on tMEx.PassFailExamResultId equals mark.PassFailExamResultId into markExam
           from mark in markExam.DefaultIfEmpty()

           join Ex in db.Exams on tMEx.ExamId equals Ex.ExamId into exam
           from Ex in exam.DefaultIfEmpty()
               // where rc.BatchClassId == gradingFilterBO.BatchClassId && modul.ModuleId == gradingFilterBO.SelectedListID && Ex.ExamId == gradingFilterBO.SelectedExam
           where rc.BatchClassId == batchClassID && trM.ModuleId == moduleID &&
             (Ex.RevisionGroupId == null ? Ex.ExamId : Ex.RevisionGroupId) == examID &&
             tMEx.ExamScore < Ex.PassingMark// &&
                                            //(mark.ExamPassed == false || mark.ExamPassed == null)


           select new GradingFilterList
           {
               TraineeID = pers.CompanyId,
               FullName = pers.FirstName + " " + pers.MiddleName + " " + pers.LastName,

               TraineeModuleId = trM.TraineeModuleId,
               Code = trM.Module.ModuleCode + "-" + trM.Module.ModuleTitle,

               ModuleId = trM.Module.ModuleId,
               Grade = tMEx.ExamScore,
               ExamId = tMEx.ExamId,
               ExamName = Ex.Name,
               ReExamCount = tMEx.ReExamCount,
               IsPassFailExam = Ex.IsPassFailExam,
               PassFailExamResultId = tMEx.PassFailExamResultId
           };


            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var item in query)
            {
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = item.TraineeID;
                gradingFilterList.FullName = item.FullName;
                gradingFilterList.TraineeModuleId = item.TraineeModuleId;
                gradingFilterList.Code = item.Code;
                gradingFilterList.ModuleId = item.ModuleId;
                gradingFilterList.ReExamCount = item.ReExamCount;
                gradingFilterList.ExamId = item.ExamId;
                gradingFilterList.ExamName = item.ExamName;
                gradingFilterList.IsPassFailExam = item.IsPassFailExam;
                //gradingFilterList.PassFailExamResultId = item.PassFailExamResultId;

                if (!item.IsPassFailExam)
                {
                    gradingFilterList.Grade = item.Grade;
                    gradingFilterList.PassFailExamResult = new List<SelectListItem>()
                    { new SelectListItem() { Text="",Value=null} };
                }
                else
                {
                    gradingFilterList.Grade = 0;
                    gradingFilterList.PassFailExamResultId = item.PassFailExamResultId;
                    gradingFilterList.PassFailExamResult = PassFailExamResult(item.PassFailExamResultId);
                }

                list.Add(gradingFilterList);
            }

            db = new PTSContext();
            var passFailExamResultList = db.PassFailExamResults.Select(item => item).ToList();

            var listGp2 = list.GroupBy(item => item.TraineeModuleId);
            list = new List<GradingFilterList>();
            foreach (var item in listGp2)
            {
                var listItems = item.ToList();
                var selected2 = listItems.Where((x => x.ReExamCount == item.Max(y => y.ReExamCount))).First();

                if (selected2.IsPassFailExam) { 
                    foreach (var passFail in passFailExamResultList)
                    {
                        if ((selected2.PassFailExamResultId == passFail.PassFailExamResultId && passFail.ExamPassed == false) || selected2.PassFailExamResultId == null)
                        {
                            list.Add(selected2);
                            break;
                        }
                    }
                }
                else
                {
                    list.Add(selected2);
                }
            }

            return list;
        }
        public List<GradingFilterList> ReCourseStudentsList(int examID, int batchClassID, int courseID)
        {
            var query =
                      from rc in db.BatchClasses
                      join c in db.TraineeBatchClasses on rc.BatchClassId equals c.BatchClassId
                      join r in db.Trainees on c.TraineeId equals r.TraineeId
                      join pers in db.Persons on r.TraineeId equals pers.PersonId
                      join tCat in db.BatchCategories on rc.BatchId equals tCat.BatchCategoryId
                      join trCours in db.TraineeCourses on tCat.BatchCategoryId equals trCours.BatchCategoryId
                      join course in db.Courses on trCours.CourseId equals course.CourseId
                      join trEx in db.TraineeCourseExams on trCours.TraineeCourseId equals trEx.TraineeCourseId/*for left outer join*/ into trainexam
                      from trEx in trainexam.DefaultIfEmpty()
                      join Ex in db.Exams on trEx.ExamId equals Ex.ExamId /*for left outer join*/ into exam
                      from Ex in exam.DefaultIfEmpty()

                      join mark in db.PassFailExamResults on trEx.PassFailExamResultId equals mark.PassFailExamResultId into markExam
                      from mark in markExam.DefaultIfEmpty()

                      where
                      rc.BatchClassId == batchClassID && (course.RevisionGroupId == null ? course.CourseId :
                      course.RevisionGroupId) == courseID &&
                      (Ex.RevisionGroupId == null ? Ex.ExamId : Ex.RevisionGroupId) == examID &&
                        trEx.ExamScore < Ex.PassingMark &&
                        mark.ExamPassed == false

                      select new GradingFilterList
                      {
                          TraineeID = pers.CompanyId,
                          FullName = pers.FirstName + " " + pers.MiddleName + " " + pers.LastName,

                          TraineeCourseId = trCours.TraineeCourseId,
                          Code = course.CourseCode + "-" + course.CourseTitle,

                          CourseId = course.CourseId,
                          Grade = trEx.ExamScore,
                          ExamId = trEx.ExamId,
                          ExamName = Ex.Name,
                          ReExamCount = trEx.ReExamCount,

                          IsPassFailExam = Ex.IsPassFailExam,
                          PassFailExamResultId = trEx.PassFailExamResultId
                      };

            List<GradingFilterList> list = new List<GradingFilterList>();
            foreach (var item in query)
            {
                GradingFilterList gradingFilterList = new GradingFilterList();
                gradingFilterList.TraineeID = item.TraineeID;
                gradingFilterList.FullName = item.FullName;
                gradingFilterList.TraineeCourseId = item.TraineeCourseId;
                gradingFilterList.Code = item.Code;
                gradingFilterList.CourseId = item.CourseId;
                gradingFilterList.ExamId = item.ExamId;
                gradingFilterList.ExamName = item.ExamName;
                gradingFilterList.ReExamCount = item.ReExamCount;

                gradingFilterList.IsPassFailExam = item.IsPassFailExam;
                //gradingFilterList.PassFailExamResultId = item.PassFailExamResultId;
                //gradingFilterList.Grade = item.Grade;

                if (!item.IsPassFailExam)
                {
                    gradingFilterList.Grade = item.Grade;
                    gradingFilterList.PassFailExamResult = new List<SelectListItem>()
                    { new SelectListItem() { Text="Select",Value=null} };
                }
                else
                {
                    gradingFilterList.Grade = 0;
                    gradingFilterList.PassFailExamResultId = item.PassFailExamResultId;
                    gradingFilterList.PassFailExamResult = PassFailExamResult(item.PassFailExamResultId);
                }

                list.Add(gradingFilterList);
            }

            db = new PTSContext();
            var passFailExamResultList = db.PassFailExamResults.Select(item => item).ToList();

            var listGp2 = list.GroupBy(item => item.TraineeCourseId);
            list = new List<GradingFilterList>();
            foreach (var item in listGp2)
            {
                var listItems = item.ToList();
                var selected2 = listItems.Where(x => x.ReExamCount == item.Max(y => y.ReExamCount)).First();

                foreach (var passFail in passFailExamResultList)
                {
                    if ((selected2.PassFailExamResultId == passFail.PassFailExamResultId && passFail.ExamPassed == false) || selected2.PassFailExamResultId == null)
                    {
                        list.Add(selected2);
                        break;
                    }
                }
            }

            return list;
        }

        public List<SelectListItem> PassFailExamResult(int? id)
        {
            db = new PTSContext();
            var passFailExamResultList = db.PassFailExamResults.Select(item => item).ToList();
            List<SelectListItem> selectListItem = new List<SelectListItem>();
            foreach (var item in passFailExamResultList)
            {
                if (id == item.PassFailExamResultId)
                    selectListItem.Add(new SelectListItem { Text = item.Name.ToString(), Value = item.PassFailExamResultId.ToString(), Selected = true });
                else
                    selectListItem.Add(new SelectListItem { Text = item.Name.ToString(), Value = item.PassFailExamResultId.ToString() });
            }
            return selectListItem;
        }

    }
}
