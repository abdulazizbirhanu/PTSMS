using GAF;
using GAF.Extensions;
using GAF.Operators;
using PTSMSDAL.Access.Curriculum.Operations;
using PTSMSDAL.Access.Scheduling.Operations;
using PTSMSDAL.Access.Scheduling.Relations;
using PTSMSDAL.Models.Curriculum.Operations;
using PTSMSDAL.Models.Scheduling.Operations;
using PTSMSDAL.Models.Scheduling.Relations;
using PTSMSDAL.Models.Scheduling.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTSMSBAL.Scheduling.Operations
{
    public class FTDAndFlyingSchedulerLogic
    {
        SchedulerAccess schedulerAccess = new SchedulerAccess();
       
        //ClassRoomAccess classRoomAccess = new ClassRoomAccess();

        #region GLOBAL Variables that hold Genes

        private static List<TraineeLessonScheduleView> TraineeLessonScheduleList = new List<TraineeLessonScheduleView>();
        //private static List<ClassRoom> ClassRoomList = new List<ClassRoom>(); 
        private static List<EquipmentView> EquipmentList = new List<EquipmentView>();//
        private static List<InstructorView> InstructorList = new List<InstructorView>();//EquipmentView

        private static List<string> TimeSlotList = new List<string>();
        String LessonCategoryType;
        #endregion

        //population size
        private static int POPSize = 30;
        //geneotype 
        private static int total_NoOfDays = 120;
        private DateTime MinimumCourseStartingDate = new DateTime();
        #region Constructor
        public FTDAndFlyingSchedulerLogic(string lessonCategoryType)
        {
            this.LessonCategoryType = lessonCategoryType;
            this.get_TraineeLessonSchedulerList();
            this.get_MinimumStartingDate();
            //this.get_ClassRoomList();
            this.get_EquipmentList();
            this.get_InstructorList();
            this.get_UnionOfWorkingDays();
            this.get_UnionOfWorkingPeriods();
            this.construct_TimeSlots();
            geneArray = new Gene[TimeSlotList.Count(), EquipmentList.Count(), InstructorList.Count];
            GeneArrayDouble = new double[TimeSlotList.Count(), EquipmentList.Count(), InstructorList.Count];
        }
        #endregion
        private static List<string> Days = new List<string>();//{ "Mon", "Tue", "Wed", "Thu", "Fri" };
        private static List<PeriodView> TimeSlotsPerADay = new List<PeriodView>();//{ "08:00-10:00", "10:00-12:00", "14:00-16:00", "16:00-17:00" };

        //Mutation rate, change it have a play
        private double mutRate = 0.02;
        //Recomination rate
        private double crossOverRate = 0.8;
        //How many tournaments should be played
        private static long Tournament = 100;

        private Gene[,,] geneArray = null;

        private static double[,,] GeneArrayDouble = null;

        private static List<double> fitnessList = new List<double>();
        Population population = new Population();


        public bool TimeTableScheduleForFTDAndFlyingr()
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
                    CrossoverType = CrossoverType.DoublePoint
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
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }


        #region Input methods for the scheduler

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
                FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();

                Chromosome fittestChrom = e.Population.GetTop(1)[0];
                List<FlyingFTDSchedule> FlyingFTDScheduleList = new List<FlyingFTDSchedule>();
                DateTime assignedDate = new DateTime();
                string period = String.Empty;
                string Equipment = String.Empty;

                int index = 0;
                for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                {
                    for (int equipmentIndex = 0; equipmentIndex < EquipmentList.Count(); equipmentIndex++)
                    {
                        for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                        {
                            if (fittestChrom.Genes != null)
                            {
                                if (fittestChrom.Genes[index].RealValue > 0.0)
                                {
                                    var traineeLessonView = TraineeLessonScheduleList.Where(bm => bm.TraineeLessonScheduleId == fittestChrom.Genes[index].RealValue).ToList();
                                   
                                    if (traineeLessonView.Count() > 0)
                                    {
                                        string timeSlot = TimeSlotList[timeSlotIndex];
                                        string[] timeSlotArray = timeSlot.Split(',');
                                        assignedDate = Convert.ToDateTime(timeSlotArray[1] + "," + timeSlotArray[2]);
                                        period = timeSlotArray[3];
                                        var PeriodView = TimeSlotsPerADay.Where(p => p.Period.Equals(period)).ToList();
                                                                              
                                        FlyingFTDScheduleList.Add(new FlyingFTDSchedule
                                        {
                                            TraineeId = traineeLessonView.FirstOrDefault().TraineeId,
                                            LessonId = traineeLessonView.FirstOrDefault().LessonId,
                                            InstructorId = InstructorList[instructorIndex].InstructorId,
                                            EquipmentId = EquipmentList[equipmentIndex].EquipmentId,
                                            ScheduleStartTime = assignedDate,
                                            Status = Enum.GetName(typeof(FlyingFTDScheduleStatus), 0)
                                        });
                                    }
                                }
                                GeneArrayDouble[timeSlotIndex, equipmentIndex, instructorIndex] = fittestChrom.Genes[index++].RealValue;
                            }
                            else
                            {
                                GeneArrayDouble[timeSlotIndex, equipmentIndex, instructorIndex] = 0.0;
                            }
                        }
                    }
                }
                if (FlyingFTDScheduleList.Count > 0)
                    fTDAndFlyingSchedulerAccess.Add(FlyingFTDScheduleList);
            }
            catch (Exception ex)
            {

            }
        }
        private void initialize_Population()
        {
            Random random = new Random();
            int selectedIndex = -1, index = 0;

            for (int i = 0; i < POPSize; i++)
            {
                //Put it into queue in order to achieve Round Robin Scheduling
                Queue<List<TraineeLessonScheduleView>> queue = new Queue<List<TraineeLessonScheduleView>>();

                //Group module by Course Id
                var traineeLessonScheduleGroup = TraineeLessonScheduleList.GroupBy(x => new { x.TraineeId }).Select(grp => grp.ToList()).ToList();
                //////////////////////////            
                foreach (List<TraineeLessonScheduleView> traineeLessonGroupItem in traineeLessonScheduleGroup)
                {
                    queue.Enqueue(traineeLessonGroupItem);
                }
                //Initialy, the instructor we are going to assign should be in the list of potential instructor list of the module
                var chromosome = new Chromosome();

                for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                {
                    int numberOfAssignedInstructor = 0;//Monitor assigned instructor at a specific DAY and TIME(Period)
                    for (int equipmentIndex = 0; equipmentIndex < EquipmentList.Count(); equipmentIndex++)
                    {
                        //Assign Instructor as long as there is unassigned instrutor at a specific DATE and TIME(Period)
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
                                    List<TraineeLessonScheduleView> currentDequeueGroup = queue.Dequeue();
                                    TraineeLessonScheduleView traineeLessonGroupToBeAssigned = null;
                                    List<TraineeLessonScheduleView> newTraineeLessons = new List<TraineeLessonScheduleView>();

                                    selectedIndex = random.Next(0, currentDequeueGroup.Count); ;
                                    traineeLessonGroupToBeAssigned = currentDequeueGroup[selectedIndex];
                                    index = 0;
                                    foreach (TraineeLessonScheduleView traineeLesson in currentDequeueGroup)
                                    {
                                        if (selectedIndex != index++)
                                            newTraineeLessons.Add(traineeLesson);
                                    }
                                    if (newTraineeLessons.Count() > 0)
                                        queue.Enqueue(newTraineeLessons);
                                    chromosome.Genes.Add(new Gene(traineeLessonGroupToBeAssigned.TraineeLessonScheduleId));
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
                        {//if there is no available instructor, don't give any Lesson i.e. Chromosome 0 
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
        private double CalculateFitness(Chromosome chromosome)
        {
            try
            {
                int index = 0;
                for (int timeSlotIndex = 0; timeSlotIndex < TimeSlotList.Count(); timeSlotIndex++)
                {
                    for (int equipmentIndex = 0; equipmentIndex < EquipmentList.Count(); equipmentIndex++)
                    {
                        for (int instructorIndex = 0; instructorIndex < InstructorList.Count; instructorIndex++)
                        {
                            if (chromosome.Genes != null)
                            {
                                GeneArrayDouble[timeSlotIndex, equipmentIndex, instructorIndex] = chromosome.Genes[index++].RealValue;
                            }
                            else
                            {
                                GeneArrayDouble[timeSlotIndex, equipmentIndex, instructorIndex] = 0.0;
                            }
                        }
                    }
                }

                //double acheivment1 = constraint_One(GeneArrayDouble);//0.2
                //double acheivment2 = constraint_Two(GeneArrayDouble);//0.15
                //double acheivment3 = constraint_Three(GeneArrayDouble);//0.32 , We must give a lot percentage % here as it matter a lot
                //double acheivment4 = constraint_Four(GeneArrayDouble);//0.13             
                //double acheivment5 = constraint_Five(GeneArrayDouble);//0.2                

                //double fitnessRate = (((((acheivment1 * 100) * (0.2 * 100)) / 100) + (((acheivment2 * 100) * (0.15 * 100)) / 100) +
                //    (((acheivment3 * 100) * (0.32 * 100)) / 100) + (((acheivment4 * 100) * (0.13 * 100)) / 100) + (((acheivment5 * 100) * (0.2 * 100)) / 100)) / 100);
                //if (fitnessRate <= 1 && fitnessRate >= 0)
                //return fitnessRate;
                return 0.0;
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        #endregion



        #region Methods for collecting input data for the scheduler
        //public void get_ClassRoomList()
        //{
        //    ClassRoomList = (List<ClassRoom>)classRoomAccess.List();
        //}

        public void get_EquipmentList()
        {
            EquipmentAccess equipmentAccess = new EquipmentAccess();
            EquipmentList = equipmentAccess.get_EquipmentList();
        }
        public void get_InstructorList()
        {
            FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();
            InstructorList = fTDAndFlyingSchedulerAccess.get_FTDAndFlyingInstructorList();
        }
        public void get_UnionOfWorkingDays()
        {
            Days = schedulerAccess.get_UnionOfWorkingDays();
        }
        public void get_UnionOfWorkingPeriods()
        {
            TimeSlotsPerADay = schedulerAccess.get_UnionOfWorkingPeriods();
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
        public void get_TraineeLessonSchedulerList()
        {
            LessonAccess lessonAccess = new LessonAccess();
            FTDAndFlyingSchedulerAccess fTDAndFlyingSchedulerAccess = new FTDAndFlyingSchedulerAccess();

            List<TraineeLessonScheduleView> TraineeLessonScheduleViewList = new List<TraineeLessonScheduleView>();

            TraineeLessonScheduleViewList = (List<TraineeLessonScheduleView>)fTDAndFlyingSchedulerAccess.get_TraineeLessonSchedulerList();
            if (TraineeLessonScheduleViewList.Count() > 1)
            {
                //Sort event to be scheduled/(Batch Module Combination) by Starting Date
                TraineeLessonScheduleViewList = TraineeLessonScheduleViewList.OrderBy(cs => cs.StartingDate).ToList();
            }
            int periodDuration = Convert.ToInt32(ConfigurationManager.AppSettings["PeriodDuration"].ToString());
            string eventIndex = String.Empty;
            //Duplicate events(Batch Module Combination) by the number of period they will be given
            //(How many period a specific module will have throughout the semester?  -that much will be schedule in the time table)
            foreach (var traineeLesson in TraineeLessonScheduleViewList)
            {
                eventIndex = Convert.ToInt32(traineeLesson.TraineeLessonScheduleId) + ".1";
                TraineeLessonScheduleList.Add(new TraineeLessonScheduleView
                {
                    TraineeLessonScheduleId = Convert.ToDouble(eventIndex),
                    TraineeId = traineeLesson.TraineeId,
                    LessonId = traineeLesson.LessonId,
                    BatchId = traineeLesson.BatchId,
                    StartingDate = traineeLesson.StartingDate,
                    BatchClassId = traineeLesson.BatchClassId,
                    PhaseScheduleId = traineeLesson.PhaseScheduleId,
                    LessonSequence = traineeLesson.LessonSequence,
                    PhaseId = traineeLesson.PhaseId
                });

                int lessonDuration = 0;
                Lesson lesson = (Lesson)lessonAccess.Details(traineeLesson.LessonId);
                if (lesson.CategoryType.Type.ToUpper().Equals("FTD"))
                {
                    if (lesson.FTDTime % 2 == 1)//Check module duration is odd or even
                        lessonDuration = Convert.ToInt16(lesson.FTDTime += 1);//Make even if it is odd  
                }
                else
                {
                    if (lesson.TimeAircraftDual > 0)
                    {
                        if (lesson.TimeAircraftDual % 2 == 1)//Check module duration is odd or even
                            lessonDuration = Convert.ToInt16(lesson.TimeAircraftDual += 1);//Make even, if it is odd  
                    }
                    else
                    {
                        if (lesson.TimeAircraftSolo % 2 == 1)//Check module duration is odd or even
                            lessonDuration = Convert.ToInt16(lesson.TimeAircraftSolo += 1);//Make even, if it is odd
                    }
                }
                //Find out how many period a module will be given throughout the semester, hence, a specific module will be schedule that much in the timetable
                int noOfLessonPeriodOccurance = (lessonDuration / periodDuration);
                if (noOfLessonPeriodOccurance > 1)
                {
                    for (int i = 1; i < noOfLessonPeriodOccurance; i++)
                    {
                        eventIndex = (Convert.ToInt32(traineeLesson.TraineeLessonScheduleId) + "." + (i + 1) + "");
                        TraineeLessonScheduleList.Add(new TraineeLessonScheduleView
                        {
                            TraineeLessonScheduleId = Convert.ToDouble(eventIndex),
                            TraineeId = traineeLesson.TraineeId,
                            LessonId = traineeLesson.LessonId,
                            BatchId = traineeLesson.BatchId,
                            StartingDate = traineeLesson.StartingDate,
                            BatchClassId = traineeLesson.BatchClassId,
                            PhaseScheduleId = traineeLesson.PhaseScheduleId,
                            LessonSequence = traineeLesson.LessonSequence,
                            PhaseId = traineeLesson.PhaseId
                        });
                    }
                }
            }
        }
        public void get_MinimumStartingDate()
        {
            MinimumCourseStartingDate = TraineeLessonScheduleList.Min(BT => BT.StartingDate);
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
    }
}

