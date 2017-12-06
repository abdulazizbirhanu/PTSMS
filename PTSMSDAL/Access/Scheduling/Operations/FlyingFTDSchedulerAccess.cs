using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class FlyingFTDSchedulerAccess
    {
        private PTSContext db = new PTSContext();
        /*Constraint #1*/
        public int PreSoloInstructorConsistency(FlyingFTDSchedule schedule)
        {
            LessonAccess lessonAccess = new LessonAccess();
            schedule.Lesson = (Lesson)lessonAccess.Details(schedule.LessonId);

            //make IsPreSolo bool instead of string
            if (!schedule.Lesson.IsPreSolo)
                return 0;

            var result=db.FlyingFTDSchedules
                .Where(sc=>sc.TraineeId.Equals(schedule.TraineeId) && sc.Lesson.IsPreSolo).OrderByDescending(o=>o.FlyingFTDScheduleId).Take(1).ToList();

            if(result.Count() == 0)
                return 10; //must not be hardcoded

            foreach (var psch in result)
            {
                return psch.EquipmentId == schedule.EquipmentId ? -10 : 10; //must not be hardcoded
            }

            return 0; //never be used
        }

        /*Constraint #5*/
        public int MultiEngineRequirementConsistency(FlyingFTDSchedule schedule)
        {
            EquipmentAccess equipmentAccess = new EquipmentAccess();
            LessonAccess lessonAccess = new LessonAccess();

            schedule.Equipment = equipmentAccess.Details(schedule.EquipmentId);
            schedule.Lesson = (Lesson)lessonAccess.Details(schedule.LessonId);

            if ((schedule.Lesson.CategoryName != "Night Flying" && schedule.Lesson.CategoryName != "Multi Engine")
                && schedule.Equipment.EquipmentModel.IsMultiEngine)
                return -10;
            return 10;
        }

        /*Constraint #6*/
        public int EquipmentWorkingHourValidity(FlyingFTDSchedule schedule)
        {
            EquipmentAccess equipmentAccess = new EquipmentAccess();
            schedule.Equipment = equipmentAccess.Details(schedule.EquipmentId);

            var startTime=Convert.ToDateTime(Convert.ToDateTime(schedule.ScheduleStartTime.Date + TimeSpan.Parse(schedule.Equipment.StartTime)).ToString("MM/dd/yyyy hh:mm")).AddHours(3);

            var timeSpan = TimeSpan.FromMinutes(schedule.Equipment.WorkingHours);
            var endTime = startTime.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).AddSeconds(timeSpan.Seconds); 

            if (startTime < schedule.ScheduleStartTime && schedule.ScheduleStartTime < endTime)
                return 10;
            return -10;
        }

        /*Constraint #7*/
        public int BatchEquipmentModelValidity(FlyingFTDSchedule schedule)
        {
            EquipmentAccess equipmentAccess = new EquipmentAccess();
            schedule.Equipment = equipmentAccess.Details(schedule.EquipmentId);

            //Batch must include Status to check determine the active running batch
            //var result = db.TraineeSyllabuses.Select(p=>p.TraineeId.Equals(schedule.TraineeId) && p.Batch.Status="Active").

            var traineeBatch = db.TraineeSyllabuses.Where(p => p.TraineeId.Equals(schedule.TraineeId)).FirstOrDefault().Batch;
            var batchModels = db.BatchEquipmentModels.Where(em => em.EquipmentModel.Equals(schedule.Equipment.EquipmentModelId));
            if (batchModels.Count() == 0)
                return -10;
            return 10;
        }

        /*Constraint #8*/
        public int EquipmentDowntimeCheck(FlyingFTDSchedule schedule)
        {
            var result = db.EquipmentDowntimeSchedules
                .Where(ed => ed.ScheduleStartDate <= schedule.ScheduleStartTime && ed.ScheduleEndDate >= schedule.ScheduleStartTime);

            if (result.Count() == 0)
                return 10;


            return -10;
        }

        /*Constraint #9*/
        public int EquipmentRecurringDowntimeCheck(FlyingFTDSchedule schedule)
        {
            EquipmentAccess equipmentAccess = new EquipmentAccess();
            schedule.Equipment = equipmentAccess.Details(schedule.EquipmentId);

            if (schedule.Equipment.EquipmentModel.EquipmentModelName != "FTD")
                return 0;

            var result = db.FTDRecurringDownTimes.ToList();

            foreach (var rt in result)
            {
                var startTime = Convert.ToDateTime(Convert.ToDateTime(schedule.ScheduleStartTime.Date + TimeSpan.Parse(rt.StartingTime)).ToString("MM/dd/yyyy hh:mm")).AddHours(3);
                var endTime = Convert.ToDateTime(Convert.ToDateTime(schedule.ScheduleStartTime.Date + TimeSpan.Parse(rt.EndingTime)).ToString("MM/dd/yyyy hh:mm")).AddHours(3);

                if (startTime < schedule.ScheduleStartTime && schedule.ScheduleStartTime < endTime)
                    return -10;
                return 10;
            }

            return 10;
        }

        /*Constraint #10*/
        public int EquipmentCerteficateExpirationCheck(FlyingFTDSchedule schedule)
        {
            EquipmentAccess equipmentAccess = new EquipmentAccess();
            schedule.Equipment = equipmentAccess.Details(schedule.EquipmentId);

            if (schedule.Equipment.EquipmentModel.EquipmentModelName == "FTD")
                return 0;

            var result = db.EquipmentCertificates
                .Where(ec => ec.StartingDate > schedule.ScheduleStartTime || ec.EndingDate < schedule.ScheduleStartTime);

            if (result.Count() == 0)
                return 10;


            return -10;
        }

        /*Constraint #11-1*/
        public int TraineeLicenseExpiryValidity(FlyingFTDSchedule schedule)
        {
            TraineeAccess traineeAccess = new TraineeAccess();
            schedule.Trainee = (Trainee)traineeAccess.Details(schedule.TraineeId);

            //if no record found, then considered as valid
            if (db.Licenses.Where(lc => lc.PersonId.Equals(schedule.Trainee.Person.PersonId)).Count() == 0)
                return 10;


            var result = db.Licenses
                .Where(lc => lc.PersonId.Equals(schedule.Trainee.Person.PersonId) && lc.ExpiryDate > schedule.ScheduleStartTime);

            if (result.Count() == 0)
                return 10;


            return -10;
        }

        /*Constraint #11-2*/
        public int InstructorLicenseExpiryValidity(FlyingFTDSchedule schedule)
        {
            InstructorAccess instructorAccess = new InstructorAccess();
            schedule.Instructor = (Instructor)instructorAccess.Details(schedule.InstructorId);

            //if no record found, then considered as valid
            if (db.Licenses.Where(lc => lc.PersonId.Equals(schedule.Instructor.Person.PersonId)).Count() == 0)
                return 10;


            var result = db.Licenses
                .Where(lc => lc.PersonId.Equals(schedule.Instructor.Person.PersonId) && lc.ExpiryDate > schedule.ScheduleStartTime);

            if (result.Count() == 0)
                return 10;


            return -10;
        }

        /*Constraint #12*/
        public int EquipmentRotationFairnessConsistency(FlyingFTDSchedule schedule)
        {

            //only considers the last schedule

            var result = db.FlyingFTDSchedules
                .Where(sc => sc.TraineeId.Equals(schedule.TraineeId)).OrderByDescending(o => o.FlyingFTDScheduleId).Take(1).ToList();

            if (result.Count() == 0)
                return 10; //must not be hardcoded

            foreach (var psch in result)
            {
                return psch.EquipmentId == schedule.EquipmentId ? -10 : 10; //must not be hardcoded
            }

            return 0; //never been used
        }

        /*Constraint #13*/
        public int PredefinedSchedulingFactorsCheck(FlyingFTDSchedule schedule)
        {
            //this requires implementation when configuration values(scheduling factors) are persisted/stored
            //requires model definition to persist it
            return 0;
        }

        /*Constraint #14*/
        public int TraineeInstructorAssociationConsistency(FlyingFTDSchedule schedule)
        {
            InstructorAccess instructorAccess = new InstructorAccess();
            schedule.Instructor = (Instructor)instructorAccess.Details(schedule.InstructorId);

            var result = db.FlyingFTDSchedules
                .Where(sc => sc.TraineeId.Equals(schedule.TraineeId)).ToList();

            if(result.Count()==0)
                return 10;

            decimal maxPercentage=0;
            int maxInstructorId=0;

            foreach (var item in result)
            {
                var percentage = result.Where(ins => ins.InstructorId.Equals(item.InstructorId)).ToList().Count() * 100 / result.Count();
                maxPercentage = percentage > maxPercentage ? percentage : maxPercentage;
                maxInstructorId = percentage > maxPercentage ? item.InstructorId : maxInstructorId;
            }

            if (maxPercentage > 90 && schedule.InstructorId.Equals(maxInstructorId))
                return 10;

            return -10;
        }

        /*Constraint #15*/
        public int PreSoloLessonMorningAssociationCheck(FlyingFTDSchedule schedule)
        {
            if (!schedule.Lesson.IsPreSolo)
                return 0;

            var midDayTime = Convert.ToDateTime(Convert.ToDateTime(schedule.ScheduleStartTime.Date + TimeSpan.Parse("12:00")).ToString("MM/dd/yyyy hh:mm")).AddHours(3);
            if (schedule.ScheduleStartTime < midDayTime)
                return 30;
            return -30;
        }
    }
}
