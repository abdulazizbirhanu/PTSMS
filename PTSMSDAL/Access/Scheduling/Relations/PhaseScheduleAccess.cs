using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class PhaseScheduleAccess
    {
        private PTSContext db = new PTSContext();

        public object List()
        {
            return db.PhaseSchedules.ToList();
        }
        public List<PhaseCourse> ListPhaseCourses(int instructorId, int batchId)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                List<PhaseCourse> PhaseCourseList = new List<PhaseCourse>();
                var PhaseCourses = (from MIS in dbContext.ModuleInstructorSchedules
                                    join BM in dbContext.BatchModules on MIS.ModuleId equals BM.ModuleId
                                    join BC in dbContext.BatchCourses on BM.BatchCourseId equals BC.BatchCourseId
                                    join PS in dbContext.PhaseSchedules on BC.BatchCategory.BatchId equals PS.BatchId
                                    where MIS.InstructorId == instructorId && PS.BatchId == batchId && PS.PhaseId == BM.PhaseId
                                    select new PhaseCourse
                                    {
                                        CourseId = BC.CourseId,
                                        PhaseScheduleId = PS.PhaseScheduleId,
                                        PhaseAndCourseName = PS.Phase.Name + "-" + BC.Course.CourseCode + " - " + BC.Course.CourseTitle,
                                        ModuleId = BM.ModuleId
                                    }).ToList();
                var phaseCoursesGroup = PhaseCourses.GroupBy(PC => new { PC.PhaseScheduleId, PC.CourseId }).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.PhaseScheduleId).ToList();


                foreach (var courses in phaseCoursesGroup)
                {
                    PhaseCourseList.Add(courses);
                }
                return PhaseCourseList.ToList();
            }
            catch (Exception ex)
            {
                return new List<PhaseCourse>();
            }
        }
        
        public List<BatchClasses> ListBatchPhase()
        {
            try
            {
                PTSContext db = new PTSContext();
                var phases = db.Phases.Where(P => P.EndDate > DateTime.Now).ToList();
                var batchClasses = db.BatchClasses.Where(B => B.EndDate > DateTime.Now).ToList();
                var Locations = db.Locations.Where(L => L.EndDate > DateTime.Now).ToList();

                List<BatchClasses> batchList = new List<BatchClasses>();
                List<PhaseView> PhaseList = null;
                List<LocationView> LocationList = null;
                BatchClasses BatchClas = null;

                var phaseScheduleList = db.PhaseSchedules.ToList();
                bool isScheduled = false;
                foreach (var batchClass in batchClasses)
                {
                    BatchClas = new BatchClasses();
                    BatchClas.BactchClass = new BactchClassView
                    {
                        Id = batchClass.BatchClassId,
                        Name = batchClass.BatchClassName
                    };
                    PhaseList = new List<PhaseView>();
                    //For Phases 
                    foreach (var phase in phases)
                    {
                        var batchPhaseSchedules = phaseScheduleList.Where(PS => PS.BatchId == batchClass.BatchId && PS.PhaseId == phase.PhaseId).ToList();

                        if (batchPhaseSchedules.Count > 0)
                            isScheduled = true;
                        else
                            isScheduled = false;

                        PhaseList.Add(new PhaseView
                        {
                            Id = phase.PhaseId,
                            Name = phase.Description,
                            isScheduled = isScheduled,
                            PhaseSequence = phase.PhaseSequence
                        });
                    }
                    //For Locations
                    LocationList = new List<LocationView>();
                    foreach (var location in Locations)
                    {
                        LocationList.Add(new LocationView
                        {
                            Id = location.LocationId,
                            Name = location.LocationName
                        });
                    }
                    //Save on the 
                    if (PhaseList.Count > 0)
                    {
                        BatchClas.PhaseList.AddRange(PhaseList);
                    }
                    if (LocationList.Count > 0)
                        BatchClas.LocationList.AddRange(LocationList);
                    if (BatchClas.PhaseList.Count > 0)
                    {
                        batchList.Add(BatchClas);
                    }
                }
                batchList = batchList.OrderBy(x => x.BactchClass.Name).ToList();
                return batchList;
            }
            catch (System.Exception)
            {
                return new List<BatchClasses>();
            }
        }

        public bool SaveCourseModuleSequence(List<Courses> CoursesModuleSequence, PhaseSchedule phaseSchedule)
        {
            using (var dbContext = new PTSContext())
            {
                using (var dbContextTransaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var result = dbContext.PhaseSchedules.Where(PS => PS.BatchId == phaseSchedule.BatchId && PS.PhaseId == phaseSchedule.PhaseId && PS.LessonCategoryTypeId == phaseSchedule.LessonCategoryTypeId).ToList();
                        phaseSchedule = result.First();

                        foreach (var courses in CoursesModuleSequence)
                        {
                            foreach (var module in courses.Modules)
                            {
                                var bModuleResult = dbContext.BatchModules.Where(MS => MS.BatchCourse.BatchCategory.BatchId == phaseSchedule.BatchId && MS.ModuleId == module.Id && MS.PhaseId == phaseSchedule.PhaseId).ToList();
                                var bModule = bModuleResult.First();
                                if (bModuleResult.Count > 0)
                                {
                                    bModule.Sequence = module.Sequence;
                                    //save to db
                                }
                            }
                        }
                        dbContext.SaveChanges();
                        dbContextTransaction.Commit();
                        return true;
                    }
                    catch (System.Exception e)
                    {
                        dbContextTransaction.Rollback();
                        return false;
                    }
                }
            }
        }
        public object Details(int id)
        {
            try
            {
                PhaseSchedule courseSchedule = db.PhaseSchedules.Find(id);
                if (courseSchedule == null)
                {
                    return false; // Not Found
                }
                return courseSchedule; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public object Add(PhaseSchedule courseSchedule)
        {
            try
            {
                db.PhaseSchedules.Add(courseSchedule);
                db.SaveChanges();
                return true; // Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public object Revise(PhaseSchedule courseSchedule)
        {
            try
            {
                db.Entry(courseSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public object Delete(int id)
        {
            try
            {
                PhaseSchedule courseSchedule = db.PhaseSchedules.Find(id);
                db.PhaseSchedules.Remove(courseSchedule);
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }

    }
    public class PhaseCourse
    {
        public int CourseId { get; set; }
        public string PhaseAndCourseName { get; set; }
        public int PhaseScheduleId { get; set; }
        public int ModuleId { get; set; }
    }
    public class PhaseModules
    {
        public int ModuleId { get; set; }
        public float ModuleDuration { get; set; }
        public string ModuleName { get; set; }
    }

    public class ScheduledModules
    {
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public int ModuleScheduleId { get; set; }
    }

}