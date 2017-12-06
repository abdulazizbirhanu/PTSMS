using System;
using System.Collections.Generic;
using System.Linq;
using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSBAL.Utility;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using PTSMSDAL.Models.Enrollment.Relations;

namespace PTSMSBAL.Scheduling.Others
{
    public class SchedulerUtility
    {
        SchedulerAccess schedulerAccess = new SchedulerAccess();
        FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();

        #region Scheduled Event listener
        public List<Scheduler> ScheduledEventList(string month, string year)
        {
            return schedulerAccess.ScheduledEventList(month, year);
        }
        public List<Scheduler> GetScheduledEventForTrainee(string companyId)
        {
            return schedulerAccess.GetScheduledEventForTrainee(companyId);
        }

        public List<Scheduler> GetScheduledEventForInstructor(string companyId, string month, string year)
        {
            return schedulerAccess.GetScheduledEventForInstructor(companyId, month, year);
        }


        public List<EquipmentScheduler> GetFTDandFlyingScheduledEvent(string month, string year)
        {
            return schedulerAccess.GetFTDandFlyingScheduledEvent(month, year);
        }
        public List<EquipmentScheduler> GetFTDandFlyingScheduledEvent(int FlyingFTDScheduleId)
        {
            return schedulerAccess.GetFTDandFlyingScheduledEvent(FlyingFTDScheduleId);
        }
        public List<EquipmentScheduler> GetEquipmentEventForInstructorUtilization()
        {
            return schedulerAccess.GetEquipmentEventForInstructorUtilization();
        }

        public List<EquipmentScheduler> GetEquipmentEventForInstructorUtilizations(string month, string year)
        {
            return schedulerAccess.GetEquipmentEventForInstructorUtilizations(month, year);
        }

        public List<EquipmentScheduler> GetFTDandFlyingScheduledEventForInstructor(string companyId, string month, string year)
        {
            return schedulerAccess.GetFTDandFlyingScheduledEventForInstructor(companyId, month, year);
        }
        public List<EquipmentScheduler> GetFTDandFlyingScheduledEventForTrainee(string companyId)
        {
            return schedulerAccess.GetFTDandFlyingScheduledEventForTrainee(companyId);
        }
        public List<SchedulerResource> GetSchedulerResource()
        {
            return schedulerAccess.GetSchedulerResource();
        }
        public List<SchedulerResourceInstructor> GetSchedulerInstructorResource()
        {
            return schedulerAccess.GetSchedulerInstructorResource();
        }

        public List<SchedulerResource> GetSchedulerResourceForInstructor(string companyId)
        {
            return schedulerAccess.GetSchedulerResourceForInstructor(companyId);
        }
        public List<SchedulerResource> GetSchedulerResourceForTrainee(string companyId)
        {
            return schedulerAccess.GetSchedulerResourceForTrainee(companyId);
        }
        public List<GroundSchedulerResource> GetGroundSchedulerResource()
        {
            return schedulerAccess.GetGroundSchedulerResource();
        }
        public List<GroundSchedulerResource> GetGroundSchedulerResourceForTrainee(string companyId)
        {
            return schedulerAccess.GetGroundSchedulerResourceForTrainee(companyId);
        }
        public List<GroundSchedulerResource> GetGroundSchedulerResourceForInstructor(string companyId)
        {
            return schedulerAccess.GetGroundSchedulerResourceForInstructor(companyId);
        }



        public List<Scheduler> FilterScheduledEventList(string FilterBy, string FilterValue)
        {
            return schedulerAccess.FilterScheduledEventList(FilterBy, FilterValue);
        }

