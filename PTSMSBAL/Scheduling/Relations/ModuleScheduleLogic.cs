using PTSMSBAL.Logic.Enrollment.Operations;
using PTSMSBAL.Scheduling.Others;
using PTSMSBAL.Utility;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Access.Enrollment.Relations;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PTSMSBAL.Logic.Scheduling.Relations
{
    public class ModuleScheduleLogic
    {
        ModuleScheduleAccess moduleScheduleAccess = new ModuleScheduleAccess();

        public object List()
        {
            return moduleScheduleAccess.List();
        }

        public object Details(int id)
        {
            return moduleScheduleAccess.Details(id);
        }

        public object Add(ModuleSchedule moduleSchedule)
        {
            return moduleScheduleAccess.Add(moduleSchedule);
        }
        public bool UpdateModuleSchedule(int moduleScheduleId, int periodId, int classRoomId, string date, int instructorId)
        {
            ModuleSchedule oldMduleSchedule = (ModuleSchedule)moduleScheduleAccess.Details(moduleScheduleId);
            bool isScheduleChanged = moduleScheduleAccess.UpdateModuleSchedule(moduleScheduleId, periodId, classRoomId, date, instructorId);
            if (isScheduleChanged)
            {
                //Notify schedule change to (Email, SMS, Notification) concerned bodies
                ModuleSchedule newModuleSchedule = (ModuleSchedule)moduleScheduleAccess.Details(moduleScheduleId);
                if (newModuleSchedule != null)
                {
                    string phoneNumber = String.Empty;
                    string email = String.Empty;
                    string SMSMessage = "Schedlue for " + newModuleSchedule.Module.ModuleCode + " has changed to " + Convert.ToDateTime(Convert.ToDateTime(newModuleSchedule.Date.Add(TimeSpan.Parse(newModuleSchedule.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(newModuleSchedule.Date.Add(TimeSpan.Parse(newModuleSchedule.Period.EndTime))).ToString("MM/dd/yyyy HH:mm"));
                    string EmailMessage = getHtmlMessageBody(oldMduleSchedule, newModuleSchedule);
                    EmailLogic emailLogicAccess = new EmailLogic();
                    TraineeBatchClassAccess traineeBatchClassAccess = new TraineeBatchClassAccess();
                    InstructorAccess instructorAccess = new InstructorAccess();
                    List<TraineeBatchClass> traineeBatchClassList = traineeBatchClassAccess.TraineeBatchClassList(newModuleSchedule.PhaseSchedule.BatchId);
                    foreach (var traineeBatchClass in traineeBatchClassList)
                    {
                        phoneNumber = traineeBatchClass.Trainee.Person.Phone;
                        email = traineeBatchClass.Trainee.Person.Email;
                        if (!String.IsNullOrEmpty(phoneNumber))
                        {
                            emailLogicAccess.SendSMS(SMSMessage, phoneNumber);
                        }
                        if (!String.IsNullOrEmpty(email))
                        {
                            emailLogicAccess.SendEmail(EmailMessage, email, "Pilot School Schedule Schange", "");
                        }
                    }
                    //Notify Instructors
                    Instructor newInstructor = instructorAccess.Details(newModuleSchedule.InstructorId);
                    phoneNumber = newInstructor.Person.Phone;
                    email = newInstructor.Person.Email;
                    if (!String.IsNullOrEmpty(phoneNumber))
                    {
                        emailLogicAccess.SendSMS(SMSMessage, phoneNumber);
                    }
                    if (!String.IsNullOrEmpty(email))
                    {
                        emailLogicAccess.SendEmail(EmailMessage, email, "Pilot School Schedule Schange", "");
                    }
                    //if there is instructor change, notifiy both the deligate and the old one.
                    if (oldMduleSchedule.InstructorId != newModuleSchedule.InstructorId)
                    {
                        Instructor oldInstructor = instructorAccess.Details(oldMduleSchedule.InstructorId);
                        phoneNumber = oldInstructor.Person.Phone;
                        email = oldInstructor.Person.Email;
                        if (!String.IsNullOrEmpty(phoneNumber))
                        {
                            emailLogicAccess.SendSMS(SMSMessage, phoneNumber);
                        }
                        if (!String.IsNullOrEmpty(email))
                        {
                            emailLogicAccess.SendEmail(EmailMessage, email, "Pilot School Schedule Schange", "");
                        }
                    }
                }
            }
            return isScheduleChanged;
        }

        public static string getHtmlMessageBody(ModuleSchedule oldMduleSchedule, ModuleSchedule newModuleSchedule)
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
                messageBody = messageBody + htmlTdStart + "Instructor Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldMduleSchedule.Instructor.Person.FirstName.Substring(0, 3) + " " + oldMduleSchedule.Instructor.Person.MiddleName.Substring(0, 1) + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newModuleSchedule.Instructor.Person.FirstName.Substring(0, 3) + " " + newModuleSchedule.Instructor.Person.MiddleName.Substring(0, 1) + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "2. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Batch Class Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldMduleSchedule.PhaseSchedule.Batch.BatchName + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newModuleSchedule.PhaseSchedule.Batch.BatchName + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "3. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Building Name" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldMduleSchedule.ClassRoom.Building.BuildingName + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newModuleSchedule.ClassRoom.Building.BuildingName + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "4. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Room No" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldMduleSchedule.ClassRoom.RoomNo + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newModuleSchedule.ClassRoom.RoomNo + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "5. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Module Code" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldMduleSchedule.Module.ModuleCode + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newModuleSchedule.Module.ModuleCode + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "6. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Module title" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldMduleSchedule.Module.ModuleCode + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newModuleSchedule.Module.ModuleCode + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "7. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Period" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + oldMduleSchedule.Period.PeriodName + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + newModuleSchedule.Period.PeriodName + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                //               
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "8. " + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Scheduled Date" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + Convert.ToDateTime(Convert.ToDateTime(oldMduleSchedule.Date.Add(TimeSpan.Parse(oldMduleSchedule.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(oldMduleSchedule.Date.Add(TimeSpan.Parse(oldMduleSchedule.Period.EndTime))).ToString("MM/dd/yyyy HH:mm")) + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + Convert.ToDateTime(Convert.ToDateTime(newModuleSchedule.Date.Add(TimeSpan.Parse(newModuleSchedule.Period.StartTime))).ToString("MM/dd/yyyy HH:mm")) + " to " + Convert.ToDateTime(Convert.ToDateTime(newModuleSchedule.Date.Add(TimeSpan.Parse(newModuleSchedule.Period.EndTime))).ToString("MM/dd/yyyy HH:mm")) + htmlTdEnd;
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

        public List<FreePeriodList> AddModuleSchedule(int phaseScheduleId, int ModuleId, string date, int noOfDays, int periodId, int classRoomId, int instructorId)
        {
            SchedulerUtility schedulerUtility = new SchedulerUtility();
            List<FreePeriodList> freePeriodList = new List<FreePeriodList>();
            var dateTime = Convert.ToDateTime(date + " 12:00:00");
            List<FreePeriodList> periodList = schedulerUtility.get_FreeTimeSlotAndRoomForSpecificDate(dateTime, instructorId, phaseScheduleId, ModuleId, noOfDays);
            foreach (var item in periodList)
            {
                if (item.Message == "OK")
                {
                    //ModuleInstructorScheduleAccess moduleInstructorScheduleAccess = new ModuleInstructorScheduleAccess();
                    ModuleSchedule moduleSchedule = new ModuleSchedule();
                    moduleSchedule.PhaseScheduleId = phaseScheduleId;
                    moduleSchedule.PeriodId = periodId;
                    moduleSchedule.Date = DateTime.ParseExact(dateTime.ToString("MM/dd/yyyy") + " 12:00:00", "MM/dd/yyyy hh:mm:ss", CultureInfo.InstalledUICulture);
                    moduleSchedule.ClassRoomId = classRoomId;
                    //ModuleInstructorSchedule moduleInstructorSchedule = moduleInstructorScheduleAccess.GetInstructorSchedule(ModuleId, instructorId);
                    //moduleSchedule.ModuleInstructorScheduleId = moduleInstructorSchedule.ModuleInstructorScheduleId;

                    moduleSchedule.ModuleId = ModuleId;
                    moduleSchedule.InstructorId = instructorId;
                    moduleSchedule.Status = SchedulerAccess.GetModuleScheduleStatusName((int)ModuleScheduleStatus.New);
                    moduleSchedule.StartDate = DateTime.Now;
                    moduleSchedule.EndDate = new DateTime(9999, 12, 31);
                    moduleSchedule.CreationDate = DateTime.Now;
                    moduleSchedule.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    moduleSchedule.RevisionDate = DateTime.Now;
                    moduleSchedule.RevisedBy = System.Web.HttpContext.Current.User.Identity.Name;

                    bool result = (bool)moduleScheduleAccess.Add(moduleSchedule);
                    if (result)
                        item.Message = "OK";
                    else
                        item.Message = "Not OK";
                    //freePeriodList.Add(item);
                    //item.Message = result;
                }
                freePeriodList.Add(item);
                dateTime = dateTime.AddDays(1);
            }
            return freePeriodList;
            //return (bool)moduleScheduleAccess.Add(moduleSchedule);
        }
        public object Revise(ModuleSchedule moduleSchedule)
        {
            return moduleScheduleAccess.Revise(moduleSchedule);
        }

        public object Delete(int id)
        {
            return moduleScheduleAccess.Delete(id);
        }
    }
}