using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchiMetrics.Analysis;
using ArchiMetrics.Analysis.Common;
using Newtonsoft.Json;

namespace AnalyzeManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            var jsonStatisticsRaw = File.ReadAllText(args[0]);
            var pathToTestedRepository = args[1];
            var jsonFormatter = new JsonFormatter();
            var readableJson = jsonFormatter.FormatJsonOutput(jsonStatisticsRaw);


            var task =  CalculateBasicMetrics(pathToTestedRepository + "\\" + pathToTestedRepository.Split('\\').Last() + ".sln");
            task.Wait();


            var filesStatistics = jsonFormatter.ConvertJsonToPlainObjectRepresentation(readableJson);
            var statisticsTransform = new StatisticsDataObjectsTransform();
            var allFilesData = statisticsTransform.GenerateStatisticsContainer(filesStatistics);
            var gitConnector = new GitManager(pathToTestedRepository);
            var filesStatisticsWithCommits = gitConnector.AddCommitsNumbersToFiles(allFilesData);
            var readyTreeObjectStructure = statisticsTransform.GenerateTreeObjectStructureFromPaths(filesStatisticsWithCommits);            
            var allStatistics = JsonConvert.SerializeObject(readyTreeObjectStructure);

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\..\\OutputsFiles\\FinalStatisticsOutput.json", allStatistics);
        }

        private static async Task CalculateBasicMetrics(string pathToRepository)
        {
            var solutionProvider = new SolutionProvider();
            var solution = await solutionProvider.Get(pathToRepository);
            var projects = solution.Projects.ToList();
            var metricsCalculator = new CodeMetricsCalculator();
            var asd = await metricsCalculator.Calculate(projects[3], solution);
            var calculateTasks = projects.Select(p => metricsCalculator.Calculate(p, solution));
            var metrics = (await Task.WhenAll(calculateTasks)).SelectMany(nm => nm);
        }



    }
}