using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.View
{
    public class TraineeLessonScheduleView
    {
        public double TraineeLessonScheduleId { get; set; }
        public int LessonId { get; set; }
        public int TraineeId { get; set; }       
        public int BatchId { get; set; }
        public int BatchClassId { get; set; }
        public DateTime StartingDate { get; set; }
        public int PhaseScheduleId { get; set; }
        public int PhaseId { get; set; }
        public int LessonSequence { get; set; }      
    }
    public class EquipmentView
    {
        public int EquipmentId { get; set; }
    }
}
