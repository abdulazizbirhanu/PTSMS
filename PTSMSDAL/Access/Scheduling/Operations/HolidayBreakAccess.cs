using PTSMSDAL.Context;
using PTSMSDAL.Models.Scheduling.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Access.Scheduling.Operations
{
    public class HolidayBreakAccess
    {
        public List<Holiday> List()
        {
            PTSContext dbContext = new PTSContext();
            return dbContext.Holidays.Where(h => h.StartDateTime > DateTime.Now).ToList();
        }
    }
}
