namespace GeneticAlgorithm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            Console.WriteLine("Enter the starting population size, for good result its recommended at least 1000 : ");
            string input = Console.ReadLine();
            int startingPopulationSize;

            while (!int.TryParse(input, out startingPopulationSize))
            {
                Console.WriteLine("Please enter a valid number: ");
                input = Console.ReadLine();
            }
            Console.Write("Please enter the type of optimization (Lamarckian, Darwinian, none): ");
            string optimizationType = Console.ReadLine();

// Validate input.
            while (optimizationType != "Lamarckian" && optimizationType != "Darwinian" && optimizationType != "none")
            {
                Console.Write("Invalid input. Please enter either 'Lamarckian', 'Darwinian', or 'none': ");
                optimizationType = Console.ReadLine();
            }
            Console.WriteLine("THANK YOU, please wait until the end of the algorithm");
            string encTextFilePath =  Path.Combine(AppContext.BaseDirectory,"InputFiles", "enc.txt");//"./InputFiles/enc.txt";
            string dictFilePath =  Path.Combine(AppContext.BaseDirectory,"InputFiles", "dict.txt"); //"./InputFiles/dict.txt";
            string letterFreqFilePath = Path.Combine(AppContext.BaseDirectory, "InputFiles", "Letter_Freq.txt"); //"./InputFiles/Letter_Freq.txt";
            GeneticAlgorithm ga = new GeneticAlgorithm(startingPopulationSize,startingPopulationSize * 2, encTextFilePath, dictFilePath, letterFreqFilePath);
            while (ga.shouldRun)
            {
                ga.EvolveNextGeneration(1000,0.1,20,"null");
            }
            Console.WriteLine("Finished");
            //StatisticsCalculate st = new StatisticsCalculate();
            //st.showBestAvarageCompareChart(encTextFilePath,dictFilePath,letterFreqFilePath,"fc");
           // st.showBestStartingPopulation(encTextFilePath,dictFilePath,letterFreqFilePath);
        }
    }
}