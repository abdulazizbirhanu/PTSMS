using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Report
{
    public class EqipmentReportView
    {
        [Display(Name = "NameOrSerialNo")]
        public string NameOrSerialNo { get; set; }
        [Display(Name = "stringEquipmentType")]
        public string stringEquipmentType { get; set; }
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "IsMultiEngine")]
        public string IsMultiEngine { get; set; }
        [Display(Name = "Planned")]
        public string Planned { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        [Display(Name = "Actual")]
        public string Actual { get; set; }
        [Display(Name = "CountFlg")]
        public string CountFlg { get; set; }
    }
}
