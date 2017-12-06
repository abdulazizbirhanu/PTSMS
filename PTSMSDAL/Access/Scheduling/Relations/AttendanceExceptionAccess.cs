using PTSMSDAL.Context;
using PTSMSDAL.Generic;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Relations
{
    public class AttendanceExceptionAccess
    {
        private PTSContext db = new PTSContext();

        public List<AttendanceException> List()
        {
            return db.AttendanceExceptions.ToList();
        }

        public AttendanceException Details(int id)
        {
            try
            {
                AttendanceException Attendances = db.AttendanceExceptions.Find(id);
                if (Attendances == null)
                {
                    return null; // Not Found
                }
                return Attendances; // Success
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }

        public bool Add(ActualModuleTaken actualModuleTaken, string[] traineeList, string note, int moduleScheduleId)
        {
            using (var dbContext = new PTSContext())
            {
                using (var dbContextTransaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        actualModuleTaken.StartDate = DateTime.Now;
                        actualModuleTaken.EndDate = Constants.EndDate;
                        actualModuleTaken.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        actualModuleTaken.CreationDate = DateTime.Now;

                        dbContext.ActualModuleTakens.Add(actualModuleTaken);
                        dbContext.SaveChanges();

                        int actualModuleTakenId = GetCurrentActualModuleTakenId();
                        
                        // Get All Trainee of a specific Batch Class
                        ModuleSchedule moduleSchedule = db.ModuleSchedules.Find(moduleScheduleId);
                        List<TraineeBatchClass> traineeBatchClass = db.TraineeBatchClasses.Where(TBC => TBC.BatchClass.BatchId == moduleSchedule.PhaseSchedule.BatchId).ToList();

                        string traineeStatus = string.Empty;
                        string Note = string.Empty;
                        foreach (var trainee in traineeBatchClass)
                        {
                            var absentTrainee = traineeList.Where(x => x == trainee.TraineeId.ToString()).ToList();

                            if (absentTrainee.Count > 0)
                            {
                                traineeStatus = Enum.GetName(typeof(TraineeAttendanceStatus), TraineeAttendanceStatus.Absent);
                                Note = note;
                            }
                            else
                            {
                                traineeStatus = Enum.GetName(typeof(TraineeAttendanceStatus), TraineeAttendanceStatus.Present);
                                Note = "";
                            }
                           
                            dbContext.AttendanceExceptions.Add(new AttendanceException
                            {
                                ActualModuleTakenId = actualModuleTakenId,
                                Note = Note,
                                TraineeId = trainee.TraineeId,
                                TraineeStatus = traineeStatus,
                                StartDate = DateTime.Now,
                                EndDate = Constants.EndDate,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                CreationDate = DateTime.Now
                            });
                            dbContext.SaveChanges();
                        }
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

        public bool Revise(AttendanceException attendanceException, ActualModuleTaken actualModuleTaken)
        {
            try
            {
                db.Entry(actualModuleTaken).State = EntityState.Modified;
                db.Entry(attendanceException).State = EntityState.Modified;
                db.SaveChanges();
                return true;// Success
            }
            catch (System.Exception e)
            {
                return false; // Exception
            }
        }
        public int GetCurrentActualModuleTakenId()
        {
            return Convert.ToInt32(db.Database.SqlQuery<decimal>("Select IDENT_CURRENT ('REL_ACTUAL_MODULE_TAKEN')", new object[0]).FirstOrDefault());
        }
    }
}
