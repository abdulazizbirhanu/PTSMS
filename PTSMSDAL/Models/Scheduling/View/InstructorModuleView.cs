using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.View
{
    public class InstructorModuleView
    {
        public int ModuleInstructorScheduleId { get; set; }
        public int InstructorId { get; set; }
        public int ModuleId { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual Module Module { get; set; }
    }
}