        public List<UnscheduledResource> get_FreeTimeSlotAndRoom(int ModuleScheduleId)
        {
            return schedulerAccess.get_FreeTimeSlotAndRoom(ModuleScheduleId);
        }
        public List<FreePeriodList> get_FreeTimeSlotAndRoomForSpecificDate(DateTime date, int instructorId, int phaseScheduleId, int phseModuleId, int noOfDays)
        {
            List<FreePeriodList> freePeriodList = new List<FreePeriodList>();
            //for (int i = 1; i <= noOfDays; i++)
            //{

            //}
            while (noOfDays > 0)
            {
                FreePeriodList list = new FreePeriodList();
                List<FreePeriod> freePeriod = schedulerAccess.get_FreeTimeSlotAndRoomForSpecificDate(date, instructorId, phaseScheduleId, phseModuleId);
                list.FreePeriod = freePeriod;

                if (freePeriod.Count() < 1)
                {
                    list.Message = "Date: " + date.ToShortDateString() + ",  No available Period/Class room or Instructor.";
                }
                else
                {
                    list.Message = "OK";
                    noOfDays--;
                }

                freePeriodList.Add(list);
                date = date.AddDays(1);
            }
            return freePeriodList;
            //return schedulerAccess.get_FreeTimeSlotAndRoomForSpecificDate(date, instructorId, phaseScheduleId, phseModuleId);
        }
        public List<FreeInstructor> GetFreeInstructorForSpecificDate(DateTime date, int batchClassId)
        {
            return schedulerAccess.GetFreeInstructorForSpecificDate(date, batchClassId);
        }
        public List<FilterView> Filter(string filterBy)
        {
            return schedulerAccess.Filter(filterBy);
        }
        public List<CourseModule> GetCourseModule(int courseId)
        {
            return schedulerAccess.GetCourseModule(courseId);
        }

        public AttendanceView GetAttendaceData(int moduleScheduleId)
        {
            return schedulerAccess.GetAttendaceData(moduleScheduleId);
        }
        public List<InstructorColor> ListScheduledInstructors()
        {
            return schedulerAccess.ListScheduledInstructors();
        }

        public ModuleSchedule ModuleScheduleDetails(int id)
        {
            return schedulerAccess.ModuleScheduleDetails(id);
        }

        public bool CancelGroundSchedule(string reason, string remark, string moduleScheduleId)
        {
            return schedulerAccess.CancelGroundSchedule(reason, remark, moduleScheduleId);
        }
        #endregion


        #region FTD and GF Scheduling
        public List<BatchClass> GetBatchClass()
        {
            return fTDAndFlyingSchedulerAccess.GetBatchClass();
        }
        public List<EquipmentColor> ListScheduledEquipment()
        {
            return schedulerAccess.ListScheduledEquipment();
        }
        public List<BatchClassColor> BatchClassColorList()
        {
            return schedulerAccess.BatchClassColorList();
        }


        public List<EquipmentType> get_EquipmentType()
        {
            return fTDAndFlyingSchedulerAccess.get_EquipmentType();
        }
        public List<InstructorColor> FlyingAndFTDInstructorColorList()
        {
            return fTDAndFlyingSchedulerAccess.FlyingAndFTDInstructorColorList();
        }
        public List<EquipmentsView> GetFreeEquipment(int flyingFTDScheduleId, DateTime date)
        {
            return fTDAndFlyingSchedulerAccess.GetFreeEquipment(flyingFTDScheduleId, date);
        }

        public List<EquipmentsViewForEdit> GetFreeEquipmentAndInstructors(int flyingFTDScheduleId, DateTime date)
        {
            return fTDAndFlyingSchedulerAccess.GetFreeEquipmentAndInstructors(flyingFTDScheduleId, date);
        }
       

        public List<FTDandFlyingInstructorView> GetFTDandFlyingInstructors(int equipmentId, DateTime startingTime, string[] traineelesson)
        {
            return fTDAndFlyingSchedulerAccess.GetFTDandFlyingInstructors(equipmentId, startingTime, traineelesson);
        }
        public List<FTDandFlyingInstructorView> GetFTDandFlyingInstructors(int flyingFTDScheduleId, string date, string time)
        {
            return fTDAndFlyingSchedulerAccess.GetFTDandFlyingInstructors(flyingFTDScheduleId, date,time);
        }
        //public List<TraineeView> GetBatchTrainee(int lessonId, int equipmentId, DateTime startingTime)
        //{
        //    return fTDAndFlyingSchedulerAccess.GetBatchTrainee(lessonId, equipmentId, startingTime);
        //}

