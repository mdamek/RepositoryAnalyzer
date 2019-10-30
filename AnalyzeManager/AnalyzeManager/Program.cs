using System.IO;
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
            var allFilesStatistics = jsonFormatter.ConvertJsonToPlainObjectRepresentation(readableJson);
            var statisticsTransform = new StatisticsDataObjectsTransform();
            var allFilesData = statisticsTransform.GenerateStatisticsContainer(allFilesStatistics);
            var gitConnector = new GitManager(pathToTestedRepository);
            var filesStatisticsWithCommits = gitConnector.AddCommitsNumbersToFiles(allFilesData);
            var readyTreeObjectStructure = statisticsTransform.GenerateTreeObjectStructureFromPaths(filesStatisticsWithCommits);
            var allStatistics = JsonConvert.SerializeObject(readyTreeObjectStructure);

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\OutputsFiles\\FinalStatisticsOutput.json", allStatistics);
        }




    }
}