
using System;
using System.Collections.Generic;
using System.Linq;
using GAF;
using GAF.Extensions;
using GAF.Operators;
using PTSMSDAL.Access.Scheduling;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Access.Enrollment.Operations;
using PTSMSDAL.Models.Enrollment.Operations;
using PTSMSDAL.Models.Scheduling.View;
using System.Configuration;


namespace PTSMSBAL.Scheduling.Operations
{
    public class SchedulerLogic
    {
        SchedulerAccess schedulerAccess = new SchedulerAccess();
        ClassRoomAccess classRoomAccess = new ClassRoomAccess();

        #region GLOBAL Variables that hold Genes

        private static List<BatchModuleView> BatchModuleList = new List<BatchModuleView>();
        private static List<ClassRoom> ClassRoomList = new List<ClassRoom>();
        private static List<InstructorView> InstructorList = new List<InstructorView>();
        private static List<string> TimeSlotList = new List<string>();
        #endregion

        //population size
        private static int POPSize = 30;
        //geneotype 
        private static int total_NoOfDays = 120;
        private DateTime MinimumCourseStartingDate = new DateTime();

        #region Constructor
        public SchedulerLogic()
        {
            this.get_BatchModuleList();
            this.get_MinimumStartingDate();
            this.get_ClassRoomList();
            this.get_InstructorList();
            //get the input data
            this.get_UnionOfWorkingDays();
            this.get_UnionOfWorkingPeriods();
            this.construct_TimeSlots();
            geneArray = new Gene[TimeSlotList.Count(), ClassRoomList.Count(), InstructorList.Count];
            GeneArrayDouble = new double[TimeSlotList.Count(), ClassRoomList.Count(), InstructorList.Count];
        }
        #endregion

        private static List<string> Days = new List<string>();//{ "Mon", "Tue", "Wed", "Thu", "Fri" };
        private static List<PeriodView> TimeSlotsPerADay = new List<PeriodView>();//{ "08:00-10:00", "10:00-12:00", "14:00-16:00", "16:00-17:00" };

        //Mutation rate, change it have a play
        private double mutRate = 0.02;
        //Recomination rate
        private double crossOverRate = 0.8;
        //How many tournaments should be played
        private static long Tournament = 1000;

        private Gene[,,] geneArray = null;

        private static double[,,] GeneArrayDouble = null;

        private static List<double> fitnessList = new List<double>();
        Population population = new Population();

        public object TimeTableScheduler()
        {
            try
            {
                //Populate Initial Population
                initialize_Population();

                //create the elite operator
                var elite = new Elite(5);

                //create the crossover operator
                var crossover = new Crossover(crossOverRate, true)
                {
                    CrossoverType = CrossoverType.SinglePoint
                };

                //create the mutation operator
                var mutate = new SwapMutate(mutRate);

                //create the GA
                var ga = new GeneticAlgorithm(population, CalculateFitness);

                //hook up to some useful events
                ga.OnGenerationComplete += ga_OnGenerationComplete;
                ga.OnRunComplete += ga_OnRunComplete;

                //add the operators
                ga.Operators.Add(elite);
                ga.Operators.Add(crossover);
                ga.Operators.Add(mutate);

                //run the GA
                ga.Run(Terminate);
            }
            catch (Exception ex)
            {

            }
            return new object();
        }

