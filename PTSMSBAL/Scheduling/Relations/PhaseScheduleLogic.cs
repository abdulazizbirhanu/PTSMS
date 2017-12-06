using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using System.Collections.Generic;

namespace PTSMSBAL.Logic.Scheduling.Relations
{
    public class PhaseScheduleLogic
    {
        PhaseScheduleAccess phaseScheduleAccess = new PhaseScheduleAccess();

        public object List()
        {
            return phaseScheduleAccess.List();
        }

        public object Details(int id)
        {
            return phaseScheduleAccess.Details(id);
        }

        public object Add(PhaseSchedule phaseSchedule)
        {
            return phaseScheduleAccess.Add(phaseSchedule);
        }

        public object Revise(PhaseSchedule phaseSchedule)
        {
            return phaseScheduleAccess.Revise(phaseSchedule);
        }

        public object Delete(int id)
        {
            return phaseScheduleAccess.Delete(id);
        }

        public List<BatchClasses> ListBatchPhase()
        {
            return phaseScheduleAccess.ListBatchPhase();
        }
        public List<Courses> ListCourseModule(int batchClassId, int phaseId, int lessonCategoryTypeId, ref bool isCourseModuleSequenceFound)
        {
            return null;

            //return phaseScheduleAccess.ListCourseModule(batchClassId, phaseId, lessonCategoryTypeId, ref isCourseModuleSequenceFound);
        }

        public List<Lessons> ListLessons(int batchClassId, int phaseId, int lessonCategoryTypeId, ref bool isLessonSequenceFound)
        {
            return null;
            //return phaseScheduleAccess.ListLessons(batchClassId, phaseId, lessonCategoryTypeId, ref isLessonSequenceFound);
        }

        public bool SaveCourseModuleSequence(List<Courses> CoursesModuleSequence, PhaseSchedule phaseSchedule)
        {
            return phaseScheduleAccess.SaveCourseModuleSequence(CoursesModuleSequence, phaseSchedule);
        }
        public bool SaveLessonSequence(List<Lessons> Lessonequence, PhaseSchedule phaseSchedule)
        {
            return false;
            //return phaseScheduleAccess.SaveLessonSequence(Lessonequence, phaseSchedule);
        }
        public List<PhaseCourse> ListPhaseCourses(int instructorId, int batchId)
        {
            return phaseScheduleAccess.ListPhaseCourses(instructorId, batchId);
        }
        public List<PhaseModules> ListPhaseModules(int instructorId, int phaseCourseId, int phaseScheduleId)
        {
            return null;
            //return phaseScheduleAccess.ListPhaseModules(instructorId, phaseCourseId, phaseScheduleId);
        }
    }
}