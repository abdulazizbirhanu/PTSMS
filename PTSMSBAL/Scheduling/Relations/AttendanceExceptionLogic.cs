using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Context;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Scheduling.Relations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.Relations
{
    public class AttendanceExceptionLogic
    {
        AttendanceExceptionAccess attendanceExceptionAccess = new AttendanceExceptionAccess();
        ModuleActivityLogAccess moduleActivityLogAccess = new ModuleActivityLogAccess();
        public bool Add(int moduleScheduleId, string takenDate, int instructorId, int classRoomId, string trainees, string Note, string moduleActivities)
        {

            string[] takenDateArray = takenDate.Split('-');
            List<string> traineeList = new List<string>();
            if (!String.IsNullOrEmpty(trainees))
            {
                string[] traineeArray = trainees.Split(',');
                traineeList = traineeArray.ToList();
            }
            DateTime startAt = Convert.ToDateTime(takenDateArray[0]);
            DateTime endAt = Convert.ToDateTime(takenDateArray[1]);

            TimeSpan startTime = startAt.TimeOfDay;
            TimeSpan endTime = endAt.TimeOfDay;
            ActualModuleTaken actualModuleTaken = new ActualModuleTaken
            {
                InstructorId = instructorId,
                ClassRoomId = classRoomId,
                ModuleScheduleId = moduleScheduleId,
                TakenDate = DateTime.ParseExact(startAt.ToString("MM/dd/yyyy") + " 12:00:00", "MM/dd/yyyy hh:mm:ss", CultureInfo.InstalledUICulture),
                StartTime = startTime,
                EndTime = endTime
            };
            //Save Taken Module Activities.
            PTSContext db = new PTSContext();
            string[] takenModuleActivitiesArray = moduleActivities.Split(',');

            foreach (var moduleActivityId in takenModuleActivitiesArray)
            {
                if (!string.IsNullOrEmpty(moduleActivityId))
                {
                    int id = Int16.Parse(moduleActivityId);
                    ModuleActivity moduleActivity = db.ModuleActivitys.Find(id);

                    if (moduleActivity != null)
                    {
                        moduleActivityLogAccess.Add(new ModuleActivityLog
                        {
                            ModuleActivityId = moduleActivity.ModuleActivityId,
                            ModuleActivityName = moduleActivity.ModuleActivityName,
                            EstimatedDuration = moduleActivity.EstimatedDuration,
                            ModuleScheduleId = moduleScheduleId
                        });
                    }
                }
            }
            return attendanceExceptionAccess.Add(actualModuleTaken, traineeList.ToArray(), Note, moduleScheduleId);
        }
    }
}
