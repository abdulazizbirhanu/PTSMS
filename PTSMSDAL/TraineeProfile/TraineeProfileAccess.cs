using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.TraineeProfile
{
    public class TraineeProfileAccess
    {
        public List<Lesson> TraineeLessons(int traineeId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<Lesson> TraineeLessonList = new List<Lesson>();

                var result = (from TL in db.TraineeLessons
                                  //join TL in db.TraineeLessons on TS.TraineeSyllabusId equals TL..TraineeCategory.TraineeProgram.TraineeSyllabusId
                              join BC in db.TraineeBatchClasses on TL.TraineeId equals BC.TraineeId
                              where TL.TraineeId == traineeId
                              select new
                              {
                                  BC,
                                  TL
                              }).ToList();
                var resultGroup = result.GroupBy(Ls => new { Ls.TL.LessonId }).Select(grp => grp.FirstOrDefault()).ToList();
                foreach (var less in resultGroup)
                {
                    var lesson = db.Lessons.Where(m => ((m.RevisionGroupId != null && m.RevisionGroupId == (less.TL.Lesson.RevisionGroupId == null ? less.TL.Lesson.LessonId : less.TL.Lesson.RevisionGroupId)) || m.RevisionGroupId == null && m.LessonId == less.TL.Lesson.LessonId) && m.Status == "Active").ToList().FirstOrDefault();

                    TraineeLessonList.Add(lesson);
                }
                return TraineeLessonList;
            }
            catch (Exception)
            {
                return new List<Lesson>();
            }
        }

        public TraineeCourseExam CourseExamDetails(int traineeCourseExamId)
        {
            try
            {
                PTSContext db = new PTSContext();
                var traineeCourseExam = db.TraineeCourseExams.Find(traineeCourseExamId);
                if (traineeCourseExam != null)
                    return traineeCourseExam;
                return new TraineeCourseExam();
            }
            catch (Exception ex)
            {
                return new TraineeCourseExam();
            }
        }

        public List<TraineeCourseExam> TraineeCourseExamList(int courseId, int traineeId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<TraineeCourseExam> traineeCourseExams = new List<TraineeCourseExam>();

                var traineeCourseExamList = db.TraineeCourseExams.Where(TCE => TCE.TraineeCourse.CourseId == courseId && TCE.TraineeCourse.TraineeId == traineeId).ToList();
                return traineeCourseExamList.GroupBy(m => m.ExamId).Select(grp => grp.FirstOrDefault()).ToList();
            }
            catch (Exception ex)
            {
                return new List<TraineeCourseExam>();
            }
        }

        public TraineeModuleExam ModuleExamDetails(int traineeModuleExamId)
        {
            try
            {
                PTSContext db = new PTSContext();
                var traineeModuleExam = db.TraineeModuleExams.Find(traineeModuleExamId);
                if (traineeModuleExam != null)
                    return traineeModuleExam;
                return new TraineeModuleExam();
            }
            catch (Exception ex)
            {
                return new TraineeModuleExam();
            }
        }

        public List<Course> TraineeCourses(int traineeId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<Course> CourseList = new List<Course>();

                var PhaseCourses = (
                    from MIS in db.TraineeCourses
                    join CM in db.CourseModules on MIS.CourseId equals CM.CourseCategory.CourseId
                    join TM in db.TraineeModules on CM.ModuleId equals TM.ModuleId
                    where MIS.TraineeId == traineeId

                    select new
                    {
                        CM
                    }).ToList();

                var phaseCoursesGroup = PhaseCourses.GroupBy(cm => cm.CM.CourseCategory.CourseId).Select(grp => grp.FirstOrDefault()).ToList();

                foreach (var courses in phaseCoursesGroup)
                {
                    var course = db.Courses.Where(c => ((c.RevisionGroupId != null && c.RevisionGroupId == (courses.CM.CourseCategory.Course.RevisionGroupId == null ? courses.CM.CourseCategory.Course.CourseId : courses.CM.CourseCategory.Course.RevisionGroupId)) || (c.RevisionGroupId == null && c.CourseId == courses.CM.CourseCategory.Course.CourseId)) && c.Status == "Active").ToList().FirstOrDefault();

                    if (course != null)
                    {
                        CourseList.Add(course);
                    }
                }
                return CourseList.ToList();
            }
            catch (Exception ex)
            {
                return new List<Course>();
            }
        }

        public List<Module> TraineeModuleList(int courseId, int traineeId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<Module> ModuleList = new List<Module>();

                var traineeModules = db.TraineeModules.Where(tm => tm.TraineeCourse.CourseId == courseId && tm.TraineeCourse.TraineeId == traineeId).ToList();
                var moduleGroup = traineeModules.GroupBy(m => m.ModuleId).Select(grp => grp.FirstOrDefault()).ToList();

                foreach (var mod in moduleGroup)
                {
                    var module = db.Modules.Where(m => ((m.RevisionGroupId != null && m.RevisionGroupId == (mod.Module.RevisionGroupId == null ? mod.Module.ModuleId : mod.Module.RevisionGroupId)) || m.RevisionGroupId == null && m.ModuleId == mod.Module.ModuleId) && m.Status == "Active").ToList().FirstOrDefault();

                    if (module != null)
                    {
                        ModuleList.Add(module);
                    }
                }
                return ModuleList;
            }
            catch (Exception ex)
            {
                return new List<Module>();
            }
        }

        public List<TraineeModuleExam> TraineeModuleExamList(int moduleId, int traineeId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<TraineeModuleExam> ModuleExamList = new List<TraineeModuleExam>();

                var traineeModuleExams = db.TraineeModuleExams.Where(tme => tme.TraineeModule.ModuleId == moduleId && tme.TraineeModule.TraineeCourse.TraineeId == traineeId).ToList();
                var traineeModuleExamsGroup = traineeModuleExams.GroupBy(m => m.ExamId).Select(grp => grp.FirstOrDefault()).ToList();

                foreach (var traineeModuleExam in traineeModuleExamsGroup)
                {
                    var exam = db.Exams.Where(e => ((e.RevisionGroupId != null && e.RevisionGroupId == (traineeModuleExam.Exam.RevisionGroupId == null ? traineeModuleExam.Exam.ExamId : traineeModuleExam.Exam.RevisionGroupId)) || e.RevisionGroupId == null && e.ExamId == traineeModuleExam.Exam.ExamId) && e.Status == "Active").ToList().FirstOrDefault();

                    if (exam != null)
                    {
                        traineeModuleExam.Exam = exam;
                        ModuleExamList.Add(traineeModuleExam);
                    }
                }
                return ModuleExamList;
            }
            catch (Exception ex)
            {
                return new List<TraineeModuleExam>();
            }
        }

    }
}
