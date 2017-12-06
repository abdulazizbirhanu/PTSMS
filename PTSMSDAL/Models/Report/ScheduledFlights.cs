namespace PTSMSDAL.Models.Report
{
    public class ScheduledFlights
    {
        public string trFname { get; set; }
        public string trLname { get; set; }
        public string traineeID { get; set; }
        public string instructorID { get; set; }
        public string inFname { get; set; }
        public string inLname { get; set; }
        public string LessonName { get; set; }
        public string NameOrSerialNo { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public string totalTime { get; set; }
        public string FlownTime { get; set; }
        public string CanceledTime { get; set; }
        public string BatchClassName { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string licenseType { get; set; }
        public string licenseDueDate { get; set; }
        public string equipmentStatus { get; set; }
    }
}
