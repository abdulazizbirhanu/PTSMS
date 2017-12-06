using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Enrollment.View
{
    public class BatchTraineeView
    {
        public int TraineeId { get; set; }
        public int BatchId { get; set; }
        public int BatchTraineeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyId { get; set; }
        public int BatchClassId { get; set; }
    }
}
