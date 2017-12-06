using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PTSMSDAL.Access.Others;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.References;
using PTSMSDAL.Models.Curriculum.Relations;
using PTSMSDAL.Models.Dispatch;
using PTSMSDAL.Models.Dispatch.Master;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.References;
using PTSMSDAL.Models.Enrollment.Relations;
using PTSMSDAL.Models.Others.Messaging;
using PTSMSDAL.Models.Others.School;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.References;
using PTSMSDAL.Models.Scheduling.Relations;

namespace PTSMSDAL.Context
{
    public class PTSContext : DbContext
    {
        private static SchoolAccess schoolAccess = new SchoolAccess();
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public PTSContext()
            : base("name=PTSContext")//GetConnectionString()
        {
        }

        public static string GetConnectionString()
        {
            //if (System.Web.HttpContext.Current.User != null)
            //{
            //    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
            //    if (identity.FindFirst("SchoolCode") != null)
            //    {
            //        string newSchoolCode = identity.FindFirst("SchoolCode").Value;
            //        if (!string.IsNullOrEmpty(newSchoolCode))
            //        {
            //            School school = schoolAccess.SchoolDetails(newSchoolCode);
            //            if (school != null)
            //            {
            //                System.Web.HttpContext.Current.Session["SchoolName"] = school.SchoolName;
            //                System.Web.HttpContext.Current.Session["SchoolCode"] = school.SchoolCode;
            //                return "Data Source=" + school.Server + ";Initial Catalog=" + school.DatabaseName + ";User ID=" + school.Username + ";Password=" + school.Password + "";
            //            }
            //        }
            //        else
            //        {
            //            return "name=PTSContext";
            //        }
            //    }
            //}
            //return "name=PTSContext";


            string enteredSchoolCode = System.Web.HttpContext.Current.Request.Form["SchoolCode"];
            //////////////
            //ApplicationUserManager UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //PTSUser requestingUser = new PTSUser(Thread.CurrentPrincipal.Identity.Name);
            //////////////////
            if (System.Web.HttpContext.Current.Session != null)
            {
                if (System.Web.HttpContext.Current.Session["SchoolCode"] != null)
                {
                    string schoolCode = System.Web.HttpContext.Current.Session["SchoolCode"].ToString();
                    School school = schoolAccess.SchoolDetails(schoolCode);
                    if (school != null)
                    {
                        System.Web.HttpContext.Current.Session["SchoolName"] = school.SchoolName;
                        System.Web.HttpContext.Current.Session["SchoolCode"] = school.SchoolCode;
                        return "Data Source=" + school.Server + ";Initial Catalog=" + school.DatabaseName + ";User ID=" + school.Username + ";Password=" + school.Password + "";
                    }
                }
            }

            if (!string.IsNullOrEmpty(enteredSchoolCode))
            {
                School school = schoolAccess.SchoolDetails(enteredSchoolCode);
                if (school != null)
                {
                    //System.Web.HttpContext.Current.Session["SchoolCode"] = enteredSchoolCode;
                    return "Data Source=" + school.Server + ";Initial Catalog=" + school.DatabaseName + ";User ID=" + school.Username + ";Password=" + school.Password + "";
                }
            }

            return "name=PTSContext";
        }
        public static PTSContext Create()
        {
            return new PTSContext();
        }
        public DbSet<Phase> Phases { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }

