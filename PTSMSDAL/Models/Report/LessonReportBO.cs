using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Report
{
    public class LessonReportBO
    {
        public string trFname { get; set; }
        public string trLname { get; set; }
        public string inFname { get; set; }
        public string inLname { get; set; }
        public string LessonName { get; set; }
        public string NameOrSerialNo { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public string BatchClassName { get; set; }
        public string Status { get; set; }
    }
}
