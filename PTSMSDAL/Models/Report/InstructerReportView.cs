using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PTSMSDAL.Models.Report
{
    public class InstructerReportView
    {
        [Display(Name = "Date")]
        public string Date { get; set; }
        [Display(Name = "InstructorId")]
        public string InstructorId { get; set; }
        [Display(Name = "InstructorName")]
        public string InstructorName { get; set; }
        [Display(Name = "WorkingHour")]
        public string WorkingHour { get; set; }
    }
}
