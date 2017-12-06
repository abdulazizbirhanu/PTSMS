using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Scheduling;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling
{
    public class SchedulerAccess
    {
        private PTSContext db = new PTSContext();
        ModuleInstructorScheduleAccess moduleInstructorScheduleAccess = new ModuleInstructorScheduleAccess();
        ClassRoomAccess classRoomAccess = new ClassRoomAccess();

        ModuleScheduleAccess moduleScheduleAccess = new ModuleScheduleAccess();

        #region Scheduling of Ground Class
        public List<BatchModuleView> get_BatchModuleList()
        {
            try
            {
                List<BatchModuleView> BatchModuleList = new List<BatchModuleView>();

                int i = 1, sequence;
                PTSContext dbContext = new PTSContext();

                var moduleInstructorSchedules = (from CM in dbContext.CourseModules
                                                 join BMS in dbContext.BatchModuleSequences on CM.ModuleId equals BMS.ModuleId
                                                 join PS in dbContext.PhaseSchedules on BMS.PhaseScheduleId equals PS.PhaseScheduleId
                                                 where BMS.Sequence > 0
                                                 select new
                                                 {
                                                     PS,
                                                     CM,
                                                     BMS
                                                 }).ToList();

                foreach (var item in moduleInstructorSchedules)
                {
                    BatchModuleList.Add(new BatchModuleView
                    {
                        BatchId = item.PS.BatchClass.Batch.BatchId,
                        PhaseId = item.PS.PhaseId,
                        BatchClassId = item.PS.BatchClassId,
                        PhaseScheduleId = item.PS.PhaseScheduleId,
                        CourseId = item.CM.CourseCategory.CourseId,
                        ModuleId = item.BMS.ModuleId,
                        StartingDate = item.PS.StartingDate,
                        CourseSequence = int.TryParse(item.BMS.Sequence.ToString(CultureInfo.InvariantCulture).Split('.')[0], out sequence) ? sequence : 1,
                        ModuleSequence = int.TryParse(item.BMS.Sequence.ToString(CultureInfo.InvariantCulture).Split('.')[1], out sequence) ? sequence : 1,
                        BatchModuleScheduleId = i++
                    });
                }
                var BatchModuleGroup = BatchModuleList.GroupBy(x => new { x.CourseId, x.ModuleId }).Select(grp => grp.FirstOrDefault()).ToList();
                return BatchModuleGroup.ToList();
            }
            catch (Exception ex)
            {
                return new List<BatchModuleView>();
            }
        }

        public List<InstructorView> get_InstructorList()
        {
            try
            {
                var result = db.ModuleInstructorSchedules.ToList();
                var distinictInstructor = result.GroupBy(x => new { x.InstructorId }).Select(grp => grp.FirstOrDefault()).ToList();

                var instructors = distinictInstructor.Select(item => new InstructorView
                {
                    InstructorId = item.InstructorId
                });
                return instructors.ToList();
            }
            catch (Exception ex)
            {
                return new List<InstructorView>();
            }
        }

        public List<string> get_UnionOfWorkingDays()
        {
            try
            {
                var result = db.Days.Where(d => d.Status.ToLower().Equals("active")).ToList();
                if (result.Count() > 0)
                {
                    var ListOfPossibleDays = result.Select(item => new
                    {
                        item.DayName
                    });

                    var dayList = ListOfPossibleDays.Select(day => day).Distinct().ToList();
                    if (dayList.Count > 0)
                    {
                        List<string> distinictDays = new List<string>();
                        foreach (var day in dayList)
                        {
                            distinictDays.Add(day.DayName);
                        }
                        return distinictDays;
                    }
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        public List<PeriodView> get_UnionOfWorkingPeriods()
        {
            try
            {
                List<PeriodView> distinictPeriodList = new List<PeriodView>();
                var periods = db.Periods.Where(p => p.Status.ToLower().Equals("active")).ToList();
                var result = periods.Select(item => new PeriodView
                {
                    Period = item.StartTime.ToString() + "-" + item.EndTime.ToString(),
                    PeriodId = item.PeriodId
                });

                var periodGroup = result.GroupBy(x => new { x.Period }).Select(grp => grp.ToList()).ToList();
                foreach (var item in periodGroup)
                {
                    var firstItem = item.FirstOrDefault();
                    distinictPeriodList.Add(firstItem);
                }

                if (distinictPeriodList.Count() > 0)
                {
                    return distinictPeriodList.ToList();
                }
                return new List<PeriodView>();
            }
            catch (Exception ex)
            {
                return new List<PeriodView>();
            }
        }

        public List<string> get_DayTemplateList(int BatchId)
        {
            try
            {
                List<string> DayList = new List<string>();
                var DayListVar = from B in db.Batches
                                 join Dt in db.DayTemplates on B.DayTemplateId equals Dt.DayTemplateId
                                 join D in db.Days on Dt.DayTemplateId equals D.DayTemplateId
                                 where B.BatchId == BatchId && D.Status.ToLower().Equals("active") && Dt.Status.ToLower().Equals("active")
                                 select new
                                 {
                                     D.DayName
                                 };
                foreach (var item in DayListVar)
                {
                    DayList.Add(item.DayName);
                }
                return DayList.ToList();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        public List<Batchdays> get_DayTemplateList()
        {
            try
            {
                List<Batchdays> BatchDayList = new List<Batchdays>();

                var DayListVar = from B in db.Batches
                                 join Dt in db.DayTemplates on B.DayTemplateId equals Dt.DayTemplateId
                                 join D in db.Days on Dt.DayTemplateId equals D.DayTemplateId
                                 where D.Status.ToLower().Equals("active") && Dt.Status.ToLower().Equals("active")
                                 select new
                                 {
                                     DayName = D.DayName,
                                     BatchId = B.BatchId
                                 };

                foreach (var objBatchDay in DayListVar)
                {
                    BatchDayList.Add(new Batchdays
                    {
                        BatchId = objBatchDay.BatchId,
                        DayName = objBatchDay.DayName
                    });
                }

                return BatchDayList.ToList();
            }
            catch (Exception ex)
            {
                return new List<Batchdays>();
            }
        }


        public List<string> get_PeriodTemplateList(int BatchId)
        {
            try
            {
                List<string> PeriodList = new List<string>();
                var DayListVar = from BT in db.Batches
                                 join Pt in db.PeriodTemplates on BT.PeriodTemplateId equals Pt.PeriodTemplateId
                                 join P in db.Periods on Pt.PeriodTemplateId equals P.PeriodTemplateId
                                 where BT.BatchId == BatchId && P.Status.ToLower().Equals("active") && Pt.Status.ToLower().Equals("active")
                                 select new
                                 {
                                     Periods = P.StartTime.ToString() + "-" + P.EndTime.ToString()
                                 };
                foreach (var item in DayListVar)
                {
                    PeriodList.Add(item.Periods);
                }
                return PeriodList.ToList();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        public List<BatchPeriod> get_PeriodTemplateList()
        {
            try
            {
                List<BatchPeriod> batchPeriodList = new List<BatchPeriod>();
                var DayListVar = from BT in db.Batches
                                 join Pt in db.PeriodTemplates on BT.PeriodTemplateId equals Pt.PeriodTemplateId
                                 join P in db.Periods on Pt.PeriodTemplateId equals P.PeriodTemplateId
                                 where P.Status.ToLower().Equals("active") && Pt.Status.ToLower().Equals("active")
                                 select new
                                 {
                                     Periods = P.StartTime.ToString() + "-" + P.EndTime.ToString(),
                                     BatchId = BT.BatchId
                                 };
                foreach (var item in DayListVar)
                {
                    batchPeriodList.Add(new BatchPeriod
                    {
                        BatchId = item.BatchId,
                        Period = item.Periods
                    });
                }
                return batchPeriodList.ToList();
            }
            catch (Exception ex)
            {
                return new List<BatchPeriod>();
            }
        }

        public List<CourseModuleView> ListCourseModule()
        {
            try
            {
                int sequence;
                PTSContext dbContext = new PTSContext();

                List<CourseModuleView> CourseModuleList = new List<CourseModuleView>();
                var courseModules = (from CM in dbContext.CourseModules
                                     join BMS in dbContext.BatchModuleSequences on CM.ModuleId equals BMS.ModuleId
                                     join PS in dbContext.PhaseSchedules on BMS.PhaseScheduleId equals PS.PhaseScheduleId
                                     where BMS.Sequence > 0
                                     select new
                                     {
                                         CM,
                                         BMS,
                                         PS
                                     }).ToList();

                foreach (var item in courseModules)
                {
                    CourseModuleList.Add(new CourseModuleView
                    {
                        BatchId = item.PS.BatchClass.Batch.BatchId,
                        BatchClassId = item.PS.BatchClassId,
                        PhaseId = item.PS.PhaseId,
                        CourseId = item.CM.CourseCategory.CourseId,
                        ModuleId = item.BMS.ModuleId,
                        ModuleSequence = int.TryParse(item.BMS.Sequence.ToString(CultureInfo.InvariantCulture).Split('.')[0], out sequence) ? sequence : 1,
                        CourseSequence = int.TryParse(item.BMS.Sequence.ToString(CultureInfo.InvariantCulture).Split('.')[1], out sequence) ? sequence : 1
                    });
                }
                return CourseModuleList.ToList();
            }
            catch (Exception ex)
            {
                return new List<CourseModuleView>();
            }
        }

        public bool Add(List<ModuleSchedule> moduleScheduleList)
        {
            try
            {
                int index = 1;
                PTSContext dbContext = new PTSContext();
                foreach (ModuleSchedule moduleSchedule in moduleScheduleList)
                {
                    //moduleSchedule.RevisionNo = index++;
                    //moduleSchedule.EffectiveDate = DateTime.Now;
                    moduleSchedule.StartDate = DateTime.Now;
                    moduleSchedule.EndDate = new DateTime(9999, 12, 31);
                    moduleSchedule.CreationDate = DateTime.Now;
                    moduleSchedule.CreatedBy = "";
                    moduleSchedule.RevisionDate = DateTime.Now;
                    moduleSchedule.RevisedBy = "";
                    dbContext.ModuleSchedules.Add(moduleSchedule);
                }
                if (dbContext.SaveChanges() > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        } 
        #endregion

        #region ScheduleReportDisplay
        public List<InstructorColor> ListScheduledInstructors()
        {
            PTSContext dbContext = new PTSContext();
            var random = new Random();

            List<Scheduler> ScheduledEvents = new List<Scheduler>();
            var scheduleList = dbContext.ModuleSchedules.ToList();

            var InstructorGroup = scheduleList.GroupBy(x => new { x.InstructorId }).Select(grp => grp.FirstOrDefault()).ToList();

            List<InstructorColor> InstructorList = InstructorGroup.Select(MS => new InstructorColor
            {
                InstructorId = MS.InstructorId,
                Color = String.Format("#{0:X6}", random.Next(0x1000000))
            }).ToList();

            if (InstructorList.Count > 0)
                return InstructorList.ToList();

            return new List<InstructorColor>();
        }
        public List<Scheduler> ScheduledEventList()
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<Scheduler> ScheduledEvents = new List<Scheduler>();
                //var result = dbContext.ModuleSchedules.ToList();
                var scheduledEvents = (
                   from CM in db.CourseModules
                   join MS in db.ModuleSchedules on CM.ModuleId equals MS.ModuleId
                   select new
                   {
                       CM,
                       MS
                   }).ToList();
                var ScheduledEventList = scheduledEvents.Select(item => new Scheduler
                                         {
                                             EventID = item.MS.ModuleScheduleId,
                                             Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.BatchClass.BatchClassName
                                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                                             + "</strong><br/>- Course Code: <strong>" + item.CM.CourseCategory.Course.CourseCode
                                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode
                                             + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName + "</strong>",
                                             Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ". -" + item.MS.PhaseSchedule.BatchClass.BatchClassName
                                             + "-" + item.MS.ClassRoom.Building.BuildingName
                                             + "-" + item.MS.ClassRoom.RoomNo
                                             + "-" + item.CM.CourseCategory.Course.CourseCode
                                             + "-" + item.MS.Module.ModuleCode + " ~" + item.MS.InstructorId,
                                             EventStart = Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.StartTime)).ToString("MM/dd/yyyy hh:mm")).AddHours(3), //
                                             EventEnd = Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.EndTime)).ToString("MM/dd/yyyy hh:mm")).AddHours(3),
                                             IsAllDay = false
                                         });

                foreach (var scheduledEvent in ScheduledEventList)
                {
                    ScheduledEvents.Add(scheduledEvent);
                }
                return ScheduledEventList.ToList();
            }
            catch (Exception ex)
            {
                return new List<Scheduler>();
            }
        }

        public List<Scheduler> ScheduledEventList(string FilterBy, string FilterValue)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                int searchValue = 0;
                List<ModuleSchedule> result = new List<ModuleSchedule>();

                List<Scheduler> ScheduledEvents = new List<Scheduler>();
                if (FilterBy.Equals("BatchClass"))
                {
                    searchValue = Convert.ToInt32(FilterValue);
                    var scheduledEvents = (from CM in db.CourseModules
                                           join MS in db.ModuleSchedules on CM.ModuleId equals MS.ModuleId
                                           where MS.PhaseSchedule.BatchClassId == searchValue
                                           select new
                                           {
                                               CM,
                                               MS
                                           }).ToList();
                    if (scheduledEvents.Count() > 0)
                    {
                        var ScheduledEventList = scheduledEvents.Select(item => new Scheduler
                        {
                            EventID = item.MS.ModuleScheduleId,
                            Description = "Instructor Name: " + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                            + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                            + ". Batch Class Name: " + item.MS.PhaseSchedule.BatchClass.BatchClassName
                            + "- Building Name: " + item.MS.ClassRoom.Building.BuildingName
                            + "- Room No: " + item.MS.ClassRoom.RoomNo
                            + "- Course Code: " + item.CM.CourseCategory.CourseId
                            + "- Module Code: " + item.MS.Module.ModuleCode,
                            Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                            + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                            + ". -" + item.MS.PhaseSchedule.BatchClass.BatchClassName
                            + "-" + item.MS.ClassRoom.Building.BuildingName
                            + "-" + item.MS.ClassRoom.RoomNo
                            + "-" + item.CM.CourseCategory.CourseId
                            + "-" + item.MS.Module.ModuleCode,
                            EventStart = item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.StartTime), //
                            EventEnd = item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.EndTime),
                            IsAllDay = false
                        });

                        foreach (var scheduledEvent in ScheduledEventList)
                        {
                            ScheduledEvents.Add(scheduledEvent);
                        }
                        return ScheduledEventList.ToList();
                    }
                }
                else if (FilterBy.Equals("Program"))
                {
                    searchValue = Convert.ToInt32(FilterValue);
                    //result = dbContext.ModuleSchedules.Where(MS => MS.PhaseSchedule.BatchClass.Batch.ProgramId == searchValue).ToList();
                    var scheduledEvents = (from CM in db.CourseModules
                                           join MS in db.ModuleSchedules on CM.ModuleId equals MS.ModuleId
                                           where MS.PhaseSchedule.BatchClass.Batch.ProgramId == searchValue
                                           select new
                                           {
                                               CM,
                                               MS
                                           }).ToList();
                    if (scheduledEvents.Count() > 0)
                    {
                        var ScheduledEventList = scheduledEvents.Select(item => new Scheduler
                        {
                            EventID = item.MS.ModuleScheduleId,
                            Description = "Instructor Name: " + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                            + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                            + ". Batch Class Name: " + item.MS.PhaseSchedule.BatchClass.BatchClassName
                            + "- Building Name: " + item.MS.ClassRoom.Building.BuildingName
                            + "- Room No: " + item.MS.ClassRoom.RoomNo
                            + "- Course Code: " + item.CM.CourseCategory.CourseId
                            + "- Module Code: " + item.MS.Module.ModuleCode,
                            Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                            + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                            + ". -" + item.MS.PhaseSchedule.BatchClass.BatchClassName
                            + "-" + item.MS.ClassRoom.Building.BuildingName
                            + "-" + item.MS.ClassRoom.RoomNo
                            + "-" + item.CM.CourseCategory.CourseId
                            + "-" + item.MS.Module.ModuleCode,
                            EventStart = item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.StartTime), //
                            EventEnd = item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.EndTime),
                            IsAllDay = false
                        });

                        foreach (var scheduledEvent in ScheduledEventList)
                        {
                            ScheduledEvents.Add(scheduledEvent);
                        }
                        return ScheduledEventList.ToList();
                    }
                }
                else if (FilterBy.Equals("Instructor"))
                {
                    searchValue = Convert.ToInt32(FilterValue);
                    var scheduledEvents = (from CM in db.CourseModules
                                           join MS in db.ModuleSchedules on CM.ModuleId equals MS.ModuleId
                                           where MS.InstructorId == searchValue
                                           select new
                                           {
                                               CM,
                                               MS
                                           }).ToList();
                    if (scheduledEvents.Count() > 0)
                    {
                        var ScheduledEventList = scheduledEvents.Select(item => new Scheduler
                        {
                            EventID = item.MS.ModuleScheduleId,
                            Description = "Instructor Name: " + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                            + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                            + ". Batch Class Name: " + item.MS.PhaseSchedule.BatchClass.BatchClassName
                            + "- Building Name: " + item.MS.ClassRoom.Building.BuildingName
                            + "- Room No: " + item.MS.ClassRoom.RoomNo
                            + "- Course Code: " + item.CM.CourseCategory.CourseId
                            + "- Module Code: " + item.MS.Module.ModuleCode,
                            Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                            + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                            + ". -" + item.MS.PhaseSchedule.BatchClass.BatchClassName
                            + "-" + item.MS.ClassRoom.Building.BuildingName
                            + "-" + item.MS.ClassRoom.RoomNo
                            + "-" + item.CM.CourseCategory.CourseId
                            + "-" + item.MS.Module.ModuleCode,
                            EventStart = item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.StartTime), //
                            EventEnd = item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.EndTime),
                            IsAllDay = false
                        });

                        foreach (var scheduledEvent in ScheduledEventList)
                        {
                            ScheduledEvents.Add(scheduledEvent);
                        }
                        return ScheduledEventList.ToList();
                    }
                }
                else if (FilterBy.Equals("Date"))
                {
                    string[] takenDateArray = FilterValue.Split('-');

                    DateTime startAt = Convert.ToDateTime(takenDateArray[0]);
                    DateTime endAt = Convert.ToDateTime(takenDateArray[1]);

                    var scheduledEvents = (from CM in db.CourseModules
                                           join MS in db.ModuleSchedules on CM.ModuleId equals MS.ModuleId
                                           where MS.Date >= startAt && MS.Date <= endAt
                                           select new
                                           {
                                               CM,
                                               MS
                                           }).ToList();
                    if (scheduledEvents.Count() > 0)
                    {
                        var ScheduledEventList = scheduledEvents.Select(item => new Scheduler
                        {
                            EventID = item.MS.ModuleScheduleId,
                            Description = "Instructor Name: " + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                            + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                            + ". Batch Class Name: " + item.MS.PhaseSchedule.BatchClass.BatchClassName
                            + "- Building Name: " + item.MS.ClassRoom.Building.BuildingName
                            + "- Room No: " + item.MS.ClassRoom.RoomNo
                            + "- Course Code: " + item.CM.CourseCategory.CourseId
                            + "- Module Code: " + item.MS.Module.ModuleCode,
                            Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                            + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                            + ". -" + item.MS.PhaseSchedule.BatchClass.BatchClassName
                            + "-" + item.MS.ClassRoom.Building.BuildingName
                            + "-" + item.MS.ClassRoom.RoomNo
                            + "-" + item.CM.CourseCategory.CourseId
                            + "-" + item.MS.Module.ModuleCode,
                            EventStart = item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.StartTime), //
                            EventEnd = item.MS.Date.Date + TimeSpan.Parse(item.MS.Period.EndTime),
                            IsAllDay = false
                        });

                        foreach (var scheduledEvent in ScheduledEventList)
                        {
                            ScheduledEvents.Add(scheduledEvent);
                        }
                        return ScheduledEventList.ToList();
                    }
                }
                return new List<Scheduler>();
            }
            catch (Exception ex)
            {
                return new List<Scheduler>();
            }
        }

        public List<UnscheduledResource> get_FreeTimeSlotAndRoom(int ModuleScheduleId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                //1. Get all free date, period and class room
                List<PeriodView> PeriodList = get_UnionOfWorkingPeriods();
                List<ClassRoom> ClassRoomList = (List<ClassRoom>)classRoomAccess.List();
                List<PeriodClassCombination> periodClassCombination = get_PeriodClassRoomCombination(PeriodList, ClassRoomList);

                List<ModuleSchedule> moduleSchedules = dbContext.ModuleSchedules.ToList();

                //Sort modules Schedule list by date: so, that Date to which the event will be edited will be displayed in Aschending order
                var sortedModuleSchedules = moduleSchedules.OrderBy(x => x.Date).ToList();

                //Group scheduled modules by Date to get all valid days in order to get free period, instructor and Class room of that day
                var moduleSchedulesGroup = sortedModuleSchedules.GroupBy(x => new { x.Date }).Select(grp => grp.ToList()).ToList();

                //2. Filter the fittest one for the clicked event
                //Get the detial of the module to be rescheduled
                ModuleSchedule moduleScheduleBO = (ModuleSchedule)moduleScheduleAccess.Details(ModuleScheduleId);

                //get potantial instructor of the module to be rescheduled
                List<ModuleInstructorSchedule> potentialInstructors = moduleInstructorScheduleAccess.PotentialInstructorList(moduleScheduleBO.ModuleId);

                //get DAY and PERIOD template of a Batch whose module is to be rescheduled 
                List<string> daysTemplate = get_DayTemplateList(moduleScheduleBO.PhaseSchedule.BatchClass.BatchId);
                List<string> periodsTemplate = get_PeriodTemplateList(moduleScheduleBO.PhaseSchedule.BatchClass.BatchId);

                List<UnscheduledResource> UnscheduledResourceList = new List<UnscheduledResource>();
                UnscheduledResource unScheduledResource = null;

                foreach (var instructor in potentialInstructors)
                {
                    unScheduledResource = new UnscheduledResource();
                    //GET INSTRUCTOR INFORMATION
                    unScheduledResource.FreeInstructor = new FreeInstructor
                    {
                        InstructorId = instructor.InstructorId,
                        NameAndCompanyId = instructor.Instructor.Person.FirstName.ToUpper().Substring(0, 3) + " " + instructor.Instructor.Person.FirstName.ToUpper().Substring(0, 1) + ". " + instructor.Instructor.Person.CompanyId
                    };

                    List<FreeDate> FreeDateList = new List<FreeDate>();
                    FreeDate FreeDate = null;

                    foreach (var moduleSchedule in moduleSchedulesGroup)
                    {
                        FreeDate = new FreeDate();
                        if (moduleSchedule.FirstOrDefault().Date >= DateTime.Now)
                        {
                            FreeDate.Date = moduleSchedule.FirstOrDefault().Date.Date.ToString("dd/MM/yyyy");

                            //Get free Period and Class Room combination/s of specific day
                            var freePeriodAndRoom = periodClassCombination.Where(PRC => !(moduleSchedule.Any(ms => (ms.PeriodId == PRC.PeriodId)
                                && (ms.ClassRoomId == PRC.ClassRoomId)))).ToList();

                            var PeiodRoomGroup = freePeriodAndRoom.GroupBy(x => new { x.PeriodId }).Select(grp => grp.ToList()).ToList();

                            List<FreePeriod> FreePeriodList = new List<FreePeriod>();
                            FreePeriod freePeriod = null;

                            foreach (var periodAndroom in PeiodRoomGroup)
                            {
                                string day = moduleSchedule.FirstOrDefault().Date.Date.ToString("dddd");
                                string period = PeriodList.Single(p => p.PeriodId == periodAndroom.FirstOrDefault().PeriodId).Period;

                                //Check if this instructor is free at this DAY and PERIOD
                                if (IsThisInstructorFree(instructor, moduleSchedule.FirstOrDefault().Date, periodAndroom.FirstOrDefault().PeriodId))
                                {
                                    //Check whether it is a suitable Batch Day template and Period template, Does Batch Class Has Another Class 
                                    //and weather the instructor has no class                
                                    if (Is_ValidDayAndPeriodForBatch(daysTemplate, periodsTemplate, day, period) && !DoesBatchClassHasAnotherScheduledClass(moduleScheduleBO.ModuleScheduleId, moduleScheduleBO.PhaseSchedule.BatchClassId, moduleSchedule.FirstOrDefault().Date, periodAndroom.FirstOrDefault().PeriodId))
                                    {
                                        freePeriod = new FreePeriod();
                                        freePeriod.Period = PeriodList.Single(p => p.PeriodId == periodAndroom.FirstOrDefault().PeriodId);
                                        List<ClassRoom> ClassRooms = new List<ClassRoom>();
                                        foreach (var room in periodAndroom)
                                        {
                                            //Check the room is reserved or not
                                            if (!IsRoomReserved(moduleSchedule.FirstOrDefault().Date, periodAndroom.FirstOrDefault().PeriodId, room.ClassRoomId))
                                            {
                                                ClassRooms.Add(ClassRoomList.Single(CR => CR.ClassRoomId == room.ClassRoomId));
                                            }
                                        }
                                        if (ClassRooms.Count > 0)
                                            freePeriod.ClassRooms.AddRange(ClassRooms);
                                        if (freePeriod.ClassRooms.Count > 0)
                                            FreePeriodList.Add(freePeriod);
                                    }
                                }
                            }

                            if (FreePeriodList.Count > 0 && freePeriod != null)
                                FreeDate.FreePeriods.AddRange(FreePeriodList);
                            if (FreeDate.FreePeriods.Count > 0 && FreeDate.Date != null)
                                FreeDateList.Add(FreeDate);
                        }
                    }
                    if (FreeDateList.Count > 0 && FreeDate != null)
                        unScheduledResource.FreeDates.AddRange(FreeDateList);
                    if (unScheduledResource.FreeDates.Count > 0)
                        UnscheduledResourceList.Add(unScheduledResource);
                }
                return UnscheduledResourceList.ToList();
            }
            catch (Exception ex)
            {
                return new List<UnscheduledResource>();
            }
        }

        public List<FreePeriod> get_FreeTimeSlotAndRoomForSpecificDate(DateTime date, int instructorId, int phaseScheduleId, int phaseModuleId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                //1. Get all free date, period and class room
                List<PeriodView> PeriodList = get_UnionOfWorkingPeriods();
                List<ClassRoom> ClassRoomList = (List<ClassRoom>)classRoomAccess.List();
                List<PeriodClassCombination> periodClassCombination = get_PeriodClassRoomCombination(PeriodList, ClassRoomList);

                //Get all Modules Scheduled at that Date
                List<ModuleSchedule> moduleSchedules = dbContext.ModuleSchedules.Where(Ms => Ms.Date == date).ToList();

                var courseSchedule = dbContext.PhaseSchedules.Where(Cs => Cs.PhaseScheduleId == phaseScheduleId).ToList();

                ////Get instructor of the module to be rescheduled                
                ModuleInstructorSchedule moduleInstructorSchedule = moduleInstructorScheduleAccess.GetInstructorSchedule(phaseModuleId, instructorId);

                //get DAY and PERIOD template of a Batch whose module is to be rescheduled 
                List<string> daysTemplate = get_DayTemplateList(courseSchedule.FirstOrDefault().BatchClass.BatchId);
                List<string> periodsTemplate = get_PeriodTemplateList(courseSchedule.FirstOrDefault().BatchClass.BatchId);

                //List<UnscheduledResource> UnscheduledResourceList = new List<UnscheduledResource>();

                //UnscheduledResource unScheduledResource = null;

                //foreach (var instructor in potentialInstructors)
                //{
                //unScheduledResource = new UnscheduledResource();

                //unScheduledResource.FreeInstructor = new FreeInstructor
                //{
                //    InstructorId = instructor.InstructorId,
                //    NameAndCompanyId = instructor.Instructor.Person.FirstName.ToUpper().Substring(0, 3) + " " + instructor.Instructor.Person.FirstName.ToUpper().Substring(0, 1) + ". " + instructor.Instructor.Person.CompanyId
                //};

                //List<FreeDate> FreeDateList = new List<FreeDate>();
                //FreeDate FreeDate = null;

                //FreeDate = new FreeDate();
                //FreeDate.Date = date.Date.ToString("dd/MM/yyyy");

                //Get free Period and Class Room combination/s of specific day
                var freePeriodAndRoom = periodClassCombination.Where(PRC => !(moduleSchedules.Any(ms => (ms.PeriodId == PRC.PeriodId)
                    && (ms.ClassRoomId == PRC.ClassRoomId)))).ToList();

                var PeiodRoomGroup = freePeriodAndRoom.GroupBy(x => new { x.PeriodId }).Select(grp => grp.ToList()).ToList();

                List<FreePeriod> FreePeriodList = new List<FreePeriod>();
                FreePeriod FreePeriod = null;
                foreach (var periodAndroom in PeiodRoomGroup)
                {
                    string day = date.Date.ToString("dddd");
                    string period = PeriodList.Single(p => p.PeriodId == periodAndroom.FirstOrDefault().PeriodId).Period;

                    //Check if this instructor is free at this DAY and PERIOD
                    if (IsThisInstructorFree(moduleInstructorSchedule, date.Date, periodAndroom.FirstOrDefault().PeriodId))
                    {
                        //Check whether it is a suitable Batch Day template and Period template, Does Batch Class Has Another Class 
                        //and weather the instructor has no class                
                        if (Is_ValidDayAndPeriodForBatch(daysTemplate, periodsTemplate, day, period) && !DoesBatchClassHasAnotherScheduledClass(-1, courseSchedule.FirstOrDefault().BatchClassId, date, periodAndroom.FirstOrDefault().PeriodId))
                        {
                            FreePeriod = new FreePeriod();
                            FreePeriod.Period = PeriodList.Single(p => p.PeriodId == periodAndroom.FirstOrDefault().PeriodId);
                            List<ClassRoom> ClassRooms = new List<ClassRoom>();
                            foreach (var room in periodAndroom)
                            {
                                //Check the room is reserved or not
                                if (!IsRoomReserved(date.Date, periodAndroom.FirstOrDefault().PeriodId, room.ClassRoomId))
                                {
                                    ClassRooms.Add(ClassRoomList.Single(CR => CR.ClassRoomId == room.ClassRoomId));
                                }
                            }
                            if (ClassRooms.Count > 0)
                                FreePeriod.ClassRooms.AddRange(ClassRooms);

                            if (FreePeriod.ClassRooms.Count > 0)
                                FreePeriodList.Add(FreePeriod);
                        }
                    }
                }
                return FreePeriodList.ToList();
            }
            catch (Exception ex)
            {
                return new List<FreePeriod>();
            }
        }


        public bool Is_ValidDayAndPeriodForBatch(List<string> daytemplate, List<string> PeriodTemplate, string day, string period)
        {
            try
            {
                if (daytemplate.Contains(day) && PeriodTemplate.Contains(period))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DoesBatchClassHasAnotherScheduledClass(int moduleScheduleId, int batchClassId, DateTime date, int periodId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var moduleScheduleBO = dbContext.ModuleSchedules.Where(ms => (ms.ModuleScheduleId != moduleScheduleId)
                && (ms.PhaseSchedule.BatchClassId == batchClassId)
                    && ((ms.Date == date) && (ms.PeriodId == periodId))).ToList();
                if (moduleScheduleBO.Count > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<PeriodClassCombination> get_PeriodClassRoomCombination(List<PeriodView> Periods, List<ClassRoom> ClassRooms)
        {
            try
            {
                List<PeriodClassCombination> periodClassCombination = new List<PeriodClassCombination>();
                foreach (var period in Periods)
                {
                    foreach (var room in ClassRooms)
                    {
                        periodClassCombination.Add(new PeriodClassCombination
                        {
                            PeriodId = period.PeriodId,
                            ClassRoomId = room.ClassRoomId
                        });
                    }
                }
                return periodClassCombination;
            }
            catch (Exception ex)
            {
                return new List<PeriodClassCombination>();
            }
        }

        public List<ModuleInstructorSchedule> GetFreeInstructors(List<ModuleInstructorSchedule> potentialInstructors, DateTime date, int periodId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<ModuleInstructorSchedule> freeInstructorList = new List<ModuleInstructorSchedule>();
                foreach (var instructor in potentialInstructors)
                {
                    var moduleSchedules = dbContext.ModuleSchedules.Where(MS => (MS.InstructorId == instructor.InstructorId)
                  && ((MS.Date == date) && (MS.PeriodId == periodId))).ToList();
                    if (freeInstructorList.Count == 0)
                    {
                        freeInstructorList.Add(instructor);
                    }
                }
                if (freeInstructorList.Count > 0)
                {
                    return freeInstructorList.ToList();
                }
                return new List<ModuleInstructorSchedule>();
            }
            catch (Exception ex)
            {
                return new List<ModuleInstructorSchedule>();
            }
        }

        public List<FilterView> Filter(string filterBy)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                ////BatchClass,Instructor,Program
                if (filterBy.Equals("BatchClass"))
                {
                    var result = dbContext.BatchClasses.ToList();
                    if (result.Count > 0)
                    {
                        var filteredResult = result.Select(item => new FilterView
                        {
                            Id = item.BatchClassId,
                            Name = item.BatchClassName
                        }).ToList();
                        if (filteredResult.Count > 0)
                            return filteredResult.ToList();
                    }
                }
                else if (filterBy.Equals("Program"))
                {
                    var result = dbContext.Programs.ToList();
                    if (result.Count > 0)
                    {
                        var filteredResult = result.Select(item => new FilterView
                        {
                            Id = item.ProgramId,
                            Name = item.ProgramName
                        }).ToList();
                        if (filteredResult.Count > 0)
                            return filteredResult.ToList();
                    }
                }
                else if (filterBy.Equals("Instructor"))
                {
                    var result = dbContext.ModuleInstructorSchedules.ToList();
                    var resultGroup = result.GroupBy(x => new { x.InstructorId }).Select(grp => grp.ToList()).ToList();
                    List<ModuleInstructorSchedule> InstructorList = new List<ModuleInstructorSchedule>();
                    foreach (var MISList in resultGroup)
                    {
                        InstructorList.Add(MISList.FirstOrDefault());
                    }
                    if (result.Count > 0)
                    {
                        var filteredResult = InstructorList.Select(item => new FilterView
                        {
                            Id = item.InstructorId,
                            Name = item.Instructor.Person.FirstName + " " + item.Instructor.Person.MiddleName + " - " + item.Instructor.Person.CompanyId
                        }).ToList();
                        if (filteredResult.Count > 0)
                            return filteredResult.ToList();
                    }
                }
                return new List<FilterView>();
            }
            catch (Exception)
            {
                return new List<FilterView>();
            }
        }
        public List<CourseModule> GetCourseModule(int courseId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();////BatchClass,Instructor,Program

                var result = dbContext.CourseModules.Where(CM => CM.CourseCategory.CourseId == courseId).ToList();
                if (result.Count > 0)
                {
                    return result.ToList();
                }
                return new List<CourseModule>();
            }
            catch (Exception)
            {
                return new List<CourseModule>();
            }
        }
        public AttendanceView GetAttendaceData(int moduleScheduleId)
        {
            try
            {
                PTSContext db = new PTSContext();
                AttendanceView attendanceView = new AttendanceView();
                ModuleSchedule moduleSchedule = db.ModuleSchedules.Single(MS => MS.ModuleScheduleId == moduleScheduleId);
                // Get All Trainee of a specific Batch Class
                List<TraineeBatchClass> TraineeBatchClass = db.TraineeBatchClasses.Where(TBC => TBC.BatchClassId == moduleSchedule.PhaseSchedule.BatchClassId).ToList();
                //Get Class Rooms
                List<ClassRoom> ClassRoomList = (List<ClassRoom>)classRoomAccess.List();
                //Get Potential Instructor
                List<ModuleInstructorSchedule> potentialInstructors = moduleInstructorScheduleAccess.PotentialInstructorList(moduleSchedule.ModuleId);

                List<TraineeView> TraineeList = new List<TraineeView>();
                if (TraineeBatchClass.Count > 0)
                {
                    TraineeList = TraineeBatchClass.Select(trainee => new TraineeView
                    {
                        TraineeId = trainee.TraineeId,
                        TraineeNameAndCompanyId = trainee.Trainee.Person.FirstName.Substring(0, 3) + ' ' + trainee.Trainee.Person.MiddleName.Substring(0, 1) + ". " + trainee.Trainee.Person.CompanyId
                    }).ToList();
                }
                List<PotentialInstructorView> instructors = new List<PotentialInstructorView>();
                if (potentialInstructors.Count > 0)
                {
                    instructors = potentialInstructors.Select(instructor => new PotentialInstructorView
                    {
                        InstructorId = instructor.InstructorId,
                        NameAndCompanyId = instructor.Instructor.Person.FirstName.Substring(0, 3) + ' ' + instructor.Instructor.Person.MiddleName.Substring(0, 1) + ". " + instructor.Instructor.Person.CompanyId
                    }).ToList();
                }
                if (TraineeBatchClass.Count > 0 && potentialInstructors.Count > 0 && ClassRoomList.Count > 0)
                {
                    attendanceView.ClassRooms.AddRange(ClassRoomList);
                    attendanceView.PotentialInstructors.AddRange(instructors);
                    attendanceView.Trainees.AddRange(TraineeList);
                    return attendanceView;
                }
                return new AttendanceView();
            }
            catch (Exception ex)
            {
                return new AttendanceView();
            }
        }

        /*Begin Newly added methods*/
        public bool IsThisInstructorFree(ModuleInstructorSchedule instructor, DateTime date, int periodId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var scheduledEvent = dbContext.ModuleSchedules.Where(MS => (MS.InstructorId == instructor.InstructorId)
              && ((MS.Date == date) && (MS.PeriodId == periodId))).ToList();
                if (scheduledEvent.Count == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsRoomReserved(DateTime date, int periodId, int roomId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var scheduledEvent = dbContext.ModuleSchedules.Where(MS => ((MS.Date == date) && (MS.PeriodId == periodId) && (MS.ClassRoomId == roomId))).ToList();
                if (scheduledEvent.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<FreeInstructor> get_FreeInstructorForSpecificDate(DateTime date)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var allInstructors = dbContext.ModuleInstructorSchedules.ToList().GroupBy(MIS => MIS.InstructorId).Select(grp => grp.FirstOrDefault());
                var busyInstructors = dbContext.ModuleSchedules.Where(MS => MS.Date == date).ToList().GroupBy(Ins => Ins.InstructorId).Select(grp => grp.FirstOrDefault()).ToList();

                List<FreeInstructor> allInstructorList = new List<FreeInstructor>();
                foreach (var instructor in allInstructors)
                {
                    allInstructorList.Add(new FreeInstructor
                                      {
                                          InstructorId = instructor.InstructorId,
                                          NameAndCompanyId = instructor.Instructor.Person.FirstName.Substring(0, 3) + " " + instructor.Instructor.Person.FirstName.Substring(0, 1) + ". " + instructor.Instructor.Person.CompanyId
                                      });
                }

                List<FreeInstructor> busyInstructorList = new List<FreeInstructor>();
                foreach (var instructor in busyInstructors)
                {
                    busyInstructorList.Add(new FreeInstructor
                    {
                        InstructorId = instructor.InstructorId,
                        NameAndCompanyId = instructor.Instructor.Person.FirstName.Substring(0, 3) + " " + instructor.Instructor.Person.FirstName.Substring(0, 1) + ". " + instructor.Instructor.Person.CompanyId
                    });
                }
                var FreeInstructors = allInstructorList.Except(busyInstructorList);
                return FreeInstructors.ToList();
            }
            catch (Exception ex)
            {
                return new List<FreeInstructor>();
            }
        }

        /*End Newly added methods*/



        //UNUSED METHODS 
        public bool InstructorHasAnotherClass(List<ModuleSchedule> scheduledModulesOfSpecificDay, ModuleSchedule moduleSchedule, DateTime date, int periodId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var moduleSchedules = scheduledModulesOfSpecificDay.Where(ms => (ms.ModuleScheduleId != moduleSchedule.ModuleScheduleId)
                && (ms.InstructorId == moduleSchedule.InstructorId)
                    && ((ms.Date == date) && (ms.PeriodId == periodId))).ToList();

                if (moduleSchedules.Count > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<int> get_FreePeriod(List<ModuleSchedule> ModuleScheduleList, List<PeriodView> Periods)
        {
            //foreach (var moduleSchedule in moduleSchedulesGroup)
            // {
            //unScheduledResourceView = new UnScheduledResourceView();
            //unScheduledResourceView.Dates = moduleSchedule.FirstOrDefault().Date;
            //List<int> freePeriod = get_FreePeriod(moduleSchedule, PeriodList);

            //List<PeriodListView> periodListView = new List<PeriodListView>();
            //PeriodListView periodsView = null;
            //foreach (int periodId in freePeriod)
            //{
            //    periodsView = new PeriodListView();

            //    freeClassRoom = get_FreeClassRoom(periodId, moduleSchedule, ClassRoomList);

            //    PeriodView periodView = null;
            //    if (freePeriod.Count > 0 && freeClassRoom.Count > 0)
            //    {
            //        periodView = new PeriodView
            //        {
            //            PeriodId = periodId,
            //            Period = PeriodList.Single(p => p.PeriodId == periodId).Period
            //        };
            //        foreach (int classRoomId in freeClassRoom)
            //        {
            //            tempClassRooms.Add(ClassRoomList.Single(CR => CR.ClassRoomId == classRoomId));
            //        }
            //        if (periodView != null && tempClassRooms.Count > 0)
            //        {
            //            periodsView.Period = periodView;
            //            periodsView.ClassRooms = tempClassRooms;
            //        }
            //    }
            //    periodListView.Add(periodsView);
            //}
            //if (periodListView.Count > 0)
            //{
            //    unScheduledResourceView.Periods.AddRange(periodListView);
            //}
            //UnScheduledResourceList.Add(unScheduledResourceView);
            //}
            try
            {
                var scheduledPeriodIdList = ModuleScheduleList.Select(ms => ms.PeriodId).ToList();
                var unionPeriodIdList = Periods.Select(P => P.PeriodId).ToList();
                var complimentPeriodIdList = unionPeriodIdList.Where(p => !(scheduledPeriodIdList.Contains(p))).ToList();
                if (complimentPeriodIdList.Count > 0)
                {
                    return complimentPeriodIdList.ToList();
                }
                return new List<int>();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }
        public List<int> get_FreeClassRoom(int periodId, List<ModuleSchedule> ModuleScheduleList, List<ClassRoom> ClassRooms)
        {
            try
            {
                var ModuleSchedules = ModuleScheduleList.Where(ms => ms.PeriodId == periodId).ToList();
                var scheduledClassRoomIdList = ModuleSchedules.Select(ms => ms.ClassRoomId).ToList();
                var unionClassRoomIdList = ClassRooms.Select(CR => CR.ClassRoomId).ToList();
                var complimentClassRoomIdList = unionClassRoomIdList.Where(CR => !(scheduledClassRoomIdList.Contains(CR))).ToList();
                if (complimentClassRoomIdList.Count > 0)
                {
                    return complimentClassRoomIdList.ToList();
                }
                return new List<int>();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }
        #endregion

    }
    public class Batchdays
    {
        public string DayName { get; set; }
        public int BatchId { get; set; }
    }

    public class BatchPeriod
    {
        public string Period { get; set; }
        public int BatchId { get; set; }
    }
}
