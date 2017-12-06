using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSDAL.Models.Scheduling.View
{
    public class BatchClasses
    {
        public BatchClasses()
        {
            this.PhaseList = new List<PhaseView>();
            this.LocationList = new List<LocationView>();
        }
        public BactchClassView BactchClass { get; set; }
        public List<PhaseView> PhaseList { get; set; }//
        public List<LocationView> LocationList { get; set; }
    }

    public class Phases
    {
        public Phases()
        {
            this.Courses = new List<Courses>();
        }
        public PhaseView Phase { get; set; }
        public List<Courses> Courses { get; set; }
    }
    public class Courses
    {
        public Courses()
        {
            this.Modules = new List<ModuleView>();
        }
        public CourseView Course { get; set; }
        public List<ModuleView> Modules { get; set; }
    }
    public class Lessons
    {        
        public int Id { get; set; }
        public string Name{ get; set; }
        public int Sequence { get; set; }
    }
    /*End of Custorm Object*/
    public class BactchClassView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CourseView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
    }
    public class ModuleView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
    }
    public class LocationView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class PhaseView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isScheduled { get; set; }
        public int PhaseSequence { get; set; }
    }
    /*End of Custorm Object*/
}