        private void initialize_Population()
        {
            Random random = new Random();
            int selectedIndex = -1, index = 0;
            for (int i = 0; i < POPSize; i++)
            {
                //Put it into queue in order to achieve Round Robin Scheduling
                Queue<List<BatchModuleView>> queue = new Queue<List<BatchModuleView>>();
                //Group module by Course Id
                var BatchModuleGroup = BatchModuleList.GroupBy(x => new { x.CourseId }).Select(grp => grp.ToList()).ToList();
                //////////////////////////            
                foreach (List<BatchModuleView> batchModuleGroupItem in BatchModuleGroup)
                {
                    queue.Enqueue(batchModuleGroupItem);
                }
                //Initialy, the instructor we are going to assign should be in the list of potential instructor list of the module
                var chromosome = new Chromosome();

                for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                {
                    int numberOfAssignedInstructor = 0;//Monitor assigned instructor at a specific DAY and TIME(Period)
                    for (int roomIndex = 0; roomIndex < ClassRoomList.Count(); roomIndex++)
                    {
                        //Assign Instructor As long as there is unassigned instrutor at a specific DATE and TIME(Period)
                        //If there is no instructor, the class room remain unassigned in that specific Date and Time(Period)

                        if (numberOfAssignedInstructor < InstructorList.Count)
                        {
                            //Make unAssigned Slot for all slots found before the Assigned instructor
                            for (int instructorIndex = 0; instructorIndex < numberOfAssignedInstructor; instructorIndex++)
                            {
                                chromosome.Genes.Add(new Gene(0));
                            }
                            //Make assigned slot when there is instuctor
                            for (int instructorIndex = numberOfAssignedInstructor; (instructorIndex < (numberOfAssignedInstructor + 1)); instructorIndex++)
                            {
                                if (queue.Count() > 0)
                                {
                                    //When Trying to assign 
                                    List<BatchModuleView> currentDequeueGroup = queue.Dequeue();
                                    BatchModuleView batchModuleGroupToBeAssigned = null;
                                    List<BatchModuleView> newBatchModule = new List<BatchModuleView>();

                                    selectedIndex = random.Next(0, currentDequeueGroup.Count); ;
                                    batchModuleGroupToBeAssigned = currentDequeueGroup[selectedIndex];
                                    index = 0;
                                    foreach (BatchModuleView batchModule in currentDequeueGroup)
                                    {
                                        if (selectedIndex != index++)
                                            newBatchModule.Add(batchModule);
                                    }
                                    if (newBatchModule.Count() > 0)
                                        queue.Enqueue(newBatchModule);
                                    chromosome.Genes.Add(new Gene(batchModuleGroupToBeAssigned.BatchModuleScheduleId));
                                }
                                else
                                {
                                    //Unassignede TimeSlot
                                    chromosome.Genes.Add(new Gene(0));
                                }
                            }
                            //Make unAssigned Slot for all slots found after the Assigned instructor
                            for (int instructorIndex = numberOfAssignedInstructor + 1; instructorIndex < InstructorList.Count; instructorIndex++)
                            {
                                chromosome.Genes.Add(new Gene(0));
                            }
                        }
                        else
                        {//if there is no available instructor, don't give any Module i.e. Chromosome 0  
                            for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                            {
                                chromosome.Genes.Add(new Gene(0));
                            }
                        }
                        numberOfAssignedInstructor++;
                    }
                }
                //var rnd = GAF.Threading.RandomProvider.GetThreadRandom();
                chromosome.Genes.ShuffleFast();
                population.Solutions.Add(chromosome);
            }
        }
        public static bool Terminate(Population population, int currentGeneration, long currentEvaluation)
        {
            Chromosome fittestChromosome = population.GetTop(1)[0];
            return currentGeneration > Tournament || fittestChromosome.Fitness >= 1.0;
        }
        private static void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            Chromosome fittestChromosome = e.Population.GetTop(1)[0];
            fitnessList.Add(fittestChromosome.Fitness);
        }
        static void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            try
            {
                SchedulerAccess schedulerAccess = new SchedulerAccess();

                Chromosome fittestChrom = e.Population.GetTop(1)[0];
                List<ModuleSchedule> moduleScheduleList = new List<ModuleSchedule>();
                DateTime assignedDate = new DateTime();
                string period = String.Empty;
                string ClassRoom = String.Empty;

                int index = 0;
                for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                {
                    for (int roomIndex = 0; roomIndex < ClassRoomList.Count(); roomIndex++)
                    {
                        for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                        {
                            if (fittestChrom.Genes != null)
                            {
                                if (fittestChrom.Genes[index].RealValue > 0.0)
                                {
                                    var BatchModuleView = BatchModuleList.Where(bm => bm.BatchModuleScheduleId == fittestChrom.Genes[index].RealValue).ToList();
                                    //index += 1;
                                    if (BatchModuleView.Count() > 0)
                                    {
                                        string timeSlot = TimeSlotList[timeSlotIndex];
                                        string[] timeSlotArray = timeSlot.Split(',');
                                        assignedDate = Convert.ToDateTime(timeSlotArray[1] + "," + timeSlotArray[2]);
                                        period = timeSlotArray[3];
                                        var PeriodView = TimeSlotsPerADay.Where(p => p.Period.Equals(period)).ToList();

                                        //ModuleInstructorSchedule moduleInstructorSchedule = moduleInstructorScheduleAccess.GetInstructorSchedule(BatchModuleView.FirstOrDefault().ModuleId, InstructorList[instructorIndex].InstructorId);

                                        moduleScheduleList.Add(new ModuleSchedule
                                        {
                                            //ModuleInstructorScheduleId = moduleInstructorSchedule.ModuleInstructorScheduleId,
                                            ModuleId = BatchModuleView.FirstOrDefault().ModuleId,
                                            InstructorId = InstructorList[instructorIndex].InstructorId,
                                            PhaseScheduleId = BatchModuleView.FirstOrDefault().PhaseScheduleId,
                                            ClassRoomId = ClassRoomList[roomIndex].ClassRoomId,
                                            Date = assignedDate,
                                            PeriodId = PeriodView.FirstOrDefault().PeriodId
                                        });

                                    }
                                }
                                GeneArrayDouble[timeSlotIndex, roomIndex, instructorIndex] = fittestChrom.Genes[index++].RealValue;
                            }
                            else
                            {
                                GeneArrayDouble[timeSlotIndex, roomIndex, instructorIndex] = 0.0;
                            }
                        }
                    }
                }
                if (moduleScheduleList.Count > 0)
                    schedulerAccess.Add(moduleScheduleList);
            }
            catch (Exception ex)
            {

            }
        }
        private double CalculateFitness(Chromosome chromosome)
        {
            try
            {
                int index = 0;
                for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                {
                    for (int roomIndex = 0; roomIndex < ClassRoomList.Count(); roomIndex++)
                    {
                        for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                        {
                            if (chromosome.Genes != null)
                            {
                                GeneArrayDouble[timeSlotIndex, roomIndex, instructorIndex] = chromosome.Genes[index++].RealValue;
                            }
                            else
                            {
                                GeneArrayDouble[timeSlotIndex, roomIndex, instructorIndex] = 0.0;
                            }
                        }
                    }
                }

                double acheivment1 = constraint_One(GeneArrayDouble);//0.2
                double acheivment2 = constraint_Two(GeneArrayDouble);//0.15
                double acheivment3 = constraint_Three(GeneArrayDouble);//0.32 , We must give a lot percentage % here as it matter a lot
                double acheivment4 = constraint_Four(GeneArrayDouble);//0.13             
                double acheivment5 = constraint_Five(GeneArrayDouble);//0.2                

                double fitnessRate = (((((acheivment1 * 100) * (0.2 * 100)) / 100) + (((acheivment2 * 100) * (0.15 * 100)) / 100) +
                    (((acheivment3 * 100) * (0.32 * 100)) / 100) + (((acheivment4 * 100) * (0.13 * 100)) / 100) + (((acheivment5 * 100) * (0.2 * 100)) / 100)) / 100);
                if (fitnessRate <= 1 && fitnessRate >= 0)
                    return fitnessRate;
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        #region Genes for the CHROMOSOMES/Solutions
        public void get_ClassRoomList()
        {
            ClassRoomList = (List<ClassRoom>)classRoomAccess.List();
        }
        public void get_InstructorList()
        {
            InstructorList = schedulerAccess.get_InstructorList();
        }
        public void construct_TimeSlots()
        {
            string day_Name = "";

            HolidayBreakAccess holidayBreakAccess = new HolidayBreakAccess();
            List<Holiday> hodidayList = holidayBreakAccess.List();

            total_NoOfDays = 120;//Maximum no of days suffiecient to the work space.

            //Construct WORKING SPACE TimeSlot of the semister or the year
            for (int i = 0; i < total_NoOfDays; i++)
            {
                //leave day off and Holidays
                day_Name = MinimumCourseStartingDate.ToString("dddd");

                while (!(Days.Contains(day_Name)) || is_Holiday(hodidayList, MinimumCourseStartingDate))
                {
                    MinimumCourseStartingDate = MinimumCourseStartingDate.AddDays(1);
                    day_Name = MinimumCourseStartingDate.ToString("dddd");
                }
                day_Name = MinimumCourseStartingDate.ToString("dddd");
                foreach (PeriodView period in TimeSlotsPerADay)
                {
                    TimeSlotList.Add(day_Name + ", " + MinimumCourseStartingDate.ToString("MMMM dd,yyyy") + " ," + period.Period + "");
                }
                //Schedule for next day
                MinimumCourseStartingDate = MinimumCourseStartingDate.AddDays(1);
            }
        }
        public void get_BatchModuleList()
        {
            ModuleAccess moduleAccess = new ModuleAccess();
            List<BatchModuleView> BatchModuleListView = new List<BatchModuleView>();

            BatchModuleListView = (List<BatchModuleView>)schedulerAccess.get_BatchModuleList();
            if (BatchModuleListView.Count() > 1)
            {
                //Sort event to be scheduled/(Batch Module Combination) by Starting Date
                BatchModuleListView = BatchModuleListView.OrderBy(cs => cs.StartingDate).ToList();
            }
            int periodDuration = Convert.ToInt32(ConfigurationManager.AppSettings["PeriodDuration"].ToString());
            string eventIndex = String.Empty;
            //Duplicate events(Batch Module Combination) by the number of period they will be given
            //(How many period a specific module will have throughout the semester?  -that much will be schedule in the time table)
            foreach (var batchModule in BatchModuleListView)
            {
                eventIndex = Convert.ToInt32(batchModule.BatchModuleScheduleId) + ".1";
                BatchModuleList.Add(new BatchModuleView
                {
                    ModuleId = batchModule.ModuleId,
                    BatchModuleScheduleId = Convert.ToDouble(eventIndex),
                    BatchId = batchModule.BatchId,
                    StartingDate = batchModule.StartingDate,
                    BatchClassId = batchModule.BatchClassId,
                    CourseId = batchModule.CourseId,
                    PhaseScheduleId = batchModule.PhaseScheduleId,
                    ModuleSequence = batchModule.ModuleSequence,
                    CourseSequence = batchModule.CourseSequence
                });
                Module module = (Module)moduleAccess.Details(batchModule.ModuleId);

                var moduleDuration = module.PracticalDuration + module.TheoreticalDuration;

                if (moduleDuration % 2 == 1)//Check module duration is odd or even
                    moduleDuration += 1;//, make even if it is odd  
                //Find out how many period a module will be given till the module completed, hence, a specific module will be schedule that much in the timetable
                int noOfModulePeriodOccurance = (Convert.ToInt32(moduleDuration) / periodDuration);
                if (noOfModulePeriodOccurance > 1)
                {
                    for (int i = 1; i < noOfModulePeriodOccurance; i++)
                    {
                        eventIndex = (Convert.ToInt32(batchModule.BatchModuleScheduleId) + "." + (i + 1) + "");
                        BatchModuleList.Add(new BatchModuleView
                        {
                            ModuleId = batchModule.ModuleId,
                            BatchModuleScheduleId = Convert.ToDouble(eventIndex),
                            BatchId = batchModule.BatchId,
                            StartingDate = batchModule.StartingDate,
                            BatchClassId = batchModule.BatchClassId,
                            CourseId = batchModule.CourseId,
                            PhaseScheduleId = batchModule.PhaseScheduleId,
                            ModuleSequence = batchModule.ModuleSequence,
                            CourseSequence = batchModule.CourseSequence
                        });
                    }
                }
            }
        }

        public bool is_Holiday(List<Holiday> hodidayList, DateTime date)
        {
            List<DateTime> IndividualHolidays = new List<DateTime>();
            foreach (var holiday in hodidayList)
            {
                if (date.Date >= holiday.StartDateTime.Date && date.Date <= holiday.EndDateTime.Date)
                    return true;
            }
            return false;
        }
        #endregion

        #region Master data for the scheduler
        public void get_MinimumStartingDate()
        {
            MinimumCourseStartingDate = BatchModuleList.Min(BT => BT.StartingDate);
        }
        public void get_UnionOfWorkingDays()
        {
            Days = schedulerAccess.get_UnionOfWorkingDays();
        }
        public void get_UnionOfWorkingPeriods()
        {
            TimeSlotsPerADay = schedulerAccess.get_UnionOfWorkingPeriods();
        }
        #endregion

        #region Fitness Evaluator
        public double constraint_One(double[,,] newGeneArray)
        {
            //No Of Duplicate Assignment of Event in same day & period and different class
            try
            {
                int noOfDuplicateAssignment = 0, totalNoOfDuplicateAssignment = 0;
                List<int> newGeneList = new List<int>();
                List<int> newGeneListOfTheSameTimeSlot = new List<int>();
                //Gene[, ,] newGeneArray = new Gene[TimeSlotList.Count(), ClassRoomList.Count, InstructorList.Count];
                //double[, ,] newGeneArray = new double[TimeSlotList.Count(), ClassRoomList.Count, InstructorList.Count];
                if (newGeneArray.Length > 0)
                {
                    //newGeneArray = (double[, ,])item.ObjectValue;
                    for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                    {
                        newGeneListOfTheSameTimeSlot = new List<int>();
                        noOfDuplicateAssignment = 0;
                        for (int roomIndex = 0; roomIndex < ClassRoomList.Count; roomIndex++)
                        {
                            for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                            {
                                if (newGeneArray[timeSlotIndex, roomIndex, instructorIndex] > 0)
                                {
                                    newGeneListOfTheSameTimeSlot.Add(Convert.ToInt32(newGeneArray[timeSlotIndex, roomIndex, instructorIndex]));
                                    newGeneList.Add(Convert.ToInt32(newGeneArray[timeSlotIndex, roomIndex, instructorIndex]));
                                }
                            }
                        }
                        if (newGeneListOfTheSameTimeSlot.GroupBy(x => x).Where(x => x.Skip(1).Any()).Any())
                        {
                            var batchModuleGroup = newGeneListOfTheSameTimeSlot.GroupBy(x => x).Select(grp => grp.ToList()).ToList();
                            foreach (var batchModuleGroupItem in batchModuleGroup)
                            {
                                if (batchModuleGroupItem.Count() > 1)
                                    noOfDuplicateAssignment += 1;
                            }
                            totalNoOfDuplicateAssignment += noOfDuplicateAssignment;
                        }
                    }
                    return (Convert.ToDouble(newGeneList.Count()) - Convert.ToDouble(totalNoOfDuplicateAssignment)) / Convert.ToDouble(newGeneList.Count());
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        public double constraint_Two(double[,,] newGeneArray)
        {
            try
            {
                List<Batchdays> BatchDayList = schedulerAccess.get_DayTemplateList();
                List<BatchPeriod> BatchPeriodList = schedulerAccess.get_PeriodTemplateList();

                //Batch Day template and Period template Corspondence
                string timeSlot = null;
                List<int> newGeneList = new List<int>();
                int numberOfInCorrectAssignment = 0;
                // Gene[, ,] newGeneArray = new Gene[TimeSlotList.Count(), ClassRoomList.Count, InstructorList.Count];
                // double[, ,] newGeneArray = new double[TimeSlotList.Count(), ClassRoomList.Count, InstructorList.Count];
                if (newGeneArray.Length > 0)
                {
                    //newGeneArray = (double[, ,])item.ObjectValue;

                    for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                    {
                        for (int roomIndex = 0; roomIndex < ClassRoomList.Count; roomIndex++)
                        {
                            for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                            {
                                if (newGeneArray[timeSlotIndex, roomIndex, instructorIndex] > 0)
                                {
                                    var BatchModuleView = BatchModuleList.Where(bm => bm.BatchModuleScheduleId == Convert.ToDouble(newGeneArray[timeSlotIndex, roomIndex, instructorIndex])).ToList();
                                    BatchModuleView batchModule = null;
                                    if (BatchModuleView.Count() > 0)
                                    {
                                        batchModule = BatchModuleView.FirstOrDefault();
                                        List<string> days = BatchDayList.Where(x => x.BatchId == batchModule.BatchId).Select(item => item.DayName).ToList();
                                        List<string> periods = BatchPeriodList.Where(x => x.BatchId == batchModule.BatchId).Select(item => item.Period).ToList();

                                        timeSlot = TimeSlotList[timeSlotIndex];
                                        string[] timeSlotArray = timeSlot.Split(',');
                                        //timeSlotArray = timeSlotArray.Select(x => !String.IsNullOrEmpty(x)).ToArray();
                                        if (!(days.Contains(timeSlotArray[0])) && !(periods.Contains(timeSlotArray[3])))
                                        {
                                            numberOfInCorrectAssignment += 2;
                                        }
                                        else if (!periods.Contains(timeSlotArray[3]))
                                        {
                                            numberOfInCorrectAssignment += 1;
                                        }
                                        else if (!(days.Contains(timeSlotArray[0])))
                                        {
                                            numberOfInCorrectAssignment += 1;
                                        }
                                        newGeneList.Add(Convert.ToInt32(newGeneArray[timeSlotIndex, roomIndex, instructorIndex]));
                                    }
                                }
                            }
                        }
                    }
                    return (Convert.ToDouble((newGeneList.Count() * 2)) - Convert.ToDouble(numberOfInCorrectAssignment)) / Convert.ToDouble((newGeneList.Count() * 2));
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        public double constraint_Three(double[,,] newGeneArray)
        {
            //Instructor Illigibility for the module
            try
            {

                ModuleInstructorScheduleAccess moduleInstructorScheduleAccess = new ModuleInstructorScheduleAccess();
                List<ModuleInstructorSchedule> potentialModulesofInstructorList = new List<ModuleInstructorSchedule>();
                List<ModuleInstructorSchedule> objModuleInstructorList = (List<ModuleInstructorSchedule>)moduleInstructorScheduleAccess.List();
                List<int> newGeneList = new List<int>();
                int numberOfInCorrectAssignment = 0;
                //Gene[, ,] newGeneArray = new Gene[TimeSlotList.Count(), ClassRoomList.Count, InstructorList.Count];
                //double[, ,] newGeneArray = new double[TimeSlotList.Count(), ClassRoomList.Count, InstructorList.Count];

                if (newGeneArray.Length > 0)
                {
                    //newGeneArray = (double[, ,])item.ObjectValue;

                    for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                    {
                        for (int roomIndex = 0; roomIndex < ClassRoomList.Count; roomIndex++)
                        {
                            for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                            {
                                if (newGeneArray[timeSlotIndex, roomIndex, instructorIndex] > 0)
                                {
                                    potentialModulesofInstructorList = objModuleInstructorList.Where(ins => ins.InstructorId == InstructorList[instructorIndex].InstructorId).ToList();//
                                    var batchModuleView = BatchModuleList.Where(bm => bm.BatchModuleScheduleId == Convert.ToDouble(newGeneArray[timeSlotIndex, roomIndex, instructorIndex])).ToList();
                                    BatchModuleView batchModule = null;
                                    if (batchModuleView.Count() > 0)
                                    {
                                        batchModule = batchModuleView.FirstOrDefault();
                                        var moduleInst = potentialModulesofInstructorList.Find(x => x.ModuleId == batchModule.ModuleId);
                                        if (moduleInst == null)
                                        {
                                            numberOfInCorrectAssignment += 1;
                                        }
                                        newGeneList.Add(Convert.ToInt32(newGeneArray[timeSlotIndex, roomIndex, instructorIndex]));
                                    }
                                }
                            }
                        }
                    }
                    return (Convert.ToDouble(newGeneList.Count()) - Convert.ToDouble(numberOfInCorrectAssignment)) / Convert.ToDouble(newGeneList.Count());
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        public double constraint_Four(double[,,] newGeneArray)
        {
            //Instructor Leave
            try
            {
                PersonLeaveAccess personLeaveAccess = new PersonLeaveAccess();
                ModuleInstructorScheduleAccess moduleInstructorScheduleAccess = new ModuleInstructorScheduleAccess();
                List<PersonLeave> personLeaveList = new List<PersonLeave>();
                List<PersonLeave> PersonLeaveList = personLeaveAccess.List();

                List<int> newGeneList = new List<int>();
                int numberOfInCorrectAssignment = 0;

                if (newGeneArray.Length > 0)
                {
                    for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                    {
                        for (int roomIndex = 0; roomIndex < ClassRoomList.Count; roomIndex++)
                        {
                            for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                            {
                                if (newGeneArray[timeSlotIndex, roomIndex, instructorIndex] > 0)
                                {
                                    personLeaveList = PersonLeaveList.Where(PL => PL.PersonId == InstructorList[instructorIndex].InstructorId).ToList();//
                                    if (personLeaveList.Count() > 0)
                                    {
                                        string timeSlot = TimeSlotList[timeSlotIndex];
                                        string[] timeSlotArray = timeSlot.Split(',');
                                        DateTime assignedDate = Convert.ToDateTime(timeSlotArray[1] + "," + timeSlotArray[2]);
                                        foreach (var personLeave in personLeaveList)
                                        {
                                            if (assignedDate >= personLeave.FromDate && assignedDate <= personLeave.ToDate)
                                            {
                                                numberOfInCorrectAssignment += 1;
                                            }
                                        }
                                    }
                                    newGeneList.Add(Convert.ToInt32(newGeneArray[timeSlotIndex, roomIndex, instructorIndex]));
                                }
                            }
                        }
                    }
                    return (Convert.ToDouble(newGeneList.Count()) - Convert.ToDouble(numberOfInCorrectAssignment)) / Convert.ToDouble(newGeneList.Count());
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        public double constraint_Five(double[,,] newGeneArray)
        {
            //Module Sequence
            //Fearness distrbution of resource--Pending Constraint 6
            try
            {
                int periodDuration = 2;
                ModuleAccess moduleAccess = new ModuleAccess();

                List<CourseModuleView> orderedModuleList = new List<CourseModuleView>();
                List<CourseModuleView> courseModuleDuplicatedList = new List<CourseModuleView>();
                //Get all scheduled modules and their sequence
                List<CourseModuleView> courseModuleList = schedulerAccess.ListCourseModule();

                //Duplicate events(Batch Module Combination) by the number of period they will be given
                //(How many period a specific module will have throughout the semester?  -that much number will be schedule in the time table)
                foreach (var courseModule in courseModuleList)
                {
                    courseModuleDuplicatedList.Add(new CourseModuleView
                    {
                        BatchId = courseModule.BatchId,
                        BatchClassId = courseModule.BatchClassId,
                        PhaseId = courseModule.PhaseId,
                        CourseId = courseModule.CourseId,
                        ModuleId = courseModule.ModuleId,
                        ModuleSequence = courseModule.ModuleSequence,
                        CourseSequence = courseModule.CourseSequence
                    });
                   
                    Module module = (Module)moduleAccess.Details(courseModule.ModuleId);
                    var moduleDuration = module.TheoreticalDuration + module.PracticalDuration;
                    if (moduleDuration % 2 == 1)
                        moduleDuration += 1;

                    //Find out how many period a module will be given throughout the semester, hence, a specific module will be schedule that much in the timetable
                    int noOfModuleOccurance = (Convert.ToInt32(moduleDuration) / periodDuration);
                    if (noOfModuleOccurance > 1)
                    {
                        for (int i = 1; i < noOfModuleOccurance; i++)
                        {
                            courseModuleDuplicatedList.Add(new CourseModuleView
                            {
                                BatchId = courseModule.BatchId,
                                BatchClassId = courseModule.BatchClassId,
                                PhaseId = courseModule.PhaseId,
                                CourseId = courseModule.CourseId,
                                ModuleId = courseModule.ModuleId,
                                ModuleSequence = courseModule.ModuleSequence,
                                CourseSequence = courseModule.CourseSequence
                            });
                        }
                    }
                }

                List<BatchModuleView> scheduledCourseModules = new List<BatchModuleView>();
                int numberOfCoursesInCorrectAssignment = 0, numberOfModulesInCorrectAssignment = 0, totalNumberOfCoursesAssignment = 0, totalNumberOfModulesAssignment = 0;
                //Get all scheduled module from the timetable
                if (newGeneArray.Length > 0)
                {
                    for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                    {
                        for (int roomIndex = 0; roomIndex < ClassRoomList.Count; roomIndex++)
                        {
                            for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                            {
                                if (newGeneArray[timeSlotIndex, roomIndex, instructorIndex] > 0)
                                {
                                    scheduledCourseModules.Add(BatchModuleList.Find(x => x.BatchModuleScheduleId == Convert.ToDouble(newGeneArray[timeSlotIndex, roomIndex, instructorIndex])));
                                }
                            }
                        }
                    }
                }


                if (courseModuleDuplicatedList.Count > 0 && scheduledCourseModules.Count > 0)
                {
                    //Group modules by Batch Class Id
                    var scheduledModuleGroupByBatchClassId = scheduledCourseModules.GroupBy(x => x.BatchClassId).Select(grp => grp.ToList()).ToList();//Sch,Usch
                    var unScheduledModuleGroupByBatchClassId = courseModuleDuplicatedList.GroupBy(x => x.BatchClassId).Select(grp => grp.ToList()).ToList();

                    var scheduledAndUnscheduledModulesByBatchClassId = scheduledModuleGroupByBatchClassId.Zip(unScheduledModuleGroupByBatchClassId, (S, U) => new { ScheduledGroupByBatchClassId = S, UnScheduledGroupByBatchClassId = U });

                    foreach (var scheduledAndUnscheduledCategoryByBatchClassId in scheduledAndUnscheduledModulesByBatchClassId)
                    {
                        //Group courses by Phase Id
                        var sheduledModuleGroupByPhaseId = scheduledAndUnscheduledCategoryByBatchClassId.ScheduledGroupByBatchClassId.GroupBy(x => x.PhaseId).Select(grp => grp.ToList()).ToList();
                        var unScheduledModuleGroupByPhaseId = scheduledAndUnscheduledCategoryByBatchClassId.UnScheduledGroupByBatchClassId.GroupBy(x => x.PhaseId).Select(grp => grp.ToList()).ToList();

                        var scheduledAndUnscheduledModulesByPhaseId = sheduledModuleGroupByPhaseId.Zip(unScheduledModuleGroupByPhaseId, (S, U) => new { ScheduledGroupByPhaseId = S, UnScheduledGroupByPhaseId = U });

                        foreach (var scheduledAndUnscheduledCategoryByPhaseId in scheduledAndUnscheduledModulesByPhaseId)
                        {
                            ///////////////////////////////////////////////////////////////////////////////////////////////
                            //Sort every group of course by their sequence number, but only the unscheduled one (Master Data)
                            var unScheduledSortedCourses = scheduledAndUnscheduledCategoryByPhaseId.UnScheduledGroupByPhaseId.OrderBy(x => x.CourseSequence).ToList();

                            var scheduledAndUnscheduledCourses = scheduledAndUnscheduledCategoryByPhaseId.ScheduledGroupByPhaseId.Zip(unScheduledSortedCourses, (S, U) => new { ScheduledCourse = S, UnScheduledCourse = U });

                            foreach (var course in scheduledAndUnscheduledCourses)
                            {
                                totalNumberOfCoursesAssignment += 1;
                                if (course.ScheduledCourse.CourseSequence != course.UnScheduledCourse.CourseSequence)
                                {
                                    numberOfCoursesInCorrectAssignment += 1;
                                }
                            }
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            //Group modules by Course Id
                            var sheduledModuleGroupByCourseId = scheduledAndUnscheduledCategoryByPhaseId.ScheduledGroupByPhaseId.GroupBy(x => x.CourseId).Select(grp => grp.ToList()).ToList();
                            var unScheduledModuleGroupByCourseId = scheduledAndUnscheduledCategoryByPhaseId.UnScheduledGroupByPhaseId.GroupBy(x => x.CourseId).Select(grp => grp.ToList()).ToList();

                            var scheduledAndUnscheduledModulesByCourseId = sheduledModuleGroupByCourseId.Zip(unScheduledModuleGroupByCourseId, (S, U) => new { ScheduledGroupByCourseId = S, UnScheduledGroupByCourseId = U });

                            foreach (var scheduledAndUnscheduledCategoryByCourseId in scheduledAndUnscheduledModulesByCourseId)
                            {
                                //Sort every group of modules by their sequence number
                                var unScheduledSortedModules = scheduledAndUnscheduledCategoryByCourseId.UnScheduledGroupByCourseId.OrderBy(x => x.ModuleSequence).ToList();

                                var scheduledAndUnscheduledModules = scheduledAndUnscheduledCategoryByCourseId.ScheduledGroupByCourseId.Zip(unScheduledSortedModules, (S, U) => new { ScheduledModule = S, UnScheduledModule = U });

                                foreach (var modules in scheduledAndUnscheduledModules)
                                {
                                    totalNumberOfModulesAssignment += 1;
                                    if (modules.ScheduledModule.ModuleSequence != modules.UnScheduledModule.ModuleSequence)
                                    {
                                        numberOfModulesInCorrectAssignment += 1;
                                    }
                                }
                            }
                        }
                    }
                }

                double fitnessOfCoursesSequence = (Convert.ToDouble(totalNumberOfCoursesAssignment) - Convert.ToDouble(numberOfCoursesInCorrectAssignment)) / Convert.ToDouble(totalNumberOfCoursesAssignment);
                double fitnessOfModulesSequence = (Convert.ToDouble(totalNumberOfModulesAssignment) - Convert.ToDouble(numberOfModulesInCorrectAssignment)) / Convert.ToDouble(totalNumberOfModulesAssignment);

                return (Convert.ToDouble((fitnessOfCoursesSequence + fitnessOfModulesSequence) / 2));
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        #endregion
    }
}