        public DbSet<CourseExam> CourseExams { get; set; }
        public DbSet<CourseReference> CourseReferences { get; set; }
        public DbSet<LessonEvaluationTemplate> LessonEvaluationTemplates { get; set; }
        public DbSet<EvaluationTemplateCategory> LessonEvaluationCategories { get; set; }
        public DbSet<LessonReference> LessonReferences { get; set; }
        public DbSet<ModuleExam> ModuleExams { get; set; }
        public DbSet<ModuleGroundLesson> ModuleGroundLessons { get; set; }
        public DbSet<ModuleReference> ModuleReferences { get; set; }
        public DbSet<ProgramCategory> ProgramCategories { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<LessonCategory> LessonCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<EvaluationCategory> EvaluationCategories { get; set; }
        public DbSet<EvaluationItem> EvaluationItems { get; set; }
        public DbSet<EvaluationTemplate> EvaluationTemplates { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<GroundLesson> GroundLessons { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Prerequisite> Prerequisites { get; set; }
        public DbSet<Reference> References { get; set; }

        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<PersonDocument> PersonDocuments { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<BatchClass> BatchClasses { get; set; }

        public DbSet<DocumentType> Documents { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<BatchCategory> BatchCategories { get; set; }
        public DbSet<BatchCourse> BatchCourses { get; set; }
        public DbSet<BatchLesson> BatchLessons { get; set; }
        public DbSet<BatchModule> BatchModules { get; set; }
        public DbSet<BatchCourseExam> BatchCourseExams { get; set; }
        public DbSet<BatchModuleExam> BatchModuleExams { get; set; }
        public DbSet<TraineeEvaluationCategory> TraineeEvaluationCategories { get; set; }
        public DbSet<TraineeEvaluationItem> TraineeEvaluationItems { get; set; }
        public DbSet<TraineeBatchClass> TraineeBatchClasses { get; set; }//////////////////

        public DbSet<TraineeCourse> TraineeCourses { get; set; }
        public DbSet<TraineeCourseExam> TraineeCourseExams { get; set; }
        public DbSet<TraineeLesson> TraineeLessons { get; set; }
        public DbSet<TraineeModule> TraineeModules { get; set; }
        public DbSet<TraineeModuleExam> TraineeModuleExams { get; set; }

        public DbSet<EquipmentMaintenance> EquipmentMaintenances { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<DayTemplate> DayTemplates { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<PeriodTemplate> PeriodTemplates { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<AttendanceException> AttendanceExceptions { get; set; }
        public DbSet<ActualModuleTaken> ActualModuleTakens { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Location> Locations { get; set; }
        //TraineePrerequisite
        public DbSet<BatchCoursePrerequisite> BatchPrerequisites { get; set; }
        public DbSet<PhaseSchedule> PhaseSchedules { get; set; }
        //public DbSet<InstructorSchedule> InstructorSchedules { get; set; }
        public DbSet<ModuleSchedule> ModuleSchedules { get; set; }
        public DbSet<ModuleToolSchedule> ModuleToolSchedules { get; set; }

        public DbSet<PersonLeave> PersonLeaves { get; set; }

        //Scheduling ModuleInstructorSchedule
        public DbSet<ModuleInstructorSchedule> ModuleInstructorSchedules { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<InstructorQualification> InstructorQualifications { get; set; }
        public DbSet<QualificationType> QualificationTypes { get; set; }
        // FTD/GF 

        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<EquipmentModel> EquipmentModels { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<BatchEquipmentModel> BatchEquipmentModels { get; set; }
        public DbSet<EquipmentDowntimeSchedule> EquipmentDowntimeSchedules { get; set; }
        public DbSet<EquipmentCertificate> EquipmentCertificates { get; set; }
        public DbSet<FTDRecurringDownTime> FTDRecurringDownTimes { get; set; }
        public DbSet<EquipmentScheduleFactor> EquipmentScheduleFactors { get; set; }
        public DbSet<LessonCategoryType> LessonCategoryTypes { get; set; }
        public DbSet<InstructorEquipmentModel> InstructorEquipmentModels { get; set; }

        public DbSet<FlyingFTDSchedule> FlyingFTDSchedules { get; set; }//
        public DbSet<EquipmentStatus> EquipmentStatuss { get; set; }
        public DbSet<DowntimeReason> DowntimeReasons { get; set; }
        public DbSet<LessonScore> LessonScores { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<CertificateType> CertificateTypes { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<FlightLog> FlightLogs { get; set; }
        //public DbSet<FlightLogNew> FlightLogNews { get; set; }

        public DbSet<BriefingAndDebriefing> BriefingAndDebriefings { get; set; }
        public DbSet<EquipmentScheduleBriefingDebriefing> EquipmentScheduleBriefingDebriefings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }


        ///////////////////////////////////start, DISPATCHER//////////////////////////////
        public DbSet<CheckInStatus> CheckInStatuss { get; set; }
        public DbSet<DepartureTimeReason> DepartureTimeReasons { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<ActivityAuthorization> ActivityAuthorizations { get; set; }
        public DbSet<ActivityCheckIn> ActivityCheckIns { get; set; }
        public DbSet<ActivityRampIn> ActivityRampIns { get; set; }
        public DbSet<ActivityRampOut> ActivityRampOuts { get; set; }
        public DbSet<InstrumentApproach> InstrumentApproachs { get; set; }
        public DbSet<OverallGrade> OverallGrades { get; set; }

        public DbSet<OverallGradeUpdateRequest> OverallGradeUpdateRequests { get; set; }
        public DbSet<OperationArea> OperationAreas { get; set; }
        public DbSet<ArrivalTimeReason> ArrivalTimeReasons { get; set; }
        public DbSet<ModuleActivity> ModuleActivitys { get; set; }
        public DbSet<ModuleActivityLog> ModuleActivityLogs { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<RescheduleReason> RescheduleReasons { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PassFailExamResult> PassFailExamResults { get; set; }
        public virtual DbSet EqipmentReportView { get; set; }
        ///////////////////////////////////end, DISPATCHER////////////////////////////////


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("ModelBuilder is NULL");
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

    }
}
