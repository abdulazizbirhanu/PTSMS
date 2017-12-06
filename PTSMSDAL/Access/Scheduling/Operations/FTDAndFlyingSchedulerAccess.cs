using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Access.Utility;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Curriculum.Relations;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class FTDAndFlyingSchedulerAccess
    {
        PTSContext db = new PTSContext();
        public List<InstructorView> get_FTDAndFlyingInstructorList()
        {
            try
            {
                //Get all instructors whose Qualification Type is FTD and Flying
                PTSContext db = new PTSContext();

                var result = db.InstructorQualifications.Where(IQ => IQ.QualificationType.Type.ToLower().Contains("flying") || IQ.QualificationType.Type.ToLower().Contains("simulation") || IQ.QualificationType.Type.ToLower().Contains("ftd")).ToList();

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

        public List<TraineeLessonScheduleView> get_TraineeLessonSchedulerList()
        {
            try
            {
                List<TraineeLessonScheduleView> traineeLessonScheduleViewList = new List<TraineeLessonScheduleView>();

                int i = 1;
                PTSContext dbContext = new PTSContext();

                var traineeLessonScheduleList = (from bCat in dbContext.BatchCategories
                                                 join tBc in dbContext.TraineeBatchClasses on bCat.BatchId equals tBc.BatchClass.BatchId
                                                 join pSched in dbContext.PhaseSchedules on bCat.BatchId equals pSched.BatchId
                                                 join bLes in dbContext.BatchLessons on bCat.BatchCategoryId equals bLes.BatchCategoryId
                                                 where bLes.Sequence > 0 && (bCat.Category.CategoryType.Type.ToUpper() == "FTD" || bCat.Category.CategoryType.Type.ToUpper() == "FLYING")
                                                 select new
                                                 {
                                                     bCat,
                                                     tBc,
                                                     pSched,
                                                     bLes
                                                 }).ToList();

                foreach (var item in traineeLessonScheduleList)
                {
                    traineeLessonScheduleViewList.Add(new TraineeLessonScheduleView
                    {
                        TraineeId = item.tBc.Trainee.Person.PersonId,
                        BatchId = item.bCat.BatchId,
                        PhaseId = item.pSched.PhaseId,
                        BatchClassId = item.tBc.BatchClassId,
                        PhaseScheduleId = item.pSched.PhaseScheduleId,
                        LessonId = item.bLes.LessonId,
                        StartingDate = item.pSched.StartingDate,
                        LessonSequence = Convert.ToInt16(item.bLes.Sequence),
                        TraineeLessonScheduleId = i++
                    });
                }

                var traineeLessonGroup = traineeLessonScheduleViewList.GroupBy(x => new { x.LessonId }).Select(grp => grp.FirstOrDefault()).ToList();
                return traineeLessonGroup.ToList();
            }
            catch (Exception ex)
            {
                return new List<TraineeLessonScheduleView>();
            }
        }


        public bool Add(List<FlyingFTDSchedule> flyingFTDScheduleList)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                foreach (FlyingFTDSchedule flyingFTDSchedule in flyingFTDScheduleList)
                {
                    dbContext.FlyingFTDSchedules.Add(flyingFTDSchedule);
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

        public FlyingFTDSchedule Details(int id)
        {
            try
            {
                PTSContext dbContext = new PTSContext();
                FlyingFTDSchedule flyingFTDSchedule = dbContext.FlyingFTDSchedules.Where(fs => fs.FlyingFTDScheduleId == id).FirstOrDefault();

                return flyingFTDSchedule;
            }
            catch (System.Exception e)
            {
                return null; // Exception
            }
        }


        public bool Revise(FlyingFTDSchedule flyingFTDSchedule)
        {
            try
            {
                db.Entry(flyingFTDSchedule).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<BatchClass> GetBatchClass()
        {
            try
            {
                PTSContext dbContext = new PTSContext();

                var result = from ps in dbContext.PhaseSchedules
                          join bc in dbContext.BatchClasses on ps.BatchId equals bc.BatchId
                          where ps.Status == "Active"
                          select new
                          {
                              ps,
                              bc
                          };

                var resultGrouping = result.GroupBy(pp => pp.bc.BatchClassId).Select(gr => gr.FirstOrDefault()).ToList();
                List<BatchClass> batches = new List<BatchClass>();
                foreach (var t in resultGrouping)
                {
                    batches.Add(t.bc);
                }

                return batches;
            }
            catch (Exception ex)
            {
                return new List<BatchClass>();
            }
        }

        public List<EquipmentsView> GetFreeEquipment(int flyingFTDScheduleId, DateTime lessonStartingDate)
        {
            try
            {

                PTSContext dbContext = new PTSContext();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                var flyingFTDSchedule = dbContext.FlyingFTDSchedules.Where(fs => fs.Status != statusName && fs.FlyingFTDScheduleId == flyingFTDScheduleId).ToList().FirstOrDefault();

                var lesson = dbContext.Lessons.Where(L => L.LessonId == flyingFTDSchedule.LessonId).ToList();

                List<EquipmentsView> EquipmentList = new List<EquipmentsView>();
                if (lesson.Count > 0)
                {
                    //Get lesson ending Time
                    DateTime lessonEndingDate = lessonStartingDate;
                    if (lesson.Count > 0 && lesson.Count > 0)
                    {
                        if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                        {
                            lessonEndingDate = (lessonStartingDate.AddHours((double)lesson.FirstOrDefault().FTDTime));
                        }
                        else if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                        {
                            double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                            lessonEndingDate = lessonStartingDate.AddHours(lessonDuration);
                        }
                    }
                    else
                    {
                        lessonEndingDate = lessonStartingDate.AddHours(1);
                    }


                    var allFlyingFTDSchedules = dbContext.FlyingFTDSchedules.Where(fs => fs.Status != statusName).ToList();

                    List<Equipment> equipmentList = GetValidEquipment(lessonStartingDate);

                    foreach (var equipment in equipmentList)
                    {
                        //Check wether the new equipment has the same EQUIPMENT TYPE with the one it has been scheduled before 
                        if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper())
                        {
                            //CKECK WHETHERE EQUIPMENT IS FREE OR NOT 
                            //Check whether the instructor is free or not in the specified date and time.
                            var flyingFTDScheduleList = allFlyingFTDSchedules.Where(FFS => FFS.EquipmentId == equipment.EquipmentId).ToList();
                            bool isEquipmentFree = false;
                            if (flyingFTDScheduleList.Count > 0)
                            {
                                var scheduledInstructor = flyingFTDScheduleList.Where(SI => ((lessonStartingDate > SI.ScheduleStartTime && lessonStartingDate < SI.ScheduleEndTime) || (lessonEndingDate > SI.ScheduleStartTime && lessonEndingDate < SI.ScheduleEndTime))).ToList();
                                if (scheduledInstructor.Count == 0)
                                    isEquipmentFree = true;
                            }
                            else
                                isEquipmentFree = true;


                            if (isEquipmentFree)
                            {
                                //Collect equipment depend on lesson Category Type Multi-Engine/Cross Country
                                if (lesson.FirstOrDefault().CategoryType.Type.ToUpper() == "ME" || lesson.FirstOrDefault().CategoryType.Type.ToUpper() == "NTF")
                                {
                                    if (equipment.EquipmentModel.IsMultiEngine)
                                    {
                                        EquipmentList.Add(new EquipmentsView
                                        {
                                            Id = equipment.EquipmentId,
                                            Name = equipment.NameOrSerialNo
                                        });
                                    }
                                }
                                else
                                {
                                    if (!(equipment.EquipmentModel.IsMultiEngine))
                                    {
                                        EquipmentList.Add(new EquipmentsView
                                        {
                                            Id = equipment.EquipmentId,
                                            Name = equipment.NameOrSerialNo
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                return EquipmentList;
            }
            catch (Exception ex)
            {
                return new List<EquipmentsView>();
            }
        }

        public List<EquipmentsViewForEdit> GetFreeEquipmentAndInstructors(int flyingFTDScheduleId, DateTime lessonStartingDate)
        {
            try
            {

                PTSContext dbContext = new PTSContext();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                var flyingFTDSchedule = dbContext.FlyingFTDSchedules.Where(fs => fs.Status != statusName && fs.FlyingFTDScheduleId == flyingFTDScheduleId).ToList().FirstOrDefault();

                var lesson = dbContext.Lessons.Where(L => L.LessonId == flyingFTDSchedule.LessonId).ToList();

                List<EquipmentsViewForEdit> EquipmentList = new List<EquipmentsViewForEdit>();
                if (lesson.Count > 0)
                {
                    //Get lesson ending Time
                    DateTime lessonEndingDate = lessonStartingDate;
                    if (lesson.Count > 0 && lesson.Count > 0)
                    {
                        if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                        {
                            lessonEndingDate = (lessonStartingDate.AddHours((double)lesson.FirstOrDefault().FTDTime));
                        }
                        else if (flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                        {
                            double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                            lessonEndingDate = lessonStartingDate.AddHours(lessonDuration);
                        }
                    }
                    else
                    {
                        lessonEndingDate = lessonStartingDate.AddHours(1);
                    }


                    var allFlyingFTDSchedules = dbContext.FlyingFTDSchedules.Where(fs => fs.Status != statusName).ToList();

                    List<Equipment> equipmentList = GetValidEquipment(lessonStartingDate);

                    foreach (var equipment in equipmentList)
                    {
                        //Check wether the new equipment has the same EQUIPMENT TYPE with the one it has been scheduled before 
                        if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == flyingFTDSchedule.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper())
                        {
                            //CKECK WHETHERE EQUIPMENT IS FREE OR NOT 
                            //Check whether the instructor is free or not in the specified date and time.
                            var flyingFTDScheduleList = allFlyingFTDSchedules.Where(FFS => FFS.EquipmentId == equipment.EquipmentId).ToList();
                            bool isEquipmentFree = false;
                            if (flyingFTDScheduleList.Count > 0)
                            {
                                var scheduledInstructor = flyingFTDScheduleList.Where(SI => ((lessonStartingDate > SI.ScheduleStartTime && lessonStartingDate < SI.ScheduleEndTime) || (lessonEndingDate > SI.ScheduleStartTime && lessonEndingDate < SI.ScheduleEndTime))).ToList();
                                if (scheduledInstructor.Count == 0)
                                    isEquipmentFree = true;
                            }
                            else
                                isEquipmentFree = true;


                            if (isEquipmentFree)
                            {
                                //Collect equipment depend on lesson Category Type Multi-Engine/Cross Country
                                if (lesson.FirstOrDefault().CategoryType.Type.ToUpper() == "ME" || lesson.FirstOrDefault().CategoryType.Type.ToUpper() == "NTF")
                                {
                                    if (equipment.EquipmentModel.IsMultiEngine)
                                    {
                                        EquipmentList.Add(new EquipmentsViewForEdit
                                        {
                                            Id = equipment.EquipmentId,
                                            Name = equipment.NameOrSerialNo
                                        });
                                    }
                                }
                                else
                                {
                                    if (!(equipment.EquipmentModel.IsMultiEngine))
                                    {
                                        EquipmentList.Add(new EquipmentsViewForEdit
                                        {
                                            Id = equipment.EquipmentId,
                                            Name = equipment.NameOrSerialNo
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                //Add free Instructors for each Equipment
                string traineeLesson = flyingFTDSchedule.TraineeId + "-" + flyingFTDSchedule.LessonId;
                string[] TraineeLessonIdArray = traineeLesson.Split('~');
                foreach (var equip in EquipmentList)
                {
                    List<FTDandFlyingInstructorView> InstructorList = GetFTDandFlyingInstructors(equip.Id, lessonStartingDate, TraineeLessonIdArray);
                    equip.Instructors = InstructorList;
                }

                return EquipmentList;
            }
            catch (Exception ex)
            {
                return new List<EquipmentsViewForEdit>();
            }
        }
        public List<InstructorColor> FlyingAndFTDInstructorColorList()
        {
            PTSContext dbContext = new PTSContext();
            var random = new Random();
            string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
            List<Scheduler> ScheduledEvents = new List<Scheduler>();
            var scheduleList = dbContext.FlyingFTDSchedules.Where(ms => ms.Status != statusName).ToList();

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

        public List<Equipment> GetValidEquipment(DateTime lessonStartTime)
        {
            try
            {
                PTSContext db = new PTSContext();

                List<Equipment> equipmentList = new List<Equipment>();
                var result = db.Equipments.ToList();

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
                                var equipmentWithExpiredCertification = equipCertification.Where(EC => (lessonStartTime.Date >= EC.StartingDate.Date && lessonStartTime.Date <= EC.EndingDate.Date)).ToList();
                                if (equipmentWithExpiredCertification.Count == 0)
                                    isCertificationValid = false;
                            }
                            if (isCertificationValid)
                            {
                                equipmentList.Add(equipment);
                            }
                        }
                        else
                        {
                            equipmentList.Add(equipment);
                        }
                    }
                }
                return equipmentList;
            }
            catch (Exception ex)
            {
                return new List<Equipment>();
            }
        }

        public bool CancelSchedule(string reason, string remark, string flyingAndFTDScheduleId)
        {
            try
            {
                PTSContext db = new PTSContext();
                int id = Convert.ToInt16(flyingAndFTDScheduleId);
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                var flyingFTDSchedule = db.FlyingFTDSchedules.Where(fs => fs.Status != statusName && fs.FlyingFTDScheduleId == id).ToList().FirstOrDefault();

                if (flyingFTDSchedule == null)
                    return false;
                else
                {
                    flyingFTDSchedule.Status = statusName;
                    flyingFTDSchedule.Reason = reason;
                    flyingFTDSchedule.Remark = remark;
                    db.Entry(flyingFTDSchedule).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        var lessonBriefingAndDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.FlyingFTDScheduleId == id).ToList();
                        if (lessonBriefingAndDebriefingList.Count > 0)
                        {
                            foreach (var briefingAndDebriefing in lessonBriefingAndDebriefingList)
                            {
                                briefingAndDebriefing.Status = "Canceled";
                                db.Entry(briefingAndDebriefing).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        //Update equipment maintenance
                        var equipmentObj = db.Equipments.Find(flyingFTDSchedule.EquipmentId);
                        var lesson = db.Lessons.Where(l => l.LessonId == flyingFTDSchedule.LessonId).ToList();
                        double lessonDuration = 0;
                        if (lesson.Count > 0 && lesson.Count > 0)
                        {
                            if (equipmentObj.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                            {
                                lessonDuration = (double)lesson.FirstOrDefault().FTDTime;
                            }
                            else if (equipmentObj.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                            {
                                lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;

                            }
                        }
                        else
                        {
                            lessonDuration = 1;
                        }

                        if (equipmentObj != null)
                        {
                            equipmentObj.EstimatedRemainingHours = equipmentObj.EstimatedRemainingHours - (float)lessonDuration;
                            db.Entry(equipmentObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<EquipmentType> get_EquipmentType()
        {
            try
            {
                PTSContext db = new PTSContext();
                return db.EquipmentTypes.Where(ET => ET.EndDate >= DateTime.Now).ToList();
            }
            catch (Exception)
            {
                return new List<EquipmentType>();
            }
        }

        public List<FTDandFlyingInstructorView> GetFTDandFlyingInstructors(int flyingFTDScheduleId, string date, string time)
        {
            try
            {
                DateTime startingTime = Convert.ToDateTime(date + " " + time);
                DateTime startDate = Convert.ToDateTime(startingTime.Date);
                DateTime endDate = startDate.AddDays(1).AddMilliseconds(-1);



                PTSContext db = new PTSContext();
                FlyingFTDSchedule schedule = db.FlyingFTDSchedules.Where(f => f.FlyingFTDScheduleId == flyingFTDScheduleId).FirstOrDefault();
                string[] traineeLesoon = new string[] { schedule.TraineeId + "-" + schedule.LessonId };
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                List<List<FTDandFlyingInstructorView>> listOfInstructor = new List<List<FTDandFlyingInstructorView>>();
                int equipmentId = schedule.EquipmentId;
                var equipment = db.Equipments.Where(E => E.EquipmentId == equipmentId).ToList();

                var allFlyingFTDScheduleList = db.FlyingFTDSchedules.Where(FFS => FFS.Status != statusName && FFS.ScheduleStartTime >= startDate && FFS.ScheduleStartTime <= endDate).ToList();
                var allBriefingAndDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.FlyingFTDSchedule.ScheduleStartTime >= startDate && b.FlyingFTDSchedule.ScheduleStartTime <= endDate).ToList();

                foreach (var item in traineeLesoon)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        string[] TraineeLessonIdPair = item.Split('-');
                        if (!(String.IsNullOrEmpty(TraineeLessonIdPair[0]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[0]) || String.IsNullOrEmpty(TraineeLessonIdPair[1]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[1])))
                        {
                            List<FTDandFlyingInstructorView> fTDandFlyingInstructorList = new List<FTDandFlyingInstructorView>();

                            int traineeId = Convert.ToInt16(TraineeLessonIdPair[0]);
                            int lessonId = Convert.ToInt16(TraineeLessonIdPair[1]);

                            var lesson = db.Lessons.Where(L => L.LessonId == lessonId).ToList();

                            DateTime endingTime = startingTime;

                            //Get lesson ending Time
                            if (lesson.Count > 0 && lesson.Count > 0)
                            {
                                if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                                {
                                    endingTime = (startingTime.AddHours((double)lesson.FirstOrDefault().FTDTime));
                                }
                                else if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                                {
                                    double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                                    endingTime = startingTime.AddHours(lessonDuration);
                                }
                            }
                            else
                            {
                                endingTime = startingTime.AddHours(1);
                            }

                            //abram: to handle bounderies
                            startingTime = startingTime.AddMinutes(1);
                            endingTime = endingTime.AddMinutes(-1);

                            //Check whether the instructor can teach FTD or Flying simulations => "equipmentTypeId"
                            //And Check whether the instructor is assigned to teach the selected equipment Models  => from "InstructorEquipmentModels" table

                            var result = (from E in db.Equipments

                                          join EM in db.EquipmentModels on E.EquipmentModelId equals EM.EquipmentModelId
                                          join EIM in db.InstructorEquipmentModels on EM.EquipmentModelId equals EIM.EquipmentModelId
                                          join INS in db.Instructors on EIM.InstructorId equals INS.InstructorId
                                          join PER in db.Persons on INS.PersonId equals PER.PersonId

                                          where E.EquipmentId == equipmentId //&& EM.EquipmentType.EquipmentTypeId == equipmentTypeId

                                          select new
                                          {
                                              EIM,
                                              PER
                                          }).ToList();

                            var resultGroup = result.GroupBy(Inst => Inst.EIM.InstructorId).Select(grp => grp.FirstOrDefault()).ToList();


                            /*Instructor shall be same throughout the pre solo lessons. Constraint #1*/
                            bool foundPreSoloLessonInstructor = false;
                            //bool isSolo = lesson.FirstOrDefault().TimeAircraftSolo > 0 && lesson.FirstOrDefault().TimeAircraftDual == 0;
                            //if (!isSolo)
                            if (lesson.FirstOrDefault().IsPreSolo)
                            {
                                //Get Instructor of the Pre-Solo Lessons for a specific Trainee
                                FTDandFlyingInstructorView instructor = GetInstructorOfThePreSoloLessons(traineeId);
                                if (instructor != null)
                                {
                                    bool isInstructorFree = false;
                                    bool isInstructorFreeFromBriefingAndDebriefing = false;
                                    //Check whether the instructor is free or not in the specified date and time.
                                    var flyingFTDScheduleList = allFlyingFTDScheduleList.Where(FFS => FFS.InstructorId == instructor.Id).ToList();
                                    if (flyingFTDScheduleList.Count > 0)
                                    {
                                        var scheduledInstructor = flyingFTDScheduleList.Where(SI => ((startingTime >= SI.ScheduleStartTime && startingTime <= SI.ScheduleEndTime) || (endingTime >= SI.ScheduleStartTime && endingTime <= SI.ScheduleEndTime))).ToList();
                                        if (scheduledInstructor.Count == 0)
                                        {
                                            isInstructorFree = true;
                                        }
                                    }
                                    else
                                    {
                                        isInstructorFree = true;
                                    }
                                    //Check Instructor has BRIEFING and DEBRIEFING in the mentioned date and time. 
                                    var briefingAndDebriefingList = allBriefingAndDebriefingList.Where(b => b.FlyingFTDSchedule.InstructorId == instructor.Id).ToList();
                                    if (briefingAndDebriefingList.Count > 0)
                                    {
                                        //var trinstructorBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime > startingTime && b.BriefingAndDebriefing.EndingTime < endingTime))).ToList();
                                        var instructorBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime >= startingTime && b.BriefingAndDebriefing.StartingTime <= endingTime) || (b.BriefingAndDebriefing.EndingTime >= startingTime && b.BriefingAndDebriefing.EndingTime <= endingTime))).ToList();

                                        if (instructorBriefing.Count == 0)
                                        {
                                            isInstructorFreeFromBriefingAndDebriefing = true;
                                        }
                                    }
                                    else
                                    {
                                        isInstructorFreeFromBriefingAndDebriefing = true;
                                    }

                                    if (isInstructorFree && isInstructorFreeFromBriefingAndDebriefing)
                                    {
                                        fTDandFlyingInstructorList.Add(instructor);
                                        foundPreSoloLessonInstructor = true;
                                    }
                                    //return fTDandFlyingInstructorList;
                                }
                            }
                            else if (!foundPreSoloLessonInstructor)
                            {
                                //Return all free and potential instructors -if it is not presolo lesson or -If lesson is the first pre solo lesson to be assigned


                                foreach (var instructorVar in resultGroup)
                                {
                                    //Check Instructor schedule factor, Maximum Lecture Hour for an instructor per a day 

                                    bool isInstructorFree = false;
                                    bool isInstructorFreeFromBriefingAndDebriefing = false;

                                    //Check whether the instructor is free or not in the specified date and time.
                                    var flyingFTDScheduleList = allFlyingFTDScheduleList.Where(FFS => FFS.InstructorId == instructorVar.EIM.InstructorId).ToList();
                                    //abram:please consider optimizing the above line since it returns all the schedules of an instructor-> make it time bounded
                                    //fish: eshi
                                    if (flyingFTDScheduleList.Count > 0)
                                    {
                                        //var scheduledInstructor = flyingFTDScheduleList.Where(SI => ((startingTime > SI.ScheduleStartTime && startingTime < SI.ScheduleEndTime) || (endingTime > SI.ScheduleStartTime && endingTime < SI.ScheduleEndTime))).ToList();
                                        var scheduledInstructor = flyingFTDScheduleList.Where(SI => (!(SI.Lesson.TimeAircraftSolo > 0 && SI.Lesson.TimeAircraftDual == 0) && ((startingTime >= SI.ScheduleStartTime && startingTime <= SI.ScheduleEndTime) || (endingTime >= SI.ScheduleStartTime && endingTime <= SI.ScheduleEndTime)))).ToList();

                                        if (scheduledInstructor.Count == 0)
                                        {
                                            isInstructorFree = true;
                                        }
                                    }
                                    else
                                    {
                                        isInstructorFree = true;
                                    }

                                    //Check instructor has briefing and debriefing in the mentioned date and time. 
                                    var briefingAndDebriefingList = allBriefingAndDebriefingList.Where(b => b.FlyingFTDSchedule.InstructorId == instructorVar.EIM.InstructorId).ToList();
                                    if (briefingAndDebriefingList.Count > 0)
                                    {
                                        var trinstructorBriefing = briefingAndDebriefingList.Where(b => ((startingTime >= b.BriefingAndDebriefing.StartingTime && startingTime <= b.BriefingAndDebriefing.EndingTime)
                                        || (endingTime >= b.BriefingAndDebriefing.StartingTime && endingTime <= b.BriefingAndDebriefing.EndingTime))).ToList();
                                        if (trinstructorBriefing.Count == 0)
                                        {
                                            isInstructorFreeFromBriefingAndDebriefing = true;
                                        }
                                    }
                                    else
                                    {
                                        isInstructorFreeFromBriefingAndDebriefing = true;
                                    }

                                    if (isInstructorFree && isInstructorFreeFromBriefingAndDebriefing)
                                    {
                                        fTDandFlyingInstructorList.Add(new FTDandFlyingInstructorView
                                        {
                                            Id = instructorVar.EIM.InstructorId,
                                            Name = instructorVar.EIM.Instructor.Person.FirstName + " " + instructorVar.EIM.Instructor.Person.MiddleName + "."
                                        });
                                    }
                                }
                            }
                            listOfInstructor.Add(fTDandFlyingInstructorList);
                        }
                    }
                }
                if (listOfInstructor.Count == 1)
                    return listOfInstructor.FirstOrDefault();

                List<FTDandFlyingInstructorView> intersactionInstructorList = listOfInstructor[0];

                //Get common instructors that can teach all the selected lesson and trainee.
                for (int i = 1; i < listOfInstructor.Count; i++)
                {
                    //intersactionInstructorList = intersactionInstructorList.Intersect(listOfInstructor[i]).ToList();
                    var result = (from a in intersactionInstructorList
                                  join b in listOfInstructor[i] on a.Id equals b.Id
                                  where a.Id == b.Id
                                  select new FTDandFlyingInstructorView
                                  {
                                      Id = a.Id,
                                      Name = a.Name
                                  }).ToList();
                    if (result.Count > 0)
                        intersactionInstructorList = result;
                    else
                        return new List<FTDandFlyingInstructorView>();
                }

                return intersactionInstructorList;
            }
            catch (Exception ex)
            {
                return new List<FTDandFlyingInstructorView>();
            }
        }

        public List<FTDandFlyingInstructorView> GetFTDandFlyingInstructors(int equipmentId, DateTime startingTime, string[] traineeLesoon)
        {
            try
            {
                DateTime startDate = Convert.ToDateTime(startingTime.Date);
                DateTime endDate = startDate.AddDays(1).AddMilliseconds(-1);

                PTSContext db = new PTSContext();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                List<List<FTDandFlyingInstructorView>> listOfInstructor = new List<List<FTDandFlyingInstructorView>>();
                var equipment = db.Equipments.Where(E => E.EquipmentId == equipmentId).ToList();

                var allFlyingFTDScheduleList = db.FlyingFTDSchedules.Where(FFS => FFS.Status != statusName && FFS.ScheduleStartTime >= startDate && FFS.ScheduleStartTime <= endDate).ToList();
                var allBriefingAndDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.Status != "Canceled" && b.FlyingFTDSchedule.ScheduleStartTime >= startDate && b.FlyingFTDSchedule.ScheduleStartTime <= endDate).ToList();

                foreach (var item in traineeLesoon)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        string[] TraineeLessonIdPair = item.Split('-');
                        if (!(String.IsNullOrEmpty(TraineeLessonIdPair[0]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[0]) || String.IsNullOrEmpty(TraineeLessonIdPair[1]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[1])))
                        {
                            List<FTDandFlyingInstructorView> fTDandFlyingInstructorList = new List<FTDandFlyingInstructorView>();

                            int traineeId = Convert.ToInt16(TraineeLessonIdPair[0]);
                            int lessonId = Convert.ToInt16(TraineeLessonIdPair[1]);

                            var lesson = db.Lessons.Where(L => L.LessonId == lessonId).ToList();

                            DateTime endingTime = startingTime;

                            //Get lesson ending Time
                            if (lesson.Count > 0 && lesson.Count > 0)
                            {
                                if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                                {
                                    endingTime = (startingTime.AddHours((double)lesson.FirstOrDefault().FTDTime));
                                }
                                else if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                                {
                                    double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                                    endingTime = startingTime.AddHours(lessonDuration);
                                }
                            }
                            else
                            {
                                endingTime = startingTime.AddHours(1);
                            }

                            //abram: to handle bounderies
                            startingTime = startingTime.AddMinutes(1);
                            endingTime = endingTime.AddMinutes(-1);

                            //Check whether the instructor can teach FTD or Flying simulations => "equipmentTypeId"
                            //And Check whether the instructor is assigned to teach the selected equipment Models  => from "InstructorEquipmentModels" table

                            var result = (from E in db.Equipments
                                          join EIM in db.InstructorEquipmentModels on E.EquipmentModel.EquipmentModelId equals EIM.EquipmentModelId
                                          join INS in db.Instructors on EIM.InstructorId equals INS.InstructorId
                                          where E.EquipmentId == equipmentId //&& EM.EquipmentType.EquipmentTypeId == equipmentTypeId

                                          select new
                                          {
                                              EIM,
                                              PER=INS.Person
                                          }).ToList();

                            var resultGroup = result.GroupBy(Inst => Inst.EIM.InstructorId).Select(grp => grp.FirstOrDefault()).ToList();


                            /*Instructor shall be same throughout the pre solo lessons. Constraint #1*/
                            bool foundPreSoloLessonInstructor = false;
                            //bool isSolo = lesson.FirstOrDefault().TimeAircraftSolo > 0 && lesson.FirstOrDefault().TimeAircraftDual == 0;
                            //if (!isSolo)
                            if (lesson.FirstOrDefault().IsPreSolo)
                            {
                                //Get Instructor of the Pre-Solo Lessons for a specific Trainee
                                FTDandFlyingInstructorView instructor = GetInstructorOfThePreSoloLessons(traineeId);
                                if (instructor != null)
                                {
                                    bool isInstructorFree = false;
                                    bool isInstructorFreeFromBriefingAndDebriefing = false;
                                    //Check whether the instructor is free or not in the specified date and time.
                                    var flyingFTDScheduleList = allFlyingFTDScheduleList.Where(FFS => FFS.InstructorId == instructor.Id).ToList();
                                    if (flyingFTDScheduleList.Count > 0)
                                    {
                                        var scheduledInstructor = flyingFTDScheduleList.Where(SI => ((startingTime >= SI.ScheduleStartTime && startingTime <= SI.ScheduleEndTime) || (endingTime >= SI.ScheduleStartTime && endingTime <= SI.ScheduleEndTime))).ToList();
                                        if (scheduledInstructor.Count == 0)
                                        {
                                            isInstructorFree = true;
                                        }
                                    }
                                    else
                                    {
                                        isInstructorFree = true;
                                    }
                                    //Check Instructor has BRIEFING and DEBRIEFING in the mentioned date and time. 
                                    var briefingAndDebriefingList = allBriefingAndDebriefingList.Where(b => b.FlyingFTDSchedule.InstructorId == instructor.Id).ToList();
                                    if (briefingAndDebriefingList.Count > 0)
                                    {
                                        //var trinstructorBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime > startingTime && b.BriefingAndDebriefing.EndingTime < endingTime))).ToList();
                                        var instructorBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime >= startingTime && b.BriefingAndDebriefing.StartingTime <= endingTime) || (b.BriefingAndDebriefing.EndingTime >= startingTime && b.BriefingAndDebriefing.EndingTime <= endingTime))).ToList();

                                        if (instructorBriefing.Count == 0)
                                        {
                                            isInstructorFreeFromBriefingAndDebriefing = true;
                                        }
                                    }
                                    else
                                    {
                                        isInstructorFreeFromBriefingAndDebriefing = true;
                                    }

                                    if (isInstructorFree && isInstructorFreeFromBriefingAndDebriefing)
                                    {
                                        fTDandFlyingInstructorList.Add(instructor);
                                        foundPreSoloLessonInstructor = true;
                                    }
                                    //return fTDandFlyingInstructorList;
                                }
                            }
                            else if (!foundPreSoloLessonInstructor)
                            {
                                //Return all free and potential instructors -if it is not presolo lesson or -If lesson is the first pre solo lesson to be assigned


                                foreach (var instructorVar in resultGroup)
                                {
                                    //Check Instructor schedule factor, Maximum Lecture Hour for an instructor per a day 

                                    bool isInstructorFree = false;
                                    bool isInstructorFreeFromBriefingAndDebriefing = false;

                                    //Check whether the instructor is free or not in the specified date and time.
                                    var flyingFTDScheduleList = allFlyingFTDScheduleList.Where(FFS => FFS.InstructorId == instructorVar.EIM.InstructorId).ToList();
                                    //abram:please consider optimizing the above line since it returns all the schedules of an instructor-> make it time bounded
                                    //fish: eshi
                                    if (flyingFTDScheduleList.Count > 0)
                                    {
                                        //var scheduledInstructor = flyingFTDScheduleList.Where(SI => ((startingTime > SI.ScheduleStartTime && startingTime < SI.ScheduleEndTime) || (endingTime > SI.ScheduleStartTime && endingTime < SI.ScheduleEndTime))).ToList();
                                        var scheduledInstructor = flyingFTDScheduleList.Where(SI => (!(SI.Lesson.TimeAircraftSolo > 0 && SI.Lesson.TimeAircraftDual == 0) && ((startingTime >= SI.ScheduleStartTime && startingTime <= SI.ScheduleEndTime) || (endingTime >= SI.ScheduleStartTime && endingTime <= SI.ScheduleEndTime)))).ToList();

                                        if (scheduledInstructor.Count == 0)
                                        {
                                            isInstructorFree = true;
                                        }
                                    }
                                    else
                                    {
                                        isInstructorFree = true;
                                    }

                                    //Check instructor has briefing and debriefing in the mentioned date and time. 
                                    var briefingAndDebriefingList = allBriefingAndDebriefingList.Where(b => b.FlyingFTDSchedule.InstructorId == instructorVar.EIM.InstructorId).ToList();
                                    if (briefingAndDebriefingList.Count > 0)
                                    {
                                        var trinstructorBriefing = briefingAndDebriefingList.Where(b => ((startingTime >= b.BriefingAndDebriefing.StartingTime && startingTime <= b.BriefingAndDebriefing.EndingTime)
                                        || (endingTime >= b.BriefingAndDebriefing.StartingTime && endingTime <= b.BriefingAndDebriefing.EndingTime))).ToList();
                                        if (trinstructorBriefing.Count == 0)
                                        {
                                            isInstructorFreeFromBriefingAndDebriefing = true;
                                        }
                                    }
                                    else
                                    {
                                        isInstructorFreeFromBriefingAndDebriefing = true;
                                    }

                                    if (isInstructorFree && isInstructorFreeFromBriefingAndDebriefing)
                                    {
                                        fTDandFlyingInstructorList.Add(new FTDandFlyingInstructorView
                                        {
                                            Id = instructorVar.EIM.InstructorId,
                                            Name = instructorVar.EIM.Instructor.Person.FirstName + " " + instructorVar.EIM.Instructor.Person.MiddleName + "."
                                        });
                                    }
                                }
                            }
                            listOfInstructor.Add(fTDandFlyingInstructorList);
                        }
                    }
                }
                if (listOfInstructor.Count == 1)
                    return listOfInstructor.FirstOrDefault();

                List<FTDandFlyingInstructorView> intersactionInstructorList = listOfInstructor[0];

                //Get common instructors that can teach all the selected lesson and trainee.
                for (int i = 1; i < listOfInstructor.Count; i++)
                {
                    //intersactionInstructorList = intersactionInstructorList.Intersect(listOfInstructor[i]).ToList();
                    var result = (from a in intersactionInstructorList
                                  join b in listOfInstructor[i] on a.Id equals b.Id
                                  where a.Id == b.Id
                                  select new FTDandFlyingInstructorView
                                  {
                                      Id = a.Id,
                                      Name = a.Name
                                  }).ToList();
                    if (result.Count > 0)
                        intersactionInstructorList = result;
                    else
                        return new List<FTDandFlyingInstructorView>();
                }

                return intersactionInstructorList;
            }
            catch (Exception ex)
            {
                return new List<FTDandFlyingInstructorView>();
            }
        }

        public bool IsInBetweenEquipmentMaintainanceTime(FlyingFTDSchedule schedule)
        {
            try
            {
                PTSContext db = new PTSContext();

                var lesson = db.Lessons.Find(schedule.LessonId);
                var equipment = db.Equipments.Find(schedule.EquipmentId);

                DateTime startDate = schedule.ScheduleStartTime.Date;
                DateTime endDate = schedule.ScheduleStartTime.AddDays(1);

                string statusNameCancled = Enum.GetName(typeof(StatusType), 3);
                string statusNameCleared = Enum.GetName(typeof(StatusType), 2);
                //get downtime equipment of the selected equipment
                var equipmentMaintenances = db.EquipmentMaintenances.Where(E => E.EquipmentId == schedule.EquipmentId && E.Status.ToString() == statusNameCancled && E.Status.ToString() == statusNameCleared && E.ScheduledCalanderStartDate >= startDate && E.ScheduledCalanderEndDate <= endDate).ToList();
                //is there downtime schedule in the selected date
                //var equipmentDowntimeSchedule = equipmentMaintenances.Where(EMT =>(EMT.ActualCalanderStartDate != null && E.ActualCalanderStartDate ).ToList();

                //Get lesson ending Time
                if (lesson==null)
                {
                    if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    {
                        schedule.ScheduleEndTime = (schedule.ScheduleStartTime.AddHours((double)lesson.FTDTime));
                    }
                    else if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    {
                        double lessonDuration = lesson.TimeAircraftSolo + lesson.TimeAircraftDual;
                        schedule.ScheduleEndTime = schedule.ScheduleStartTime.AddHours(lessonDuration);
                    }
                }
                else
                {
                    schedule.ScheduleEndTime = (schedule.ScheduleEndTime.AddHours(1));
                }

                bool IsInBetweenEquipmentMaintenanceTime = false;
                if (equipmentMaintenances.Count > 0)
                {
                    foreach (var maintenanceSchedule in equipmentMaintenances)
                    {
                        //Check whether or not we are trying to schedule in between the Equipment working hours
                        if (((schedule.ScheduleStartTime >= maintenanceSchedule.ActualCalanderStartDate && schedule.ScheduleStartTime <= maintenanceSchedule.ActualCalanderEndDate) && (schedule.ScheduleEndTime >= maintenanceSchedule.ActualCalanderStartDate && schedule.ScheduleEndTime <= maintenanceSchedule.ActualCalanderEndDate)) || (maintenanceSchedule.Status == StatusType.Pending) || (maintenanceSchedule.Status == StatusType.Progressing))
                        {
                            IsInBetweenEquipmentMaintenanceTime = true;
                            break;
                        }
                    }
                }
                return IsInBetweenEquipmentMaintenanceTime;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsTraineeFree(FlyingFTDSchedule schedule)
        {
            try
            {
                PTSContext db = new PTSContext();

                var lesson = db.Lessons.Find(schedule.LessonId);
                var equipment = db.Equipments.Find(schedule.EquipmentId);

                //Get lesson ending Time
                if (lesson!=null)
                {
                    if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    {
                        schedule.ScheduleEndTime = (schedule.ScheduleStartTime.AddHours((double)lesson.FTDTime));
                    }
                    else if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    {
                        double lessonDuration = lesson.TimeAircraftSolo + lesson.TimeAircraftDual;
                        schedule.ScheduleEndTime = schedule.ScheduleStartTime.AddHours(lessonDuration);
                    }
                }
                else
                {
                    schedule.ScheduleEndTime = (schedule.ScheduleEndTime.AddHours(1));
                }

                ////Check whether a trainee is free or not

                //var traineeSchedules = db.FlyingFTDSchedules.Where(sc => sc.TraineeId == schedule.TraineeId && sc.Status != "Canceled").ToList();
                //var scheduledEquipment = traineeSchedules.Where(b => ((b.ScheduleStartTime > schedule.ScheduleStartTime && b.ScheduleStartTime < schedule.ScheduleEndTime) || (b.ScheduleEndTime > schedule.ScheduleStartTime && b.ScheduleEndTime < schedule.ScheduleEndTime)) || ((b.ScheduleStartTime > tempStartingTime && b.ScheduleEndTime < tempEndingTime) && !((b.ScheduleStartTime > schedule.ScheduleStartTime && b.ScheduleStartTime < schedule.ScheduleEndTime) || (b.ScheduleEndTime > schedule.ScheduleStartTime && b.ScheduleEndTime < schedule.ScheduleEndTime)))).ToList();
                //// var scheduledEquipment = traineeSchedules.Where(SI => ((schedule.ScheduleStartTime > SI.ScheduleStartTime && schedule.ScheduleStartTime < SI.ScheduleEndTime) || (schedule.ScheduleEndTime > SI.ScheduleStartTime && schedule.ScheduleEndTime < SI.ScheduleEndTime))).ToList();

                //if (scheduledEquipment.Count() == 0)
                //{
                //    return true;
                //}


                bool isTraineeFree = false;
                bool isTraineeFreeFromBriefingAndDebriefingTime = false;
                DateTime tempStartingTime = schedule.ScheduleStartTime.AddMinutes(-1);
                DateTime tempEndingTime = schedule.ScheduleEndTime.AddMinutes(1);

                DateTime startDate = schedule.ScheduleStartTime.Date;
                DateTime endDate = schedule.ScheduleStartTime.AddDays(1);

                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                var flyingFTDScheduleTraineeList = db.FlyingFTDSchedules.Where(FFS => FFS.TraineeId == schedule.TraineeId && FFS.Status != statusName && FFS.ScheduleStartTime >= startDate && FFS.ScheduleStartTime <= endDate).ToList();

                //Check whether the trainee has already a period in the mentioned date and time. 
                if (flyingFTDScheduleTraineeList.Count > 0)
                {
                    // var scheduledTrainee = flyingFTDScheduleTraineeList.Where(SI => ((schedule.ScheduleStartTime > SI.ScheduleStartTime && schedule.ScheduleStartTime < SI.ScheduleEndTime) || (schedule.ScheduleEndTime > SI.ScheduleStartTime && schedule.ScheduleEndTime < SI.ScheduleEndTime))).ToList();
                    var scheduledTrainee = flyingFTDScheduleTraineeList.Where(b => ((b.ScheduleStartTime > schedule.ScheduleStartTime && b.ScheduleStartTime < schedule.ScheduleEndTime) || (b.ScheduleEndTime > schedule.ScheduleStartTime && b.ScheduleEndTime < schedule.ScheduleEndTime)) || ((b.ScheduleStartTime > tempStartingTime && b.ScheduleEndTime < tempEndingTime) && !((b.ScheduleStartTime > schedule.ScheduleStartTime && b.ScheduleStartTime < schedule.ScheduleEndTime) || (b.ScheduleEndTime > schedule.ScheduleStartTime && b.ScheduleEndTime < schedule.ScheduleEndTime)))).ToList();
                    if (scheduledTrainee.Count == 0)
                    {
                        isTraineeFree = true;
                    }
                }
                else
                {
                    isTraineeFree = true;
                }

                //Check trainee has briefing and debriefing in the mentioned date and time. 
                var briefingAndDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDSchedule.TraineeId == schedule.TraineeId && b.Status != "Canceled" && b.BriefingAndDebriefing.StartingTime >= startDate && b.BriefingAndDebriefing.StartingTime <= endDate).ToList();
                if (briefingAndDebriefingList.Count > 0)
                {
                    //var traineeBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime > schedule.ScheduleStartTime && b.BriefingAndDebriefing.StartingTime < schedule.ScheduleEndTime) || (b.BriefingAndDebriefing.EndingTime > schedule.ScheduleStartTime && b.BriefingAndDebriefing.EndingTime < schedule.ScheduleEndTime))).ToList();
                    var traineeBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime > schedule.ScheduleStartTime && b.BriefingAndDebriefing.StartingTime < schedule.ScheduleEndTime) || (b.BriefingAndDebriefing.EndingTime > schedule.ScheduleStartTime && b.BriefingAndDebriefing.EndingTime < schedule.ScheduleEndTime)) || ((b.BriefingAndDebriefing.StartingTime > tempStartingTime && b.BriefingAndDebriefing.EndingTime < tempEndingTime) && !((b.BriefingAndDebriefing.StartingTime > schedule.ScheduleStartTime && b.BriefingAndDebriefing.StartingTime < schedule.ScheduleEndTime) || (b.BriefingAndDebriefing.EndingTime > schedule.ScheduleStartTime && b.BriefingAndDebriefing.EndingTime < schedule.ScheduleEndTime)))).ToList();

                    //var traineeBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime > startingTime && b.BriefingAndDebriefing.EndingTime < endingTime))).ToList();
                    if (traineeBriefing.Count == 0)
                    {
                        isTraineeFreeFromBriefingAndDebriefingTime = true;
                    }
                }
                else
                {
                    isTraineeFreeFromBriefingAndDebriefingTime = true;
                }


                if (isTraineeFree && isTraineeFreeFromBriefingAndDebriefingTime)
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

        public object IsTraineeInstructorAndEquipmentFee(DateTime startingDateTime, DateTime endingDateTime, int flyingAndFTDScheduleId, int equipmentId)
        {
            List<string> messageList = new List<string>();
            try
            {
                TraineeAccess traineeAccess = new TraineeAccess();

                bool isSuccess = true;
                PTSContext db = new PTSContext();

                FlyingFTDSchedule flyingFTDSchedule = db.FlyingFTDSchedules.Find(flyingAndFTDScheduleId);

                if (flyingFTDSchedule != null)
                {
                    string traineeLesson = flyingFTDSchedule.TraineeId + "-" + flyingFTDSchedule.LessonId + "~";
                    string[] traineeLessonArray = traineeLesson.Split('~');

                    ///////////////////////////////start, Check whether Equipment is free or not/////////////////////////////////////
                    if (!(IsAssignedEquipmentFree(equipmentId, startingDateTime, endingDateTime, flyingAndFTDScheduleId, false)))
                    {
                        isSuccess = false;
                        messageList.Add("Equipment is not free from " + startingDateTime + " to " + endingDateTime + ".");
                    }
                    ///////////////////////////////end, Check whether Equipment is free or not/////////////////////////////////////

                    ///////////////////////////////start, Check whether Instructor is free or not/////////////////////////////////////
                    if (!(IsInstructorFree(flyingFTDSchedule.InstructorId, flyingFTDSchedule.LessonId, startingDateTime, endingDateTime, flyingAndFTDScheduleId, false, false, false)))
                    {
                        isSuccess = false;
                        messageList.Add("Instructor is not free from " + startingDateTime + " to " + endingDateTime + ".");
                    }
                    ///////////////////////////////end, Check whether Equipment is free or not/////////////////////////////////////

                    if (!IsTraineeFree(flyingFTDSchedule.TraineeId, startingDateTime, endingDateTime, flyingAndFTDScheduleId, false))
                    {
                        isSuccess = false;
                        Trainee trainee = (Trainee)traineeAccess.Details(flyingFTDSchedule.TraineeId);
                        messageList.Add(trainee.Person.FirstName + " " + trainee.Person.MiddleName + " is not free from " + startingDateTime + " to " + endingDateTime + ".");
                    }


                    ////////////////////////////////start, Check lesson can be given in the dropped equipment//////////////////////////////////// 
                    List<LessonView> equipmentLessons = GetTraineeLessons(equipmentId);

                    LessonView lesson = equipmentLessons.Where(l => l.Id == flyingFTDSchedule.LessonId).FirstOrDefault();
                    //if (lesson == null)
                    //{
                    //    isSuccess = false;
                    //    messageList.Add(flyingFTDSchedule.Lesson.LessonName + " can not be given in the equipment it is dropped on.");
                    //}
                    ////////////////////////////////start, Check whether instructor is scheduled to teach the dropped lesson on Equipment Model on which the lesson dropped on//////////////////////////////////// 

                    List<FTDandFlyingInstructorView> instructors = GetValidatedInstructors(equipmentId, traineeLessonArray);
                    FTDandFlyingInstructorView instructor = instructors.Where(I => I.Id == flyingFTDSchedule.InstructorId).FirstOrDefault();
                    //if (lesson == null)
                    //{
                    //    isSuccess = false;
                    //    messageList.Add("Instructor may not assigned to teach the selected equipment Models or the lesson is Pre-Solo and there may not have Instructor to teach the Pre-Solo lesson.");
                    //}
                }
                return new { isSuccess = isSuccess, message = messageList };
            }
            catch (Exception ex)
            {
                messageList.Add(ex.Message);
                return new { isSuccess = false, message = messageList };
            }
        }

        public object IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTime(string EventApplyingType, string date,DateTime startingTime, int instructorId, string[] traineeLessonIdArray, int equipmentId, string briefingTimeId, string debriefingTimeId, int fTDAndFlyingScheduleId, bool isReschedule, bool isCustomBriefingTime, bool isCustomDebriengTime, string briefingStartingTime, string debriefingStartTime)
        {
            List<string> messageList = new List<string>();
            try
            {
                bool isSuccess = true;
                BriefingAndDebriefingAccess briefingAndDebriefingAccess = new BriefingAndDebriefingAccess();
                TraineeAccess traineeAccess = new TraineeAccess();
                PTSContext db = new PTSContext();

                ///////////////////////////////Get BRIEFING and DEBRIEFING time//////////////////////////////////
                BriefingAndDebriefing objBriefing = new BriefingAndDebriefing();
                objBriefing.IsBriefing = true;
                BriefingAndDebriefing objDebriefing = new BriefingAndDebriefing();
                objDebriefing.IsDebriefing = true;
                string briefingAndDebriefingLength = ConfigurationManager.AppSettings["BriefingAndDebriefingLength"].ToString();
                EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
                EquipmentScheduleBriefingDebriefing briefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(fTDAndFlyingScheduleId, true, false);


                //Briefing time
                if ((String.IsNullOrEmpty(briefingTimeId) || String.IsNullOrWhiteSpace(briefingTimeId)) || isCustomBriefingTime)
                {
                    if (isCustomBriefingTime)
                    {
                        //briefingStartingTime = " " + briefingStartingTime;
                        objBriefing.StartingTime = Convert.ToDateTime(date+ " " + briefingStartingTime);
                        objBriefing.EndingTime = objBriefing.StartingTime + TimeSpan.FromHours(Convert.ToDouble(briefingAndDebriefingLength));

                    }
                    else
                    {
                        if (briefing != null)
                            objBriefing = briefingAndDebriefingAccess.Details(Convert.ToInt16(briefing.BriefingAndDebriefing.BriefingAndDebriefingId));
                        else
                        {
                            objBriefing.StartingTime = startingTime.AddHours(-1);
                            objBriefing.EndingTime = startingTime;
                        }
                    }
                }
                else
                {
                    objBriefing = briefingAndDebriefingAccess.Details(Convert.ToInt16(briefingTimeId));
                }

                //Add debriefing time    
                EquipmentScheduleBriefingDebriefing debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(fTDAndFlyingScheduleId, false, true);
                if ((String.IsNullOrEmpty(debriefingTimeId) || String.IsNullOrWhiteSpace(debriefingTimeId)) || isCustomDebriengTime)
                {
                    if (isCustomDebriengTime)
                    {
                        //debriefingStartTime = " " + debriefingStartTime;
                        objDebriefing.StartingTime = Convert.ToDateTime(date + " " + debriefingStartTime);
                        objDebriefing.EndingTime = objDebriefing.StartingTime + TimeSpan.FromHours(Convert.ToDouble(briefingAndDebriefingLength));

                    }
                    else
                    {
                        objDebriefing = GetDebriefingTime(fTDAndFlyingScheduleId, startingTime, equipmentId, traineeLessonIdArray);
                    }
                }
                else
                {
                    objDebriefing = briefingAndDebriefingAccess.Details(int.Parse(debriefingTimeId));
                }
                ///////////////////////////////Get BRIEFING and DEBRIEFING time/////////////////////////////////////

                ///////////////////////////////start, Get Equipment start and end date/////////////////////////////////////
                DateTime equipmetStartTime = startingTime;
                DateTime equipmetEndTime = new DateTime();
                if (EventApplyingType == "Lesson")
                {
                     equipmetEndTime = GetEquipmentStartAndEndTime(startingTime, equipmentId, traineeLessonIdArray);
                }
                else if (EventApplyingType == "Debriefing")
                {
                     equipmetEndTime = objDebriefing.EndingTime;
                }
                else if (EventApplyingType == "Briefing")
                {
                     equipmetEndTime = objBriefing.EndingTime;
                }
                    ///////////////////////////////end, Get Equipment start and end date/////////////////////////////////////

                    ///////////////////////////////start, Check whether Equipment is free or not/////////////////////////////////////
               
                    if (!(IsAssignedEquipmentFree(equipmentId, equipmetStartTime, equipmetEndTime, fTDAndFlyingScheduleId, isReschedule)))
                    {
                        isSuccess = false;
                        messageList.Add("Equipment is not free from " + equipmetStartTime + " to " + equipmetEndTime + ".");
                    }
                
                ///////////////////////////////end, Check whether Equipment is free or not/////////////////////////////////////

                ///////////////////////////////start, Check whether Instructor is free or not/////////////////////////////////////
                /*
                 if (!(IsInstructorFree(instructorId, 0,objBriefing.StartingTime, objBriefing.EndingTime, fTDAndFlyingScheduleId, isReschedule, true, false) && IsInstructorFree(instructorId, 0, objDebriefing.StartingTime, objDebriefing.EndingTime, fTDAndFlyingScheduleId, isReschedule, false, true) && IsInstructorFree(instructorId, 0, equipmetStartTime, equipmetEndTime, fTDAndFlyingScheduleId, isReschedule, false, false)))
                 {
                     isSuccess = false;
                     messageList.Add("Instructor is not free at briefing and debriefing time.");
                 }
                 */
                ///////////////////////////////end, Check whether Equipment is free or not/////////////////////////////////////


                ///////////////////////////////start, Check whether Trainees are free or not//////////////////////////////////
                DateTime individualTraineeStartTime = startingTime;
                DateTime individualTraineeEndingTime = startingTime;

                foreach (var item in traineeLessonIdArray)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        string[] TraineeLessonIdPair = item.Split('-');
                        if (!(String.IsNullOrEmpty(TraineeLessonIdPair[0]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[0]) || String.IsNullOrEmpty(TraineeLessonIdPair[1]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[1])))
                        {
                            List<FTDandFlyingInstructorView> fTDandFlyingInstructorList = new List<FTDandFlyingInstructorView>();

                            int traineeId = Convert.ToInt16(TraineeLessonIdPair[0]);
                            int lessonId = Convert.ToInt16(TraineeLessonIdPair[1]);

                            var lesson = db.Lessons.Where(L => L.LessonId == lessonId).ToList();
                            var equipment = db.Equipments.Where(E => E.EquipmentId == equipmentId).ToList();



                            if (!(IsInstructorFree(instructorId, lessonId, objBriefing.StartingTime, objBriefing.EndingTime, fTDAndFlyingScheduleId, isReschedule, true, false) && IsInstructorFree(instructorId, lessonId, objDebriefing.StartingTime, objDebriefing.EndingTime, fTDAndFlyingScheduleId, isReschedule, false, true) && IsInstructorFree(instructorId, lessonId, equipmetStartTime, equipmetEndTime, fTDAndFlyingScheduleId, isReschedule, false, false)))
                            {
                                isSuccess = false;
                                messageList.Add("Instructor is not free at briefing and debriefing time.");
                            }

                            //Get lesson ending Time
                            if (lesson.Count > 0 && lesson.Count > 0)
                            {
                                if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                                {
                                    if (EventApplyingType == "Lesson")
                                    {
                                        individualTraineeEndingTime = (startingTime.AddHours((double)lesson.FirstOrDefault().FTDTime));
                                    }
                                    else if (EventApplyingType == "Debriefing")
                                    {
                                        individualTraineeEndingTime = objDebriefing.EndingTime;
                                    }
                                    else if (EventApplyingType == "Briefing")
                                    {
                                        individualTraineeEndingTime = objBriefing.EndingTime;
                                    }
                                }
                                else if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                                {
                                    double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                                    
                                    if (EventApplyingType == "Lesson")
                                    {
                                        individualTraineeEndingTime = startingTime.AddHours(lessonDuration);
                                    }
                                    else if (EventApplyingType == "Debriefing")
                                    {
                                        individualTraineeEndingTime = objDebriefing.EndingTime;
                                    }
                                    else if (EventApplyingType == "Briefing")
                                    {
                                        individualTraineeEndingTime = objBriefing.EndingTime;
                                    }
                                }
                            }
                            else
                            {
                                individualTraineeEndingTime = startingTime.AddHours(1);
                            }
                            if (!IsTraineeFree(traineeId, individualTraineeStartTime, individualTraineeEndingTime, fTDAndFlyingScheduleId, isReschedule))
                            {
                                isSuccess = false;
                                Trainee trainee = (Trainee)traineeAccess.Details(traineeId);
                                messageList.Add(trainee.Person.FirstName + " " + trainee.Person.MiddleName + " is not free from " + individualTraineeStartTime + " to " + individualTraineeEndingTime + ".");
                            }
                            individualTraineeStartTime = individualTraineeEndingTime;
                        }
                    }
                }
                ///////////////////////////////End, Check whether Trainees are free or not//////////////////////////////////
                return new { isSuccess = isSuccess, message = messageList };
            }
            catch (Exception ex)
            {
                messageList.Add(ex.Message);
                return new { isSuccess = false, message = messageList };
            }
        }

        ////////////////////////////////////start, Validation briefing and debriefing ////////////////////////////////////////////// 
        public bool IsInstructorFree(int instructorId, int lessonId, DateTime startingTime, DateTime endingTime, int flyingAndFTDScheduleId, bool isReschdule, bool isBriefing, bool isDebriefing)
        {
            try
            {
                PTSContext db = new PTSContext();
                bool isInstructorFree = true;
                List<FlyingFTDSchedule> flyingFTDScheduleList = new List<FlyingFTDSchedule>();

                var lesson = db.Lessons.Find(lessonId);

                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                //Check whether the instructor is free or not in the specified date and time.
                if (isReschdule)
                    flyingFTDScheduleList = db.FlyingFTDSchedules.Where(s => s.InstructorId == instructorId && s.Status != statusName).ToList();
                else
                    flyingFTDScheduleList = db.FlyingFTDSchedules.Where(s => s.InstructorId == instructorId && s.Status != statusName && s.FlyingFTDScheduleId != flyingAndFTDScheduleId).ToList();

                startingTime=startingTime.AddMinutes(1);
                endingTime=endingTime.AddMinutes(-1);

                DateTime tempStartingTime = startingTime.AddMinutes(1);
                DateTime tempEndingTime = endingTime.AddMinutes(-1);

                if (flyingFTDScheduleList.Count > 0)
                {
                    var scheduledInstructor = flyingFTDScheduleList.Where(b => (!(lesson.TimeAircraftSolo > 0 && lesson.TimeAircraftDual == 0) && !(b.Lesson.TimeAircraftSolo > 0 && b.Lesson.TimeAircraftDual == 0) && ((b.ScheduleStartTime >= startingTime && b.ScheduleStartTime <= endingTime) || (b.ScheduleEndTime >= startingTime && b.ScheduleEndTime <= endingTime)) || ((b.ScheduleStartTime >= tempStartingTime && b.ScheduleEndTime <= tempEndingTime) && !((b.ScheduleStartTime >= startingTime && b.ScheduleStartTime <= endingTime) || (b.ScheduleEndTime >= startingTime && b.ScheduleEndTime <= endingTime))))).ToList();
                    //var scheduledInstructor = flyingFTDScheduleList.Where(b => (!(lesson.TimeAircraftSolo > 0 && lesson.TimeAircraftDual == 0) && !(b.Lesson.TimeAircraftSolo > 0 && b.Lesson.TimeAircraftDual == 0) && ((b.ScheduleStartTime >= startingTime && b.ScheduleStartTime <= startingTime)|| (b.ScheduleStartTime >= endingTime && b.ScheduleStartTime <= endingTime)))).ToList();

                    if (scheduledInstructor.Count > 0)
                        isInstructorFree = false;
                }

                bool isInstructorFreeFromBriefingAndDebriefingTime = true;
                //Check INSTRUCTOR has briefing and debriefing in the mentioned date and time. 
                int briefingId = 0, debriefingId = 0;
                var briefing = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDScheduleId == flyingAndFTDScheduleId && b.BriefingAndDebriefing.IsBriefing).FirstOrDefault();
                var debriefing = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDScheduleId == flyingAndFTDScheduleId && b.BriefingAndDebriefing.IsDebriefing).FirstOrDefault();
                if (briefing != null)
                    briefingId = briefing.BriefingAndDebriefingId;
                if (debriefing != null)
                    debriefingId = debriefing.BriefingAndDebriefingId;

                var briefingAndDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDSchedule.InstructorId == instructorId && b.Status != statusName && b.BriefingAndDebriefingId != briefingId && b.BriefingAndDebriefingId != debriefingId).ToList();
                briefingAndDebriefingList = briefingAndDebriefingList.Where(p => p.BriefingAndDebriefingId != flyingAndFTDScheduleId).ToList();
               
                if (briefingAndDebriefingList.Count > 0)
                {
                    var instructorBriefing = briefingAndDebriefingList
                        .Where
                        (
                            b =>
                                (
                                    !(lesson.TimeAircraftSolo > 0 && lesson.TimeAircraftDual == 0)
                                    &&
                                    (
                                        (b.BriefingAndDebriefing.StartingTime >= startingTime && b.BriefingAndDebriefing.StartingTime <= endingTime)
                                        || (b.BriefingAndDebriefing.EndingTime >= startingTime && b.BriefingAndDebriefing.EndingTime <= endingTime)
                                    )
                                )
                                ||
                                (
                                    !(lesson.TimeAircraftSolo > 0 && lesson.TimeAircraftDual == 0)
                                    && (b.BriefingAndDebriefing.StartingTime >= tempStartingTime && b.BriefingAndDebriefing.EndingTime <= tempEndingTime)
                                    &&
                                    !(
                                        (b.BriefingAndDebriefing.StartingTime >= startingTime && b.BriefingAndDebriefing.StartingTime <= endingTime)
                                        || (b.BriefingAndDebriefing.EndingTime >= startingTime && b.BriefingAndDebriefing.EndingTime <= endingTime)
                                    )
                                )
                        ).ToList();

                    var removeSharedDebriefingOrDebriefing = instructorBriefing.Where(b => !((b.BriefingAndDebriefing.StartingTime >= startingTime && b.BriefingAndDebriefing.EndingTime <= endingTime) && ((b.BriefingAndDebriefing.IsBriefing == true && isBriefing == true) || (b.BriefingAndDebriefing.IsDebriefing == true && isDebriefing == true)))).ToList();

                    if (removeSharedDebriefingOrDebriefing.Count > 0)
                        isInstructorFreeFromBriefingAndDebriefingTime = false;
                }
                return (isInstructorFree && isInstructorFreeFromBriefingAndDebriefingTime) ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsAssignedEquipmentFree(int equipmentId, DateTime startingTime, DateTime endingTime, int fTDAndFlyingScheduleId, bool isReschedule)
        {
            try
            {
                PTSContext db = new PTSContext();
                bool isEquipmentFree = true;
                List<FlyingFTDSchedule> flyingFTDScheduleList = new List<FlyingFTDSchedule>();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                //Check whether the instructor is free or not in the specified date and time.
                if (isReschedule)
                    flyingFTDScheduleList = db.FlyingFTDSchedules.Where(s => s.EquipmentId == equipmentId && s.Status != statusName).ToList();
                else
                    flyingFTDScheduleList = db.FlyingFTDSchedules.Where(s => s.EquipmentId == equipmentId && s.Status != statusName && s.FlyingFTDScheduleId != fTDAndFlyingScheduleId).ToList();

                DateTime tempStartingTime = startingTime.AddMinutes(-1);
                DateTime tempEndingTime = endingTime.AddMinutes(1);

                if (flyingFTDScheduleList.Count > 0)
                {
                    var scheduledEquipment = flyingFTDScheduleList.Where(b => ((b.ScheduleStartTime > startingTime && b.ScheduleStartTime < endingTime) || (b.ScheduleEndTime > startingTime && b.ScheduleEndTime < endingTime)) || ((b.ScheduleStartTime > tempStartingTime && b.ScheduleEndTime < tempEndingTime) && !((b.ScheduleStartTime > startingTime && b.ScheduleStartTime < endingTime) || (b.ScheduleEndTime > startingTime && b.ScheduleEndTime < endingTime)))).ToList();
                    //var scheduledEquipment = flyingFTDScheduleList.Where(b => (b.ScheduleStartTime<=startingTime&& b.ScheduleEndTime>=startingTime)||(b.ScheduleStartTime<=endingTime&& b.ScheduleEndTime>=endingTime)).ToList();

                    if (scheduledEquipment.Count > 0)
                        isEquipmentFree = false;
                }
                return isEquipmentFree;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsTraineeFree(int traineeId, DateTime startingTime, DateTime endingTime, int fTDAndFlyingScheduleId, bool isReschedule)
        {
            try
            {
                PTSContext db = new PTSContext();
                bool isTraineeFree = true;
                List<FlyingFTDSchedule> flyingFTDScheduleList = new List<FlyingFTDSchedule>();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                //Check whether the instructor is free or not in the specified date and time.
                if (isReschedule)
                    flyingFTDScheduleList = db.FlyingFTDSchedules.Where(s => s.TraineeId == traineeId && s.Status != statusName).ToList();
                else
                    flyingFTDScheduleList = db.FlyingFTDSchedules.Where(s => s.TraineeId == traineeId && s.Status != statusName && s.FlyingFTDScheduleId != fTDAndFlyingScheduleId).ToList();

                DateTime tempStartingTime = startingTime.AddMinutes(-1);
                DateTime tempEndingTime = endingTime.AddMinutes(1);
                if (flyingFTDScheduleList.Count > 0)
                {
                    var scheduledTrainee = flyingFTDScheduleList.Where(b => ((b.ScheduleStartTime > startingTime && b.ScheduleStartTime < endingTime) || (b.ScheduleEndTime > startingTime && b.ScheduleEndTime < endingTime)) || ((b.ScheduleStartTime > tempStartingTime && b.ScheduleEndTime < tempEndingTime) && !((b.ScheduleStartTime > startingTime && b.ScheduleStartTime < endingTime) || (b.ScheduleEndTime > startingTime && b.ScheduleEndTime < endingTime)))).ToList();
                    //var scheduledTrainee = flyingFTDScheduleList.Where(b => (b.ScheduleStartTime <= startingTime && b.ScheduleEndTime >= startingTime) || (b.ScheduleStartTime <= endingTime && b.ScheduleEndTime >= endingTime)).ToList();

                    if (scheduledTrainee.Count > 0)
                        isTraineeFree = false;
                }

                bool isTraineeFreeFromBriefingAndDebriefingTime = false;
                //Check TRAINEE has briefing and debriefing in the mentioned date and time. 
                int briefingId = 0, debriefingId = 0;
                var briefing = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDScheduleId == fTDAndFlyingScheduleId && b.BriefingAndDebriefing.IsBriefing).FirstOrDefault();
                var debriefing = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDScheduleId == fTDAndFlyingScheduleId && b.BriefingAndDebriefing.IsDebriefing).FirstOrDefault();
                if (briefing != null)
                    briefingId = briefing.BriefingAndDebriefingId;
                if (debriefing != null)
                    debriefingId = debriefing.BriefingAndDebriefingId;

                var briefingAndDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDSchedule.TraineeId == traineeId && b.Status != "Canceled" && b.BriefingAndDebriefingId != briefingId && b.BriefingAndDebriefingId != debriefingId).ToList();
                briefingAndDebriefingList = briefingAndDebriefingList.Where(p => p.BriefingAndDebriefingId != fTDAndFlyingScheduleId).ToList();
                //var briefingAndDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDSchedule.TraineeId == traineeId && b.Status != "Canceled").ToList();
                if (briefingAndDebriefingList.Count > 0)
                {
                    var traineeBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime > startingTime && b.BriefingAndDebriefing.StartingTime < endingTime) || (b.BriefingAndDebriefing.EndingTime > startingTime && b.BriefingAndDebriefing.EndingTime < endingTime)) || ((b.BriefingAndDebriefing.StartingTime > tempStartingTime && b.BriefingAndDebriefing.EndingTime < tempEndingTime) && !((b.BriefingAndDebriefing.StartingTime > startingTime && b.BriefingAndDebriefing.StartingTime < endingTime) || (b.BriefingAndDebriefing.EndingTime > startingTime && b.BriefingAndDebriefing.EndingTime < endingTime)))).ToList();
                    if (traineeBriefing.Count == 0)
                    {
                        isTraineeFreeFromBriefingAndDebriefingTime = true;
                    }
                }
                else
                {
                    isTraineeFreeFromBriefingAndDebriefingTime = true;
                }
                return (isTraineeFree && isTraineeFreeFromBriefingAndDebriefingTime) ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public BriefingAndDebriefing GetDebriefingTime(int flyingAndFTDScheduleId, DateTime startingTime, int equipmentId, string[] traineeLessonIdArray)
        {
            try
            {
                PTSContext db = new PTSContext();
                BriefingAndDebriefing briefingAndDebriefing = new BriefingAndDebriefing();
                //Get Trainee1 + Trainee2 + ... + Trainee n LESSON TIME                
                DateTime allTraineeLessonEndtTime = startingTime;
                foreach (var item in traineeLessonIdArray)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        string[] TraineeLessonIdPair = item.Split('-');
                        if (!(String.IsNullOrEmpty(TraineeLessonIdPair[0]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[0]) || String.IsNullOrEmpty(TraineeLessonIdPair[1]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[1])))
                        {
                            List<FTDandFlyingInstructorView> fTDandFlyingInstructorList = new List<FTDandFlyingInstructorView>();

                            int traineeId = Convert.ToInt16(TraineeLessonIdPair[0]);
                            int lessonId = Convert.ToInt16(TraineeLessonIdPair[1]);

                            var lesson = db.Lessons.Where(L => L.LessonId == lessonId).ToList();
                            var equipment = db.Equipments.Where(E => E.EquipmentId == equipmentId).ToList();
                            //Get lesson ending Time
                            if (lesson.Count > 0 && lesson.Count > 0)
                            {
                                if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                                {
                                    allTraineeLessonEndtTime = (allTraineeLessonEndtTime.AddHours((double)lesson.FirstOrDefault().FTDTime));
                                }
                                else if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                                {
                                    double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                                    allTraineeLessonEndtTime = allTraineeLessonEndtTime.AddHours(lessonDuration);
                                }
                            }
                            else
                            {
                                allTraineeLessonEndtTime = (allTraineeLessonEndtTime.AddHours(1));
                            }
                        }
                    }
                }
                if (flyingAndFTDScheduleId != 0)
                {
                    EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
                    EquipmentScheduleBriefingDebriefing Debriefing = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(flyingAndFTDScheduleId, false, true);
                    if ((Debriefing.BriefingAndDebriefing.StartingTime - allTraineeLessonEndtTime).TotalMinutes < 0)
                    {
                        briefingAndDebriefing.StartingTime = allTraineeLessonEndtTime;
                        briefingAndDebriefing.EndingTime = allTraineeLessonEndtTime.AddHours(1);
                    }
                    else
                    {
                        briefingAndDebriefing.StartingTime = Debriefing.BriefingAndDebriefing.StartingTime;
                        briefingAndDebriefing.EndingTime = Debriefing.BriefingAndDebriefing.EndingTime;
                    }
                }
                else
                {
                    briefingAndDebriefing.StartingTime = allTraineeLessonEndtTime;
                    briefingAndDebriefing.EndingTime = allTraineeLessonEndtTime.AddHours(1);
                }
                return briefingAndDebriefing;
            }
            catch (Exception ex)
            {
                return new BriefingAndDebriefing();
            }
        }

        public DateTime GetEquipmentStartAndEndTime(DateTime startingTime, int equipmentId, string[] traineeLessonIdArray)
        {
            try
            {
                PTSContext db = new PTSContext();

                //Get Trainee1 + Trainee2 + ... + Trainee n LESSON TIME                
                DateTime equipmentEndtTime = startingTime;
                foreach (var item in traineeLessonIdArray)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        string[] TraineeLessonIdPair = item.Split('-');
                        if (!(String.IsNullOrEmpty(TraineeLessonIdPair[0]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[0]) || String.IsNullOrEmpty(TraineeLessonIdPair[1]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[1])))
                        {
                            List<FTDandFlyingInstructorView> fTDandFlyingInstructorList = new List<FTDandFlyingInstructorView>();

                            int traineeId = Convert.ToInt16(TraineeLessonIdPair[0]);
                            int lessonId = Convert.ToInt16(TraineeLessonIdPair[1]);

                            var lesson = db.Lessons.Where(L => L.LessonId == lessonId).ToList();
                            var equipment = db.Equipments.Where(E => E.EquipmentId == equipmentId).ToList();
                            //Get lesson ending Time
                            if (lesson.Count > 0 && lesson.Count > 0)
                            {
                                if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                                {
                                    equipmentEndtTime = (equipmentEndtTime.AddHours((double)lesson.FirstOrDefault().FTDTime));
                                }
                                else if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                                {
                                    double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                                    equipmentEndtTime = equipmentEndtTime.AddHours(lessonDuration);
                                }
                            }
                            else
                            {
                                equipmentEndtTime = (equipmentEndtTime.AddHours(1));
                            }
                        }
                    }
                }

                return equipmentEndtTime;
            }
            catch (Exception ex)
            {
                return startingTime.AddHours(1);
            }
        }

        /// <summary>
        /// Constraints to be checked in this method:
        ///1. Check whether instructor is assigned to teach the selected equipment Models.
        ///2. Check for if the lesson pre solo lesson, if so Instructor shall be same throughout the Pre-Solo Lesson.
        ///3. Briefing must be before the lesson and debriefing after the lesson end time
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <param name="startingTime"></param>
        /// <param name="traineeLesoon"></param>
        /// <returns></returns>

        public List<FTDandFlyingInstructorView> GetValidatedInstructors(int equipmentId, string[] traineeLesoon)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<List<FTDandFlyingInstructorView>> listOfInstructor = new List<List<FTDandFlyingInstructorView>>();
                var equipment = db.Equipments.Where(E => E.EquipmentId == equipmentId).ToList();


                foreach (var item in traineeLesoon)
                {
                    if (!(String.IsNullOrEmpty(item) || String.IsNullOrWhiteSpace(item)))
                    {
                        string[] TraineeLessonIdPair = item.Split('-');
                        if (!(String.IsNullOrEmpty(TraineeLessonIdPair[0]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[0]) || String.IsNullOrEmpty(TraineeLessonIdPair[1]) || String.IsNullOrWhiteSpace(TraineeLessonIdPair[1])))
                        {
                            List<FTDandFlyingInstructorView> fTDandFlyingInstructorList = new List<FTDandFlyingInstructorView>();

                            int traineeId = Convert.ToInt16(TraineeLessonIdPair[0]);
                            int lessonId = Convert.ToInt16(TraineeLessonIdPair[1]);

                            var lesson = db.Lessons.Where(L => L.LessonId == lessonId).ToList();

                            //1. Check whether the instructor is assigned to teach the selected equipment Models  => from "InstructorEquipmentModels" table

                            var result = (from E in db.Equipments
                                          join EM in db.EquipmentModels on E.EquipmentModelId equals EM.EquipmentModelId
                                          join EIM in db.InstructorEquipmentModels on EM.EquipmentModelId equals EIM.EquipmentModelId
                                          where E.EquipmentId == equipmentId //&& EM.EquipmentType.EquipmentTypeId == equipmentTypeId
                                          select new
                                          {
                                              EIM
                                          }).ToList();

                            var resultGroup = result.GroupBy(Inst => Inst.EIM.InstructorId).Select(grp => grp.FirstOrDefault()).ToList();


                            /* 2. Instructor shall be same throughout the pre solo lessons. Constraint #1*/
                            bool foundPreSoloLessonInstructor = false;
                            if (lesson.FirstOrDefault().IsPreSolo)
                            {
                                //Get Instructor of the Pre-Solo Lessons for a specific Trainee
                                FTDandFlyingInstructorView instructor = GetInstructorOfThePreSoloLessons(traineeId);
                                if (instructor != null)
                                {
                                    fTDandFlyingInstructorList.Add(instructor);
                                    foundPreSoloLessonInstructor = true;
                                }
                            }
                            else if (!foundPreSoloLessonInstructor)
                            {
                                //Return all free and potential instructors -if it is not presolo lesson or -If lesson is the first pre solo lesson to be assigned
                                foreach (var instructorVar in resultGroup)
                                {
                                    fTDandFlyingInstructorList.Add(new FTDandFlyingInstructorView
                                    {
                                        Id = instructorVar.EIM.InstructorId,
                                        Name = instructorVar.EIM.Instructor.Person.FirstName.Substring(0, 3) + " " + instructorVar.EIM.Instructor.Person.MiddleName.Substring(0, 1) + "."
                                    });
                                }
                            }
                            listOfInstructor.Add(fTDandFlyingInstructorList);
                        }
                    }
                }
                if (listOfInstructor.Count == 1)
                    return listOfInstructor.FirstOrDefault();

                List<FTDandFlyingInstructorView> intersactionInstructorList = listOfInstructor[0];

                //Get common instructors that can teach all the selected lesson and trainee.
                for (int i = 1; i < listOfInstructor.Count; i++)
                {
                    //intersactionInstructorList = intersactionInstructorList.Intersect(listOfInstructor[i]).ToList();
                    var result = (from a in intersactionInstructorList
                                  join b in listOfInstructor[i] on a.Id equals b.Id
                                  where a.Id == b.Id
                                  select new FTDandFlyingInstructorView
                                  {
                                      Id = a.Id,
                                      Name = a.Name
                                  }).ToList();
                    if (result.Count > 0)
                        intersactionInstructorList = result;
                    else
                        return new List<FTDandFlyingInstructorView>();
                }
                return intersactionInstructorList;
            }
            catch (Exception ex)
            {
                return new List<FTDandFlyingInstructorView>();
            }
        }


        ////////////////////////////////////end, Validation briefing and debriefing ////////////////////////////////////////////// 

        /*
        public BriefingAndDebriefingView GetBriefingAndDebriefing(DateTime startingTime, int instructorId)
        {
            try
            {
                PTSContext db = new PTSContext();
                //DateTime scheduleDate = DateTime.ParseExact(startingTime + " 12:00:00", "MM/dd/yyyy hh:mm:ss", CultureInfo.InstalledUICulture);

                var result = db.EquipmentScheduleBriefingDebriefings.Where(r => r.FlyingFTDSchedule.InstructorId == instructorId).ToList();
                var breifingTimeList = result.Where(b => (b.BriefingAndDebriefing.EndingTime.Date == startingTime.Date && b.BriefingAndDebriefing.EndingTime > startingTime) && b.BriefingAndDebriefing.IsBriefing).ToList();

                BriefingAndDebriefingView briefingAndDebriefing = new BriefingAndDebriefingView();
                List<DisplayDropDownOption> BriefingList = new List<DisplayDropDownOption>();

                //Group to get who are the Trainee that share the same breifing time
                var breifingTimeListGroup = breifingTimeList.GroupBy(b => new { b.BriefingAndDebriefingId }).Select(grp => grp.ToList()).ToList();

                string traineeList = ""; int i = 0;
                foreach (var breifing in breifingTimeListGroup)
                {
                    traineeList = "";
                    i = 1;
                    foreach (var breifingItem in breifing)
                    {//Collect trainees that share the same breifing time
                        if (i == 1)
                            traineeList = traineeList + breifingItem.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + breifingItem.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1);
                        else
                            traineeList = traineeList + " & " + breifingItem.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + breifingItem.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1);
                        i++;
                    }
                    BriefingList.Add(new DisplayDropDownOption
                    {
                        Id = breifing.FirstOrDefault().BriefingAndDebriefingId,
                        Name = breifing.FirstOrDefault().BriefingAndDebriefing.StartingTime.ToString("HH:mm") + " - " + breifing.FirstOrDefault().BriefingAndDebriefing.EndingTime.ToString("HH:mm") + ", " + traineeList
                    });
                }

                briefingAndDebriefing.Briefing = BriefingList;
                var deBreifingTimeList = result.Where(b => (b.BriefingAndDebriefing.EndingTime.Date == startingTime.Date && b.BriefingAndDebriefing.EndingTime > startingTime) && b.BriefingAndDebriefing.IsDebriefing).ToList();

                List<DisplayDropDownOption> DeBreifingList = new List<DisplayDropDownOption>();

                //Group to get who are the trainee that share the same debreifing time
                var deBreifingTimeListGroup = deBreifingTimeList.GroupBy(b => new { b.BriefingAndDebriefingId }).Select(grp => grp.ToList()).ToList();

                foreach (var deBreifing in deBreifingTimeListGroup)
                {
                    traineeList = "";
                    i = 1;
                    foreach (var deBreifingItem in deBreifing)
                    {
                        //Collect trainees that share the same debreifing time
                        if (i == 1)
                            traineeList = traineeList + deBreifingItem.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + deBreifingItem.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1);
                        else
                            traineeList = traineeList + " & " + deBreifingItem.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + deBreifingItem.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1);
                        i++;
                    }
                    DeBreifingList.Add(new DisplayDropDownOption
                    {
                        Id = deBreifing.FirstOrDefault().BriefingAndDebriefingId,
                        Name = deBreifing.FirstOrDefault().BriefingAndDebriefing.StartingTime.ToString("HH:mm") + " - " + deBreifing.FirstOrDefault().BriefingAndDebriefing.EndingTime.ToString("HH:mm") + ", " + traineeList
                    });
                }

                briefingAndDebriefing.Debriefing = DeBreifingList;
                return briefingAndDebriefing;
            }
            catch (Exception ex)
            {
                return new BriefingAndDebriefingView();
            }
        }

        */

        public BriefingAndDebriefingView GetBriefingAndDebriefing(DateTime startingTime, int instructorId, int equipmentId, string[] traineeLesson)
        {
            try
            {
                PTSContext db = new PTSContext();

                var result = db.EquipmentScheduleBriefingDebriefings.Where(r => r.FlyingFTDSchedule.InstructorId == instructorId && r.Status != "Canceled").ToList();

                var breifingTimeList = result.Where(b => (b.BriefingAndDebriefing.EndingTime >= startingTime.Date.AddHours(-6) && b.BriefingAndDebriefing.EndingTime <= startingTime) && b.BriefingAndDebriefing.IsBriefing).ToList();

                BriefingAndDebriefingView briefingAndDebriefing = new BriefingAndDebriefingView();
                List<DisplayDropDownOption> BriefingList = new List<DisplayDropDownOption>();

                //Group to get who are the Trainee that share the same breifing time
                var breifingTimeListGroup = breifingTimeList.GroupBy(b => new { b.BriefingAndDebriefingId }).Select(grp => grp.ToList()).ToList();

                string traineeList = ""; int i = 0;
                foreach (var breifing in breifingTimeListGroup)
                {
                    traineeList = "";
                    i = 1;
                    foreach (var breifingItem in breifing)
                    {//Collect trainees that share the same breifing time
                        if (i == 1)
                            traineeList = traineeList + breifingItem.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + breifingItem.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1);
                        else
                            traineeList = traineeList + " & " + breifingItem.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + breifingItem.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1);
                        i++;
                    }
                    BriefingList.Add(new DisplayDropDownOption
                    {
                        Id = breifing.FirstOrDefault().BriefingAndDebriefingId,
                        Name = breifing.FirstOrDefault().BriefingAndDebriefing.StartingTime.ToString("HH:mm") + " - " + breifing.FirstOrDefault().BriefingAndDebriefing.EndingTime.ToString("HH:mm") + ", " + traineeList
                    });
                }

                briefingAndDebriefing.Briefing = BriefingList;
                BriefingAndDebriefing objBriefingAndDebriefing = GetDebriefingTime(0, startingTime, equipmentId, traineeLesson);

                var deBreifingTimeList = result.Where(b => (b.BriefingAndDebriefing.EndingTime <= startingTime.Date.AddDays(1).AddHours(6) && b.BriefingAndDebriefing.StartingTime >= objBriefingAndDebriefing.StartingTime) && b.BriefingAndDebriefing.IsDebriefing).ToList();

                List<DisplayDropDownOption> DeBreifingList = new List<DisplayDropDownOption>();

                //Group to get who are the trainee that share the same debreifing time
                var deBreifingTimeListGroup = deBreifingTimeList.GroupBy(b => new { b.BriefingAndDebriefingId }).Select(grp => grp.ToList()).ToList();

                foreach (var deBreifing in deBreifingTimeListGroup)
                {
                    traineeList = "";
                    i = 1;
                    foreach (var deBreifingItem in deBreifing)
                    {
                        //Collect trainees that share the same debreifing time
                        if (i == 1)
                            traineeList = traineeList + deBreifingItem.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + deBreifingItem.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1);
                        else
                            traineeList = traineeList + " & " + deBreifingItem.FlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + deBreifingItem.FlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1);
                        i++;
                    }
                    DeBreifingList.Add(new DisplayDropDownOption
                    {
                        Id = deBreifing.FirstOrDefault().BriefingAndDebriefingId,
                        Name = deBreifing.FirstOrDefault().BriefingAndDebriefing.StartingTime.ToString("HH:mm") + " - " + deBreifing.FirstOrDefault().BriefingAndDebriefing.EndingTime.ToString("HH:mm") + ", " + traineeList
                    });
                }
                briefingAndDebriefing.Debriefing = DeBreifingList;
                return briefingAndDebriefing;
            }
            catch (Exception ex)
            {
                return new BriefingAndDebriefingView();
            }
        }
        public string GetLeaveAndLicenceExpiry(DateTime startingTime, int instructorId, string[] traineeLesson)
        {
            string message = string.Empty;
            int traineeId;
            string temp = traineeLesson[0].Split('-')[0];
            traineeId = Convert.ToInt32(temp);
            try
            { //Licence expiry and Leave check
                LicenseAccess licenceAccess = new LicenseAccess();
                PersonLeaveAccess personeLeave = new PersonLeaveAccess();
                InstructorAccess insAccess = new InstructorAccess();
                TraineeAccess traiAccess = new TraineeAccess();

                Instructor instructor = insAccess.Details(instructorId);
                Trainee trainee = traiAccess.Details(traineeId);

                List<License> licenceExpiryInstructor = licenceAccess.ListExpiredLicenceWithId(startingTime, instructor.PersonId).ToList();
                List<License> licenceExpiryTrainee = licenceAccess.ListExpiredLicenceWithId(startingTime, trainee.Person.PersonId).ToList();
                List<PersonLeave> personeLeaveInstructor = personeLeave.LeavesWithDate(instructor.PersonId, startingTime);
                List<PersonLeave> personeLeaveTrainee = personeLeave.LeavesWithDate(trainee.Person.PersonId, startingTime);

                if (licenceExpiryInstructor == null && licenceExpiryTrainee == null && personeLeaveInstructor == null && personeLeaveTrainee == null)
                {
                    return message;
                }
                else
                {
                    List<resultSet> errorerrorList = new List<resultSet>();
                    foreach (License licenceexpired in licenceExpiryInstructor)
                    {
                        resultSet error = new resultSet();
                        error.resultType = licenceexpired.Person.FirstName + " " + licenceexpired.Person.MiddleName + " has expired licenses : ";
                        error.resultValue = licenceexpired.LicenseType.Description;
                        resultSet exist = errorerrorList.Where(r => r.resultType == error.resultType).FirstOrDefault();
                        if (exist != null)
                            exist.resultValue = exist.resultValue + "," + error.resultValue;
                        else
                            errorerrorList.Add(error);


                    }
                    foreach (License licenceexpired in licenceExpiryTrainee)
                    {
                        resultSet error = new resultSet();
                        error.resultType = licenceexpired.Person.FirstName + " " + licenceexpired.Person.MiddleName + " has expired licenses : ";
                        error.resultValue = licenceexpired.LicenseType.Description;
                        resultSet exist = errorerrorList.Where(r => r.resultType == error.resultType).FirstOrDefault();
                        if (exist != null)
                            exist.resultValue = exist.resultValue + "," + error.resultValue;
                        else
                            errorerrorList.Add(error);


                    }
                    foreach (PersonLeave personLeave in personeLeaveInstructor)
                    {
                        resultSet error = new resultSet();
                        error.resultType = personLeave.Person.FirstName + " " + personLeave.Person.MiddleName + " has leave : ";
                        error.resultValue = personLeave.LeaveType.Description;
                        resultSet exist = errorerrorList.Where(r => r.resultType == error.resultType).FirstOrDefault();
                        if (exist != null)
                            exist.resultValue = exist.resultValue + "," + error.resultValue;
                        else
                            errorerrorList.Add(error);


                    }
                    foreach (PersonLeave personLeave in personeLeaveTrainee)
                    {
                        resultSet error = new resultSet();
                        error.resultType = personLeave.Person.FirstName + " " + personLeave.Person.MiddleName + " has leave : ";
                        error.resultValue = personLeave.LeaveType.Description;
                        resultSet exist = errorerrorList.Where(r => r.resultType == error.resultType).FirstOrDefault();
                        if (exist != null)
                            exist.resultValue = exist.resultValue + "," + error.resultValue;
                        else
                            errorerrorList.Add(error);


                    }
                    foreach (resultSet messages in errorerrorList)
                        message = message + System.Environment.NewLine + messages.resultType + messages.resultValue;
                    return message;
                }
            }
            catch (Exception ex)
            {
                return message;
            }
        }

        public List<TraineeView> GetTraineeList(int equipmentId, DateTime startingTime)
        {
            try
            {

                PTSContext db = new PTSContext();
                List<TraineeView> traineeViewList = new List<TraineeView>();
                List<TraineeView> traineeList = new List<TraineeView>();
                var equipment = db.Equipments.Find(equipmentId);

                var batchEquipmentModule = db.BatchEquipmentModels.Where(bem => bem.EquipmentModelId == equipment.EquipmentModelId).ToList();

                var batchClassGroup = batchEquipmentModule.GroupBy(b => new { b.BatchId }).Select(item => item.FirstOrDefault()).ToList();

                foreach (var batchClass in batchClassGroup)
                {

                    var result = (from TL in db.BatchLessons
                                  join BC in db.TraineeBatchClasses on TL.BatchCategory.BatchId equals BC.BatchClass.BatchId
                                  join EBM in db.BatchEquipmentModels on BC.BatchClass.BatchId equals EBM.BatchId
                                  where BC.BatchClass.BatchId == batchClass.BatchId
                                  && EBM.EquipmentModelId == equipment.EquipmentModelId
                                  select new
                                  {
                                      BC,
                                      TL
                                  }).ToList();

                    var resultGroup = result.GroupBy(Ls => new { Ls.BC.TraineeId }).Select(grp => grp.FirstOrDefault()).ToList();
                    foreach (var trainee in resultGroup)
                    {
                        traineeViewList.Add(new TraineeView
                        {
                            TraineeId = trainee.BC.TraineeId,
                            TraineeNameAndLesson = trainee.BC.Trainee.Person.FirstName + " " + trainee.BC.Trainee.Person.MiddleName + " - " + trainee.BC.BatchClass.BatchClassName
                            //TraineeNameAndLesson=!string.IsNullOrEmpty(trainee.BC.Trainee.Person.ShortName) ? trainee.BC.Trainee.Person.ShortName : trainee.BC.Trainee.Person.FirstName.Substring(0, 3) + " " + trainee.BC.Trainee.Person.MiddleName.Substring(0, 1)
                        });
                    }
                }

                /*
                /////////////

                var lesson = db.Lessons.Where(L => L.LessonId == lessonId).ToList();

                DateTime endingTime = startingTime;

                //Get lesson ending Time
                if (lesson.Count > 0 && lesson.Count > 0)
                {
                    if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    {
                        endingTime = (startingTime.AddHours((double)lesson.FirstOrDefault().FTDTime));
                    }
                    else if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    {
                         double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                        endingTime = startingTime.AddHours(lessonDuration);                      
                    }
                }
                else
                {
                    endingTime = startingTime.AddHours(1);
                }



                bool isTraineeFree = false;
                bool isTraineeAlreadyTakenALesson = true;
                bool isTraineeFreeFromBriefingAndDebriefingTime = false;

                var flyingFTDScheduleList = db.FlyingFTDSchedules.Where(fs => fs.Status != "Canceled").ToList();

                foreach (var trainee in traineeViewList)
                {
                    isTraineeFree = false;
                    isTraineeAlreadyTakenALesson = true;

                    var flyingFTDScheduleTraineeList = flyingFTDScheduleList.Where(FFS => FFS.TraineeId == trainee.TraineeId).ToList();

                    //Check whether the trainee has already a period in the mentioned date and time. 
                    if (flyingFTDScheduleTraineeList.Count > 0)
                    {
                        var scheduledTrainee = flyingFTDScheduleTraineeList.Where(SI => ((startingTime > SI.ScheduleStartTime && startingTime < SI.ScheduleEndTime) || (endingTime > SI.ScheduleStartTime && endingTime < SI.ScheduleEndTime))).ToList();
                        if (scheduledTrainee.Count == 0)
                        {
                            isTraineeFree = true;
                        }
                    }
                    else
                    {
                        isTraineeFree = true;
                    }

                    //Check trainee has briefing and debriefing in the mentioned date and time. 
                    var briefingAndDebriefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDSchedule.TraineeId == trainee.TraineeId && b.Status != "Canceled").ToList();
                    if (briefingAndDebriefingList.Count > 0)
                    {
                        var traineeBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime > startingTime && b.BriefingAndDebriefing.StartingTime < endingTime) || (b.BriefingAndDebriefing.EndingTime > startingTime && b.BriefingAndDebriefing.EndingTime < endingTime))).ToList();

                        //var traineeBriefing = briefingAndDebriefingList.Where(b => ((b.BriefingAndDebriefing.StartingTime > startingTime && b.BriefingAndDebriefing.EndingTime < endingTime))).ToList();
                        if (traineeBriefing.Count == 0)
                        {
                            isTraineeFreeFromBriefingAndDebriefingTime = true;
                        }
                    }
                    else
                    {
                        isTraineeFreeFromBriefingAndDebriefingTime = true;
                    }

                    //Check whether the trainee has already taken the mentioned lesson
                    var flyingFTDScheduleLessonList = flyingFTDScheduleList.Where(FFS => FFS.LessonId == lessonId && FFS.TraineeId == trainee.TraineeId).ToList();

                    if (flyingFTDScheduleLessonList.Count == 0)
                    {
                        isTraineeAlreadyTakenALesson = false;
                    }

                    if (isTraineeFree && !isTraineeAlreadyTakenALesson && isTraineeFreeFromBriefingAndDebriefingTime)
                    {
                        traineeList.Add(trainee);
                    }
                   
                } */
                return traineeViewList;
            }
            catch (Exception ex)
            {
                return new List<TraineeView>();
            }
        }

        public List<TraineeView> GetBatchClassTraineesSimplified(int batchClassId, string lessonType)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<TraineeView> traineeViewList = new List<TraineeView>();

                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), (int)FlyingFTDScheduleStatus.Canceled);

                var traineeBatchClass = db.TraineeBatchClasses.Where(t => t.BatchClassId == batchClassId).ToList();
                string lessonName = "";

                foreach (var bTrainee in traineeBatchClass)
                {

                    traineeViewList.Add(new TraineeView
                    {
                        TraineeId = bTrainee.TraineeId,
                        TraineeName = !string.IsNullOrEmpty(bTrainee.Trainee.Person.ShortName) ? bTrainee.Trainee.Person.ShortName : bTrainee.Trainee.Person.FirstName.Substring(0, 3) + " " + bTrainee.Trainee.Person.MiddleName.Substring(0, 1) + ".",
                        TraineeNameAndLesson = !string.IsNullOrEmpty(bTrainee.Trainee.Person.ShortName) ? bTrainee.Trainee.Person.ShortName : bTrainee.Trainee.Person.FirstName.Substring(0, 3) + " " + bTrainee.Trainee.Person.MiddleName.Substring(0, 1) + ". <strong>" + lessonName + "</strong>",
                        NearestFutureLessonSequence = 0
                    });
                }

                return traineeViewList;
            }
            catch (Exception ex)
            {
                return new List<TraineeView>();
            }
        }
        public List<TraineeView> GetBatchClassTrainees(int batchClassId, string lessonType)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<TraineeView> traineeViewList = new List<TraineeView>();

                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), (int)FlyingFTDScheduleStatus.Canceled);
                
                var traineeBatchClass = db.TraineeBatchClasses.Where(t => t.BatchClassId == batchClassId).ToList();
                string lessonName = "";


                int nearestFutureLessonId = 0;
                foreach (var trainee in traineeBatchClass)
                {
                    /*
                    lessonName = "";
                    var lastTakenLesson = db.FlyingFTDSchedules
                        .Where(s => s.TraineeId == trainee.TraineeId 
                        && s.Status != statusName 
                        && s.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == lessonType)
                        .OrderByDescending(tr => tr.ScheduleEndTime).FirstOrDefault();

                    int lastTakenLessonPhaseId = 0, lastTakenLessonSequence = 0;
                    if (lastTakenLesson != null)
                    {
                        var batchLessonResult = db.BatchLessons.Where(bl=>bl.LessonId == lastTakenLesson.LessonId && bl.BatchCategory.BatchId==trainee.BatchClass.BatchId).ToList();
                        if (batchLessonResult.Count() > 0)
                        {
                            lastTakenLessonPhaseId = batchLessonResult.First().PhaseId;
                            lastTakenLessonSequence = batchLessonResult.First().Sequence;
                        }
                    }

                    List<PhaseLessonView> phaseLessonList = GetTraineePhaseLesson(trainee.TraineeId, lessonType);

                    if (lastTakenLessonPhaseId == 0 && lastTakenLessonSequence == 0)
                    {
                        //If lesson is not assigned so far    
                        nearestFutureLessonId = phaseLessonList.FirstOrDefault().LessonList.FirstOrDefault().Id;
                        lessonName = phaseLessonList.FirstOrDefault().LessonList.FirstOrDefault().Name + " - " + phaseLessonList.FirstOrDefault().LessonList.FirstOrDefault().LessonDuration;
                    }
                    else
                    {
                        for (int i = 0; i < phaseLessonList.Count; i++)
                        {
                            if (lastTakenLessonPhaseId == phaseLessonList[i].Id)
                            {
                                int nextLessonSequence = lastTakenLessonSequence + 1;
                                var nextLessonToBeGiven = phaseLessonList[i].LessonList.Where(L => L.LessonSequence == nextLessonSequence).ToList().FirstOrDefault();
                                if (nextLessonToBeGiven != null)
                                {
                                    //If there is a lesson to be given in that phase
                                    nearestFutureLessonId = nextLessonToBeGiven.Id;
                                    lessonName = nextLessonToBeGiven.Name + " - " + nextLessonToBeGiven.LessonDuration;
                                    break;
                                }
                                else
                                {
                                    //If there is no lesson to be given in that phase, get the first lesson from the next phase
                                    int nextPhaseIndex = (i + 1);
                                    if (nextPhaseIndex < phaseLessonList.Count)
                                    {
                                        var nextPhaseLessonToBeGiven = phaseLessonList[nextPhaseIndex].LessonList.FirstOrDefault();
                                        if (nextPhaseLessonToBeGiven != null)
                                        {
                                            //If there is a lesson to be given in that phase
                                            nearestFutureLessonId = nextPhaseLessonToBeGiven.Id;
                                            lessonName = nextPhaseLessonToBeGiven.Name + " - " + nextPhaseLessonToBeGiven.LessonDuration;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    */

                    var lessonResult = db.FlyingFTDSchedules
                        .Where(ts => ts.TraineeId == trainee.TraineeId)
                        .OrderByDescending(o => o.ScheduleStartTime);

                    if (lessonResult.Count() > 0)
                        lessonName = lessonResult.First().Lesson.LessonName;

                    traineeViewList.Add(new TraineeView
                    {
                        TraineeId = trainee.TraineeId,
                        TraineeName = !string.IsNullOrEmpty(trainee.Trainee.Person.ShortName) ? trainee.Trainee.Person.ShortName : trainee.Trainee.Person.FirstName.Substring(0, 3) + " " + trainee.Trainee.Person.MiddleName.Substring(0, 1) + ".",
                        TraineeNameAndLesson = !string.IsNullOrEmpty(trainee.Trainee.Person.ShortName) ? trainee.Trainee.Person.ShortName + " <strong>" + lessonName + "</strong>" : trainee.Trainee.Person.FirstName.Substring(0, 3) + " " + trainee.Trainee.Person.MiddleName.Substring(0, 1) + " <strong>" + lessonName + "</strong>",
                        NearestFutureLessonSequence = nearestFutureLessonId
                    });
                }
                if (traineeViewList.Count > 1)
                    traineeViewList = traineeViewList.OrderBy(l => l.NearestFutureLessonSequence).ToList();
                return traineeViewList;
            }
            catch (Exception ex)
            {
                return new List<TraineeView>();
            }
        }
        public List<TraineeView> GetBatchClassTraineesFullName(int batchClassId, string lessonType)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<TraineeView> traineeViewList = new List<TraineeView>();

                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), (int)FlyingFTDScheduleStatus.Canceled);
                var flyingFTDSchedules = db.FlyingFTDSchedules.Where(s => s.Status != statusName).ToList();

                var traineeBatchClass = db.TraineeBatchClasses.Where(t => t.BatchClassId == batchClassId).ToList();
                string lessonName = "";


                int nearestFutureLessonId = 0;
                foreach (var trainee in traineeBatchClass)
                {
                    lessonName = "";
                    var flyingFTDScheduleList = db.FlyingFTDSchedules.Where(s => s.TraineeId == trainee.TraineeId && s.Status != statusName && s.Equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == lessonType).ToList();

                    /////////////////////////////////Get last lesson phase, to determine where the next lesson is found ///////////////////////
                    var lastTakenLesson = flyingFTDScheduleList.OrderByDescending(tr => tr.ScheduleEndTime).FirstOrDefault();
                    int lastTakenLessonPhaseId = 0, lastTakenLessonSequence = 0;
                    if (lastTakenLesson != null)
                    {
                        lastTakenLessonPhaseId = 0;
                        lastTakenLessonSequence = 100;
                    }

                    List<PhaseLessonView> phaseLessonList = GetTraineePhaseLesson(trainee.TraineeId, lessonType);

                    if (lastTakenLessonPhaseId == 0 && lastTakenLessonSequence == 0)
                    {
                        //If lesson is not assigned so far    
                        nearestFutureLessonId = phaseLessonList.FirstOrDefault().LessonList.FirstOrDefault().Id;
                        lessonName = phaseLessonList.FirstOrDefault().LessonList.FirstOrDefault().Name + " - " + phaseLessonList.FirstOrDefault().LessonList.FirstOrDefault().LessonDuration;
                    }
                    else
                    {
                        for (int i = 0; i < phaseLessonList.Count; i++)
                        {
                            if (lastTakenLessonPhaseId == phaseLessonList[i].Id)
                            {
                                int nextLessonSequence = lastTakenLessonSequence + 1;
                                var nextLessonToBeGiven = phaseLessonList[i].LessonList.Where(L => L.LessonSequence == nextLessonSequence).ToList().FirstOrDefault();
                                if (nextLessonToBeGiven != null)
                                {
                                    //If there is a lesson to be given in that phase
                                    nearestFutureLessonId = nextLessonToBeGiven.Id;
                                    lessonName = nextLessonToBeGiven.Name + " - " + nextLessonToBeGiven.LessonDuration;
                                    break;
                                }
                                else
                                {
                                    //If there is no lesson to be given in that phase, get the first lesson from the next phase
                                    int nextPhaseIndex = (i + 1);
                                    if (nextPhaseIndex < phaseLessonList.Count)
                                    {
                                        var nextPhaseLessonToBeGiven = phaseLessonList[nextPhaseIndex].LessonList.FirstOrDefault();
                                        if (nextPhaseLessonToBeGiven != null)
                                        {
                                            //If there is a lesson to be given in that phase
                                            nearestFutureLessonId = nextPhaseLessonToBeGiven.Id;
                                            lessonName = nextPhaseLessonToBeGiven.Name + " - " + nextPhaseLessonToBeGiven.LessonDuration;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    /////////////////////////////////
                    //lessonName = "";
                    //nearestFutureLessonId = 0;
                    //if (lastTakenLesson != null)
                    //{
                    //    lessonName = "- " + lastTakenLesson.Lesson.LessonName;
                    //    var batchLessonSequence = db.BatchLessonSequences.Where(BLS => BLS.LessonId == lastTakenLesson.LessonId).ToList().FirstOrDefault();
                    //    if (batchLessonSequence != null)
                    //    {
                    //        nearestFutureLessonId = Int16.Parse(batchLessonSequence.Sequence.ToString());
                    //    }
                    //}

                    traineeViewList.Add(new TraineeView
                    {
                        TraineeId = trainee.TraineeId,
                        TraineeNameAndLesson = trainee.Trainee.Person.FirstName + " " + trainee.Trainee.Person.MiddleName,
                        NearestFutureLessonSequence = nearestFutureLessonId
                    });
                }
                if (traineeViewList.Count > 1)
                    traineeViewList = traineeViewList.OrderBy(l => l.NearestFutureLessonSequence).ToList();
                return traineeViewList;
            }
            catch (Exception ex)
            {
                return new List<TraineeView>();
            }
        }
        public List<TraineeView> GetBatchClassTraineesForEdit(int batchClassId, string lessonType)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<TraineeView> traineeViewList = new List<TraineeView>();

                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), (int)FlyingFTDScheduleStatus.Canceled);
                var flyingFTDSchedules = db.FlyingFTDSchedules.Where(s => s.Status != statusName).ToList();

                
                var traineeBatchClass = db.TraineeBatchClasses.Where(t => t.BatchClassId == batchClassId).ToList();
                


                int nearestFutureLessonId = 0;
                foreach (var trainee in traineeBatchClass)
                {
                    traineeViewList.Add(new TraineeView
                    {
                        TraineeId = trainee.TraineeId,
                        TraineeNameAndLesson =  trainee.Trainee.Person.ShortName +" - "+ trainee.Trainee.Person.FirstName + " " + trainee.Trainee.Person.MiddleName,
                        NearestFutureLessonSequence = nearestFutureLessonId
                    });
                }
                if (traineeViewList.Count > 1)
                    traineeViewList = traineeViewList.OrderBy(l => l.NearestFutureLessonSequence).ToList();
                return traineeViewList;
            }
            catch (Exception ex)
            {
                return new List<TraineeView>();
            }
        }
        public List<PhaseLessonView> GetTraineePhaseLesson(int traineeId, string lessonType)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<LessonView> TraineeLessonList = new List<LessonView>();

                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), (int)FlyingFTDScheduleStatus.Canceled);


                var traineeLessonList = (from tBc in db.TraineeBatchClasses 
                                         join bLes in db.BatchLessons on tBc.BatchClass.BatchId equals bLes.BatchCategory.BatchId
                                         where tBc.TraineeId==traineeId && bLes.Lesson.EquipmentType.EquipmentTypeName.ToUpper() == lessonType
                                         select new
                                         {
                                             tBc,
                                             bLes
                                         }).ToList();
                //var result = db.TraineeLessons.Where(TL => TL.TraineeCategory.TraineeProgram.TraineeSyllabus.TraineeId == traineeId && TL.Lesson.EquipmentType.EquipmentTypeName == equipment.EquipmentModel.EquipmentType.EquipmentTypeName).ToList();

                var lessonResultGroup = traineeLessonList.GroupBy(Ls => new { Ls.bLes.LessonId }).Select(grp => grp.FirstOrDefault()).ToList();

                bool isTraineeAlreadyTakenALesson = true;
                foreach (var lesson in lessonResultGroup)
                {
                    //Check whether the trainee has already taken the mentioned lesson
                    var flyingFTDScheduleLessonList = db.FlyingFTDSchedules.Where(FFS => FFS.Status != statusName && FFS.LessonId == lesson.bLes.LessonId && FFS.TraineeId == traineeId).ToList();

                    if (flyingFTDScheduleLessonList.Count == 0)
                    {
                        isTraineeAlreadyTakenALesson = false;
                    }
                    if (!isTraineeAlreadyTakenALesson)
                    {
                        float lessonDuration = 0;
                        if (lesson.bLes.Lesson.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                        {
                            lessonDuration = lesson.bLes.Lesson.FTDTime;
                        }
                        else if (lesson.bLes.Lesson.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                        {
                            lessonDuration = lesson.bLes.Lesson.TimeAircraftSolo + lesson.bLes.Lesson.TimeAircraftDual;
                        }
                        TraineeLessonList.Add(new LessonView
                        {
                            Id = lesson.bLes.LessonId,
                            Name = lesson.bLes.Lesson.LessonName,
                            LessonSequence = Int16.Parse(lesson.bLes.Sequence.ToString()),
                            PhaseId = lesson.bLes.PhaseId,
                            LessonDuration = lessonDuration
                        });
                    }
                }

                List<PhaseLessonView> phaseLessonList = new List<PhaseLessonView>();
                var traineeLessonGroup = TraineeLessonList.GroupBy(l => l.PhaseId).Select(grp => grp.ToList()).ToList();

                PhaseLessonView phaseLesson = null;
                foreach (var traineeLessonGrp in traineeLessonGroup)
                {
                    phaseLesson = new PhaseLessonView();
                    var phase = db.Phases.Find(traineeLessonGrp.FirstOrDefault().PhaseId);
                    if (phase != null)
                    {
                        phaseLesson.Name = phase.Name;
                        phaseLesson.PhaseSequence = phase.PhaseSequence;
                        phaseLesson.Id = phase.PhaseId;
                        var sortedLessonList = traineeLessonGrp.OrderBy(l => l.LessonSequence).ToList();
                        phaseLesson.LessonList = sortedLessonList.ToList();
                        phaseLessonList.Add(phaseLesson);
                    }
                }

                if (phaseLessonList.Count > 1)
                    phaseLessonList = phaseLessonList.OrderBy(l => l.PhaseSequence).ToList();
                return phaseLessonList;
            }
            catch (Exception)
            {
                return new List<PhaseLessonView>();
            }
        }



        public List<FlyingFTDSchedule> GetReservedDateTimes(int equipmentId, string date)
        {
            try
            {
                PTSContext db = new PTSContext();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);

                DateTime startDate = Convert.ToDateTime(date);
                DateTime endDate = startDate.AddDays(1);

                //DateTime.ParseExact(date + " 12:00:00", "MM/dd/yyyy HH:mm:ss", CultureInfo.InstalledUICulture);
                var result = db.FlyingFTDSchedules.Where(S => S.EquipmentId == equipmentId && S.Status != statusName && S.ScheduleStartTime >= startDate && S.ScheduleStartTime <= endDate).ToList();
                //var resultExtract = result.Where(Sc => Sc.ScheduleStartTime.Date == datetim.Date).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return new List<FlyingFTDSchedule>();
            }
        }


        public List<LessonView> GetTraineeLessonList(DateTime startingTime, int traineeId, int equipmentId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<LessonView> TraineeLessonList = new List<LessonView>();

                var equipment = db.Equipments.Find(equipmentId);

                var result = db.TraineeLessons.Where(TL => TL.TraineeId == traineeId && TL.Lesson.EquipmentType.EquipmentTypeName == equipment.EquipmentModel.EquipmentType.EquipmentTypeName).ToList();

                var lessonResultGroup = result.GroupBy(Ls => new { Ls.LessonId }).Select(grp => grp.FirstOrDefault()).ToList();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                var flyingFTDScheduleList = db.FlyingFTDSchedules.Where(s => s.Status != statusName).ToList();
                bool isTraineeAlreadyTakenALesson = true;
                foreach (var lesson in lessonResultGroup)
                {
                    //Check whether the trainee has already taken the mentioned lesson
                    var flyingFTDScheduleLessonList = flyingFTDScheduleList.Where(FFS => FFS.LessonId == lesson.LessonId && FFS.TraineeId == traineeId).ToList();

                    if (flyingFTDScheduleLessonList.Count == 0)
                    {
                        isTraineeAlreadyTakenALesson = false;
                    }

                    if (!isTraineeAlreadyTakenALesson)
                    {
                        TraineeLessonList.Add(new LessonView
                        {
                            Id = lesson.LessonId,
                            Name = lesson.Lesson.LessonName
                        });
                    }
                }
                return TraineeLessonList;
            }
            catch (Exception)
            {
                return new List<LessonView>();
            }
        }


        public List<PhaseLessonView> GetTraineeLessonList(int equipmentId, int traineeId, DateTime ActualclickedDate)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<LessonView> TraineeLessonList = new List<LessonView>();

                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), FlyingFTDScheduleStatus.Canceled);
                string statusName1 = Enum.GetName(typeof(FlyingFTDScheduleStatus), FlyingFTDScheduleStatus.New);

                var equipment = db.Equipments.Find(equipmentId);

                DateTime startDate = ActualclickedDate.AddDays(-1);
                DateTime endDate = ActualclickedDate.AddDays(1);

                var traineeLessonList = (from bLes in db.BatchLessons
                                         join tBat in db.TraineeBatchClasses on bLes.BatchCategory.BatchId equals tBat.BatchClass.BatchId
                                         where
                                         tBat.TraineeId == traineeId
                                         && bLes.Lesson.EquipmentType.EquipmentTypeName == equipment.EquipmentModel.EquipmentType.EquipmentTypeName

                                         select new
                                         {
                                             bLes,
                                             tBat
                                         }).ToList();


                //var result = db.TraineeLessons.Where(TL => TL.TraineeCategory.TraineeProgram.TraineeSyllabus.TraineeId == traineeId && TL.Lesson.EquipmentType.EquipmentTypeName == equipment.EquipmentModel.EquipmentType.EquipmentTypeName).ToList();

                //var lessonResultGroup = traineeLessonList.GroupBy(Ls => new { Ls.bLes.LessonId }).Select(grp => grp.FirstOrDefault()).ToList();

                //var flyingFTDScheduleList = db.FlyingFTDSchedules.Where(FFS => FFS.TraineeId == traineeId && FFS.ScheduleStartTime > startDate && FFS.ScheduleEndTime < endDate).ToList();
                //bool isTraineeAlreadyTakenALesson = true;
                //bool IsAlreadyScheduled = false;
                foreach (var lesson in traineeLessonList)
                {
                    /*

                    IsAlreadyScheduled = false;
                    isTraineeAlreadyTakenALesson = true;
                    //Check whether the trainee has already taken the mentioned lesson

                    var flyingFTDScheduleLessonList = flyingFTDScheduleList.Where(FFS => FFS.LessonId == lesson.bLes.LessonId).ToList();

                    if (flyingFTDScheduleLessonList.Count == 0)
                    {
                        isTraineeAlreadyTakenALesson = false;
                    }
                    else
                    {
                        foreach (var flyingAndFTDSchedule in flyingFTDScheduleLessonList)
                        {
                            if (flyingAndFTDSchedule.Status == statusName)
                            {
                                IsAlreadyScheduled = false;
                                isTraineeAlreadyTakenALesson = false;
                            }
                            else
                            {
                                IsAlreadyScheduled = true;
                                isTraineeAlreadyTakenALesson = true;
                                break;
                            }
                        }
                    }
                    if (!isTraineeAlreadyTakenALesson)
                    {
                        TraineeLessonList.Add(new LessonView
                        {
                            Id = lesson.bLes.LessonId,
                            Name = lesson.bLes.Lesson.LessonName,
                            LessonSequence = Int16.Parse(lesson.bLes.Sequence.ToString()),
                            PhaseId = lesson.bLes.PhaseId,
                            IsAlreadyScheduled = IsAlreadyScheduled
                        });
                    }
                }
                */

                    TraineeLessonList.Add(new LessonView
                    {
                        Id = lesson.bLes.LessonId,
                        Name = lesson.bLes.Lesson.LessonName,
                        LessonSequence = Int16.Parse(lesson.bLes.Sequence.ToString()),
                        PhaseId = lesson.bLes.PhaseId,
                        IsAlreadyScheduled = false//IsAlreadyScheduled
                    });
                }

                    List<PhaseLessonView> phaseLessonList = new List<PhaseLessonView>();
                var traineeLessonGroup = TraineeLessonList.GroupBy(l => l.PhaseId).Select(grp => grp.ToList()).ToList();

                PhaseLessonView phaseLesson = null;
                foreach (var traineeLessonGrp in traineeLessonGroup)
                {
                    phaseLesson = new PhaseLessonView();
                    var phase = db.Phases.Find(traineeLessonGrp.FirstOrDefault().PhaseId);
                    if (phase != null)
                    {
                        phaseLesson.Name = phase.Name;
                        phaseLesson.PhaseSequence = phase.PhaseSequence;
                        phaseLesson.Id = phase.PhaseId;
                        var sortedLessonList = traineeLessonGrp.OrderBy(l => l.LessonSequence).ToList();
                        phaseLesson.LessonList = sortedLessonList.ToList();
                        phaseLessonList.Add(phaseLesson);
                    }
                }

                if (phaseLessonList.Count > 1)
                    phaseLessonList = phaseLessonList
                        .OrderBy(l => l.PhaseSequence).ToList();
                return phaseLessonList;
            }
            catch (Exception)
            {
                return new List<PhaseLessonView>();
            }
        }
        public List<LessonView> GetTraineeLessons(int equipmentId)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<LessonView> TraineeLessonList = new List<LessonView>();

                var equipment = db.Equipments.Find(equipmentId);

                var batchEquipmentModule = db.BatchEquipmentModels.Where(bem => bem.EquipmentModelId == equipment.EquipmentModelId).ToList();

                var batchClassGroup = batchEquipmentModule.GroupBy(b => new { b.BatchId }).Select(item => item.FirstOrDefault()).ToList();

                foreach (var batchClass in batchClassGroup)
                {
                    var trainee = db.TraineeBatchClasses.Where(tbc => tbc.BatchClassId == batchClass.BatchId).FirstOrDefault();

                    var result = (from bLes in db.BatchLessons
                                      //join TL in db.TraineeLessons on TS.TraineeSyllabusId equals TL..TraineeCategory.TraineeProgram.TraineeSyllabusId
                                  join tBat in db.TraineeBatchClasses on bLes.BatchCategory.BatchId equals tBat.BatchClass.BatchId
                                  where tBat.TraineeId == trainee.TraineeId && tBat.BatchClass.BatchId == batchClass.BatchId
                                  && bLes.Lesson.EquipmentType.EquipmentTypeName == equipment.EquipmentModel.EquipmentType.EquipmentTypeName
                                  select new
                                  {
                                      bLes,
                                      tBat
                                  }).ToList();
                    var resultGroup = result.GroupBy(Ls => new { Ls.bLes.LessonId }).Select(grp => grp.FirstOrDefault()).ToList();
                    foreach (var lesson in resultGroup)
                    {
                        TraineeLessonList.Add(new LessonView
                        {
                            Id = lesson.bLes.LessonId,
                            Name = lesson.bLes.Lesson.LessonName
                        });
                    }
                }
                return TraineeLessonList;
            }
            catch (Exception e)
            {
                return new List<LessonView>();
            }
        }
        public string AddNewTraineeLessonSchedule(List<FlyingFTDSchedule> flyingFTDScheduleList, string[] traineeLessonIdArray, DateTime startingTime, bool isCustomBriefingTime, bool isCustomDebriefingTime, string briefingTimeId, string debriefingTimeId, string briefingStartingTime, string debriefingStartingTime, string rescheduledReasonId)
        {
            try
            {
                string message = "";
                PTSContext db = new PTSContext();
                BriefingAndDebriefingAccess briefingAndDebriefingAccess = new BriefingAndDebriefingAccess();
                UtilityClass utilityClass = new UtilityClass();
                double lessonDuration = 0;

                ///////////////////////////////start, Get and Save BRIEFING and DEBRIEFING time//////////////////////////////////
                BriefingAndDebriefing objBriefing = new BriefingAndDebriefing();
                BriefingAndDebriefing objDebriefing = new BriefingAndDebriefing();

                int briefingAndDebriefingIdForBriefing = 0;
                int briefingAndDebriefingIdForDebriefing = 0;

                string briefingAndDebriefingLength = ConfigurationManager.AppSettings["BriefingAndDebriefingLength"].ToString();

                //Briefing time
                if ((String.IsNullOrEmpty(briefingTimeId) || String.IsNullOrWhiteSpace(briefingTimeId)) || isCustomBriefingTime)
                {
                    if (isCustomBriefingTime)
                    {
                        //briefingStartingTime = " " + briefingStartingTime;
                        objBriefing.StartingTime = Convert.ToDateTime(briefingStartingTime);
                        objBriefing.EndingTime = objBriefing.StartingTime + TimeSpan.FromHours(Convert.ToDouble(briefingAndDebriefingLength));
                    }
                    else
                    {
                        objBriefing.StartingTime = startingTime.AddHours(-1);
                        objBriefing.EndingTime = startingTime;
                    }
                    objBriefing.IsBriefing = true;
                    objBriefing.IsDebriefing = false;
                    if (briefingAndDebriefingAccess.Add(objBriefing))
                    {
                        //Get Briefing Id 
                        briefingAndDebriefingIdForBriefing = objBriefing.BriefingAndDebriefingId; //utilityClass.GetLatestIdNumber("BRIEFING_AND_DEBRIEFING");
                    }
                }
                else
                {//Get Briefing Id
                    briefingAndDebriefingIdForBriefing = Convert.ToInt16(briefingTimeId);
                }

                //Add debriefing time                
                if ((String.IsNullOrEmpty(debriefingTimeId) || String.IsNullOrWhiteSpace(debriefingTimeId)) || isCustomDebriefingTime)
                {
                    if (isCustomDebriefingTime)
                    {
                        //debriefingStartingTime = " " + debriefingStartingTime;
                        objDebriefing.StartingTime = Convert.ToDateTime(debriefingStartingTime);
                        objDebriefing.EndingTime = objDebriefing.StartingTime + TimeSpan.FromHours(Convert.ToDouble(briefingAndDebriefingLength));
                    }
                    else
                    {
                        objDebriefing = GetDebriefingTime(0, startingTime, flyingFTDScheduleList.FirstOrDefault().EquipmentId, traineeLessonIdArray);//pass '0' as scheduleID b/c we donn't have scheduleID as long as the schedule is not created
                    }
                    objDebriefing.IsBriefing = false;
                    objDebriefing.IsDebriefing = true;
                    if (briefingAndDebriefingAccess.Add(objDebriefing))
                    {
                        //Get  Debriefing Id
                        briefingAndDebriefingIdForDebriefing = objDebriefing.BriefingAndDebriefingId; //utilityClass.GetLatestIdNumber("BRIEFING_AND_DEBRIEFING");
                    }
                }
                else
                {
                    briefingAndDebriefingIdForDebriefing = Convert.ToInt16(debriefingTimeId);
                }
                ///////////////////////////////end, Get and Save BRIEFING and DEBRIEFING time/////////////////////////////////////

                TraineeEvaluationTemplateAccess traineeEvaluationTemplateAccess=new TraineeEvaluationTemplateAccess();

                DateTime individualTraineeStartTime = startingTime;
                DateTime individualTraineeEndingTime = startingTime;

                foreach (var flyingFTDSchedule in flyingFTDScheduleList)
                {
                    if (!string.IsNullOrEmpty(rescheduledReasonId))
                    {
                        flyingFTDSchedule.RescheduleReasonId = Int16.Parse(rescheduledReasonId);
                        //Call a method and populate TRAINEE LESSON by cloning the existing one 
                        //Use flyingFTDSchedule.LessonId  and  flyingFTDSchedule.TraineeId=> This might return more than one list
                        traineeEvaluationTemplateAccess.CloneEvaluationTemplate(flyingFTDSchedule.TraineeId, flyingFTDSchedule.LessonId);
                        //if so; sort and select the one that has Greater Sequence =
                        //Where to get Module Id
                    }
                    var lesson = db.Lessons.Find(flyingFTDSchedule.LessonId);
                    var equipment = db.Equipments.Find(flyingFTDSchedule.EquipmentId);

                    //Get lesson length to find schedule end time
                    if (lesson != null  && equipment!= null)
                    {
                        if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                        {
                            individualTraineeEndingTime = (individualTraineeStartTime + TimeSpan.FromHours((double)lesson.FTDTime));
                        }
                        else if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                        {
                            lessonDuration = lesson.TimeAircraftSolo + lesson.TimeAircraftDual;
                            individualTraineeEndingTime = (individualTraineeStartTime + TimeSpan.FromHours(lessonDuration));
                        }
                    }
                    else
                    {
                        individualTraineeEndingTime = individualTraineeStartTime.AddHours(1);
                    }
                    //Assign Period
                    flyingFTDSchedule.ScheduleStartTime = individualTraineeStartTime;
                    flyingFTDSchedule.ScheduleEndTime = individualTraineeEndingTime;
                    individualTraineeStartTime = individualTraineeEndingTime;
                    List<FlyingFTDSchedule> flyingFTDScheduletemp = db.FlyingFTDSchedules.Where(f => f.LessonId == flyingFTDSchedule.LessonId && f.TraineeId == flyingFTDSchedule.TraineeId&& f.Status!="Cancled").ToList();
                    TraineeLesson traineeLessonTemp = db.TraineeLessons.Where(TL => TL.LessonId == flyingFTDSchedule.LessonId && TL.TraineeId == flyingFTDSchedule.TraineeId).OrderByDescending(tl => tl.Sequence).FirstOrDefault();
                    if (traineeLessonTemp == null)
                        traineeLessonTemp = traineeEvaluationTemplateAccess.CloneEvaluationTemplate(flyingFTDSchedule.LessonId, flyingFTDSchedule.TraineeId);

                    if (flyingFTDScheduletemp.Count > 0)
                    {
                        //Get Trainee Lesoon Sequence 
                        TraineeLesson traineeLesson = new TraineeLesson();
                        //creating trainee lesson
                        traineeLesson.TraineeId = flyingFTDSchedule.TraineeId;
                        traineeLesson.Sequence = traineeLessonTemp.Sequence + 1;
                        traineeLesson.AgreedDate = traineeLessonTemp.AgreedDate;
                        traineeLesson.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        traineeLesson.CreationDate = DateTime.Now;
                        traineeLesson.RevisionDate = DateTime.Now;
                        traineeLesson.EvaluationDate = DateTime.Now;
                        traineeLesson.StartDate = DateTime.Now;
                        traineeLesson.EndDate = DateTime.Now;


                        traineeLesson.EndDate = traineeLessonTemp.EndDate;
                        traineeLesson.LessonId = traineeLessonTemp.LessonId;
                        traineeLesson.BatchCategoryId = traineeLessonTemp.BatchCategoryId;
                        traineeLesson.EvaluationTemplateId = traineeLessonTemp.EvaluationTemplateId;
                        traineeLesson.AgreedDate = traineeLessonTemp.AgreedDate;

                        db.TraineeLessons.Add(traineeLesson);
                        db.SaveChanges();
                        flyingFTDSchedule.Lesson = db.Lessons.Find(traineeLessonTemp.LessonId);
                        int? lessonRevisionGroupid = flyingFTDSchedule.Lesson.RevisionGroupId==null?flyingFTDSchedule.LessonId: flyingFTDSchedule.Lesson.RevisionGroupId;

                        flyingFTDSchedule.Sequence = traineeLesson.Sequence;
                    }
                   else
                    flyingFTDSchedule.Sequence = 1;
                    
                    //else
                    //  message = message + " Trainee lesson not found.";
                    string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
                    //Check for duplicate schedule with same resource
                    var existingSchedule = db.FlyingFTDSchedules.Where(FS => FS.InstructorId == flyingFTDSchedule.InstructorId
                    && FS.LessonId == flyingFTDSchedule.LessonId && FS.TraineeId == flyingFTDSchedule.TraineeId
                    && FS.EquipmentId == flyingFTDSchedule.EquipmentId && FS.Status != statusName).ToList();

                    if (existingSchedule.Count > 0)
                    {
                        //Check for time clash                   
                        bool isLessonGiveTimeClash = false;
                        foreach (var schedule in existingSchedule)
                        {
                            if ((flyingFTDSchedule.ScheduleStartTime > schedule.ScheduleStartTime && flyingFTDSchedule.ScheduleStartTime < schedule.ScheduleEndTime) ||
                            (flyingFTDSchedule.ScheduleEndTime > schedule.ScheduleStartTime && flyingFTDSchedule.ScheduleEndTime < schedule.ScheduleEndTime))
                            {
                                isLessonGiveTimeClash = true;
                            }
                        }
                        if (!isLessonGiveTimeClash)
                        {
                            db.FlyingFTDSchedules.Add(flyingFTDSchedule);
                            if (db.SaveChanges() > 0)
                            {
                                //Save FTD Flying Schedule RELATION Briefing And Debriefing 
                                SaveEquipmentBriefingAndDebriefingTime(briefingAndDebriefingIdForBriefing);
                                SaveEquipmentBriefingAndDebriefingTime(briefingAndDebriefingIdForDebriefing);

                                equipment.EstimatedRemainingHours = equipment.EstimatedRemainingHours + (float)lessonDuration;
                                db.Entry(equipment).State = EntityState.Modified;
                                db.SaveChanges();
                                message = (message + " Event successfully scheduled for " + traineeLessonTemp.Trainee.Person.FirstName + " " + traineeLessonTemp.Trainee.Person.MiddleName + ". ");
                            }
                        }
                        else
                            message = message + " Lesson Time Clash.";
                    }
                    else
                    {
                        db.FlyingFTDSchedules.Add(flyingFTDSchedule);
                        if (db.SaveChanges() > 0)
                        {
                            //Save FTD Flying Schedule RELATION to Briefing And Debriefing 
                            //For briefing
                            SaveEquipmentBriefingAndDebriefingTime(briefingAndDebriefingIdForBriefing);
                            //for Debriefing
                            SaveEquipmentBriefingAndDebriefingTime(briefingAndDebriefingIdForDebriefing);

                            equipment.EstimatedRemainingHours = equipment.EstimatedRemainingHours + (float)lessonDuration;
                            db.Entry(equipment).State = EntityState.Modified;
                            db.SaveChanges();
                            message = (message + " Event(s) successfully scheduled");
                        }
                    }
                }
                return message;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public bool SaveEquipmentBriefingAndDebriefingTime(int idNumber)
        {
            try
            {
                EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
                UtilityClass utilityClass = new UtilityClass();

                int flyingFTDScheduleId = utilityClass.GetLatestIdNumber("FLYING_FTD_SCHEDULE");

                EquipmentScheduleBriefingDebriefing equipmentScheduleBriefingDebriefing = new EquipmentScheduleBriefingDebriefing();
                equipmentScheduleBriefingDebriefing.FlyingFTDScheduleId = flyingFTDScheduleId;
                equipmentScheduleBriefingDebriefing.BriefingAndDebriefingId = idNumber;
                equipmentScheduleBriefingDebriefing.Status = "New";

                return equipmentScheduleBriefingDebriefingAccess.Add(equipmentScheduleBriefingDebriefing);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveEquipmentBriefingAndDebriefingTime(int idNumber, int flyingFTDScheduleId)
        {
            try
            {
                EquipmentScheduleBriefingDebriefingAccess equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();
                UtilityClass utilityClass = new UtilityClass();

                EquipmentScheduleBriefingDebriefing equipmentScheduleBriefingDebriefing = new EquipmentScheduleBriefingDebriefing();
                equipmentScheduleBriefingDebriefing.FlyingFTDScheduleId = flyingFTDScheduleId;
                equipmentScheduleBriefingDebriefing.BriefingAndDebriefingId = idNumber;
                equipmentScheduleBriefingDebriefing.Status = "New";

                return equipmentScheduleBriefingDebriefingAccess.Add(equipmentScheduleBriefingDebriefing);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public OperationResult UpdateTraineeLessonSchedule(FlyingFTDSchedule flyingFTDSchedule)
        {
            string message = string.Empty;
            bool isSuccessfullyUpdated;
            FlyingFTDSchedule tobeUpdated = db.FlyingFTDSchedules.Where(f => f.FlyingFTDScheduleId == flyingFTDSchedule.FlyingFTDScheduleId).FirstOrDefault();
            tobeUpdated.TraineeId = flyingFTDSchedule.TraineeId;
            tobeUpdated.LessonId = flyingFTDSchedule.LessonId;
            if (Revise(tobeUpdated))
            {
                message = message + " Event successfully updated";
                isSuccessfullyUpdated = true;
            }
            else
            {
                message = message + " Event not updated";
                isSuccessfullyUpdated = false;
            }
            return new OperationResult
            {
                IsSuccess = isSuccessfullyUpdated,
                Message = message
            };
        }

        public OperationResult UpdateTraineeLessonSchedule(DateTime startingTime, int flyingAndFTDScheduleId, int equipmentId, int instructorId, bool isReschedule, string briefingTimeId, string debriefingTimeId, bool isCustomBriefingTime, bool isCustomDebriefingTime, string briefingStartingTime, string debriefingStartingTime, string date)
        {
            try
            {
                int month = startingTime.Month;
                PTSContext db = new PTSContext();
                string message = ""; double lessonDuration = 0;
                bool isSuccessfullyUpdated = false;
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), (int)FlyingFTDScheduleStatus.Canceled);
                var scheduleToBeUpdated = db.FlyingFTDSchedules.Where(fs => fs.Status != statusName && fs.FlyingFTDScheduleId == flyingAndFTDScheduleId).ToList().FirstOrDefault();
                if (equipmentId == 0)
                    equipmentId = scheduleToBeUpdated.EquipmentId;
                if (instructorId == 0)
                    instructorId = scheduleToBeUpdated.InstructorId;
                DateTime dateToUpdate = Convert.ToDateTime(startingTime.Month + "/" + startingTime.Day + "/" + startingTime.Year);
                DateTime dateCurrent = Convert.ToDateTime(scheduleToBeUpdated.ScheduleStartTime.Month + "/" + scheduleToBeUpdated.ScheduleStartTime.Day + "/" + scheduleToBeUpdated.ScheduleStartTime.Year);
                int dateDifference = (dateToUpdate - dateCurrent).Days;
                string traineeLessonIdPair = scheduleToBeUpdated.TraineeId + "-" + scheduleToBeUpdated.LessonId + "~";
                string[] traineeLessonIdArray = traineeLessonIdPair.Split('~');

                BriefingAndDebriefingAccess briefingAndDebriefingAccess = new BriefingAndDebriefingAccess();
                var equipmentScheduleBriefingDebriefingAccess = new EquipmentScheduleBriefingDebriefingAccess();

                UtilityClass utilityClass = new UtilityClass();

                ///////////////////////////////start, Get and Save BRIEFING and DEBRIEFING time//////////////////////////////////
                BriefingAndDebriefing objBriefing = new BriefingAndDebriefing();
                BriefingAndDebriefing objDebriefing = new BriefingAndDebriefing();

                int briefingAndDebriefingIdForBriefing = 0;
                int briefingAndDebriefingIdForDebriefing = 0;

                string briefingAndDebriefingLength = ConfigurationManager.AppSettings["BriefingAndDebriefingLength"].ToString();

                EquipmentScheduleBriefingDebriefing CurrentBriefingSchedule = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(flyingAndFTDScheduleId, true, false);
                EquipmentScheduleBriefingDebriefing CurrentDebriefingSchedule = equipmentScheduleBriefingDebriefingAccess.GetEquipmentSchduleBriefingDebriefingByScheduleId(flyingAndFTDScheduleId, false, true);

                BriefingAndDebriefing BriefingToUpdate = briefingAndDebriefingAccess.Details(CurrentBriefingSchedule.BriefingAndDebriefingId);

                //Briefing time
                if (instructorId == scheduleToBeUpdated.InstructorId)
                {
                    if ((String.IsNullOrEmpty(briefingTimeId) || String.IsNullOrWhiteSpace(briefingTimeId)) || isCustomBriefingTime)
                    {
                        if (isCustomBriefingTime)
                        {
                            //briefingStartingTime = " " + briefingStartingTime;
                            objBriefing.StartingTime = Convert.ToDateTime(date + " " + briefingStartingTime);
                            objBriefing.EndingTime = objBriefing.StartingTime + TimeSpan.FromHours(Convert.ToDouble(briefingAndDebriefingLength));
                            if (objBriefing.EndingTime > startingTime)
                            {
                                objBriefing.EndingTime = startingTime;
                                objBriefing.StartingTime = objBriefing.EndingTime - TimeSpan.FromHours(Convert.ToDouble(briefingAndDebriefingLength));
                            }
                        }
                        else
                        {
                            if (dateDifference != 0)
                            {
                                objBriefing.StartingTime = BriefingToUpdate.StartingTime.AddDays(dateDifference);
                                objBriefing.EndingTime = BriefingToUpdate.EndingTime.AddDays(dateDifference);
                                if ((startingTime - objBriefing.StartingTime).TotalMinutes < 1)
                                {
                                    objBriefing.StartingTime = startingTime.AddHours(-1);
                                    objBriefing.EndingTime = startingTime;
                                }

                            }
                            else
                            {
                                if ((startingTime - CurrentBriefingSchedule.BriefingAndDebriefing.StartingTime).TotalMinutes < 1)
                                {
                                    objBriefing.StartingTime = startingTime.AddHours(-1);
                                    objBriefing.EndingTime = startingTime;
                                }
                                else
                                {
                                    objBriefing.StartingTime = CurrentBriefingSchedule.BriefingAndDebriefing.StartingTime;
                                    objBriefing.EndingTime = CurrentBriefingSchedule.BriefingAndDebriefing.EndingTime;
                                }
                            }
                        }
                        objBriefing.IsBriefing = true;
                        objBriefing.IsDebriefing = false;

                        if (BriefingToUpdate.EndingTime <= startingTime && dateDifference == 0)
                        {
                            BriefingToUpdate.StartingTime = objBriefing.StartingTime;
                            BriefingToUpdate.EndingTime = objBriefing.EndingTime;
                            db.Entry(BriefingToUpdate).State = EntityState.Modified;
                            db.SaveChanges();
                            briefingAndDebriefingIdForBriefing = BriefingToUpdate.BriefingAndDebriefingId;
                        }
                        else
                        {
                            if (briefingAndDebriefingAccess.Add(objBriefing))
                            {
                                //Get Briefing Id 
                                briefingAndDebriefingIdForBriefing = utilityClass.GetLatestIdNumber("BRIEFING_AND_DEBRIEFING");
                            }
                        }
                    }
                    else
                    {//Get Briefing Id
                        briefingAndDebriefingIdForBriefing = Convert.ToInt16(briefingTimeId);
                    }
                }
                else
                {
                    objBriefing.StartingTime = BriefingToUpdate.StartingTime;
                    objBriefing.EndingTime = BriefingToUpdate.EndingTime;
                    objBriefing.IsBriefing = true;
                    objBriefing.IsDebriefing = false;

                    if (briefingAndDebriefingAccess.Add(objBriefing))
                    {
                        //Get Briefing Id 
                        briefingAndDebriefingIdForBriefing = utilityClass.GetLatestIdNumber("BRIEFING_AND_DEBRIEFING");
                    }
                }

                //Add debriefing time                

                ///////////////////////////////end, Get and Save BRIEFING and DEBRIEFING time/////////////////////////////////////


                DateTime LessonEndingTime = startingTime;

                var lesson = db.Lessons.Where(L => L.LessonId == scheduleToBeUpdated.LessonId).ToList();
                var equipment = db.Equipments.Where(E => E.EquipmentId == scheduleToBeUpdated.EquipmentId).ToList();
                var equipmentObj = equipment.FirstOrDefault();
                //get schedule end time
                if (lesson.Count > 0 && lesson.Count > 0)
                {
                    if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    {
                        LessonEndingTime = (startingTime + TimeSpan.FromHours((double)lesson.FirstOrDefault().FTDTime));
                    }
                    else if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    {
                        lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                        LessonEndingTime = (startingTime + TimeSpan.FromHours(lessonDuration));
                    }
                }
                else
                {
                    LessonEndingTime = startingTime.AddHours(1);
                }
                BriefingAndDebriefing DebriefingToUpdate = briefingAndDebriefingAccess.Details(CurrentDebriefingSchedule.BriefingAndDebriefingId);

                if (instructorId == scheduleToBeUpdated.InstructorId)
                {
                    if ((String.IsNullOrEmpty(debriefingTimeId) || String.IsNullOrWhiteSpace(debriefingTimeId)) || isCustomDebriefingTime)
                    {
                        BriefingAndDebriefing objDebriefingTemp = new BriefingAndDebriefing();


                        objDebriefingTemp = GetDebriefingTime(flyingAndFTDScheduleId, startingTime, equipmentId, traineeLessonIdArray);

                        if (isCustomDebriefingTime)
                        {
                            //debriefingStartingTime = " " + debriefingStartingTime;
                            objDebriefing.StartingTime = Convert.ToDateTime(date + " " + debriefingStartingTime);
                            objDebriefing.EndingTime = objDebriefing.StartingTime + TimeSpan.FromHours(Convert.ToDouble(briefingAndDebriefingLength));

                            if (objDebriefing.StartingTime < LessonEndingTime)
                            {
                                objDebriefing.StartingTime = LessonEndingTime;
                                objDebriefing.EndingTime = LessonEndingTime.AddHours(1);
                            }
                        }
                        else
                        {

                            objDebriefing = objDebriefingTemp;
                        }

                        objDebriefing.IsBriefing = false;
                        objDebriefing.IsDebriefing = true;
                        //if (dateDifference!=0)
                        //{
                        //    objDebriefing.StartingTime = DebriefingToUpdate.StartingTime.AddDays(dateDifference);
                        //    objDebriefing.EndingTime = DebriefingToUpdate.EndingTime.AddDays(dateDifference);

                        //}
                        if (DebriefingToUpdate.StartingTime >= LessonEndingTime && dateDifference == 0)
                        {
                            DebriefingToUpdate.EndingTime = objDebriefing.EndingTime;
                            DebriefingToUpdate.StartingTime = objDebriefing.StartingTime;
                            db.Entry(DebriefingToUpdate).State = EntityState.Modified;
                            db.SaveChanges();
                            briefingAndDebriefingIdForDebriefing = DebriefingToUpdate.BriefingAndDebriefingId;
                        }
                        else
                        {
                            objDebriefing.StartingTime = objDebriefing.StartingTime.AddDays(dateDifference);
                            objDebriefing.EndingTime = objDebriefing.EndingTime.AddDays(dateDifference);
                            if (briefingAndDebriefingAccess.Add(objDebriefing))
                            {
                                //Get  Debriefing Id
                                briefingAndDebriefingIdForDebriefing = utilityClass.GetLatestIdNumber("BRIEFING_AND_DEBRIEFING");
                            }
                        }
                    }
                    else
                    {
                        briefingAndDebriefingIdForDebriefing = Convert.ToInt16(debriefingTimeId);
                    }
                }
                else
                {
                    objDebriefing.StartingTime = DebriefingToUpdate.StartingTime;
                    objDebriefing.EndingTime = DebriefingToUpdate.EndingTime;
                    objDebriefing.IsDebriefing = true;
                    objDebriefing.IsBriefing = false;

                    if (briefingAndDebriefingAccess.Add(objDebriefing))
                    {
                        //Get Briefing Id 
                        briefingAndDebriefingIdForDebriefing = utilityClass.GetLatestIdNumber("BRIEFING_AND_DEBRIEFING");
                    }
                }

                var otherExistingSchedules = db.FlyingFTDSchedules.Where(FS => FS.InstructorId == instructorId
                && FS.LessonId == scheduleToBeUpdated.LessonId && FS.TraineeId == scheduleToBeUpdated.TraineeId
                && FS.EquipmentId == equipmentId && FS.FlyingFTDScheduleId != flyingAndFTDScheduleId && FS.Status != statusName).ToList();

                bool isLessonGiveTimeClash = false;
                if (otherExistingSchedules.Count > 0)
                {
                    //Check for time clash 
                    foreach (var otherSchedule in otherExistingSchedules)
                    {
                        if ((startingTime >= otherSchedule.ScheduleStartTime && startingTime <= otherSchedule.ScheduleEndTime) ||
                        (LessonEndingTime >= otherSchedule.ScheduleStartTime && LessonEndingTime <= otherSchedule.ScheduleEndTime))
                        {
                            isLessonGiveTimeClash = true;
                        }
                    }
                }

                string statusName1 = Enum.GetName(typeof(FlyingFTDScheduleStatus), 0);
                if (!isLessonGiveTimeClash || otherExistingSchedules.Count == 0)
                {
                    if (isReschedule)
                    {
                        TraineeEvaluationTemplateAccess traineeEvaluationTemplateAccess = new TraineeEvaluationTemplateAccess();
                        //•	If reschedule, Clone TRAINEE LESSON, Trainee Evaluation TEMPLATE, CATEGORY and ITEM and save as new record where TraineeLessonId will be a foreign Key for Trainee Evaluation Template.
                        var newTraineeLesson = traineeEvaluationTemplateAccess.CloneEvaluationTemplate(scheduleToBeUpdated.TraineeId, scheduleToBeUpdated.LessonId);

                        FlyingFTDSchedule flyingFTDSchedule = new FlyingFTDSchedule();
                        flyingFTDSchedule.EquipmentId = equipmentId;
                        flyingFTDSchedule.InstructorId = instructorId;
                        flyingFTDSchedule.LessonId = scheduleToBeUpdated.LessonId;
                        flyingFTDSchedule.TraineeId = scheduleToBeUpdated.TraineeId;
                        flyingFTDSchedule.ScheduleStartTime = startingTime;
                        flyingFTDSchedule.ScheduleEndTime = LessonEndingTime;
                        flyingFTDSchedule.Status = statusName1;
                        flyingFTDSchedule.Sequence = newTraineeLesson.Sequence;
                        flyingFTDSchedule.IsNotified = false;

                        db.FlyingFTDSchedules.Add(flyingFTDSchedule);
                        if (db.SaveChanges() > 0)
                        {
                            //Save the new FTD Flying Schedule and Briefing And Debriefing Association
                            SaveEquipmentBriefingAndDebriefingTime(briefingAndDebriefingIdForBriefing);
                            //Save the new FTD Flying Schedule and Briefing And Debriefing Association
                            SaveEquipmentBriefingAndDebriefingTime(briefingAndDebriefingIdForDebriefing);
                            isSuccessfullyUpdated = true;

                            equipmentObj.EstimatedRemainingHours = equipmentObj.EstimatedRemainingHours + (float)lessonDuration;
                            db.Entry(equipmentObj).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        scheduleToBeUpdated.ScheduleStartTime = startingTime;
                        scheduleToBeUpdated.ScheduleEndTime = LessonEndingTime;
                        scheduleToBeUpdated.EquipmentId = equipmentId;
                        scheduleToBeUpdated.InstructorId = instructorId;

                        db.Entry(scheduleToBeUpdated).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            //////////Delete previous BRIEFING and DEBRIEFING //////////////////
                            var lessonBriefings = db.EquipmentScheduleBriefingDebriefings.Where(b => b.FlyingFTDScheduleId == scheduleToBeUpdated.FlyingFTDScheduleId).ToList();
                            if (lessonBriefings.Count > 0)
                            {
                                //Delete the previous relationship
                                foreach (var briefing in lessonBriefings)
                                {
                                    db.EquipmentScheduleBriefingDebriefings.Remove(briefing);
                                    db.SaveChanges();

                                    var briefingList = db.EquipmentScheduleBriefingDebriefings.Where(b => b.BriefingAndDebriefingId == briefing.BriefingAndDebriefingId && b.EquipmentScheduleBriefingDebriefingId != briefing.EquipmentScheduleBriefingDebriefingId).ToList();

                                    //Delete the actaul beriefing and debriefing if it is not referenced by other schedule 
                                    if (briefingList.Count == 0)
                                    {
                                        var briefingAndDebriefing = db.BriefingAndDebriefings.Where(b => b.BriefingAndDebriefingId == briefing.BriefingAndDebriefingId && b.BriefingAndDebriefingId != briefingAndDebriefingIdForBriefing && b.BriefingAndDebriefingId != briefingAndDebriefingIdForDebriefing).ToList().FirstOrDefault();
                                        if (briefingAndDebriefing != null)
                                        {
                                            db.BriefingAndDebriefings.Remove(briefingAndDebriefing);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            //Save BRIEFING
                            SaveEquipmentBriefingAndDebriefingTime(briefingAndDebriefingIdForBriefing, scheduleToBeUpdated.FlyingFTDScheduleId);
                            //Save DEBRIEFING
                            SaveEquipmentBriefingAndDebriefingTime(briefingAndDebriefingIdForDebriefing, scheduleToBeUpdated.FlyingFTDScheduleId);
                            isSuccessfullyUpdated = true;
                        }
                    }
                }
                else
                    message = message + " Lesson Time Clash.";

                if (isSuccessfullyUpdated)
                    message = message + " Event successfully updated";
                return new OperationResult
                {
                    IsSuccess = isSuccessfullyUpdated,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public bool UpdateScheduleStatus(int flyingFTDScheduleId, FlyingFTDScheduleStatus status)
        {
            try
            {
                PTSContext db = new PTSContext();
                var flyingFTDSchedule = db.FlyingFTDSchedules.Find(flyingFTDScheduleId);
                if (flyingFTDSchedule != null)
                {
                    flyingFTDSchedule.Status = Enum.GetName(typeof(FlyingFTDScheduleStatus), (int)status);
                    db.Entry(flyingFTDSchedule).State = EntityState.Modified;
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region Checking Constraints
        /*Constraint #1 Instructor shall be same throughout the pre solo lessons. */
        public FTDandFlyingInstructorView GetInstructorOfThePreSoloLessons(int traineeId)
        {
            try
            {
                //Make IsPreSolo bool instead of string
                PTSContext db = new PTSContext();
                string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);

                var result = db.FlyingFTDSchedules.Where(sc => sc.TraineeId == traineeId && sc.Lesson.IsPreSolo && sc.Status != statusName).OrderByDescending(o => o.FlyingFTDScheduleId).Take(1).ToList();

                if (result.Count() > 0)
                {
                    return new FTDandFlyingInstructorView
                    {
                        Id = result.FirstOrDefault().InstructorId,
                        Name = result.FirstOrDefault().Instructor.Person.FirstName.Substring(0, 3) + " " + result.FirstOrDefault().Instructor.Person.MiddleName.Substring(0, 1) + "."
                    }; //must not be hardcoded
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /*Constraint #6 Validity Equipment working hours*/
        public bool IsInBetweenEquipmentWorkingHour(FlyingFTDSchedule schedule)
        {
            try
            {
                PTSContext db = new PTSContext();

                var lesson = db.Lessons.Where(L => L.LessonId == schedule.LessonId).ToList();
                var equipment = db.Equipments.Where(E => E.EquipmentId == schedule.EquipmentId).ToList();

                var workingHours = equipment.FirstOrDefault().WorkingHours;

                DateTime equipmentStartTime = Convert.ToDateTime((schedule.ScheduleStartTime.Date + equipment.FirstOrDefault().StartTime).ToString("MM/dd/yyyy HH:mm"));
                DateTime equipmentEndTime = (equipmentStartTime.AddHours((double)workingHours));

                //Get lesson ending Time
                if (lesson.Count > 0 && lesson.Count > 0)
                {
                    if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    {
                        schedule.ScheduleEndTime = (schedule.ScheduleStartTime.AddHours((double)lesson.FirstOrDefault().FTDTime));
                    }
                    else if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    {
                        double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                        schedule.ScheduleEndTime = schedule.ScheduleStartTime.AddHours(lessonDuration);
                    }
                }
                else
                {
                    equipmentEndTime = equipmentStartTime.AddHours(1);
                    schedule.ScheduleEndTime = (schedule.ScheduleEndTime.AddHours(1));
                }

                //Check whether or not we are trying to schedule in between the Equipment working hours
                if (((schedule.ScheduleStartTime >= equipmentStartTime && schedule.ScheduleStartTime <= equipmentEndTime) && (schedule.ScheduleEndTime >= equipmentStartTime && schedule.ScheduleEndTime <= equipmentEndTime)))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        /*Constraint #8 Validity Equipment Downtime schedule*/
        public bool IsInBetweenEquipmentDowntime(FlyingFTDSchedule schedule)
        {
            try
            {
                PTSContext db = new PTSContext();

                var lesson = db.Lessons.Where(L => L.LessonId == schedule.LessonId).ToList();
                var equipment = db.Equipments.Where(E => E.EquipmentId == schedule.EquipmentId).ToList();

                //get downtime equipment of the selected equipment
                var result = db.EquipmentDowntimeSchedules.Where(E => E.EquipmentId == schedule.EquipmentId).ToList();
                //is there downtime schedule in the selected date
                var equipmentDowntimeSchedule = result.Where(EDT => (EDT.ScheduleStartDate.Date == schedule.ScheduleStartTime.Date)).ToList();

                //Get lesson ending Time
                if (lesson.Count > 0 && lesson.Count > 0)
                {
                    if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    {
                        schedule.ScheduleEndTime = (schedule.ScheduleStartTime.AddHours((double)lesson.FirstOrDefault().FTDTime));
                    }
                    else if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FLYING")
                    {
                        double lessonDuration = lesson.FirstOrDefault().TimeAircraftSolo + lesson.FirstOrDefault().TimeAircraftDual;
                        schedule.ScheduleEndTime = schedule.ScheduleStartTime.AddHours(lessonDuration);
                    }
                }
                else
                {
                    schedule.ScheduleEndTime = (schedule.ScheduleEndTime.AddHours(1));
                }

                bool IsInBetweenEquipmentDowntime = false;
                if (equipmentDowntimeSchedule.Count > 0)
                {
                    foreach (var downtimeSchedule in equipmentDowntimeSchedule)
                    {
                        //Check whether or not we are trying to schedule in between the Equipment working hours
                        if (((schedule.ScheduleStartTime >= downtimeSchedule.ScheduleStartDate && schedule.ScheduleStartTime <= downtimeSchedule.ScheduleEndDate) && (schedule.ScheduleEndTime >= downtimeSchedule.ScheduleStartDate && schedule.ScheduleEndTime <= downtimeSchedule.ScheduleEndDate)))
                            IsInBetweenEquipmentDowntime = true;
                    }
                }
                return IsInBetweenEquipmentDowntime;
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        /*Constraint #9, Validity Equipment Recurring Downtime Schedule*/
        public bool IsInBetweenRecurringDowntimeSchedule(FlyingFTDSchedule schedule)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<FTDRecurringDownTime> FTDRecurringDownTimeList = new List<FTDRecurringDownTime>();
                var equipment = db.Equipments.Where(E => E.EquipmentId == schedule.EquipmentId).ToList();

                if (equipment.Count > 0)
                {
                    if (equipment.FirstOrDefault().EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    {
                        var FTDRecurringDownTimesList = db.FTDRecurringDownTimes.ToList();
                        if (FTDRecurringDownTimesList.Count > 0)
                        {
                            string day_Name = schedule.ScheduleStartTime.ToString("dddd");
                            foreach (var recurringDownTime in FTDRecurringDownTimesList)
                            {
                                if (day_Name.ToLower() == recurringDownTime.Day)
                                {
                                    if (((schedule.ScheduleStartTime >= (schedule.ScheduleStartTime.Date + recurringDownTime.StartingDate)
                                        && schedule.ScheduleStartTime <= (schedule.ScheduleStartTime.Date + recurringDownTime.EndingDate))
                                        && (schedule.ScheduleEndTime >= (schedule.ScheduleStartTime.Date + recurringDownTime.StartingDate)
                                        && schedule.ScheduleEndTime <= (schedule.ScheduleStartTime.Date + recurringDownTime.EndingDate))))
                                        return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*Constraint #10, Check for equipment Certification expiry date */
        public bool IsEquipmentCerteficateExpired(int equipmentId, DateTime date)
        {
            PTSContext db = new PTSContext();

            var result = db.EquipmentCertificates.Where(E => E.EquipmentId == equipmentId).ToList();
            var resultExtract = result.Where(EQ => EQ.StartingDate.Date <= date.Date).ToList();
            if (resultExtract.Count() == 0)
            {
                return false;
            }
            return true;
        }


        /*Constraint #11-> 1, Check for trainee Certification expiry date */
        /* public bool IsTraineeCerteficateExpired(int traineeId, DateTime date)
           {
               PTSContext db = new PTSContext();

               var result = db.TraineeCertificates.Where(E => E.TraineeId == traineeId).ToList();
               var resultExtract = result.Where(EQ => EQ.StartingDate.Date <= date.Date).ToList();
               if (resultExtract.Count() == 0)
               {
                   return false;
               }
               return true;
           }
           */
        /*Constraint #11-> 2, Check for trainee Certification expiry date */
        /*   public bool IsInstructorCerteficateExpired(int instructorId, DateTime date)
           {
               PTSContext db = new PTSContext();

               var result = db.InstructorCertificates.Where(E => E.InstructorId == instructorId).ToList();
               var resultExtract = result.Where(EQ => EQ.StartingDate.Date <= date.Date).ToList();
               if (resultExtract.Count() == 0)
               {
                   return false;
               }
               return true;
           }

           */

        /*Is Equipment Free*/
        public bool IsEquipmentFree(FlyingFTDSchedule schedule)
        {
            PTSContext db = new PTSContext();
            DateTime tempStartingTime = schedule.ScheduleStartTime.AddMinutes(-1);
            DateTime tempEndingTime = schedule.ScheduleEndTime.AddMinutes(1);
            string statusName = Enum.GetName(typeof(FlyingFTDScheduleStatus), 1);
            var result = db.FlyingFTDSchedules.Where(sc => sc.EquipmentId == schedule.EquipmentId && sc.Status != statusName).ToList();
            var scheduledEquipment = result.Where(b => ((b.ScheduleStartTime > schedule.ScheduleStartTime && b.ScheduleStartTime < schedule.ScheduleEndTime) || (b.ScheduleEndTime > schedule.ScheduleStartTime && b.ScheduleEndTime < schedule.ScheduleEndTime)) || ((b.ScheduleStartTime > tempStartingTime && b.ScheduleEndTime < tempEndingTime) && !((b.ScheduleStartTime > schedule.ScheduleStartTime && b.ScheduleStartTime < schedule.ScheduleEndTime) || (b.ScheduleEndTime > schedule.ScheduleStartTime && b.ScheduleEndTime < schedule.ScheduleEndTime)))).ToList();
            //var scheduledEquipment = result.Where(SI => ((schedule.ScheduleStartTime > SI.ScheduleStartTime && schedule.ScheduleStartTime < SI.ScheduleEndTime) || (schedule.ScheduleEndTime > SI.ScheduleStartTime && schedule.ScheduleEndTime < SI.ScheduleEndTime))).ToList();

            if (scheduledEquipment.Count() == 0)
            {
                return true;
            }
            return false;
        }
        public List<EquipmentDowntimeSchedule> GetEquipmentDowntimeSchedule(int equipmentId, DateTime date)
        {
            try
            {
                PTSContext db = new PTSContext();

                DateTime startDate = date.Date;
                DateTime endDate = date.AddDays(1);

                var result = db.EquipmentDowntimeSchedules.Where(SDT => SDT.EquipmentId == equipmentId && SDT.ScheduleStartDate >= startDate && SDT.ScheduleStartDate <=endDate).ToList();
                //var resultExtract = result.Where(Sc => Sc.ScheduleStartDate.Date == date.Date).ToList();
                if (result.Count() > 0)
                    return result.ToList();
                else
                    return new List<EquipmentDowntimeSchedule>();
            }
            catch (Exception ex)
            {
                return new List<EquipmentDowntimeSchedule>();
            }
        }
        public List<FTDRecurringDownTime> GetEquipmentRecurringDowntimeSchedule(int equipmentId, DateTime date)
        {
            try
            {
                PTSContext db = new PTSContext();
                List<FTDRecurringDownTime> FTDRecurringDownTimeList = new List<FTDRecurringDownTime>();
                var equipment = db.Equipments.Find(equipmentId);
                if (equipment != null)
                {
                    if (equipment.EquipmentModel.EquipmentType.EquipmentTypeName.ToUpper() == "FTD")
                    {
                        var FTDRecurringDownTimesList = db.FTDRecurringDownTimes.ToList();
                        if (FTDRecurringDownTimesList.Count > 0)
                        {
                            string day_Name = date.ToString("dddd");
                            foreach (var recurringDownTime in FTDRecurringDownTimesList)
                            {
                                if (day_Name.ToLower() == recurringDownTime.Day)
                                {
                                    FTDRecurringDownTimeList.Add(recurringDownTime);
                                }
                            }
                        }
                    }
                }
                return FTDRecurringDownTimeList.ToList();
            }
            catch (Exception ex)
            {
                return new List<FTDRecurringDownTime>();
            }
        }
        #endregion
    }

    public class EquipmentsView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EquipmentsViewForEdit
    {
        public EquipmentsViewForEdit()
        {
            this.Instructors = new List<FTDandFlyingInstructorView>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FTDandFlyingInstructorView> Instructors { get; set; }
    }

    public class FTDandFlyingInstructorView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class LessonView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LessonSequence { get; set; }
        public int PhaseId { get; set; }
        public float LessonDuration { get; set; }
        public bool IsAlreadyScheduled { get; set; }
    }
    public class PhaseLessonView
    {
        public PhaseLessonView()
        {
            this.LessonList = new List<LessonView>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int PhaseSequence { get; set; }
        public List<LessonView> LessonList { get; set; }
    }



    public class DisplayDropDownOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class BriefingAndDebriefingView
    {
        public List<DisplayDropDownOption> Briefing { get; set; }
        public List<DisplayDropDownOption> Debriefing { get; set; }
    }
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
