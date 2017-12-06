namespace PTSMSDAL.Models.Report
{
    public class AircraftFlownReport
    {
        public string EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string PlannedTime { get; set; }
        public string FlownTime { get; set; }
        public string CanceledTime { get; set; }
        public string Status { get; set; }
    }
}
