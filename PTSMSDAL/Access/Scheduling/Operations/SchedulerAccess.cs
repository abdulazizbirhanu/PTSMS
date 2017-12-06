using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;

namespace PTSMSDAL.Access.Scheduling.Operations
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

                var moduleInstructorSchedules = (from CM in dbContext.BatchModules
                                                 join PS in dbContext.PhaseSchedules on CM.BatchCourse.BatchCategory.BatchId equals PS.BatchId
                                                 where CM.Sequence >= 1.0 && CM.PhaseId == PS.PhaseId //a minimum of Course Sequence 1 and any Module Sequence 
                                                 select new
                                                 {
                                                     PS,
                                                     CM
                                                 }).ToList();

                foreach (var item in moduleInstructorSchedules)
                {
                    BatchModuleList.Add(new BatchModuleView
                    {
                        BatchId = item.PS.BatchId,
                        PhaseId = item.PS.PhaseId,
                        //BatchClassId = item.PS.BatchClassId,
                        PhaseScheduleId = item.PS.PhaseScheduleId,
                        CourseId = item.CM.BatchCourse.CourseId,
                        ModuleId = item.CM.ModuleId,
                        StartingDate = item.PS.StartingDate,
                        CourseSequence = int.TryParse(item.CM.Sequence.ToString(CultureInfo.InvariantCulture).Split('.')[0], out sequence) ? sequence : 1,
                        ModuleSequence = int.TryParse(item.CM.Sequence.ToString(CultureInfo.InvariantCulture).Split('.')[1], out sequence) ? sequence : 1,
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

        public bool CancelGroundSchedule(string reason, string remark, string moduleScheduleId)
        {
            try
            {
                PTSContext db = new PTSContext();
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

                int id = Convert.ToInt16(moduleScheduleId);
                var moduleSchedule = db.ModuleSchedules.Where(ms => ms.Status != statusName && ms.ModuleScheduleId == id).ToList().FirstOrDefault();
                if (moduleSchedule == null)
                    return false;
                else
                {
                    moduleSchedule.Status = statusName;
                    moduleSchedule.Reason = reason;
                    moduleSchedule.Remark = remark;
                    db.Entry(moduleSchedule).State = EntityState.Modified;
                    return db.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
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


        public List<PeriodView> get_PeriodTemplateList(int BatchId)
        {
            try
            {
                List<PeriodView> PeriodList = new List<PeriodView>();
                var DayListVar = from BT in db.Batches
                                 join Pt in db.PeriodTemplates on BT.PeriodTemplateId equals Pt.PeriodTemplateId
                                 join P in db.Periods on Pt.PeriodTemplateId equals P.PeriodTemplateId
                                 where BT.BatchId == BatchId && P.Status.ToLower().Equals("active") && Pt.Status.ToLower().Equals("active")
                                 select new
                                 {
                                     Periods = P.StartTime.ToString() + "-" + P.EndTime.ToString(),
                                     PeriodId = P.PeriodId
                                 };
                foreach (var item in DayListVar)
                {
                    PeriodList.Add(new PeriodView
                    {
                        Period = item.Periods,
                        PeriodId = item.PeriodId
                    });
                }
                return PeriodList;
            }
            catch (Exception ex)
            {
                return new List<PeriodView>();
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
                var courseModules = (from CM in dbContext.BatchModules
                                     join PS in dbContext.PhaseSchedules on CM.BatchCourse.BatchCategory.BatchId equals PS.BatchId
                                     where CM.Sequence >= 1.0 && CM.PhaseId==PS.PhaseId //a minimum of Course Sequence 1 and any Module Sequence
                                     select new
                                     {
                                         CM,
                                         PS
                                     }).ToList();

                foreach (var item in courseModules)
                {
                    CourseModuleList.Add(new CourseModuleView
                    {
                        BatchId = item.PS.BatchId,
                        PhaseId = item.PS.PhaseId,
                        CourseId = item.CM.BatchCourse.CourseId,
                        ModuleId = item.CM.ModuleId,
                        ModuleSequence = int.TryParse(item.CM.Sequence.ToString(CultureInfo.InvariantCulture).Split('.')[0], out sequence) ? sequence : 1,
                        CourseSequence = int.TryParse(item.CM.Sequence.ToString(CultureInfo.InvariantCulture).Split('.')[1], out sequence) ? sequence : 1
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
                PTSContext dbContext = new PTSContext();
                foreach (ModuleSchedule moduleSchedule in moduleScheduleList)
                {
                    //moduleSchedule.RevisionNo = index++;
                    //moduleSchedule.EffectiveDate = DateTime.Now;
                    moduleSchedule.StartDate = DateTime.Now;
                    moduleSchedule.EndDate = new DateTime(9999, 12, 31);
                    moduleSchedule.CreationDate = DateTime.Now;
                    moduleSchedule.CreatedBy = HttpContext.Current.User.Identity.Name;
                    moduleSchedule.RevisionDate = DateTime.Now;
                    moduleSchedule.RevisedBy = HttpContext.Current.User.Identity.Name;
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

        public ModuleSchedule ModuleScheduleDetails(int id)
        {
            try
            {
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

                PTSContext db = new PTSContext();
                ModuleSchedule moduleSchedule = db.ModuleSchedules.Where(ms => ms.Status != statusName && ms.ModuleScheduleId == id).ToList().FirstOrDefault();
                return moduleSchedule;
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }
        #endregion

        #region ScheduleReportDisplay
        public List<InstructorColor> ListScheduledInstructors()
        {
            PTSContext dbContext = new PTSContext();
            var random = new Random();
            string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

            List<Scheduler> ScheduledEvents = new List<Scheduler>();
            var scheduleList = dbContext.ModuleSchedules.Where(ms => ms.Status != statusName).ToList();

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
        public List<EquipmentColor> ListScheduledEquipment()
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                var random = new Random();

                var equipmentScheduleList = dbContext.FlyingFTDSchedules.ToList();

                var EquipmentGroup = equipmentScheduleList.GroupBy(x => new { x.EquipmentId }).Select(grp => grp.FirstOrDefault()).ToList();
                //FFA500
                List<EquipmentColor> EquipmentList = new List<EquipmentColor>();
                string color = "";
                foreach (var item in EquipmentGroup)
                {
                    color = String.Format("#{0:X6}", random.Next(0x1000000));
                    //int Num;
                    GenerateAnotherUniqueColorCode:
                    color = String.Format("#{0:X6}", random.Next(0x1000000));
                    if (color == "FFA500")
                        goto GenerateAnotherUniqueColorCode;

                    EquipmentList.Add(new EquipmentColor
                    {
                        EquipmentId = item.EquipmentId,
                        Color = color
                    });
                }
                //List<EquipmentColor> EquipmentList = EquipmentGroup.Select(MS => new EquipmentColor
                //{
                //    EquipmentId = MS.EquipmentId,
                //    Color = String.Format("#{0:X6}", random.Next(0x1000000))
                //}).ToList();

                if (EquipmentList.Count > 0)
                    return EquipmentList.ToList();

                return new List<EquipmentColor>();

            }
            catch (Exception et)
            {
                return new List<EquipmentColor>();
            }

        }

        public List<BatchClassColor> BatchClassColorList()
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                var random = new Random();

                var result = db.PhaseSchedules.ToList();
                var phaseSchedules = result.GroupBy(PS => PS.BatchId).Select(grp => grp.FirstOrDefault()).ToList();

                List<BatchClassColor> BatchClassColorList = phaseSchedules.Select(PS => new BatchClassColor
                {
                    BatchId = PS.BatchId,
                    Color = String.Format("#{0:X6}", random.Next(0x1000000))
                }).ToList();

                if (BatchClassColorList.Count > 0)
                    return BatchClassColorList.ToList();

                return new List<BatchClassColor>();
            }
            catch (Exception et)
            {
                return new List<BatchClassColor>();
            }

        }

        public List<BatchView> GetValidBatchClassToBeMerged(int moduleScheduleId)
        {
            try
            {
                //	If the batch class to be merged is eligible for the module,
                //	not already taken the module and

                PTSContext db = new PTSContext();
                List<BatchView> batchList = new List<BatchView>();
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

                bool isBatchClassHasAnotherScheduleAtThisTime = false;
                var moduleScheduleList = db.ModuleSchedules.Where(ms => ms.Status != statusName).ToList();
                var moduleSchedule = moduleScheduleList.Where(ms => ms.ModuleScheduleId == moduleScheduleId).FirstOrDefault();

                var result = db.PhaseSchedules.Where(ps => ps.BatchId != moduleSchedule.PhaseSchedule.BatchId).ToList();
                var batchClasses = result.GroupBy(PS => PS.BatchId).Select(grp => grp.FirstOrDefault()).ToList();


                if (moduleSchedule != null)
                {
                    foreach (var batchClass in batchClasses)
                    {
                        //Constraint ONE, If the batch class to be merged is eligible for the module,
                        var batchModuleList = (from BM in db.BatchModules
                                               join PS in db.PhaseSchedules on BM.BatchCourse.BatchCategory.BatchId equals PS.BatchId
                                               where BM.PhaseId == PS.PhaseId
                                               select new
                                               {
                                                   BM
                                               }).ToList();
                        if (batchModuleList.Count > 0)
                        {

                            //Constraint TWO, Is not scheduled for another class during that time slot.
                            var batchClassScheduledEvents = moduleScheduleList.Where(MS => (MS.PhaseSchedule.BatchId == batchClass.BatchId) && (MS.Date == moduleSchedule.Date)).ToList();


                            DateTime periodStartTime = Convert.ToDateTime(Convert.ToDateTime(moduleSchedule.Date + TimeSpan.Parse(moduleSchedule.Period.StartTime)));
                            DateTime periodEndTime = Convert.ToDateTime(Convert.ToDateTime(moduleSchedule.Date + TimeSpan.Parse(moduleSchedule.Period.EndTime)));

                            DateTime scheduledStartTime = DateTime.Now;
                            DateTime scheduledEndTime = DateTime.Now;
                            foreach (var schedule in batchClassScheduledEvents)
                            {
                                scheduledStartTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(schedule.Period.StartTime)));
                                scheduledEndTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(schedule.Period.EndTime)));

                                if ((schedule.PeriodId == moduleSchedule.PeriodId) || ((periodStartTime >= scheduledStartTime && periodStartTime <= scheduledEndTime) || (periodEndTime >= scheduledStartTime && periodEndTime <= scheduledEndTime)))
                                {
                                    isBatchClassHasAnotherScheduleAtThisTime = true;
                                    break;
                                }
                            }
                            if (!isBatchClassHasAnotherScheduleAtThisTime)
                            {
                                batchList.Add(new BatchView
                                {
                                    Id = batchClass.BatchId,
                                    Name = batchClass.Batch.BatchName
                                });
                            }
                        }
                    }
                }
                return batchList;
            }
            catch (Exception ex)
            {
                return new List<BatchView>();
            }
        }

        public string GetCourseModule(int moduleScheduleId, string batchClassList, int moduleId)
        {

            try
            {
                PTSContext db = new PTSContext();

                var existingModuleSchedule = db.ModuleSchedules.Find(moduleScheduleId);

                //int d = Convert.ToInt32(phaseScheduleId.ToString());
                if (existingModuleSchedule != null)
                {
                    string[] batchClassArray = batchClassList.Split(',');
                    foreach (var batchClass in batchClassArray)
                    {
                        if (!string.IsNullOrEmpty(batchClass))
                        {
                            int batchID = Int16.Parse(batchClass);
                            var phaseScheduleId = from phase in db.PhaseSchedules
                                                  join batchModule in db.BatchModules on phase.BatchId equals batchModule.BatchCourse.BatchCategory.BatchId
                                                  where phase.BatchId == batchID && batchModule.ModuleId == moduleId && phase.PhaseId==batchModule.PhaseId
                                                  select new { phase.PhaseScheduleId };

                            var phaseScheduleID = phaseScheduleId.FirstOrDefault();
                            var newModuleSchedule = new ModuleSchedule();

                            //newModuleSchedule.ClassRoomId = Int16.Parse(batchClass);
                            newModuleSchedule.ClassRoomId = existingModuleSchedule.ClassRoomId;// Int16.Parse(batchClass);
                            newModuleSchedule.ParentScheduleId = existingModuleSchedule.ModuleScheduleId;
                            newModuleSchedule.Date = existingModuleSchedule.Date;
                            newModuleSchedule.ModuleId = existingModuleSchedule.ModuleId;
                            //newModuleSchedule.PhaseScheduleId = existingModuleSchedule.PhaseScheduleId;
                            newModuleSchedule.PhaseScheduleId = phaseScheduleID.PhaseScheduleId;
                            newModuleSchedule.PeriodId = existingModuleSchedule.PeriodId;
                            newModuleSchedule.Status = existingModuleSchedule.Status;
                            newModuleSchedule.Remark = existingModuleSchedule.Remark;
                            newModuleSchedule.Reason = existingModuleSchedule.Reason;
                            newModuleSchedule.InstructorId = existingModuleSchedule.InstructorId;

                            newModuleSchedule.StartDate = DateTime.Now;
                            newModuleSchedule.EndDate = new DateTime(9999, 12, 31);
                            newModuleSchedule.CreationDate = DateTime.Now;
                            newModuleSchedule.RevisionDate = DateTime.Now;
                            newModuleSchedule.CreatedBy = HttpContext.Current.User.Identity.Name;
                            newModuleSchedule.RevisedBy = HttpContext.Current.User.Identity.Name;

                            db.ModuleSchedules.Add(newModuleSchedule);
                        }
                    }
                    if (db.SaveChanges() > 0)
                        return "Batch class has successfully merged.";
                    return "Failed to merge batch class.";
                }
                return "Failed to merge batch class. Existing schedule is not found.";
            }
            catch (Exception ex)
            {
                return "Error has occured. " + ex.Message;
            }
        }

        public List<Scheduler> ScheduledEventList(string month, string year)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<Scheduler> ScheduledEvents = new List<Scheduler>();
                //var result = dbContext.ModuleSchedules.ToList();

                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                {
                    startDate = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }

                var scheduledEvents = (
                   from CM in db.CourseModules
                   join MS in db.ModuleSchedules on CM.ModuleId equals MS.Module.RevisionGroupId == null ? MS.ModuleId : MS.Module.RevisionGroupId
                   where MS.Date >= startDate && MS.Date <= endDate
                   select new
                   {
                       CM,
                       MS
                   }).ToList();

                var distinictModuleSchedule = scheduledEvents.GroupBy(MS => MS.MS.ModuleScheduleId).Select(grp => grp.FirstOrDefault()).ToList();

                var ScheduledEventList = distinictModuleSchedule.Select(item => new Scheduler
                {
                    EventID = item.MS.ModuleScheduleId,
                    ResourceId = item.MS.PhaseSchedule.BatchId,
                    Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                                              + "</strong><br/>- Course Code: <strong>" + item.CM.CourseCategory.Course.CourseCode + "-" + item.CM.CourseCategory.Course.CourseTitle
                                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode + "-" + item.MS.Module.ModuleTitle
                                             + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName
                                             //  + "</strong><br/>- Scheduled Date: from <strong>" + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("MM/dd/yyyy HH:mm"))
                                             + "</strong><br/>- Status: <strong>" + item.MS.Status + "</strong>",
                    Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ". -" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "-" + item.MS.ClassRoom.Building.BuildingName
                                             + "-" + item.MS.ClassRoom.RoomNo
                                             + "-" + item.CM.CourseCategory.Course.CourseCode
                                             + "-" + item.MS.Module.ModuleCode
                                             + "~" + item.MS.InstructorId,
                    EventStart = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))), //
                    EventEnd = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))),
                    IsAllDay = false,
                    CourseID = item.CM.CourseCategory.Course.CourseId,
                    CourseCode = item.CM.CourseCategory.Course.CourseCode,
                    ModuleID = item.MS.Module.ModuleId,
                    ModuleCode = item.MS.Module.ModuleCode,
                    BatchID = item.MS.PhaseSchedule.BatchId,
                    BatchClassName = item.MS.PhaseSchedule.Batch.BatchName
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
        public List<Scheduler> GetScheduledEventForTrainee(string companyId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<Scheduler> ScheduledEvents = new List<Scheduler>();
                //var result = dbContext.ModuleSchedules.ToList();
                var scheduledEvents = (
                   from CM in db.BatchModules
                   join MS in db.ModuleSchedules on CM.ModuleId equals MS.Module.RevisionGroupId == null ? MS.ModuleId : MS.Module.RevisionGroupId
                   join TBC in db.TraineeBatchClasses on CM.BatchCourse.BatchCategory.BatchId equals TBC.BatchClass.BatchId
                   where TBC.Trainee.Person.CompanyId == companyId
                   select new
                   {
                       CM,
                       MS
                   }).ToList();

                var distinictModuleSchedule = scheduledEvents.GroupBy(MS => MS.MS.ModuleScheduleId).Select(grp => grp.FirstOrDefault()).ToList();

                var ScheduledEventList = distinictModuleSchedule.Select(item => new Scheduler
                {
                    EventID = item.MS.ModuleScheduleId,
                    ResourceId = item.MS.PhaseSchedule.BatchId,
                    Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                                              + "</strong><br/>- Course Code: <strong>" + item.CM.BatchCourse.Course.CourseCode + "-" + item.CM.BatchCourse.Course.CourseTitle
                                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode + "-" + item.MS.Module.ModuleTitle
                                              + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName
                                             + "</strong><br/>- Scheduled Date: from <strong>" + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("MM/dd/yyyy HH:mm"))
                                             + "</strong><br/>- Status: <strong>" + item.MS.Status + "</strong>",
                    Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ". -" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "-" + item.MS.ClassRoom.Building.BuildingName
                                             + "-" + item.MS.ClassRoom.RoomNo
                                             + "-" + item.CM.BatchCourse.Course.CourseCode
                                             + "-" + item.MS.Module.ModuleCode,
                    EventStart = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))), //
                    EventEnd = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))),
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
        public List<Scheduler> GetScheduledEventForInstructor(string companyId, string month, string year)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<Scheduler> ScheduledEvents = new List<Scheduler>();

                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                {
                    startDate = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), 1);
                    endDate = startDate.AddMonths(1).AddMilliseconds(-1);
                }

                var scheduledEvents = (
                   from CM in db.CourseModules
                   join MS in db.ModuleSchedules on CM.ModuleId equals MS.Module.RevisionGroupId == null ? MS.ModuleId : MS.Module.RevisionGroupId
                   where MS.Instructor.Person.CompanyId == companyId && MS.Date >= startDate && MS.Date <= endDate
                   select new
                   {
                       CM,
                       MS
                   }).ToList();
                List<Scheduler> SchedulerList = new List<Scheduler>();
                var distinictModuleSchedule = scheduledEvents.GroupBy(MS => MS.MS.ModuleScheduleId).Select(grp => grp.FirstOrDefault()).ToList();
                //foreach (var item in distinictModuleSchedule)
                //{
                //    //item.MS.Date != null && item.MS.Period.StartTime != null && item.MS.Period.EndTime != null ? Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("MM/dd/yyyy HH:mm")) : item.MS.Date + " " + item.MS.Period.StartTime + " - " + item.MS.Date + " " + item.MS.Period.EndTime
                //    DateTime eventStart = item.MS.Date != null && item.MS.Period.StartTime != null ? Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("dd/MM/yyyy HH:mm")) : item.MS.Date; // Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime)));
                //    DateTime eventEnd = item.MS.Date != null && item.MS.Period.EndTime != null ? Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("dd/MM/yyyy HH:mm")) : item.MS.Date; //Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime)));

                //    SchedulerList.Add(new Scheduler
                //    {
                //        EventID = item.MS.ModuleScheduleId,
                //        ResourceId = item.MS.PhaseSchedule.BatchClassId,
                //        Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                //                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                //                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.BatchClass.BatchClassName
                //                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                //                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                //                             + "</strong><br/>- Course Code: <strong>" + item.CM.CourseCategory.Course.CourseCode
                //                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode
                //                             + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName
                //                             + "</strong><br/>- Scheduled Date: from <strong>" + eventStart + " - " + eventEnd
                //                             + "</strong><br/>- Status: <strong>" + item.MS.Status + "</strong>",
                //        Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                //                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                //                             + ". -" + item.MS.PhaseSchedule.BatchClass.BatchClassName
                //                             + "-" + item.MS.ClassRoom.Building.BuildingName
                //                             + "-" + item.MS.ClassRoom.RoomNo
                //                             + "-" + item.CM.CourseCategory.Course.CourseCode
                //                             + "-" + item.MS.Module.ModuleCode,
                //        EventStart = eventStart, //
                //        EventEnd = eventEnd,
                //        IsAllDay = false,
                //        CourseID = item.CM.CourseCategory.Course.CourseId,
                //        CourseCode = item.CM.CourseCategory.Course.CourseCode,
                //        ModuleID = item.MS.Module.ModuleId,
                //        ModuleCode = item.MS.Module.ModuleCode,
                //        BatchClassID = item.MS.PhaseSchedule.BatchClass.BatchClassId,
                //        BatchClassName = item.MS.PhaseSchedule.BatchClass.BatchClassName
                //    });
                //}
                var ScheduledEventList = distinictModuleSchedule.Select(item => new Scheduler
                {
                    EventID = item.MS.ModuleScheduleId,
                    ResourceId = item.MS.PhaseSchedule.BatchId,
                    Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                                             + "</strong><br/>- Course Code: <strong>" + item.CM.CourseCategory.Course.CourseCode + "-" + item.CM.CourseCategory.Course.CourseTitle
                                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode + "-" + item.MS.Module.ModuleTitle
                                            + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName
                                             + "</strong><br/>- Scheduled Date: from <strong>" + item.MS.Date != null && item.MS.Period.StartTime != null && item.MS.Period.EndTime != null ? Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("MM/dd/yyyy HH:mm")) : item.MS.Date + " " + item.MS.Period.StartTime + " - " + item.MS.Date + " " + item.MS.Period.EndTime
                                             + "</strong><br/>- Status: <strong>" + item.MS.Status + "</strong>",
                    Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ". -" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "-" + item.MS.ClassRoom.Building.BuildingName
                                             + "-" + item.MS.ClassRoom.RoomNo
                                             + "-" + item.CM.CourseCategory.Course.CourseCode
                                             + "-" + item.MS.Module.ModuleCode,
                    EventStart = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))), //
                    EventEnd = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))),
                    IsAllDay = false,
                    CourseID = item.CM.CourseCategory.Course.CourseId,
                    CourseCode = item.CM.CourseCategory.Course.CourseCode,
                    ModuleID = item.MS.Module.ModuleId,
                    ModuleCode = item.MS.Module.ModuleCode,
                    BatchID = item.MS.PhaseSchedule.BatchId,
                    BatchClassName = item.MS.PhaseSchedule.Batch.BatchName


                }).ToList();

                foreach (var scheduledEvent in ScheduledEventList)
                {
                    ScheduledEvents.Add(scheduledEvent);
                }
                return ScheduledEvents;
                // return SchedulerList.ToList();
            }
            catch (Exception ex)
            {
                return new List<Scheduler>();
            }
        }
        public List<EquipmentScheduler> GetFTDandFlyingScheduledEvent(int FlyingFTDScheduleId)
        {
            FlyingFTDSchedule FlyingFTDSchedule = db.FlyingFTDSchedules.Where(f => f.FlyingFTDScheduleId == FlyingFTDScheduleId).FirstOrDefault();
            List<EquipmentScheduler> EquipmentScheduledEventList = new List<EquipmentScheduler>();
            EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
            EquipmentScheduleBriefingDebriefing briefing = null;
            EquipmentScheduleBriefingDebriefing debriefing = null;

            var scheduledEvent = (
                   from TBC in db.TraineeBatchClasses
                   join FFS in db.FlyingFTDSchedules on TBC.TraineeId equals FFS.TraineeId
                   where FFS.FlyingFTDScheduleId == FlyingFTDScheduleId
                   select new
                   {
                       TBC,
                       FFS
                   }).FirstOrDefault();
            var briefingDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.FlyingFTDSchedule.ScheduleStartTime >= FlyingFTDSchedule.ScheduleStartTime && b.FlyingFTDSchedule.ScheduleStartTime <= FlyingFTDSchedule.ScheduleEndTime).ToList();

            briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, true, false, briefingDebriefingList);
            debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, false, true, briefingDebriefingList);

            EquipmentScheduledEventList.Add(new EquipmentScheduler
            {
                EventID = scheduledEvent.FFS.FlyingFTDScheduleId,
                ResourceId = scheduledEvent.FFS.EquipmentId,
                Description = " - Trainee Name: <strong>" + scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + " " + scheduledEvent.FFS.Trainee.Person.CompanyId
                                             + " </strong> <br/> - Instructor Name: <strong>" + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + scheduledEvent.TBC.BatchClass.BatchClassName
                                              + "</strong><br/>- Equipment: <strong>" + scheduledEvent.FFS.Equipment.NameOrSerialNo
                                             + "</strong><br/>- Location: <strong>" + scheduledEvent.FFS.Equipment.Location.LocationName
                                             + "</strong><br/>- Room No: <strong>" + scheduledEvent.FFS.Equipment.RoomNo
                                             + "</strong><br/>- Lesson Name: <strong>" + scheduledEvent.FFS.Lesson.LessonName
                                             + "</strong><br/>- Duration: <strong>" + scheduledEvent.FFS.ScheduleStartTime + " -  " + scheduledEvent.FFS.ScheduleEndTime
                                             + "</strong><br/>- Status: <strong>" + scheduledEvent.FFS.Status
                                             + "</strong><br/>- Briefing: <strong>" + (briefing != null ? (briefing.BriefingAndDebriefing.StartingTime + " - " + briefing.BriefingAndDebriefing.EndingTime) : "")
                                             + "</strong><br/>- Debriefing: <strong>" + (debriefing != null ? (debriefing.BriefingAndDebriefing.StartingTime + " - " + debriefing.BriefingAndDebriefing.EndingTime) : "") + "</strong>",

                Title = scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + ". & " + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1) + ".",
                EventStart = scheduledEvent.FFS.ScheduleStartTime, //
                EventEnd = scheduledEvent.FFS.ScheduleEndTime,
                IsAllDay = false
            });
            var briefingDebriefingGroupList = briefingDebriefingList.GroupBy(b => new { b.BriefingAndDebriefingId, b.FlyingFTDSchedule.EquipmentId }).Select(grp => grp.ToList()).ToList();


            string traineeList = "";
            int i = 0;
            foreach (var briefingGroup in briefingDebriefingGroupList)
            {
                i = 1;
                bool isThereActiveScheduleForThisBriefingAndDebriefing = false;
                foreach (var item in briefingGroup)
                {
                    if (i == 1)
                        traineeList = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                    else
                        traineeList = traineeList + " <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                    i++;
                    if (item.FlyingFTDSchedule.Status != "Canceled")
                    {
                        isThereActiveScheduleForThisBriefingAndDebriefing = true;
                    }
                }
                if (isThereActiveScheduleForThisBriefingAndDebriefing)
                {
                    EquipmentScheduledEventList.Add(new EquipmentScheduler
                    {
                        EventID = briefingGroup.FirstOrDefault().BriefingAndDebriefingId,
                        ResourceId = briefingGroup.FirstOrDefault().FlyingFTDSchedule.EquipmentId,
                        Description = " - Trainee/s Detail: <br /><strong>" + traineeList
                                        + "</strong><br/>- Instructor Name: <strong>" + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.FirstName + " " + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.MiddleName
                                        + "</strong><br/>- Duration: <strong>" + briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime + " -  " + briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime
                                        + "</strong><br/>- Type: <strong>" + (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing") + "<strong>",


                        Title = (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing"),
                        EventStart = briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime, //
                        EventEnd = briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime,
                        IsAllDay = false
                    });
                }
            }
            return EquipmentScheduledEventList;
        }
        public List<EquipmentScheduler> GetFTDandFlyingScheduledEvent(string month, string year)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                List<EquipmentScheduler> EquipmentScheduledEventList = new List<EquipmentScheduler>();
                EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                {
                    startDate = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), 1);
                    endDate = startDate.AddMonths(1).AddMilliseconds(-1);
                }

                var scheduledEvents = (
                   from TBC in db.TraineeBatchClasses
                   join FFS in db.FlyingFTDSchedules on TBC.TraineeId equals FFS.TraineeId
                   where FFS.ScheduleStartTime >= startDate && FFS.ScheduleStartTime <= endDate
                   select new
                   {
                       TBC,
                       FFS
                   }).ToList();

                var groupedResult = scheduledEvents.GroupBy(s => s.FFS.FlyingFTDScheduleId).Select(grp => grp.FirstOrDefault()).ToList();
                var briefingDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.FlyingFTDSchedule.ScheduleStartTime >= startDate && b.FlyingFTDSchedule.ScheduleStartTime <= endDate).ToList();


                EquipmentScheduleBriefingDebriefing briefing = null;
                EquipmentScheduleBriefingDebriefing debriefing = null;
                foreach (var scheduledEvent in groupedResult)
                {
                    briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, true, false, briefingDebriefingList);
                    debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, false, true, briefingDebriefingList);

                    EquipmentScheduledEventList.Add(new EquipmentScheduler
                    {
                        EventID = scheduledEvent.FFS.FlyingFTDScheduleId,
                        ResourceId = scheduledEvent.FFS.EquipmentId,
                        Description = " - Trainee Name: <strong>" + (!string.IsNullOrEmpty(scheduledEvent.FFS.Trainee.Person.ShortName) ? scheduledEvent.FFS.Trainee.Person.ShortName : scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + " " + scheduledEvent.FFS.Trainee.Person.CompanyId)
                                             + " </strong> <br/> - Instructor Name: <strong>" + (!string.IsNullOrEmpty(scheduledEvent.FFS.Instructor.Person.ShortName) ? scheduledEvent.FFS.Instructor.Person.ShortName : scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1))
                                             + ".</strong><br/>- Batch Class Name: <strong>" + scheduledEvent.TBC.BatchClass.BatchClassName
                                              + "</strong><br/>- Equipment: <strong>" + scheduledEvent.FFS.Equipment.NameOrSerialNo
                                             + "</strong><br/>- Location: <strong>" + scheduledEvent.FFS.Equipment.Location.LocationName
                                             + "</strong><br/>- Room No: <strong>" + scheduledEvent.FFS.Equipment.RoomNo
                                             + "</strong><br/>- Lesson Name: <strong>" + scheduledEvent.FFS.Lesson.LessonName
                                             + "</strong><br/>- Duration: <strong>" + scheduledEvent.FFS.ScheduleStartTime + " -  " + scheduledEvent.FFS.ScheduleEndTime
                                             + "</strong><br/>- Status: <strong>" + scheduledEvent.FFS.Status
                                             + "</strong><br/>- Briefing: <strong>" + (briefing != null ? (briefing.BriefingAndDebriefing.StartingTime + " - " + briefing.BriefingAndDebriefing.EndingTime) : "")
                                             + "</strong><br/>- Debriefing: <strong>" + (debriefing != null ? (debriefing.BriefingAndDebriefing.StartingTime + " - " + debriefing.BriefingAndDebriefing.EndingTime) : "") + "</strong>",

                        Title = !string.IsNullOrEmpty(scheduledEvent.FFS.Trainee.Person.ShortName) ? scheduledEvent.FFS.Trainee.Person.ShortName : scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + ". & " + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1) + ".",
                        EventStart = scheduledEvent.FFS.ScheduleStartTime, //
                        EventEnd = scheduledEvent.FFS.ScheduleEndTime,
                        IsAllDay = false
                    });
                }

                //Get briefing and debriefing time to display as an event with a unique color 
                //var equipmentScheduleBriefingDebriefingList = equipmentScheduleBriefingDebriefingAccess.List();
                var briefingDebriefingGroupList = briefingDebriefingList.GroupBy(b => new { b.BriefingAndDebriefingId, b.FlyingFTDSchedule.EquipmentId }).Select(grp => grp.ToList()).ToList();


                string traineeList = "";
                int i = 0;
                foreach (var briefingGroup in briefingDebriefingGroupList)
                {
                    i = 1;
                    bool isThereActiveScheduleForThisBriefingAndDebriefing = false;
                    foreach (var item in briefingGroup)
                    {
                        if (i == 1)
                            traineeList = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + (!string.IsNullOrEmpty(item.FlyingFTDSchedule.Trainee.Person.ShortName) ? item.FlyingFTDSchedule.Trainee.Person.ShortName : item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3)) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        else
                            traineeList = traineeList + " <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + (!string.IsNullOrEmpty(item.FlyingFTDSchedule.Trainee.Person.ShortName) ? item.FlyingFTDSchedule.Trainee.Person.ShortName : item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3)) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        i++;
                        if (item.FlyingFTDSchedule.Status != "Canceled")
                        {
                            isThereActiveScheduleForThisBriefingAndDebriefing = true;
                        }
                    }
                    if (isThereActiveScheduleForThisBriefingAndDebriefing)
                    {
                        EquipmentScheduledEventList.Add(new EquipmentScheduler
                        {
                            EventID = briefingGroup.FirstOrDefault().BriefingAndDebriefingId,
                            ResourceId = briefingGroup.FirstOrDefault().FlyingFTDSchedule.EquipmentId,
                            Description = " - Trainee/s Detail: <br /><strong>" + traineeList
                                            + "</strong><br/>- Instructor Name: <strong>" + (!string.IsNullOrEmpty(briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.ShortName) ? briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.ShortName : briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.FirstName.Substring(0, 3)
                                            + " " + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.MiddleName.Substring(0, 1))
                                            + "</strong><br/>- Duration: <strong>" + briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime + " -  " + briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime
                                            + "</strong><br/>- Type: <strong>" + (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing") + "<strong>",


                            Title = (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing"),
                            EventStart = briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime, //
                            EventEnd = briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime,
                            IsAllDay = false
                        });
                    }
                }
                return EquipmentScheduledEventList;
            }
            catch (Exception ex)
            {
                return new List<EquipmentScheduler>();
            }
        }
        public List<EquipmentScheduler> GetEquipmentEventForInstructorUtilization()
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                List<EquipmentScheduler> EquipmentScheduledEventList = new List<EquipmentScheduler>();
                EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();


                var scheduledEvents = (
                   from TBC in db.TraineeBatchClasses
                   join FFS in db.FlyingFTDSchedules on TBC.TraineeId equals FFS.TraineeId
                   select new
                   {
                       TBC,
                       FFS
                   }).ToList();

                var groupedResult = scheduledEvents.GroupBy(s => s.FFS.FlyingFTDScheduleId).Select(grp => grp.FirstOrDefault()).ToList();
                EquipmentScheduleBriefingDebriefing briefing = null;
                EquipmentScheduleBriefingDebriefing debriefing = null;
                foreach (var scheduledEvent in groupedResult)
                {
                    briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, true, false);
                    debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, false, true);

                    EquipmentScheduledEventList.Add(new EquipmentScheduler
                    {
                        EventID = scheduledEvent.FFS.FlyingFTDScheduleId,
                        ResourceId = scheduledEvent.FFS.InstructorId,
                        Description = " - Trainee Name: <strong>" + scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + " " + scheduledEvent.FFS.Trainee.Person.CompanyId
                                             + " </strong> <br/> - Instructor Name: <strong>" + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + scheduledEvent.TBC.BatchClass.BatchClassName
                                              + "</strong><br/>- Equipment: <strong>" + scheduledEvent.FFS.Equipment.NameOrSerialNo
                                             + "</strong><br/>- Location: <strong>" + scheduledEvent.FFS.Equipment.Location.LocationName
                                             + "</strong><br/>- Room No: <strong>" + scheduledEvent.FFS.Equipment.RoomNo
                                             + "</strong><br/>- Lesson Name: <strong>" + scheduledEvent.FFS.Lesson.LessonName
                                             + "</strong><br/>- Duration: <strong>" + scheduledEvent.FFS.ScheduleStartTime + " -  " + scheduledEvent.FFS.ScheduleEndTime
                                             + "</strong><br/>- Status: <strong>" + scheduledEvent.FFS.Status
                                             + "</strong><br/>- Briefing: <strong>" + (briefing != null ? (briefing.BriefingAndDebriefing.StartingTime + " - " + briefing.BriefingAndDebriefing.EndingTime) : "")
                                             + "</strong><br/>- Debriefing: <strong>" + (debriefing != null ? (debriefing.BriefingAndDebriefing.StartingTime + " - " + debriefing.BriefingAndDebriefing.EndingTime) : "") + "</strong>",

                        Title = scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + ". & " + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1) + ".",
                        EventStart = scheduledEvent.FFS.ScheduleStartTime, //
                        EventEnd = scheduledEvent.FFS.ScheduleEndTime,
                        IsAllDay = false
                    });
                }
                //Get briefing and debriefing time to display as an event with a unique color 
                //var equipmentScheduleBriefingDebriefingList = equipmentScheduleBriefingDebriefingAccess.List();

                var briefingDebriefingGroupList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled").GroupBy(b => new { b.BriefingAndDebriefingId, b.FlyingFTDSchedule.EquipmentId }).Select(grp => grp.ToList()).ToList();

                string traineeList = "";
                int i = 0;
                foreach (var briefingGroup in briefingDebriefingGroupList)
                {
                    i = 1;
                    bool isThereActiveScheduleForThisBriefingAndDebriefing = false;
                    foreach (var item in briefingGroup)
                    {
                        if (i == 1)
                            traineeList = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        else
                            traineeList = traineeList + " <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        i++;
                        if (item.FlyingFTDSchedule.Status != Enum.GetName(typeof(FlyingFTDScheduleStatus), 1))
                        {
                            isThereActiveScheduleForThisBriefingAndDebriefing = true;
                        }
                    }
                    if (isThereActiveScheduleForThisBriefingAndDebriefing)
                    {
                        EquipmentScheduledEventList.Add(new EquipmentScheduler
                        {
                            EventID = briefingGroup.FirstOrDefault().BriefingAndDebriefingId,
                            ResourceId = briefingGroup.FirstOrDefault().FlyingFTDSchedule.InstructorId,
                            Description = " - Trainee/s Detail: <br /><strong>" + traineeList
                                            + "</strong><br/>- Instructor Name: <strong>" + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.FirstName + " " + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.MiddleName
                                            + "</strong><br/>- Duration: <strong>" + briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime + " -  " + briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime
                                            + "</strong><br/>- Type: <strong>" + (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing") + "<strong>",


                            Title = (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing"),
                            EventStart = briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime, //
                            EventEnd = briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime,
                            IsAllDay = false
                        });
                    }
                }
                return EquipmentScheduledEventList;
            }
            catch (Exception ex)
            {
                return new List<EquipmentScheduler>();
            }
        }
        public List<EquipmentScheduler> GetEquipmentEventForInstructorUtilizations(string month, string year)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                List<EquipmentScheduler> EquipmentScheduledEventList = new List<EquipmentScheduler>();
                EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                {
                    startDate = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), 1);
                    endDate = startDate.AddMonths(1).AddMilliseconds(-1);
                }

                var scheduledEvents = (
                   from TBC in db.TraineeBatchClasses
                   join FFS in db.FlyingFTDSchedules on TBC.TraineeId equals FFS.TraineeId
                   where FFS.ScheduleStartTime >= startDate && FFS.ScheduleStartTime <= endDate
                   select new
                   {
                       TBC,
                       FFS
                   }).ToList();

                var groupedResult = scheduledEvents.GroupBy(s => s.FFS.FlyingFTDScheduleId).Select(grp => grp.FirstOrDefault()).ToList();
                var briefingDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.FlyingFTDSchedule.ScheduleStartTime >= startDate && b.FlyingFTDSchedule.ScheduleStartTime <= endDate).ToList();
                EquipmentScheduleBriefingDebriefing briefing = null;
                EquipmentScheduleBriefingDebriefing debriefing = null;
                foreach (var scheduledEvent in groupedResult)
                {
                    briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, true, false, briefingDebriefingList);
                    debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, false, true, briefingDebriefingList);

                    EquipmentScheduledEventList.Add(new EquipmentScheduler
                    {
                        EventID = scheduledEvent.FFS.FlyingFTDScheduleId,
                        ResourceId = scheduledEvent.FFS.InstructorId,
                        Description = " - Trainee Name: <strong>" + scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + " " + scheduledEvent.FFS.Trainee.Person.CompanyId
                                             + " </strong> <br/> - Instructor Name: <strong>" + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + scheduledEvent.TBC.BatchClass.BatchClassName
                                              + "</strong><br/>- Equipment: <strong>" + scheduledEvent.FFS.Equipment.NameOrSerialNo
                                             + "</strong><br/>- Location: <strong>" + scheduledEvent.FFS.Equipment.Location.LocationName
                                             + "</strong><br/>- Room No: <strong>" + scheduledEvent.FFS.Equipment.RoomNo
                                             + "</strong><br/>- Lesson Name: <strong>" + scheduledEvent.FFS.Lesson.LessonName
                                             + "</strong><br/>- Duration: <strong>" + scheduledEvent.FFS.ScheduleStartTime + " -  " + scheduledEvent.FFS.ScheduleEndTime
                                             + "</strong><br/>- Status: <strong>" + scheduledEvent.FFS.Status
                                             + "</strong><br/>- Briefing: <strong>" + (briefing != null ? (briefing.BriefingAndDebriefing.StartingTime + " - " + briefing.BriefingAndDebriefing.EndingTime) : "")
                                             + "</strong><br/>- Debriefing: <strong>" + (debriefing != null ? (debriefing.BriefingAndDebriefing.StartingTime + " - " + debriefing.BriefingAndDebriefing.EndingTime) : "") + "</strong>",

                        Title = scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + ". & " + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1) + ".",
                        EventStart = scheduledEvent.FFS.ScheduleStartTime, //
                        EventEnd = scheduledEvent.FFS.ScheduleEndTime,
                        IsAllDay = false
                    });
                }
                //Get briefing and debriefing time to display as an event with a unique color 
                //var equipmentScheduleBriefingDebriefingList = equipmentScheduleBriefingDebriefingAccess.List();

                var briefingDebriefingGroupList = briefingDebriefingList.GroupBy(b => new { b.BriefingAndDebriefingId, b.FlyingFTDSchedule.EquipmentId }).Select(grp => grp.ToList()).ToList();

                string traineeList = "";
                int i = 0;
                foreach (var briefingGroup in briefingDebriefingGroupList)
                {
                    i = 1;
                    bool isThereActiveScheduleForThisBriefingAndDebriefing = false;
                    foreach (var item in briefingGroup)
                    {
                        if (i == 1)
                            traineeList = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        else
                            traineeList = traineeList + " <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        i++;
                        if (item.FlyingFTDSchedule.Status != Enum.GetName(typeof(FlyingFTDScheduleStatus), 1))
                        {
                            isThereActiveScheduleForThisBriefingAndDebriefing = true;
                        }
                    }
                    if (isThereActiveScheduleForThisBriefingAndDebriefing)
                    {
                        EquipmentScheduledEventList.Add(new EquipmentScheduler
                        {
                            EventID = briefingGroup.FirstOrDefault().BriefingAndDebriefingId,
                            ResourceId = briefingGroup.FirstOrDefault().FlyingFTDSchedule.InstructorId,
                            Description = " - Trainee/s Detail: <br /><strong>" + traineeList
                                            + "</strong><br/>- Instructor Name: <strong>" + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.FirstName + " " + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.MiddleName
                                            + "</strong><br/>- Duration: <strong>" + briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime + " -  " + briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime
                                            + "</strong><br/>- Type: <strong>" + (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing") + "<strong>",


                            Title = (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing"),
                            EventStart = briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime, //
                            EventEnd = briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime,
                            IsAllDay = false
                        });
                    }
                }
                return EquipmentScheduledEventList;
            }
            catch (Exception ex)
            {
                return new List<EquipmentScheduler>();
            }
        }

        public List<EquipmentScheduler> GetFTDandFlyingScheduledEventForInstructor(string companyId, string month, string year)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                List<EquipmentScheduler> EquipmentScheduledEventList = new List<EquipmentScheduler>();
                EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();

                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                {
                    startDate = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }

                var scheduledEvents = (
                   from TBC in db.TraineeBatchClasses
                   join FFS in db.FlyingFTDSchedules on TBC.TraineeId equals FFS.TraineeId
                   where FFS.Instructor.Person.CompanyId == companyId && FFS.ScheduleStartTime >= startDate && FFS.ScheduleStartTime <= endDate
                   select new
                   {
                       TBC,
                       FFS
                   }).ToList();

                var groupedResult = scheduledEvents.GroupBy(s => s.FFS.FlyingFTDScheduleId).Select(grp => grp.FirstOrDefault()).ToList();
                var briefingDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.FlyingFTDSchedule.Instructor.Person.CompanyId == companyId && b.FlyingFTDSchedule.ScheduleStartTime >= startDate && b.FlyingFTDSchedule.ScheduleStartTime <= endDate).ToList();

                EquipmentScheduleBriefingDebriefing briefing = null;
                EquipmentScheduleBriefingDebriefing debriefing = null;
                foreach (var scheduledEvent in groupedResult)
                {
                    briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, true, false, briefingDebriefingList);
                    debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, false, true, briefingDebriefingList);

                    EquipmentScheduledEventList.Add(new EquipmentScheduler
                    {
                        EventID = scheduledEvent.FFS.FlyingFTDScheduleId,
                        ResourceId = scheduledEvent.FFS.EquipmentId,
                        Description = " - Trainee Name: <strong>" + scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + " " + scheduledEvent.FFS.Trainee.Person.CompanyId
                                             + " </strong> <br/> - Instructor Name: <strong>" + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + scheduledEvent.TBC.BatchClass.BatchClassName
                                              + "</strong><br/>- Equipment: <strong>" + scheduledEvent.FFS.Equipment.NameOrSerialNo
                                             + "</strong><br/>- Location: <strong>" + scheduledEvent.FFS.Equipment.Location.LocationName
                                             + "</strong><br/>- Room No: <strong>" + scheduledEvent.FFS.Equipment.RoomNo
                                             + "</strong><br/>- Lesson Name: <strong>" + scheduledEvent.FFS.Lesson.LessonName
                                             + "</strong><br/>- Duration: <strong>" + scheduledEvent.FFS.ScheduleStartTime + " -  " + scheduledEvent.FFS.ScheduleEndTime
                                             + "</strong><br/>- Status: <strong>" + scheduledEvent.FFS.Status
                                             + "</strong><br/>- Briefing: <strong>" + (briefing != null ? (briefing.BriefingAndDebriefing.StartingTime + " - " + briefing.BriefingAndDebriefing.EndingTime) : "")
                                             + "</strong><br/>- Debriefing: <strong>" + (debriefing != null ? (debriefing.BriefingAndDebriefing.StartingTime + " - " + debriefing.BriefingAndDebriefing.EndingTime) : "") + "</strong>",

                        Title = scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + ". & " + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1) + ".",
                        EventStart = scheduledEvent.FFS.ScheduleStartTime, //
                        EventEnd = scheduledEvent.FFS.ScheduleEndTime,
                        IsAllDay = false
                    });
                }
                //Get briefing and debriefing time to display as an event with a unique color 
                //var equipmentScheduleBriefingDebriefingList = equipmentScheduleBriefingDebriefingAccess.List();

                var briefingDebriefingGroupList = briefingDebriefingList.GroupBy(b => new { b.BriefingAndDebriefingId, b.FlyingFTDSchedule.EquipmentId }).Select(grp => grp.ToList()).ToList();

                string traineeList = "";
                int i = 0;
                foreach (var briefingGroup in briefingDebriefingGroupList)
                {
                    i = 1;
                    bool isThereActiveScheduleForThisBriefingAndDebriefing = false;
                    foreach (var item in briefingGroup)
                    {
                        if (i == 1)
                            traineeList = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        else
                            traineeList = traineeList + " <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        i++;
                        if (item.FlyingFTDSchedule.Status != Enum.GetName(typeof(FlyingFTDScheduleStatus), 1))
                        {
                            isThereActiveScheduleForThisBriefingAndDebriefing = true;
                        }
                    }
                    if (isThereActiveScheduleForThisBriefingAndDebriefing)
                    {
                        EquipmentScheduledEventList.Add(new EquipmentScheduler
                        {
                            EventID = briefingGroup.FirstOrDefault().BriefingAndDebriefingId,
                            ResourceId = briefingGroup.FirstOrDefault().FlyingFTDSchedule.EquipmentId,
                            Description = " - Trainee/s Detail: <br /><strong>" + traineeList
                                            + "</strong><br/>- Instructor Name: <strong>" + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.FirstName + " " + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.MiddleName
                                            + "</strong><br/>- Duration: <strong>" + briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime + " -  " + briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime
                                            + "</strong><br/>- Type: <strong>" + (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing") + "<strong>",


                            Title = (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing"),
                            EventStart = briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime, //
                            EventEnd = briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime,
                            IsAllDay = false
                        });
                    }
                }

                return EquipmentScheduledEventList;
            }
            catch (Exception ex)
            {
                return new List<EquipmentScheduler>();
            }
        }
        public List<EquipmentScheduler> GetFTDandFlyingScheduledEventForTrainee(string companyId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                List<EquipmentScheduler> EquipmentScheduledEventList = new List<EquipmentScheduler>();
                EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();


                var scheduledEvents = (
                   from TBC in db.TraineeBatchClasses
                   join FFS in db.FlyingFTDSchedules on TBC.TraineeId equals FFS.TraineeId
                   where FFS.Trainee.Person.CompanyId == companyId
                   select new
                   {
                       TBC,
                       FFS
                   }).ToList();

                var groupedResult = scheduledEvents.GroupBy(s => s.FFS.FlyingFTDScheduleId).Select(grp => grp.FirstOrDefault()).ToList();
                EquipmentScheduleBriefingDebriefing briefing = null;
                EquipmentScheduleBriefingDebriefing debriefing = null;
                foreach (var scheduledEvent in groupedResult)
                {
                    briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, true, false);
                    debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefing(scheduledEvent.FFS.FlyingFTDScheduleId, false, true);

                    EquipmentScheduledEventList.Add(new EquipmentScheduler
                    {
                        EventID = scheduledEvent.FFS.FlyingFTDScheduleId,
                        ResourceId = scheduledEvent.FFS.EquipmentId,
                        Description = " - Trainee Name: <strong>" + scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + " " + scheduledEvent.FFS.Trainee.Person.CompanyId
                                             + " </strong> <br/> - Instructor Name: <strong>" + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + scheduledEvent.TBC.BatchClass.BatchClassName
                                              + "</strong><br/>- Equipment: <strong>" + scheduledEvent.FFS.Equipment.NameOrSerialNo
                                             + "</strong><br/>- Location: <strong>" + scheduledEvent.FFS.Equipment.Location.LocationName
                                             + "</strong><br/>- Room No: <strong>" + scheduledEvent.FFS.Equipment.RoomNo
                                             + "</strong><br/>- Lesson Name: <strong>" + scheduledEvent.FFS.Lesson.LessonName
                                             + "</strong><br/>- Duration: <strong>" + scheduledEvent.FFS.ScheduleStartTime + " -  " + scheduledEvent.FFS.ScheduleEndTime
                                             + "</strong><br/>- Status: <strong>" + scheduledEvent.FFS.Status
                                             + "</strong><br/>- Briefing: <strong>" + (briefing != null ? (briefing.BriefingAndDebriefing.StartingTime + " - " + briefing.BriefingAndDebriefing.EndingTime) : "")
                                             + "</strong><br/>- Debriefing: <strong>" + (debriefing != null ? (debriefing.BriefingAndDebriefing.StartingTime + " - " + debriefing.BriefingAndDebriefing.EndingTime) : "") + "</strong>",

                        Title = scheduledEvent.FFS.Trainee.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Trainee.Person.MiddleName.Substring(0, 1)
                                             + ". & " + scheduledEvent.FFS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + scheduledEvent.FFS.Instructor.Person.MiddleName.Substring(0, 1) + ".",
                        EventStart = scheduledEvent.FFS.ScheduleStartTime, //
                        EventEnd = scheduledEvent.FFS.ScheduleEndTime,
                        IsAllDay = false
                    });
                }
                //Get briefing and debriefing time to display as an event with a unique color 
                //var equipmentScheduleBriefingDebriefingList = equipmentScheduleBriefingDebriefingAccess.List();

                var briefingDebriefingGroupList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.FlyingFTDSchedule.Trainee.Person.CompanyId == companyId).GroupBy(b => new { b.BriefingAndDebriefingId, b.FlyingFTDSchedule.EquipmentId }).Select(grp => grp.ToList()).ToList();

                string traineeList = "";
                int i = 0;
                foreach (var briefingGroup in briefingDebriefingGroupList)
                {
                    i = 1;
                    bool isThereActiveScheduleForThisBriefingAndDebriefing = false;
                    foreach (var item in briefingGroup)
                    {
                        if (i == 1)
                            traineeList = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        else
                            traineeList = traineeList + " <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i + ". " + item.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + item.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + ". - " + item.FlyingFTDSchedule.Lesson.LessonName + " - " + item.FlyingFTDSchedule.ScheduleStartTime + " -  " + item.FlyingFTDSchedule.ScheduleEndTime;
                        i++;
                        if (item.FlyingFTDSchedule.Status != Enum.GetName(typeof(FlyingFTDScheduleStatus), 1))
                        {
                            isThereActiveScheduleForThisBriefingAndDebriefing = true;
                        }
                    }
                    if (isThereActiveScheduleForThisBriefingAndDebriefing)
                    {
                        EquipmentScheduledEventList.Add(new EquipmentScheduler
                        {
                            EventID = briefingGroup.FirstOrDefault().BriefingAndDebriefingId,
                            ResourceId = briefingGroup.FirstOrDefault().FlyingFTDSchedule.EquipmentId,
                            Description = " - Trainee/s Detail: <br /><strong>" + traineeList
                                            + "</strong><br/>- Instructor Name: <strong>" + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.FirstName + " " + briefingGroup.FirstOrDefault().FlyingFTDSchedule.Instructor.Person.MiddleName
                                            + "</strong><br/>- Duration: <strong>" + briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime + " -  " + briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime
                                            + "</strong><br/>- Type: <strong>" + (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing") + "<strong>",


                            Title = (briefingGroup.FirstOrDefault().BriefingAndDebriefing.IsBriefing ? "Briefing" : "Debriefing"),
                            EventStart = briefingGroup.FirstOrDefault().BriefingAndDebriefing.StartingTime, //
                            EventEnd = briefingGroup.FirstOrDefault().BriefingAndDebriefing.EndingTime,
                            IsAllDay = false
                        });
                    }
                }

                return EquipmentScheduledEventList;
            }
            catch (Exception ex)
            {
                return new List<EquipmentScheduler>();
            }
        }

        public List<SchedulerResource> GetSchedulerResource()
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<SchedulerResource> schedulerResourceList = new List<SchedulerResource>();

                var result = db.Equipments.ToList();
                string equipmentModel = "", workingHour = "";
                var equipmentCertificateList = db.EquipmentCertificates.ToList();
                bool isCertificationValid = false;

                foreach (var equipment in result)
                {

                    var batchEquipmentModel = db.BatchEquipmentModels.Where(bem => bem.EquipmentModelId == equipment.EquipmentModelId).ToList();

                    if (batchEquipmentModel.Count > 0)
                    {
                        if (!(equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper().Contains("FTD")))
                        {
                            isCertificationValid = true;
                            var equipCertification = equipmentCertificateList.Where(EC => EC.EquipmentId == equipment.EquipmentId).ToList();
                            if (equipCertification.Count > 0)
                            {
                                var equipmentWithExpiredCertification = equipCertification.Where(EC => (DateTime.Now.Date >= EC.StartingDate.Date && DateTime.Now.Date <= EC.EndingDate.Date)).ToList();
                                if (equipmentWithExpiredCertification.Count == 0)
                                    isCertificationValid = false;
                            }

                            if (isCertificationValid)
                            {
                                equipmentModel = equipment.EquipmentModel.EquipmentModelName;// "Flying";
                                workingHour = (DateTime.Now.Date + equipment.StartTime).ToString("HH:mm") + "-" + ((DateTime.Now.Date + equipment.StartTime).AddHours((double)equipment.WorkingHours)).ToString("HH:mm");

                                schedulerResourceList.Add(new SchedulerResource
                                {
                                    EquipmentId = equipment.EquipmentId,
                                    EquipmentName = equipment.NameOrSerialNo,
                                    EquipmentModel = equipmentModel,
                                    WorkingHours = workingHour,
                                    EstimatedRemainingHours = equipment.EstimatedRemainingHours.ToString() + "/" + equipment.ActualRemainingHours.ToString()
                                });
                            }
                        }
                        else
                        {
                            equipmentModel = equipment.EquipmentModel.EquipmentModelName; //"Simulator";
                            workingHour = (DateTime.Now.Date + equipment.StartTime).ToString("HH:mm") + "-" + ((DateTime.Now.Date + equipment.StartTime).AddHours((double)equipment.WorkingHours)).ToString("HH:mm");

                            schedulerResourceList.Add(new SchedulerResource
                            {
                                EquipmentId = equipment.EquipmentId,
                                EquipmentName = equipment.NameOrSerialNo,
                                EquipmentModel = equipmentModel,
                                WorkingHours = workingHour,
                                EstimatedRemainingHours = equipment.EstimatedRemainingHours.ToString() + "/" + equipment.ActualRemainingHours.ToString()
                            });
                        }
                    }
                }
                return schedulerResourceList;
            }
            catch (Exception ex)
            {
                return new List<SchedulerResource>();
            }
        }
        public List<SchedulerResourceInstructor> GetSchedulerInstructorResource()
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<SchedulerResourceInstructor> schedulerResourceList = new List<SchedulerResourceInstructor>();

                var result = (from E in db.Equipments
                              join EM in db.EquipmentModels on E.EquipmentModelId equals EM.EquipmentModelId
                              join EIM in db.InstructorEquipmentModels on EM.EquipmentModelId equals EIM.EquipmentModelId
                              select new
                              {
                                  EIM
                              }).ToList();

                var resultGroup = result.GroupBy(Inst => Inst.EIM.InstructorId).Select(grp => grp.FirstOrDefault()).ToList();

                foreach (var instructor in resultGroup)
                {
                    //var batchEquipmentModel = db.BatchEquipmentModels.Where(bem => bem.EquipmentModelId == instructor.EIM.EquipmentModelId).ToList();

                    if ((instructor.EIM.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper().Contains("FTD")))
                    {
                        schedulerResourceList.Add(new SchedulerResourceInstructor
                        {
                            InstructorId = instructor.EIM.InstructorId,
                            InstructorName = instructor.EIM.Instructor.Person.FirstName + " " + instructor.EIM.Instructor.Person.MiddleName + " " + instructor.EIM.Instructor.Person.LastName,
                            EquipmentType = "FTD"
                        });
                    }
                    else
                    {
                        schedulerResourceList.Add(new SchedulerResourceInstructor
                        {
                            InstructorId = instructor.EIM.InstructorId,
                            InstructorName = instructor.EIM.Instructor.Person.FirstName + " " + instructor.EIM.Instructor.Person.MiddleName + " " + instructor.EIM.Instructor.Person.LastName,
                            EquipmentType = "FLYING"
                        });
                    }

                }
                return schedulerResourceList;
            }
            catch (Exception ex)
            {
                return new List<SchedulerResourceInstructor>();
            }
        }

        public List<SchedulerResource> GetSchedulerResourceForInstructor(string companyId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<SchedulerResource> schedulerResourceList = new List<SchedulerResource>();

                var result = db.Equipments.ToList();
                string equipmentModel = "", workingHour = "";
                var equipmentCertificateList = db.EquipmentCertificates.ToList();
                bool isCertificationValid = false;

                foreach (var equipment in result)
                {
                    var scheduledEquipment = (from E in db.Equipments
                                              join BEM in db.BatchEquipmentModels on E.EquipmentModelId equals BEM.EquipmentModelId
                                              join EIM in db.InstructorEquipmentModels on BEM.EquipmentModelId equals EIM.EquipmentModelId
                                              where EIM.Instructor.Person.CompanyId == companyId && BEM.EquipmentModelId == equipment.EquipmentModelId
                                              select new
                                              {
                                                  EIM
                                              }).ToList();

                    //var batchEquipmentModel = db.BatchEquipmentModels.Where(bem => bem.EquipmentModelId == equipment.EquipmentModelId).ToList();

                    if (scheduledEquipment.Count > 0)
                    {
                        if (!(equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper().Contains("FTD")))
                        {
                            isCertificationValid = true;
                            var equipCertification = equipmentCertificateList.Where(EC => EC.EquipmentId == equipment.EquipmentId).ToList();
                            if (equipCertification.Count > 0)
                            {
                                var equipmentWithExpiredCertification = equipCertification.Where(EC => (DateTime.Now.Date >= EC.StartingDate.Date && DateTime.Now.Date <= EC.EndingDate.Date)).ToList();
                                if (equipmentWithExpiredCertification.Count == 0)
                                    isCertificationValid = false;
                            }

                            if (isCertificationValid)
                            {
                                equipmentModel = equipment.EquipmentModel.EquipmentModelName;// "Flying";
                                workingHour = (DateTime.Now.Date + equipment.StartTime).ToString("HH:mm") + "-" + ((DateTime.Now.Date + equipment.StartTime).AddHours((double)equipment.WorkingHours)).ToString("HH:mm");

                                schedulerResourceList.Add(new SchedulerResource
                                {
                                    EquipmentId = equipment.EquipmentId,
                                    EquipmentName = equipment.NameOrSerialNo,
                                    EquipmentModel = equipmentModel,
                                    WorkingHours = workingHour
                                });
                            }
                        }
                        else
                        {
                            equipmentModel = equipment.EquipmentModel.EquipmentModelName; //"Simulator";
                            workingHour = (DateTime.Now.Date + equipment.StartTime).ToString("HH:mm") + "-" + ((DateTime.Now.Date + equipment.StartTime).AddHours((double)equipment.WorkingHours)).ToString("HH:mm");

                            schedulerResourceList.Add(new SchedulerResource
                            {
                                EquipmentId = equipment.EquipmentId,
                                EquipmentName = equipment.NameOrSerialNo,
                                EquipmentModel = equipmentModel,
                                WorkingHours = workingHour
                            });
                        }
                    }
                }
                return schedulerResourceList;
            }
            catch (Exception ex)
            {
                return new List<SchedulerResource>();
            }
        }

        public List<SchedulerResource> GetSchedulerResourceForTrainee(string companyId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<SchedulerResource> schedulerResourceList = new List<SchedulerResource>();

                var result = db.Equipments.ToList();
                string equipmentModel = "", workingHour = "";
                var equipmentCertificateList = db.EquipmentCertificates.ToList();
                bool isCertificationValid = false;

                //var result = (from TL in db.TraineeLessons
                //              join BC in db.TraineeBatchClasses on TL.TraineeCategory.TraineeProgram.TraineeSyllabus.TraineeId equals BC.TraineeId
                //              where TL.LessonId == lessonId && BC.BatchClassId == batchClass.BatchClassId
                //              && TL.Lesson.EquipmentType.EquipmentTypeName == equipment.EquipmentModel.EquipmentType.EquipmentTypeName
                //              select new
                //              {
                //                  BC,
                //                  TL
                //              }).ToList();

                foreach (var equipment in result)
                {
                    var scheduledEquipment = (from E in db.Equipments
                                              join BEM in db.BatchEquipmentModels on E.EquipmentModelId equals BEM.EquipmentModelId
                                              //join EIM in db.InstructorEquipmentModels on BEM.EquipmentModelId equals EIM.EquipmentModelId
                                              join TBC in db.TraineeBatchClasses on BEM.BatchId equals TBC.BatchClass.BatchId
                                              where BEM.EquipmentModelId == equipment.EquipmentModelId && TBC.Trainee.Person.CompanyId == companyId
                                              select new
                                              {
                                                  BEM
                                              }).ToList();

                    //var batchEquipmentModel = db.BatchEquipmentModels.Where(bem => bem.EquipmentModelId == equipment.EquipmentModelId).ToList();

                    if (scheduledEquipment.Count > 0)
                    {
                        if (!(equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper().Contains("FTD")))
                        {
                            isCertificationValid = true;
                            var equipCertification = equipmentCertificateList.Where(EC => EC.EquipmentId == equipment.EquipmentId).ToList();
                            if (equipCertification.Count > 0)
                            {
                                var equipmentWithExpiredCertification = equipCertification.Where(EC => (DateTime.Now.Date >= EC.StartingDate.Date && DateTime.Now.Date <= EC.EndingDate.Date)).ToList();
                                if (equipmentWithExpiredCertification.Count == 0)
                                    isCertificationValid = false;
                            }

                            if (isCertificationValid)
                            {
                                equipmentModel = equipment.EquipmentModel.EquipmentModelName;// "Flying";
                                workingHour = (DateTime.Now.Date + equipment.StartTime).ToString("HH:mm") + "-" + ((DateTime.Now.Date + equipment.StartTime).AddHours((double)equipment.WorkingHours)).ToString("HH:mm");

                                schedulerResourceList.Add(new SchedulerResource
                                {
                                    EquipmentId = equipment.EquipmentId,
                                    EquipmentName = equipment.NameOrSerialNo,
                                    EquipmentModel = equipmentModel,
                                    WorkingHours = workingHour
                                });
                            }
                        }
                        else
                        {
                            equipmentModel = equipment.EquipmentModel.EquipmentModelName; //"Simulator";
                            workingHour = (DateTime.Now.Date + equipment.StartTime).ToString("HH:mm") + "-" + ((DateTime.Now.Date + equipment.StartTime).AddHours((double)equipment.WorkingHours)).ToString("HH:mm");

                            schedulerResourceList.Add(new SchedulerResource
                            {
                                EquipmentId = equipment.EquipmentId,
                                EquipmentName = equipment.NameOrSerialNo,
                                EquipmentModel = equipmentModel,
                                WorkingHours = workingHour
                            });
                        }
                    }
                }
                return schedulerResourceList;
            }
            catch (Exception ex)
            {
                return new List<SchedulerResource>();
            }
        }




        public List<GroundSchedulerResource> GetGroundSchedulerResource()
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<GroundSchedulerResource> GroundSchedulerResourceList = new List<GroundSchedulerResource>();

                var result = db.PhaseSchedules.ToList();
                var phaseSchedules = result.GroupBy(PS => PS.BatchId).Select(grp => grp.FirstOrDefault()).ToList();
                int index = 1;
                foreach (var phaseSchedule in phaseSchedules)
                {
                    GroundSchedulerResourceList.Add(new GroundSchedulerResource
                    {
                        SerialNumber = index.ToString(),
                        BatchId = phaseSchedule.BatchId,
                        BatchClassName = phaseSchedule.Batch.BatchName
                    });
                    index = index + 1;
                }
                GroundSchedulerResourceList = GroundSchedulerResourceList.OrderBy(x => x.BatchClassName).ToList();
                return GroundSchedulerResourceList;
            }
            catch (Exception ex)
            {
                return new List<GroundSchedulerResource>();
            }
        }
        public List<GroundSchedulerResource> GetGroundSchedulerResourceForTrainee(string companyId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<GroundSchedulerResource> GroundSchedulerResourceList = new List<GroundSchedulerResource>();

                var result = db.PhaseSchedules.ToList();

                var scheduledEvents = (from PS in db.PhaseSchedules
                                       join TBC in db.TraineeBatchClasses on PS.BatchId equals TBC.BatchClass.BatchId
                                       where TBC.Trainee.Person.CompanyId == companyId
                                       select new
                                       {
                                           PS,
                                           TBC
                                       }).ToList();

                var phaseSchedules = scheduledEvents.GroupBy(s => s.PS.BatchId).Select(grp => grp.FirstOrDefault()).ToList();

                int index = 1;
                foreach (var phaseSchedule in phaseSchedules)
                {
                    GroundSchedulerResourceList.Add(new GroundSchedulerResource
                    {
                        SerialNumber = index.ToString(),
                        BatchId = phaseSchedule.PS.BatchId,
                        BatchClassName = phaseSchedule.PS.Batch.BatchName
                    });
                    index = index + 1;
                }
                return GroundSchedulerResourceList;
            }
            catch (Exception ex)
            {
                return new List<GroundSchedulerResource>();
            }
        }
        public List<GroundSchedulerResource> GetGroundSchedulerResourceForInstructor(string companyId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                List<GroundSchedulerResource> GroundSchedulerResourceList = new List<GroundSchedulerResource>();

                var result = db.PhaseSchedules.ToList();

                var scheduledEvents = (from PS in db.PhaseSchedules
                                       join BMS in db.BatchModules on PS.BatchId equals BMS.BatchCourse.BatchCategory.BatchId
                                       join MIS in db.ModuleInstructorSchedules on BMS.ModuleId equals MIS.ModuleId
                                       where MIS.Instructor.Person.CompanyId == companyId && PS.PhaseId==BMS.PhaseId

                                       select new
                                       {
                                           PS,
                                           BMS
                                       }).ToList();

                var phaseSchedules = scheduledEvents.GroupBy(s => s.PS.BatchId).Select(grp => grp.FirstOrDefault()).ToList();

                int index = 1;
                foreach (var phaseSchedule in phaseSchedules)
                {
                    GroundSchedulerResourceList.Add(new GroundSchedulerResource
                    {
                        SerialNumber = index.ToString(),
                        BatchId = phaseSchedule.PS.BatchId,
                        BatchClassName = phaseSchedule.PS.Batch.BatchName
                    });
                    index = index + 1;
                }
                return GroundSchedulerResourceList;
            }
            catch (Exception ex)
            {
                return new List<GroundSchedulerResource>();
            }
        }


        public List<Scheduler> FilterScheduledEventList(string FilterBy, string FilterValue)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                int searchValue = 0;
                List<ModuleSchedule> result = new List<ModuleSchedule>();

                List<Scheduler> ScheduledEvents = new List<Scheduler>();
                if (FilterBy.Equals("Batch"))
                {
                    searchValue = Convert.ToInt32(FilterValue);
                    var scheduledEvents = (from CM in db.CourseModules
                                           join MS in db.ModuleSchedules on CM.ModuleId equals MS.ModuleId
                                           where MS.PhaseSchedule.BatchId == searchValue
                                           select new
                                           {
                                               CM,
                                               MS
                                           }).ToList();
                    if (scheduledEvents.Count() > 0)
                    {
                        var distinictModuleSchedule = scheduledEvents.GroupBy(MS => MS.MS.ModuleScheduleId).Select(grp => grp.FirstOrDefault()).ToList();

                        var ScheduledEventList = distinictModuleSchedule.Select(item => new Scheduler
                        {
                            EventID = item.MS.ModuleScheduleId,
                            ResourceId = item.MS.PhaseSchedule.BatchId,
                            Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                                             + "</strong><br/>- Course Code: <strong>" + item.CM.CourseCategory.Course.CourseCode + "-" + item.CM.CourseCategory.Course.CourseTitle
                                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode + "-" + item.MS.Module.ModuleTitle
                                             + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName
                                             + "</strong><br/>- Scheduled Date: from <strong>" + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("MM/dd/yyyy HH:mm"))
                                              + "</strong><br/>- Status: <strong>" + item.MS.Status + "</strong>",
                            Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ". -" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "-" + item.MS.ClassRoom.Building.BuildingName
                                             + "-" + item.MS.ClassRoom.RoomNo
                                             + "-" + item.CM.CourseCategory.Course.CourseCode
                                             + "-" + item.MS.Module.ModuleCode,
                            EventStart = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))),
                            EventEnd = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))),
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
                                           where MS.PhaseSchedule.Batch.ProgramId == searchValue
                                           select new
                                           {
                                               CM,
                                               MS
                                           }).ToList();
                    if (scheduledEvents.Count() > 0)
                    {
                        var distinictModuleSchedule = scheduledEvents.GroupBy(MS => MS.MS.ModuleScheduleId).Select(grp => grp.FirstOrDefault()).ToList();

                        var ScheduledEventList = distinictModuleSchedule.Select(item => new Scheduler
                        {
                            EventID = item.MS.ModuleScheduleId,
                            ResourceId = item.MS.PhaseSchedule.BatchId,
                            Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                                             + "</strong><br/>- Course Code: <strong>" + item.CM.CourseCategory.Course.CourseCode + "-" + item.CM.CourseCategory.Course.CourseTitle
                                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode + "-" + item.MS.Module.ModuleTitle
                                            + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName
                                             + "</strong><br/>- Scheduled Date: from <strong>" + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("MM/dd/yyyy HH:mm"))
                                             + "</strong><br/>- Status: <strong>" + item.MS.Status + "</strong>",
                            Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ". -" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "-" + item.MS.ClassRoom.Building.BuildingName
                                             + "-" + item.MS.ClassRoom.RoomNo
                                             + "-" + item.CM.CourseCategory.Course.CourseCode
                                             + "-" + item.MS.Module.ModuleCode,
                            EventStart = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))),
                            EventEnd = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))),
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
                        var distinictModuleSchedule = scheduledEvents.GroupBy(MS => MS.MS.ModuleScheduleId).Select(grp => grp.FirstOrDefault()).ToList();

                        var ScheduledEventList = distinictModuleSchedule.Select(item => new Scheduler
                        {
                            EventID = item.MS.ModuleScheduleId,
                            ResourceId = item.MS.PhaseSchedule.BatchId,
                            Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                                             + "</strong><br/>- Course Code: <strong>" + item.CM.CourseCategory.Course.CourseCode + "-" + item.CM.CourseCategory.Course.CourseTitle
                                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode + "-" + item.MS.Module.ModuleTitle
                                            + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName
                                             + "</strong><br/>- Scheduled Date: from <strong>" + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("MM/dd/yyyy HH:mm"))
                                             + "</strong><br/>- Status: <strong>" + item.MS.Status + "</strong>",
                            Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ". -" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "-" + item.MS.ClassRoom.Building.BuildingName
                                             + "-" + item.MS.ClassRoom.RoomNo
                                             + "-" + item.CM.CourseCategory.Course.CourseCode
                                             + "-" + item.MS.Module.ModuleCode,
                            EventStart = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))),
                            EventEnd = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))),
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
                        var distinictModuleSchedule = scheduledEvents.GroupBy(MS => MS.MS.ModuleScheduleId).Select(grp => grp.FirstOrDefault()).ToList();
                        var ScheduledEventList = distinictModuleSchedule.Select(item => new Scheduler
                        {
                            EventID = item.MS.ModuleScheduleId,
                            ResourceId = item.MS.PhaseSchedule.BatchId,
                            Description = "<br/>- Instructor Name: <strong>" + item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ".</strong><br/>- Batch Class Name: <strong>" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "</strong><br/>- Building Name: <strong>" + item.MS.ClassRoom.Building.BuildingName
                                             + "</strong><br/>- Room No: <strong>" + item.MS.ClassRoom.RoomNo
                                              + "</strong><br/>- Course Code: <strong>" + item.CM.CourseCategory.Course.CourseCode + "-" + item.CM.CourseCategory.Course.CourseTitle
                                             + "</strong><br/>- Module Code: <strong>" + item.MS.Module.ModuleCode + "-" + item.MS.Module.ModuleTitle
                                            + "</strong><br/>- Period: <strong>" + item.MS.Period.PeriodName
                                             + "</strong><br/>- Scheduled Date: from <strong>" + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))).ToString("MM/dd/yyyy HH:mm"))
                                             + "</strong><br/>- Status: <strong>" + item.MS.Status + "</strong>",
                            Title = item.MS.Instructor.Person.FirstName.Substring(0, 3)
                                             + " " + item.MS.Instructor.Person.MiddleName.Substring(0, 1)
                                             + ". -" + item.MS.PhaseSchedule.Batch.BatchName
                                             + "-" + item.MS.ClassRoom.Building.BuildingName
                                             + "-" + item.MS.ClassRoom.RoomNo
                                             + "-" + item.CM.CourseCategory.Course.CourseCode
                                             + "-" + item.MS.Module.ModuleCode,
                            EventStart = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.StartTime))), //
                            EventEnd = Convert.ToDateTime(item.MS.Date.Add(TimeSpan.Parse(item.MS.Period.EndTime))),
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

        public List<UnscheduledResource> get_FreeTimeSlotAndRoom(int moduleScheduleId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

                //1. Get all free date, period and class room
                List<PeriodView> PeriodList = get_UnionOfWorkingPeriods();
                List<ClassRoom> ClassRoomList = (List<ClassRoom>)classRoomAccess.List();
                List<PeriodClassCombination> periodClassCombination = get_PeriodClassRoomCombination(PeriodList, ClassRoomList);

                List<ModuleSchedule> moduleSchedules = dbContext.ModuleSchedules.Where(ms => ms.Status != statusName).ToList();

                //Sort modules Schedule list by date: so, that Date to which the event will be edited will be displayed in Aschending order
                var sortedModuleSchedules = moduleSchedules.OrderBy(x => x.Date).ToList();

                //Group scheduled modules by Date to get all valid days in order to get free period, instructor and Class room of that day
                var moduleSchedulesGroup = sortedModuleSchedules.GroupBy(x => new { x.Date }).Select(grp => grp.ToList()).ToList();


                //2. Filter the fittest one for the clicked event
                //Get the detial of the module to be rescheduled
                ModuleSchedule moduleScheduleBO = (ModuleSchedule)moduleScheduleAccess.Details(moduleScheduleId);

                List<DateTime> dateTimeSlot = constructTimeSlots(moduleScheduleBO.PhaseSchedule.Batch.DayTemplateId);

                int lastIndex = dateTimeSlot.Count - 1;
                DateTime firstDate = dateTimeSlot[0];
                DateTime lastDate = dateTimeSlot[lastIndex];
                var batchModuleSchedule = db.ModuleSchedules.Where(ms => ms.PhaseSchedule.BatchId == moduleScheduleBO.PhaseSchedule.BatchId && (ms.Date >= firstDate && ms.Date <= lastDate) && ms.Status != statusName).ToList();

                var moduleRevisionGroupId = moduleScheduleBO.Module.RevisionGroupId == null ? moduleScheduleBO.ModuleId : moduleScheduleBO.Module.RevisionGroupId;

                //Get potantial instructor of the module to be rescheduled
                List<ModuleInstructorSchedule> potentialInstructors = moduleInstructorScheduleAccess.PotentialInstructorList((int)moduleRevisionGroupId);

                //Get DAY and PERIOD template of a Batch whose module is to be rescheduled 
                List<string> daysTemplate = get_DayTemplateList(moduleScheduleBO.PhaseSchedule.BatchId);
                List<PeriodView> periodsTemplate = get_PeriodTemplateList(moduleScheduleBO.PhaseSchedule.BatchId);

                List<UnscheduledResource> UnscheduledResourceList = new List<UnscheduledResource>();
                UnscheduledResource unScheduledResource = null;

                List<ModuleSchedule> allmoduleScheduleList = db.ModuleSchedules.Where(ms => ms.Status != statusName).ToList();

                foreach (var instructor in potentialInstructors)
                {
                    unScheduledResource = new UnscheduledResource();
                    //GET INSTRUCTOR INFORMATION
                    unScheduledResource.FreeInstructor = new FreeInstructor
                    {
                        InstructorId = instructor.InstructorId,
                        NameAndCompanyId = instructor.Instructor.Person.FirstName.ToUpper() + " " + instructor.Instructor.Person.MiddleName.ToUpper()
                    };

                    List<FreeDate> FreeDateList = new List<FreeDate>();
                    FreeDate FreeDate = null;

                    var instructorModuleSchedule = db.ModuleSchedules.Where(ms => ms.InstructorId == instructor.InstructorId && (ms.Date >= firstDate && ms.Date <= lastDate) && ms.Status != statusName).ToList();

                    foreach (var date in dateTimeSlot)
                    {
                        List<ModuleSchedule> moduleScheduleList = allmoduleScheduleList.Where(ms => ms.Date.Date == date.Date).ToList();

                        FreeDate = new FreeDate();
                        if (date.Date >= DateTime.Now)
                        {
                            FreeDate.Date = date.Date.Date.ToString("dd/MM/yyyy");

                            //Get free Period and Class Room combination/s of specific day
                            var freePeriodAndRoom = periodClassCombination.Where(PRC => !(moduleScheduleList.Any(ms => (ms.PeriodId == PRC.PeriodId)
                                && (ms.ClassRoomId == PRC.ClassRoomId)))).ToList();

                            var PeiodRoomGroup = freePeriodAndRoom.GroupBy(x => new { x.PeriodId }).Select(grp => grp.ToList()).ToList();

                            List<FreePeriod> FreePeriodList = new List<FreePeriod>();
                            FreePeriod freePeriod = null;

                            foreach (var periodAndroom in PeiodRoomGroup)
                            {
                                string day = date.Date.Date.ToString("dddd");
                                //string period = PeriodList.Single(p => p.PeriodId == periodAndroom.FirstOrDefault().PeriodId).Period;

                                //Check if this instructor is free at this DAY and PERIOD
                                if (IsThisInstructorFree(instructor, date.Date, periodAndroom.FirstOrDefault().PeriodId, instructorModuleSchedule, moduleScheduleId))
                                {
                                    //Check whether it is a suitable Batch Day template and Period template, Does Batch Class Has Another Class 
                                    //and weather the instructor has no class                
                                    if (Is_ValidDayAndPeriodForBatch(daysTemplate, periodsTemplate, day, periodAndroom.FirstOrDefault().PeriodId) && !DoesBatchClassHasAnotherScheduledClass(moduleScheduleBO.ModuleScheduleId, moduleScheduleBO.PhaseSchedule.BatchId, date.Date, periodAndroom.FirstOrDefault().PeriodId, batchModuleSchedule))
                                    {
                                        freePeriod = new FreePeriod();
                                        freePeriod.Period = PeriodList.Single(p => p.PeriodId == periodAndroom.FirstOrDefault().PeriodId);
                                        List<ClassRoom> ClassRooms = new List<ClassRoom>();
                                        foreach (var room in periodAndroom)
                                        {
                                            //Check the room is reserved or not
                                            if (!IsRoomReserved(date.Date, periodAndroom.FirstOrDefault().PeriodId, room.ClassRoomId, instructorModuleSchedule))
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


        public List<DateTime> constructTimeSlots(int dayTemplateId)
        {
            try
            {
                HolidayBreakAccess holidayBreakAccess = new HolidayBreakAccess();

                string dayName = "";
                ///////////////////////////start, Get possible days based on trhe given day template/////////////////////////
                List<string> Days = new List<string>();

                var result = db.Days.Where(d => d.Status.ToLower().Equals("active") && d.DayTemplateId == dayTemplateId).ToList();

                if (result.Count() > 0)
                {
                    var ListOfPossibleDays = result.Select(item => new
                    {
                        item.DayName
                    });
                    var dayList = ListOfPossibleDays.Select(day => day).Distinct().ToList();
                    if (dayList.Count > 0)
                    {
                        foreach (var day in dayList)
                        {
                            Days.Add(day.DayName);
                        }
                    }
                }
                ///////////////////////////start, Get possible days based on trhe given day template/////////////////////////


                List<PeriodView> TimeSlotsPerADay = get_UnionOfWorkingPeriods();
                List<DateTime> TimeSlotList = new List<DateTime>();

                List<Holiday> hodidayList = holidayBreakAccess.List();

                int totalNoOfDays = 31;//Maximum no of days suffiecient to the work space.
                DateTime minimumCourseStartingDate = DateTime.Now;
                //Construct WORKING SPACE TimeSlot of the semister or the year
                for (int i = 0; i < totalNoOfDays; i++)
                {
                    dayName = minimumCourseStartingDate.ToString("dddd");

                    //leave day off and Holidays
                    while (!(Days.Contains(dayName)) || is_Holiday(hodidayList, minimumCourseStartingDate))
                    {
                        minimumCourseStartingDate = minimumCourseStartingDate.AddDays(1);
                        dayName = minimumCourseStartingDate.ToString("dddd");
                    }
                    dayName = minimumCourseStartingDate.ToString("dddd");
                    TimeSlotList.Add(minimumCourseStartingDate);
                    //Schedule for next day
                    minimumCourseStartingDate = minimumCourseStartingDate.AddDays(1);
                }
                return TimeSlotList;
            }
            catch (Exception ex)
            {
                return new List<DateTime>();
            }
        }

        public bool is_Holiday(List<Holiday> hodidayList, DateTime date)
        {
            List<DateTime> IndividualHolidays = new List<DateTime>();
            foreach (var holiday in hodidayList)
            {
                if (date.Date >= holiday.StartDateTime.Date && date.Date <= holiday.EndDateTime.Date)
                    return true;
            }
            return false;
        }

        public List<FreePeriod> get_FreeTimeSlotAndRoomForSpecificDate(DateTime date, int instructorId, int phaseScheduleId, int phaseModuleId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

                //1. Get all free date, period and class room
                List<PeriodView> PeriodList = get_UnionOfWorkingPeriods();
                List<ClassRoom> ClassRoomList = (List<ClassRoom>)classRoomAccess.List();
                List<PeriodClassCombination> periodClassCombination = get_PeriodClassRoomCombination(PeriodList, ClassRoomList);

                //Get all Modules Scheduled at that Date
                List<ModuleSchedule> moduleSchedules = dbContext.ModuleSchedules.Where(Ms => Ms.Date == date && Ms.Status != statusName).ToList();

                var courseSchedule = dbContext.PhaseSchedules.Where(Cs => Cs.PhaseScheduleId == phaseScheduleId).ToList();

                ////Get instructor of the module to be rescheduled                
                ModuleInstructorSchedule moduleInstructorSchedule = moduleInstructorScheduleAccess.GetInstructorSchedule(phaseModuleId, instructorId);

                //get DAY and PERIOD template of a Batch whose module is to be rescheduled 
                List<string> daysTemplate = get_DayTemplateList(courseSchedule.FirstOrDefault().BatchId);
                List<PeriodView> periodsTemplate = get_PeriodTemplateList(courseSchedule.FirstOrDefault().BatchId);


                //Get free Period and Class Room combination/s of specific day
                var freePeriodAndRoom = periodClassCombination.Where(PRC => !(moduleSchedules.Any(ms => (ms.PeriodId == PRC.PeriodId)
                    && (ms.ClassRoomId == PRC.ClassRoomId)))).ToList();

                var PeiodRoomGroup = freePeriodAndRoom.GroupBy(x => new { x.PeriodId }).Select(grp => grp.ToList()).ToList();




                List<FreePeriod> FreePeriodList = new List<FreePeriod>();
                FreePeriod FreePeriod = null;
                foreach (var periodAndroom in PeiodRoomGroup)
                {
                    string day = date.Date.ToString("dddd");
                    //string period = PeriodList.Single(p => p.PeriodId == periodAndroom.FirstOrDefault().PeriodId).Period;

                    //Check if this instructor is free at this DAY and PERIOD
                    if (IsThisInstructorFree(moduleInstructorSchedule, date.Date, periodAndroom.FirstOrDefault().PeriodId))
                    {
                        //Check whether it is a suitable Batch Day template and Period template, Does Batch Class Has Another Class 
                        //and weather the instructor has no class                
                        if (Is_ValidDayAndPeriodForBatch(daysTemplate, periodsTemplate, day, periodAndroom.FirstOrDefault().PeriodId) && !DoesBatchClassHasAnotherScheduledClass(-1, courseSchedule.FirstOrDefault().BatchId, date, periodAndroom.FirstOrDefault().PeriodId))
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
                        else
                        {
                            //class/room
                        }
                    }
                    else
                    {
                        // instractor is not free
                    }
                }
                return FreePeriodList.ToList();
            }
            catch (Exception ex)
            {
                return new List<FreePeriod>();
            }
        }


        public bool Is_ValidDayAndPeriodForBatch(List<string> daytemplate, List<PeriodView> PeriodTemplate, string day, int periodId)
        {
            try
            {
                var period = PeriodTemplate.Where(p => p.PeriodId == periodId).ToList();
                if (daytemplate.Contains(day) && period.Count > 0)
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

        public bool DoesBatchClassHasAnotherScheduledClass(int moduleScheduleId, int batchClassId, DateTime date, int periodId, List<ModuleSchedule> batchModuleSchedule)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var moduleScheduleBO = batchModuleSchedule.Where(ms => (ms.ModuleScheduleId != moduleScheduleId)
                && (ms.PhaseSchedule.BatchId == batchClassId)
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

        public bool DoesBatchClassHasAnotherScheduledClass(int moduleScheduleId, int batchClassId, DateTime date, int periodId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);


                var moduleScheduleBO = db.ModuleSchedules.Where(ms => (ms.ModuleScheduleId != moduleScheduleId)
                && (ms.PhaseSchedule.BatchId == batchClassId)
                    && ((ms.Date == date) && (ms.PeriodId == periodId)) && ms.Status != statusName).ToList();
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
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);


                List<ModuleInstructorSchedule> freeInstructorList = new List<ModuleInstructorSchedule>();
                foreach (var instructor in potentialInstructors)
                {
                    var moduleSchedules = dbContext.ModuleSchedules.Where(MS => (MS.InstructorId == instructor.InstructorId)
                  && ((MS.Date == date) && (MS.PeriodId == periodId)) && MS.Status != statusName).ToList();
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
                    var result = db.PhaseSchedules.ToList();

                    if (result.Count > 0)
                    {
                        var phaseSchedules = result.GroupBy(PS => PS.BatchId).Select(grp => grp.FirstOrDefault()).ToList();

                        var filteredResult = phaseSchedules.Select(item => new FilterView
                        {
                            Id = item.BatchId,
                            Name = item.Batch.BatchName
                        }).ToList();
                        if (filteredResult.Count > 0)
                            return filteredResult.ToList();
                    }
                }
                else if (filterBy.Equals("Program"))
                {
                    var result = dbContext.Programs.Where(p => p.Status == "Active" && p.EndDate > DateTime.Now).ToList();
                    if (result.Count > 0)
                    {
                        var filteredResult = result.Select(item => new FilterView
                        {
                            Id = (int)(item.RevisionGroupId == null ? item.ProgramId : item.RevisionGroupId),

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
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

                PTSContext db = new PTSContext();
                AttendanceView attendanceView = new AttendanceView();
                ModuleSchedule moduleSchedule = db.ModuleSchedules.Where(ms => ms.Status != statusName && ms.ModuleScheduleId == moduleScheduleId).ToList().FirstOrDefault();
                if (moduleSchedule != null)
                {
                    // Get All Trainee of a specific Batch Class
                    List<TraineeBatchClass> TraineeBatchClass = db.TraineeBatchClasses.Where(TBC => TBC.BatchClassId == moduleSchedule.PhaseSchedule.BatchId).ToList();
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
                            TraineeNameAndLesson = trainee.Trainee.Person.FirstName + ' ' + trainee.Trainee.Person.MiddleName + " - " + trainee.Trainee.Person.CompanyId
                        }).ToList();
                    }
                    List<PotentialInstructorView> instructors = new List<PotentialInstructorView>();
                    if (potentialInstructors.Count > 0)
                    {
                        instructors = potentialInstructors.Select(instructor => new PotentialInstructorView
                        {
                            InstructorId = instructor.InstructorId,
                            NameAndCompanyId = instructor.Instructor.Person.FirstName + ' ' + instructor.Instructor.Person.MiddleName + " - " + instructor.Instructor.Person.CompanyId
                        }).ToList();
                    }

                    //Get Module Activities ISTAken
                    var moduleActivies = db.ModuleActivitys.Where(ma => ma.ModuleId == moduleSchedule.ModuleId).ToList();
                    List<ModuleActivityView> moduleActivityList = new List<ModuleActivityView>();
                    bool IsTaken = false;
                    foreach (var moduleActivity in moduleActivies)
                    {
                        IsTaken = false;
                        var moduleActivityLog = db.ModuleActivityLogs.Where(MAL => MAL.ModuleSchedule.PhaseSchedule.BatchId == moduleSchedule.PhaseSchedule.BatchId && MAL.ModuleSchedule.ModuleId == moduleSchedule.ModuleId && MAL.ModuleActivityId == moduleActivity.ModuleActivityId).ToList();
                        if (moduleActivityLog.Count > 0)
                            IsTaken = true;
                        moduleActivityList.Add(new ModuleActivityView
                        {
                            Id = moduleActivity.ModuleActivityId,
                            Name = moduleActivity.ModuleActivityName,
                            IsTaken = IsTaken
                        });
                    }

                    attendanceView.ClassRooms.AddRange(ClassRoomList);
                    attendanceView.PotentialInstructors.AddRange(instructors);
                    attendanceView.Trainees.AddRange(TraineeList);
                    if (moduleActivityList.Count > 0)
                        attendanceView.ModuleActivities.AddRange(moduleActivityList);
                    else
                        attendanceView.ModuleActivities.AddRange(new List<ModuleActivityView>());
                    return attendanceView;
                }
                else
                    return new AttendanceView();
            }
            catch (Exception ex)
            {
                return new AttendanceView();
            }
        }

        /*Begin Newly added methods*/
        public bool IsThisInstructorFree(ModuleInstructorSchedule instructor, DateTime date, int periodId, List<ModuleSchedule> moduleScheduleList, int moduleScheduleId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var scheduledEvents = moduleScheduleList.Where(MS => (MS.InstructorId == instructor.InstructorId) && (MS.Date == date) && MS.ModuleScheduleId != moduleScheduleId).ToList();

                var period = dbContext.Periods.Find(periodId);
                DateTime periodStartTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.StartTime)));
                DateTime periodEndTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.EndTime)));

                bool IsInstructorFree = true;
                DateTime scheduledStartTime = DateTime.Now;
                DateTime scheduledEndTime = DateTime.Now;
                foreach (var schedule in scheduledEvents)
                {
                    var objPeriod = dbContext.Periods.Find(schedule.PeriodId);
                    scheduledStartTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(objPeriod.StartTime)));
                    scheduledEndTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(objPeriod.EndTime)));

                    if ((schedule.PeriodId == periodId) || ((periodStartTime >= scheduledStartTime && periodStartTime <= scheduledEndTime) || (periodEndTime >= scheduledStartTime && periodEndTime <= scheduledEndTime)))
                    {
                        IsInstructorFree = false;
                        break;
                    }
                }
                return IsInstructorFree;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsThisInstructorFree(ModuleInstructorSchedule instructor, DateTime date, int periodId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);

                var scheduledEvents = db.ModuleSchedules.Where(MS => (MS.InstructorId == instructor.InstructorId) && (MS.Date == date) && MS.Status != statusName).ToList();

                var period = dbContext.Periods.Find(periodId);
                DateTime periodStartTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.StartTime)));
                DateTime periodEndTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.EndTime)));

                bool IsInstructorFree = true;
                DateTime scheduledStartTime = DateTime.Now;
                DateTime scheduledEndTime = DateTime.Now;
                foreach (var schedule in scheduledEvents)
                {
                    var objPeriod = dbContext.Periods.Find(schedule.PeriodId);
                    scheduledStartTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(objPeriod.StartTime)));
                    scheduledEndTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(objPeriod.EndTime)));

                    if ((schedule.PeriodId == periodId) || ((periodStartTime > scheduledStartTime && periodStartTime < scheduledEndTime) || (periodEndTime > scheduledStartTime && periodEndTime < scheduledEndTime)))
                    {
                        IsInstructorFree = false;
                        break;
                    }
                }
                return IsInstructorFree;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsRoomReserved(DateTime date, int periodId, int roomId, List<ModuleSchedule> moduleScheduleList)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var periodList = db.Periods.Where(p => p.EndDate > DateTime.Now).ToList();
                var period = periodList.Where(p => p.PeriodId == periodId).FirstOrDefault();


                DateTime periodStartTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.StartTime)));
                DateTime periodEndTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.EndTime)));

                var scheduledEvent = moduleScheduleList.Where(MS => ((MS.Date == date) && (MS.ClassRoomId == roomId))).ToList();

                bool IsRoomReserved = false;
                DateTime scheduledStartTime = DateTime.Now;
                DateTime scheduledEndTime = DateTime.Now;

                foreach (var schedule in scheduledEvent)
                {
                    var objPeriod = periodList.Where(p => p.PeriodId == schedule.PeriodId).FirstOrDefault();
                    scheduledStartTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(objPeriod.StartTime)));
                    scheduledEndTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(objPeriod.EndTime)));

                    if ((schedule.PeriodId == periodId) || ((periodStartTime >= scheduledStartTime && periodStartTime <= scheduledEndTime) || (periodEndTime >= scheduledStartTime && periodEndTime <= scheduledEndTime)))
                    {
                        IsRoomReserved = true;
                        break;
                    }
                }
                return IsRoomReserved;
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
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);


                var periodList = db.Periods.Where(p => p.EndDate > DateTime.Now).ToList();
                var period = periodList.Where(p => p.PeriodId == periodId).FirstOrDefault();


                DateTime periodStartTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.StartTime)));
                DateTime periodEndTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.EndTime)));

                var scheduledEvent = dbContext.ModuleSchedules.Where(MS => ((MS.Date == date) && (MS.ClassRoomId == roomId) && MS.Status != statusName)).ToList();

                bool IsRoomReserved = false;
                DateTime scheduledStartTime = DateTime.Now;
                DateTime scheduledEndTime = DateTime.Now;

                foreach (var schedule in scheduledEvent)
                {
                    var objPeriod = periodList.Where(p => p.PeriodId == schedule.PeriodId).FirstOrDefault();
                    scheduledStartTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(objPeriod.StartTime)));
                    scheduledEndTime = Convert.ToDateTime(Convert.ToDateTime(schedule.Date + TimeSpan.Parse(objPeriod.EndTime)));

                    if ((schedule.PeriodId == periodId) || ((periodStartTime >= scheduledStartTime && periodStartTime <= scheduledEndTime) || (periodEndTime >= scheduledStartTime && periodEndTime <= scheduledEndTime)))
                    {
                        IsRoomReserved = true;
                        break;
                    }
                }
                return IsRoomReserved;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<FreeInstructor> GetFreeInstructorForSpecificDate(DateTime date, int batchId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                string statusName = GetModuleScheduleStatusName((int)ModuleScheduleStatus.Canceled);


                //var allInstructors = dbContext.ModuleInstructorSchedules.ToList().GroupBy(MIS => MIS.InstructorId).Select(grp => grp.FirstOrDefault());

                var instructors = (from BC in dbContext.BatchModules
                                   join MI in dbContext.ModuleInstructorSchedules on (BC.Module.RevisionGroupId == null ? BC.ModuleId : BC.Module.RevisionGroupId) equals MI.ModuleId
                                   where BC.BatchCourse.BatchCategory.BatchId == batchId
                                   select new FreeInstructor
                                   {
                                       InstructorId = MI.InstructorId,
                                       NameAndCompanyId = MI.Instructor.Person.FirstName + " " + MI.Instructor.Person.MiddleName + " " + MI.Instructor.Person.CompanyId
                                   }).ToList();

                var distinictInstructor = instructors.GroupBy(I => I.InstructorId).Select(grp => grp.FirstOrDefault()).ToList();





                //Check whether Instructor is busy all the period or not
                List<FreeInstructor> freeInstructorList = new List<FreeInstructor>();
                foreach (var instructor in distinictInstructor)
                {
                    var instructorDetail = db.Instructors.Find(instructor.InstructorId);
                    //Check whether instructor is on leave or not 
                    var personLeaveList = db.PersonLeaves.Where(pl => pl.PersonId == instructorDetail.PersonId).ToList();
                    if (personLeaveList.Where(pl => pl.FromDate <= date && pl.EndDate >= date).ToList().Count == 0)
                    {
                        var instructorSchedules = dbContext.ModuleSchedules.Where(MS => MS.Date == date && MS.InstructorId == instructor.InstructorId && MS.Status != statusName).ToList();
                        foreach (var schedule in instructorSchedules)
                        {
                            //Check Potential instructor/s assigned to teach modules of the selected batch class are busy in predefined period template. 
                            if (IsThisInstructorHasFreeTime(instructor.InstructorId, batchId, date, schedule.PeriodId))
                            {
                                freeInstructorList.Add(instructor);
                                break;
                            }
                        }
                        if (instructorSchedules.Count == 0)
                        {
                            freeInstructorList.Add(instructor);
                        }
                    }
                }
                return freeInstructorList.ToList();
            }
            catch (Exception ex)
            {
                return new List<FreeInstructor>();
            }
        }

        public bool IsThisInstructorHasFreeTime(int instructorId, int batchClassId, DateTime date, int scheduledPeriodId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                //Get all possible period of that batch class               
                var objBatchClass = dbContext.BatchClasses.Find(batchClassId);
                var batchClassPeriods = dbContext.Periods.Where(p => p.PeriodTemplateId == objBatchClass.Batch.PeriodTemplateId).ToList();

                var objPeriod = dbContext.Periods.Find(scheduledPeriodId);
                DateTime periodStartTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(objPeriod.StartTime)));
                DateTime periodEndTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(objPeriod.EndTime)));

                bool isPeriodValid = false;

                DateTime scheduledStartTime = DateTime.Now;
                DateTime scheduledEndTime = DateTime.Now;

                foreach (var period in batchClassPeriods)
                {
                    scheduledStartTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.StartTime)));
                    scheduledEndTime = Convert.ToDateTime(Convert.ToDateTime(date.Date + TimeSpan.Parse(period.EndTime)));

                    if (!((period.PeriodId == scheduledPeriodId) || ((periodStartTime >= scheduledStartTime && periodStartTime <= scheduledEndTime) || (periodEndTime >= scheduledStartTime && periodEndTime <= scheduledEndTime))))
                    {
                        isPeriodValid = true;
                        break;
                    }
                }
                return isPeriodValid;
            }
            catch (Exception ex)
            {
                return false;
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
                var moduleSchedules = ModuleScheduleList.Where(ms => ms.PeriodId == periodId).ToList();
                var scheduledClassRoomIdList = moduleSchedules.Select(ms => ms.ClassRoomId).ToList();
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

        public static string GetModuleScheduleStatusName(int moduleScheduleStatus)
        {
            return Enum.GetName(typeof(ModuleScheduleStatus), moduleScheduleStatus);
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