        public List<TraineeView> GetTraineeList(int equipmentId, DateTime startingTime)
        {
            return fTDAndFlyingSchedulerAccess.GetTraineeList(equipmentId, startingTime);
        }
        public List<TraineeView> GetBatchClassTraineesSimplified(int batchClassId, string lessonType)
        {
            return fTDAndFlyingSchedulerAccess.GetBatchClassTraineesSimplified(batchClassId, lessonType);
        }
        public List<TraineeView> GetBatchClassTrainees(int batchClassId, string lessonType)
        {
            return fTDAndFlyingSchedulerAccess.GetBatchClassTrainees(batchClassId, lessonType);
        }
        public List<TraineeView> GetBatchClassTraineesForEdit(int batchClassId, string lessonType)
        {
            return fTDAndFlyingSchedulerAccess.GetBatchClassTraineesFullName(batchClassId, lessonType);
        }
        public List<TraineeView> GetBatchClassTraineesForTraineeUpdate(int batchClassId, string lessonType)
        {
            return fTDAndFlyingSchedulerAccess.GetBatchClassTraineesForEdit(batchClassId, lessonType);
        }
        public List<FlyingFTDSchedule> GetReservedDateTimes(int equipmentId, string date)
        {
            return fTDAndFlyingSchedulerAccess.GetReservedDateTimes(equipmentId, date);
        }
        public List<LessonView> GetTraineeLessonList(DateTime startingTime, int traineeId, int equipmentId)
        {
            return fTDAndFlyingSchedulerAccess.GetTraineeLessonList(startingTime, traineeId, equipmentId);
        }

        public List<PhaseLessonView> GetTraineeLessonList(int equipmentId, int traineeId, DateTime ActualclickedDate)
        {
            return fTDAndFlyingSchedulerAccess.GetTraineeLessonList(equipmentId, traineeId, ActualclickedDate);
        }
        public string AddNewTraineeLessonSchedule(List<FlyingFTDSchedule> flyingFTDScheduleList, string[] traineeLessonIdArray, DateTime startingTime, bool isCustomBriefingTime, bool isCustomDebriefingTime, string briefingTimeId, string debriefingTimeId, string briefingStartingTime, string debriefingStartingTime, string rescheduledReasonId)
        {
            return fTDAndFlyingSchedulerAccess.AddNewTraineeLessonSchedule(flyingFTDScheduleList, traineeLessonIdArray, startingTime, isCustomBriefingTime, isCustomDebriefingTime, briefingTimeId, debriefingTimeId, briefingStartingTime, debriefingStartingTime, rescheduledReasonId);
        }
        public string UpdateTraineeLessonSchedule(FlyingFTDSchedule flyingFTDSchedule)
        {
            OperationResult operationResult = fTDAndFlyingSchedulerAccess.UpdateTraineeLessonSchedule(flyingFTDSchedule);
            return operationResult.Message;
        }
        public string UpdateTraineeLessonSchedule(DateTime LessonStartingTime, int flyingAndFTDScheduleId, int equipmentId, int instructorId, bool isReschedule, string briefingTimeId, string debriefingTimeId, bool isCustomBriefingTime, bool isCustomDebriefingTime, string briefingStartingTime, string debriefingStartingTime, string date)
        {
            OperationResult operationResult = fTDAndFlyingSchedulerAccess.UpdateTraineeLessonSchedule(LessonStartingTime, flyingAndFTDScheduleId, equipmentId, instructorId, isReschedule, briefingTimeId, debriefingTimeId, isCustomBriefingTime, isCustomDebriefingTime, briefingStartingTime, debriefingStartingTime, date);

            try
            {
                FlyingFTDSchedule oldFlyingFTDSchedule = fTDAndFlyingSchedulerAccess.Details(flyingAndFTDScheduleId);
                if (operationResult.IsSuccess)
                {
                    //Notify schedule change to (Email, SMS, Notification) concerned bodies
                    FlyingFTDSchedule newFlyingFTDSchedule = fTDAndFlyingSchedulerAccess.Details(flyingAndFTDScheduleId);
                    if (newFlyingFTDSchedule != null)
                    {
                        if (newFlyingFTDSchedule.IsNotified)
                        {
                            string phoneNumber = String.Empty;
                            string email = String.Empty;
                            string SMSMessage = "Schedlue for " + oldFlyingFTDSchedule.Lesson.LessonName + " has changed to " + oldFlyingFTDSchedule.ScheduleStartTime + " to " + oldFlyingFTDSchedule.ScheduleEndTime;
                            string EmailMessage = getHtmlMessageBody(oldFlyingFTDSchedule, newFlyingFTDSchedule);
                            EmailLogic emailLogic = new EmailLogic();
                            TraineeLogic traineeLogic = new TraineeLogic();
                            InstructorAccess instructorAccess = new InstructorAccess();
                            Trainee trainee = (Trainee)traineeLogic.Details(newFlyingFTDSchedule.TraineeId);
                            if (trainee != null)
                            {
                                phoneNumber = trainee.Person.Phone;
                                email = trainee.Person.Email;
                                if (!String.IsNullOrEmpty(phoneNumber))
                                {
                                    emailLogic.SendSMS(SMSMessage, phoneNumber);
                                }
                                if (!String.IsNullOrEmpty(email))
                                {
                                    emailLogic.SendEmail(EmailMessage, email, "Pilot School Schedule Schange", "");
                                }
                            }
                            //Notify Instructors
                            Instructor newInstructor = instructorAccess.Details(newFlyingFTDSchedule.InstructorId);
                            phoneNumber = newInstructor.Person.Phone;
                            email = newInstructor.Person.Email;
                            if (!String.IsNullOrEmpty(phoneNumber))
                            {
                                emailLogic.SendSMS(SMSMessage, phoneNumber);
                            }
                            if (!String.IsNullOrEmpty(email))
                            {
                                emailLogic.SendEmail(EmailMessage, email, "Pilot School Schedule Schange", "");
                            }
                            //if there is instructor change, notifiy both the deligate and the old one.
                            if (oldFlyingFTDSchedule.InstructorId != newFlyingFTDSchedule.InstructorId)
                            {
                                Instructor oldInstructor = instructorAccess.Details(oldFlyingFTDSchedule.InstructorId);
                                phoneNumber = oldInstructor.Person.Phone;
                                email = oldInstructor.Person.Email;
                                if (!String.IsNullOrEmpty(phoneNumber))
                                {
                                    emailLogic.SendSMS(SMSMessage, phoneNumber);
                                }
                                if (!String.IsNullOrEmpty(email))
                                {
                                    emailLogic.SendEmail(EmailMessage, email, "Pilot School Schedule Schange", "");
                                }
                            }
                        }
                    }
                }
                return operationResult.Message;
            }
            catch (Exception)
            {
                return operationResult.Message;
            }
        }

