namespace GeneticAlgorithm;
using ScottPlot;

public class StatisticsCalculate
{
    public  void showBestAvarageCompareChart(string encTextFilePath,string dictFilePath,string letterFreqFilePath,string type)
    {
        double normalValue = runForBest(encTextFilePath, dictFilePath, letterFreqFilePath, 10, "null",type,1000,2000);
        double darwinValue = runForBest(encTextFilePath, dictFilePath, letterFreqFilePath, 10, "Darwinian",type,1000,2000);
        double lamarValue = runForBest(encTextFilePath, dictFilePath, letterFreqFilePath, 10, "Lamarckian",type,1000,2000);
        double[] x = { normalValue, darwinValue, lamarValue };
        //double[] y = {0,100,100000};
        string[] labels = { "Normal", "Lamarckian", "Darwinian" };
        var plt = new Plot(600, 400);
        plt.Grid(enable:false, lineStyle: LineStyle.Dot);
        plt.AddBar(x);
        plt.SetAxisLimits(yMin: 0);
        plt.XTicks(labels);
        plt.Title("Comparison of Average Fitness Calls");
        plt.YLabel("Fitness Calls");
        plt.SaveFig("comparison.png");
    }

    public void showBestStartingPopulation(string encTextFilePath, string dictFilePath, string letterFreqFilePath)
    {
        int[] populationSize = { 500, 1000, 2000, 3000 };
        int[] maxSize = { 1000, 2000, 4000, 6000 };
        double[] sizeBestAverage = new double[4];
        double[] sizeFitnessCallsAverage = new double[4];
        for (int i = 0; i < 4; i++)
        {
            sizeBestAverage[i] = runForBest(encTextFilePath, dictFilePath, letterFreqFilePath, 10, "null","ts",populationSize[i],maxSize[i]);
            sizeFitnessCallsAverage[i] = runForBest(encTextFilePath, dictFilePath, letterFreqFilePath, 10, "null","fc",populationSize[i],maxSize[i]);
        }
        string[] labels = { "500", "1000", "2000","3000" };
        var plt = new Plot(600, 400);
        plt.Grid(enable:false, lineStyle: LineStyle.Dot);
        plt.AddBar(sizeBestAverage);
        plt.SetAxisLimits(yMin: 0);
        plt.XTicks(labels);
        plt.Title("Comparison of Average Best Solution");
        plt.YLabel("Best solution value");
        plt.SaveFig("SolutionValuecomparison.png");

        var plt2 = new Plot(600, 400);
        plt2.Grid(enable:false, lineStyle: LineStyle.Dot);
        plt2.AddBar(sizeFitnessCallsAverage);
        plt2.SetAxisLimits(yMin: 0);
        plt2.XTicks(labels);
        plt2.Title("Comparison of Fitness Function Calls");
        plt2.YLabel("Fitness Function Calls");
        plt2.SaveFig("Fitnesscomparison.png");
    }
    
    //ts = top solution;
    // fc = function calls;
    //
    private double runForBest(string encTextFilePath,string dictFilePath,string letterFreqFilePath,int IterationNumber,string opt,string typeCompare,int size,int maxSizes)
    {
        double avarageBestResult = 0;
        // i run only 10 because the algorithm is quite heavy.
        for (int i = 0; i < IterationNumber; i++)
        {
            Console.WriteLine(i);
            GeneticAlgorithm ga = new GeneticAlgorithm(size,maxSizes, encTextFilePath, dictFilePath, letterFreqFilePath);
            while (ga.shouldRun)
            {
                ga.EvolveNextGeneration(size,0.1,20,opt);
            }
    
            if(typeCompare == "ts") avarageBestResult += ga.bestSolution.FitnessValue;
            else if (typeCompare == "fc") avarageBestResult += ga.fitnessFunctionCalls;
        }

        avarageBestResult /= IterationNumber;
        return avarageBestResult;
    }
    


}