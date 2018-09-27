using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using MO_QAP.QapStrategies;
using System.Diagnostics;
using System.Timers;

namespace MO_QAP
{
    class Program
    {
        const int millisecondsInSeconds = 1000;
        static void HandleHelp()
        {
            System.Console.WriteLine("Ussage:");
            System.Console.WriteLine("MO_QAP [-h]                                   : show this help");
            System.Console.WriteLine("MO_QAP <file> <timeout> <runs> [<output>]     : run analysis");
            System.Console.WriteLine("  <file> - input file to use");
            System.Console.WriteLine("  <timeout> - time that algorithms will run for in seconds");
            System.Console.WriteLine("  <runs> - how many times should each algorithm be run");
            System.Console.WriteLine("  <output> - output file in csv forma");
            System.Console.WriteLine();
            System.Console.WriteLine("Each run starts with randomly generated permutation");
            System.Console.WriteLine("Initial permutation is the same, for each algorithm");
            Environment.Exit(0);
        }
        static void Main(string[] args)
        {
            if(args.Contains("-h")) HandleHelp();

            
            var file = args[0];
            var timeout = int.Parse(args[1]);
            var runs = int.Parse(args[2]);
            var output = args.Count() == 4
                ? args[3]
                : string.Empty;

            System.Console.WriteLine($"Input params:");
            System.Console.WriteLine($"File: {file}");
            System.Console.WriteLine($"Timeout: {timeout}[s]");
            System.Console.WriteLine($"Runs: {runs}");
            System.Console.WriteLine($"Output: {output}");


            System.Console.WriteLine($"Reading data from input file");
            var data = ReadData(file);
            System.Console.WriteLine("OK");

            var randomResults = new List<QapResult<int>>();
            var steepestResults = new List<QapResult<int>>();
            var greedyResults = new List<QapResult<int>>();
            

            for(var currentRun = 1; currentRun<=runs; currentRun++)
            {
                System.Console.WriteLine();
                System.Console.WriteLine($"### Run: {currentRun} ###");
                
                var initialPermutation = QapMath.Permutate(QapMath.Range(1,data.MatricesSize));
                System.Console.WriteLine($"Initial permutation: {string.Join(",",initialPermutation)}");
                
                var cancelationToken = new CancelationToken();
                
                var randomStrategy = new RandomStrategy();
                var steepestStrategy = new SteepestStrategy();
                var greedyStrategy = new GreedyStrategy();

                Func<IEnumerable<int>, float> scoringFunction = (x)=>Score(data,x);

                var timer = new Timer(timeout*millisecondsInSeconds);
                timer.Elapsed += (sender,eargs) => cancelationToken.Cancel();
                
                System.Console.WriteLine("Starting tasks");
                var randomStartegyTask = Task.Run(()=>randomStrategy.SearchBest(scoringFunction, initialPermutation, cancelationToken));
                var steepestStrategyTask = Task.Run(()=>randomStrategy.SearchBest(scoringFunction, initialPermutation, cancelationToken));
                var greedyStrategyTask = Task.Run(()=>greedyStrategy.SearchBest(scoringFunction, initialPermutation, cancelationToken));
                
                System.Console.WriteLine("OK");

                System.Console.WriteLine("Starting timer");
                timer.Start();
                System.Console.WriteLine("OK");

                System.Console.WriteLine("Awaiting tasks to finish");
                Task.WaitAll(new []{randomStartegyTask, steepestStrategyTask,greedyStrategyTask});
                System.Console.WriteLine("OK");

                System.Console.WriteLine("Collecting results");
                QapResult<int> randomResult = randomStartegyTask.Result as QapResult<int> ?? null;
                QapResult<int> steepestResult = steepestStrategyTask.Result as QapResult<int> ?? null;
                QapResult<int> greedyResult = greedyStrategyTask.Result as QapResult<int> ?? null;

                randomResults.Add(randomResult);
                steepestResults.Add(steepestResult);
                greedyResults.Add(greedyResult);

                System.Console.WriteLine("OK");

                System.Console.WriteLine();
                System.Console.WriteLine($"# Results for run: {currentRun}");
                System.Console.WriteLine("Results in format <foundIntime>:<solution>/<ofTotalSeen>:<steps>:<score>:<Permutation>");
                System.Console.WriteLine($"Best random: {randomResult.FoundIn.TotalMilliseconds}:{randomResult.SeenAsSolutionNumber}/{randomResult.TotalSolutionsSeen}:{randomResult.Steps}:{randomResult.Score}:[{String.Join(",",randomResult.Solution)}]");
                System.Console.WriteLine($"Best steepest: {steepestResult.FoundIn.TotalMilliseconds}:{steepestResult.SeenAsSolutionNumber}/{steepestResult.TotalSolutionsSeen}:{steepestResult.Steps}:{steepestResult.Score}:[{String.Join(",",steepestResult.Solution)}]");
                System.Console.WriteLine($"Best greedy: {greedyResult.FoundIn.TotalMilliseconds}:{greedyResult.SeenAsSolutionNumber}/{greedyResult.TotalSolutionsSeen}:{greedyResult.Steps}:{greedyResult.Score}:[{String.Join(",",greedyResult.Solution)}]");
            }

            if(!string.IsNullOrEmpty(output))
            {
                using(var writer = new StreamWriter(File.OpenWrite(output)))
                {
                    writer.WriteLine("algorithm,time,solutionNr,totalSolutions,steps,score,solution");

                    foreach(var solution in randomResults)
                    {
                        writer.WriteLine($"random,{solution.ToCsvLine()}");
                    }
                    foreach(var solution in steepestResults)
                    {
                        writer.WriteLine($"steepest,{solution.ToCsvLine()}");
                    }
                    foreach(var solution in greedyResults)
                    {
                        writer.WriteLine($"greedy,{solution.ToCsvLine()}");
                    }
                }
            }
        }

        static float Score<T>(DataMatrices data, IEnumerable<T> permutation)
        {
            var permutationArray = permutation.Cast<int>().Select(x=>x-1).ToArray();
            var score = 0f;
            var maxIndex = permutation.Count()-1;
            
            foreach(var i in QapMath.Range(0,maxIndex))
                foreach(var j in QapMath.Range(0,maxIndex))
                {
                    score += data.MatrixA[i,j] * data.MatrixB[permutationArray[i],permutationArray[j]];
                }
            return score;
        }

        static DataMatrices ReadData(string path)
        {

            int[,] ParseMatrix(int size, IEnumerable<string> rawData)
            {
                int[,] matrix = new int[size,size];
                
                int x = 0;
                foreach(var line in rawData)
                {
                    var values = line.Split(' ');
                    int y = 0;
                    foreach(var value in values.Where(v=>!string.IsNullOrEmpty(v)))
                    {
                        matrix[x,y] = int.Parse(value);
                        y++;
                    }
                    x++;
                }
                return matrix;
            }

            var data = File.ReadAllLines(path).Where(x=>!string.IsNullOrEmpty(x)).ToArray();
            var result = new DataMatrices(int.Parse(data[0].Trim()));

            data = data.Skip(1).ToArray();

            var rawMatrixA = data.Take(result.MatricesSize);
            var rawMatrixb = data.Skip(result.MatricesSize);

            result.MatrixA = ParseMatrix(result.MatricesSize, rawMatrixA);
            result.MatrixB = ParseMatrix(result.MatricesSize, rawMatrixb);

            return result;
        }
    }
}