        public static string getHtmlMessageBody(FlyingFTDSchedule oldFlyingFTDSchedule, FlyingFTDSchedule newFlyingFTDSchedule)
        {
            try
            {
                string messageBody = "<font><b>There is schedule change:</b> </font><br><br>";

                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style =\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style =\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";

                messageBody += htmlTableStart;
                //Construct Header
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "No." + htmlTdEnd;
                messageBody += htmlTdStart + "Info" + htmlTdEnd;
                messageBody += htmlTdStart + "Old Schedule" + htmlTdEnd;
                messageBody += htmlTdStart + "New Schedule" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                //Contstuct table body               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "1. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Trainee Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldFlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + oldFlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Trainee.Person.FirstName.Substring(0, 3) + " " + newFlyingFTDSchedule.Trainee.Person.MiddleName.Substring(0, 1) + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                //       
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "2. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Instructor Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldFlyingFTDSchedule.Instructor.Person.FirstName.Substring(0, 3) + " " + oldFlyingFTDSchedule.Instructor.Person.MiddleName.Substring(0, 1) + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Instructor.Person.FirstName.Substring(0, 3) + " " + newFlyingFTDSchedule.Instructor.Person.MiddleName.Substring(0, 1) + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //       
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "3. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Equipment" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldFlyingFTDSchedule.Equipment.NameOrSerialNo + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Equipment.NameOrSerialNo + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "4. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Location" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldFlyingFTDSchedule.Equipment.Location.LocationName + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Equipment.Location.LocationName + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "5. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Room No" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldFlyingFTDSchedule.Equipment.RoomNo + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Equipment.RoomNo + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "6. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Lesson Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldFlyingFTDSchedule.Lesson.LessonName + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Lesson.LessonName + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "7. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Duration" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldFlyingFTDSchedule.ScheduleStartTime + " - " + oldFlyingFTDSchedule.ScheduleEndTime + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.ScheduleStartTime + " - " + newFlyingFTDSchedule.ScheduleEndTime + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "8. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Status" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Replaced" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newFlyingFTDSchedule.Status + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //
                messageBody = messageBody + htmlTableEnd;


                return messageBody;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string MergeBatchClass(int moduleScheduleId, string batchClassList, int moduleId)
        {
            return schedulerAccess.GetCourseModule(moduleScheduleId, batchClassList, moduleId);
        }

        public List<BatchView> GetValidBatchClassToBeMerged(int moduleScheduleId)
        {
            return schedulerAccess.GetValidBatchClassToBeMerged(moduleScheduleId);
        }


        #endregion

        #region FTD and Flying Scheduling Validation
        public bool IsInBetweenEquipmentWorkingHour(FlyingFTDSchedule schedule)
        {
            return fTDAndFlyingSchedulerAccess.IsInBetweenEquipmentWorkingHour(schedule);
        }
        public bool IsInBetweenEquipmentDowntime(FlyingFTDSchedule schedule)
        {
            return fTDAndFlyingSchedulerAccess.IsInBetweenEquipmentDowntime(schedule);
        }
        public bool IsInBetweenRecurringDowntimeSchedule(FlyingFTDSchedule schedule)
        {
            return fTDAndFlyingSchedulerAccess.IsInBetweenRecurringDowntimeSchedule(schedule);
        }
        public FlyingFTDSchedule Details(int id)
        {
            return fTDAndFlyingSchedulerAccess.Details(id);
        }
        public bool IsEquipmentFree(FlyingFTDSchedule schedule)
        {
            return fTDAndFlyingSchedulerAccess.IsEquipmentFree(schedule);
        }
        public List<EquipmentDowntimeSchedule> GetEquipmentDowntimeSchedule(int equipmentId, DateTime date)
        {
            return fTDAndFlyingSchedulerAccess.GetEquipmentDowntimeSchedule(equipmentId, date);
        }
        public List<FTDRecurringDownTime> GetEquipmentRecurringDowntimeSchedule(int equipmentId, DateTime date)
        {
            return fTDAndFlyingSchedulerAccess.GetEquipmentRecurringDowntimeSchedule(equipmentId, date);
        }

        public bool CancelSchedule(string reason, string remark, string flyingAndFTDScheduleId)
        {
            return fTDAndFlyingSchedulerAccess.CancelSchedule(reason, remark, flyingAndFTDScheduleId);
        }

        public BriefingAndDebriefingView GetBriefingAndDebriefing(DateTime startingTime, int instructorId, int equipmentId, string[] traineeLesson)
        {
            return fTDAndFlyingSchedulerAccess.GetBriefingAndDebriefing(startingTime, instructorId, equipmentId, traineeLesson);
        }
        public string GetLeaveAndLicenceExpiry(DateTime startingTime, int instructorId, string[] traineeLesson)
        {
            return fTDAndFlyingSchedulerAccess.GetLeaveAndLicenceExpiry(startingTime, instructorId, traineeLesson);
        }
        public object IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTime(string EventApplyingType, string date,DateTime startingTime, int instructorId, string[] traineeLessonIdArray, int equipmentId, string briefingtime, string debriefingTime, int fTDAndFlyingScheduleId, bool isReschedule, bool isCustomBriefingTime, bool isCustomDebriengTime, string briefingStartingTime, string debriefingStartTime)
        {
            return fTDAndFlyingSchedulerAccess.IsTraineeInstructorAndEquipmentFeeBriefingAndDebriefingTime(EventApplyingType, date,startingTime, instructorId, traineeLessonIdArray, equipmentId, briefingtime, debriefingTime, fTDAndFlyingScheduleId, isReschedule, isCustomBriefingTime, isCustomDebriengTime, briefingStartingTime, debriefingStartTime);
        }

        public object IsTraineeInstructorAndEquipmentFee(DateTime startingDateTime, DateTime endingDateTime, int flyingAndFTDScheduleId, int equipmentId)
        {
            return fTDAndFlyingSchedulerAccess.IsTraineeInstructorAndEquipmentFee(startingDateTime, endingDateTime, flyingAndFTDScheduleId, equipmentId);
        }

        public bool IsTraineeFree(FlyingFTDSchedule schedule)
        {
            return fTDAndFlyingSchedulerAccess.IsTraineeFree(schedule);
        }

        public bool IsInBetweenEquipmentMaintainanceTime(FlyingFTDSchedule schedule)
        {
            return fTDAndFlyingSchedulerAccess.IsInBetweenEquipmentMaintainanceTime(schedule);
        }
        #endregion
    }
}
