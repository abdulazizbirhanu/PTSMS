using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMSDAL.InstructorProfile
{
    public class InstructorProfileAccess
    {
        public List<Lesson> InstructorLessons(int personId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<Lesson> InstructorLessonList = new List<Lesson>();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);

                var result = db.FlyingFTDSchedules.Where(s => s.Instructor.Person.PersonId == personId && s.Status != statusName).ToList();
                var resultGroup = result.GroupBy(s => new { s.LessonId }).Select(grp => grp.FirstOrDefault()).ToList();
                foreach (var schedule in resultGroup)
                {
                    InstructorLessonList.Add(schedule.Lesson);
                }
                return InstructorLessonList;
            }
            catch (Exception)
            {
                return new List<Lesson>();
            }
        }

        public List<ModuleExam> ModuleExamList(int moduleId, int personId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<ModuleExam> ModuleExamList = new List<ModuleExam>();

                var moduleExams = db.ModuleExams.Where(tme => tme.CourseModule.ModuleId == moduleId).ToList();
                var moduleExamsGroup = moduleExams.GroupBy(m => m.ExamId).Select(grp => grp.FirstOrDefault()).ToList();

                foreach (var moduleExam in moduleExamsGroup)
                {
                    ModuleExamList.Add(moduleExam);
                }
                return ModuleExamList;
            }
            catch (Exception ex)
            {
                return new List<ModuleExam>();
            }
        }

        public List<Batch> GetBatches(int personId)
        {
            try
            {
                

                PTSContext db = new PTSContext();
                List<Batch> batchList = new List<Batch>();
                string statusNameForLesson = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);

                string statusNameForModule = SchedulerAccess.GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

                //Get FTD and Flying batch for FTD and FLTYING Instructor
                var flyingAndFTDBatchList = (from TBC in db.TraineeBatchClasses
                                             join FFS in db.FlyingFTDSchedules on TBC.TraineeId equals FFS.TraineeId
                                             where FFS.Instructor.Person.PersonId == personId && FFS.Status != statusNameForLesson
                                             select new
                                             {
                                                 TBC
                                             }).ToList();

                
                var flyingAndFTDBatchListGroup = flyingAndFTDBatchList.GroupBy(s => s.TBC.BatchClass.BatchId).Select(grp => grp.FirstOrDefault()).ToList();

                foreach (var batch in flyingAndFTDBatchListGroup)
                {
                    batchList.Add(batch.TBC.BatchClass.Batch);
                }
                 
                //Get Ground Class Batch for GROUD INSTRUCTOR
                var moduleScheduleList = db.ModuleSchedules.Where(ms=>ms.Instructor.PersonId == personId && ms.Status != statusNameForModule).ToList();


                var moduleScheduleListGroup = moduleScheduleList.GroupBy(s => s.PhaseSchedule.BatchId).Select(grp => grp.FirstOrDefault()).ToList();

                foreach (var schedule in moduleScheduleListGroup)
                {
                    batchList.Add(schedule.PhaseSchedule.Batch);
                }
                return batchList;
            }
            catch (Exception ex)
            {
                return new List<Batch>();
            }
        }

        public List<Module> ModuleList(int courseId, int personId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<Module> InstructorModuleList = new List<Module>();

                var result = db.ModuleInstructorSchedules.Where(s => s.Instructor.Person.PersonId == personId && s.Module.CourseId == courseId && s.EndDate > DateTime.Now).ToList();

                var resultGroup = result.GroupBy(s => new { s.ModuleId }).Select(grp => grp.FirstOrDefault()).ToList();
                foreach (var schedule in resultGroup)
                {
                    InstructorModuleList.Add(schedule.Module);
                }
                return InstructorModuleList;
            }
            catch (Exception)
            {
                return new List<Module>();
            }
        }

        public List<Course> InstructorCourses(int personId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<Course> InstructorCourseList = new List<Course>();

                var result = db.ModuleInstructorSchedules.Where(s => s.Instructor.Person.PersonId == personId && s.EndDate > DateTime.Now).ToList();

                var resultGroup = result.GroupBy(s => new { s.Module.CourseId }).Select(grp => grp.FirstOrDefault()).ToList();
                foreach (var schedule in resultGroup)
                {
                    InstructorCourseList.Add(schedule.Module.Course);
                }
                return InstructorCourseList;
            }
            catch (Exception)
            {
                return new List<Course>();
            }
        }
    }
}
