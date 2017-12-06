
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using GAF.Extensions;
using GAF.Operators;

namespace PTSMSBAL.Scheduling.Others
{   
    public class SampleGA
    {
        //population size
        private static int POPSize = 30;
        //geneotype
        private static int chromoLEN = 10;

        private static int noOfRooms = 10;
        private static int noOfDays = 10;
        private static int noOfTimeSlotPerDays = 10;

        //mutation rate, change it have a play
        private double MUT = 0.02;
        //recomination rate
        private double REC = 0.8;
        //how many tournaments should be played
        private static long Tournaments = 1000;
        //the sum pile, end result for the SUM pile
        //card1 + card2 + card3 + card4 + card5, MUST = 36 for a good GA
        private double SUMTARG = 36;
        //the product pile, end result for the PRODUCT pile
        //card1 * card2 * card3 * card4 * card5, MUST = 360 for a good GA
        private double PRODTARG = 360;
        //the genes array, 30 members, 10 cards each
        private int[,] gene = new int[POPSize, chromoLEN];
        private Gene[,] geneArray = new Gene[POPSize, chromoLEN];
        //used to create randomness (Simulates selection process in nature)
        //randomly selects genes
        Random rnd = new Random();
        Population population = new Population();

        public object TimeTableScheduler()
        {
            try
            {
                init_pop();


                //create the elite operator
                var elite = new Elite(5);

                //create the crossover operator
                var crossover = new Crossover(0.8, true)
                {
                    CrossoverType = CrossoverType.SinglePoint
                };

                //create the mutation operator
                var mutate = new SwapMutate(0.02);

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
        public static bool Terminate(Population population, int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > Tournaments;
        }
        private static void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            Console.WriteLine("Generation: {0}, Fitness: {1}", e.Generation, fittest.Fitness);
            Console.WriteLine("\r\n==============================\r\n");
            Console.WriteLine("After " + e.Evaluations + " tournaments, Solution sum pile (should be 36) cards are : ");
            for (int i = 0; i < fittest.Genes.Count; i++)
            {
                if (fittest.Genes[i].BinaryValue == 0)
                {
                    Console.WriteLine(i + 1);
                }
            }
            Console.WriteLine("\r\nAnd Product pile (should be 360)  cards are : ");
            for (int i = 0; i < chromoLEN; i++)
            {
                if (fittest.Genes[i].BinaryValue == 1)
                {
                    Console.WriteLine(i + 1);
                }
            }
        }

        static void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            string sum = "";
            string product = "";
            var fittest = e.Population.GetTop(1)[0];
            Console.WriteLine("Generation: {0}, Fitness: {1}", e.Generation, fittest.Fitness);
            Console.WriteLine("\r\n==============================\r\n");
            Console.WriteLine("After " + e.Evaluations + " tournaments, Solution sum pile (should be 36) cards are : ");
            for (int i = 0; i < fittest.Genes.Count; i++)
            {
                if (fittest.Genes[i].BinaryValue == 0)
                {
                    sum = sum + "," + (i + 1);
                    Console.WriteLine(i + 1);
                }
            }
            Console.WriteLine("\r\nAnd Product pile (should be 360)  cards are : ");
            for (int i = 0; i < chromoLEN; i++)
            {
                if (fittest.Genes[i].BinaryValue == 1)
                {
                    product = product + "," + (i + 1);
                    Console.WriteLine(i + 1);
                }
            }
        }
        private double CalculateFitness(Chromosome chromo)
        {
            //initialise field values
            int sum = 0, prod = 1;
            double scaled_sum_error, scaled_prod_error, combined_error;
            //loop though all genes for this population member
            for (int i = 0; i < chromo.Genes.Count; i++)
            {
                //if the gene value is 0, then put it in the sum (pile 0), and calculate sum
                if (chromo.Genes[i].RealValue == 0)
                {
                    sum = sum + (1 + i);
                }
                //if the gene value is 1, then put it in the product (pile 1), and calculate product
                else
                {
                    prod = prod * (1 + i);
                }
            }
            //work out how good this population member is, based on an overall error
            //for the problem domain
            //NOTE : The fitness function will change for every problem domain.
            scaled_sum_error = (sum - SUMTARG) / SUMTARG;

            scaled_prod_error = (prod - PRODTARG) / PRODTARG;
            combined_error = System.Math.Abs(scaled_sum_error) + System.Math.Abs(scaled_prod_error);
            if (combined_error > 1.0)
                return 0.0;
            if (1 - combined_error == 1.0)
            {
            }
            return 1 - combined_error;
        }

        private void init_pop()
        {
            //for entire population
            for (int i = 0; i < POPSize; i++)
            {
                var chromosome = new Chromosome();
                //for all genes
                for (int j = 0; j < chromoLEN; j++)
                {
                    //randomly create gene values
                    if (rnd.NextDouble() < 0.5)
                    {
                        //gene[i, j] = 0;
                        chromosome.Genes.Add(new Gene(0));
                    }
                    else
                    {
                        //gene[i, j] = 1;
                        chromosome.Genes.Add(new Gene(1));
                    }
                }
                chromosome.Genes.ShuffleFast();
                population.Solutions.Add(chromosome);
            }
        }

    }
}
