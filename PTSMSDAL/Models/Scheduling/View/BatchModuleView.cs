using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.View
{
    public class BatchModuleView
    {
        public double BatchModuleScheduleId { get; set; }
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public int BatchId { get; set; }
        public int BatchClassId { get; set; }
        public DateTime StartingDate { get; set; }
        public int PhaseScheduleId { get; set; }
        public int PhaseId { get; set; }
        public int CourseSequence { get; set; }
        public int ModuleSequence { get; set; }
    }

    public class InstructorView
    {
        public int InstructorId { get; set; }
    }
    public class InstructorColor
    {
        public int InstructorId { get; set; }
        public string Color { get; set; }
    }
    public class EquipmentColor
    {
        public int EquipmentId { get; set; }
        public string Color { get; set; }
    }
    public class BatchClassColor
    {
        public int BatchId { get; set; }
        public string Color { get; set; }
    }

    public class CourseModuleView
    {
        public int BatchId { get; set; }
        public int BatchClassId { get; set; }
        public int PhaseId { get; set; }
        public int CourseId { get; set; }
        public int CourseSequence { get; set; }
        public int ModuleId { get; set; }
        public int ModuleSequence { get; set; }


    }
    public class PeriodView
    {
        public string Period { get; set; }
        public int PeriodId { get; set; }
    }

    public class DayView
    {
        public string Day { get; set; }
        public int DayId { get; set; }
    }

    public class Scheduler
    {
        public int EventID { get; set; }
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public bool IsAllDay { get; set; }

        public int CourseID { get; set; }
        public string CourseCode { get; set; }
        public int ModuleID { get; set; }
        public string ModuleCode { get; set; }
        public int BatchID { get; set; }
        public string BatchClassName { get; set; }

    }

    public class EquipmentScheduler
    {
        public int EventID { get; set; }
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public bool IsAllDay { get; set; }
    }
    public class SchedulerResource
    {
        public int EquipmentId { get; set; }
        public string EquipmentModel { get; set; }
        public string EquipmentName { get; set; }
        public string WorkingHours { get; set; }
        public string EstimatedRemainingHours { get; set; }
    }

    public class SchedulerResourceInstructor
    {
        public int InstructorId { get; set; }
        public string EquipmentType { get; set; }
        public string InstructorName { get; set; }
    }

    public class GroundSchedulerResource
    { //id,equipmentType,EquipmentName,WorkingHours
        public int BatchId { get; set; }
        public string SerialNumber { get; set; }
        public string BatchClassName { get; set; }
    }

    public class UnScheduledResourceView
    {
        public UnScheduledResourceView()
        {
            this.Periods = new List<PeriodListView>();
        }
        public string Dates { get; set; }
        public List<PeriodListView> Periods { get; set; }
    }

    /*End of Custorm Object*/
    public class UnscheduledResource
    {
        public UnscheduledResource()
        {
            this.FreeDates = new List<FreeDate>();
        }
        public FreeInstructor FreeInstructor { get; set; }
        public List<FreeDate> FreeDates { get; set; }
    }

    public class FreeInstructor
    {
        public int InstructorId { get; set; }
        public string NameAndCompanyId { get; set; }
    }

    public class FreeDate
    {
        public FreeDate()
        {
            this.FreePeriods = new List<FreePeriod>();
        }
        public string Date { get; set; }
        public List<FreePeriod> FreePeriods { get; set; }
    }

    public class FreePeriod
    {
        public FreePeriod()
        {
            this.ClassRooms = new List<ClassRoom>();
        }
        public PeriodView Period { get; set; }
        public List<ClassRoom> ClassRooms { get; set; }
    }

    public class FreePeriodList
    {
        public FreePeriodList()
        {
        }
        public List<FreePeriod> FreePeriod { get; set; }
        public string Message { get; set; }
    }

    /*End of Custorm Object*/
    public class PeriodListView
    {
        public PeriodListView()
        {
            this.ClassRooms = new List<ClassRoom>();
            this.PotentialInstructors = new List<PotentialInstructorView>();
        }
        public PeriodView Period { get; set; }
        public List<PotentialInstructorView> PotentialInstructors { get; set; }
        public List<ClassRoom> ClassRooms { get; set; }
    }

    public class PeriodClassCombination
    {
        public int ClassRoomId { get; set; }
        public int PeriodId { get; set; }
    }

    public class PotentialInstructorView
    {
        public int InstructorId { get; set; }
        public string NameAndCompanyId { get; set; }
    }

    public class FilterView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class BatchView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AttendanceView
    {
        public AttendanceView()
        {
            this.Trainees = new List<TraineeView>();
            this.ClassRooms = new List<ClassRoom>();
            this.PotentialInstructors = new List<PotentialInstructorView>();
            this.ModuleActivities = new List<ModuleActivityView>();
        }
        public List<TraineeView> Trainees { get; set; }
        public List<ClassRoom> ClassRooms { get; set; }
        public List<PotentialInstructorView> PotentialInstructors { get; set; }
        public List<ModuleActivityView> ModuleActivities { get; set; }
    }

    public class ModuleActivityView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsTaken { get; set; }
    }
    public class TraineeView
    {
        public int TraineeId { get; set; }
        public string TraineeName { get; set; }
        public string TraineeNameAndLesson { get; set; }
        public int NearestFutureLessonSequence { get; set; }
        public float LessonLength { get; set; }
    }

}
