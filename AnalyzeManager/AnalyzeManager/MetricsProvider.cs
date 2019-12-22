using System.IO;
using System.Linq;
using AnalyzeManager.Providers;
using AnalyzeManager.Tools;
using Newtonsoft.Json;

namespace AnalyzeManager
{
    public static class MetricsProvider
    {
        static void Main(string[] args)
        {
            var pathToFolderWithMetrics = args[0];
            var pathToTestedRepository = args[1];
            
            var volumeMetricsProvider = new VolumeMetricsProvider(pathToFolderWithMetrics);
            var allFilesData = volumeMetricsProvider.ProvideVolumeMetrics();

            var gitConnector = new GitProvider(pathToTestedRepository);
            var volumeMetricsWithCommits = gitConnector.AddCommitsMetrics(allFilesData);

            var basicMetricsProvider = new BasicMetricsProvider(pathToFolderWithMetrics);
            var basicMetrics = basicMetricsProvider.ProvideBasicMetrics();

            var metricsAggregator = new MetricsAggregator();
            var aggregatedMetrics = metricsAggregator.AggregateMetrics(basicMetrics, volumeMetricsWithCommits);

            var additionalInformationCreator = new AdditionalInformationCreator();
            additionalInformationCreator.SaveToFileLimitValues(aggregatedMetrics);
            additionalInformationCreator.SaveToFileAsJson(aggregatedMetrics);

            var treeStructureConverter = new TreeStructureConverter();
            var treeStructureMetrics = treeStructureConverter.GenerateTreeStructureFromPaths(aggregatedMetrics);
            var jsonTreeStructureMetrics = JsonConvert.SerializeObject(treeStructureMetrics);

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\OutputsFiles\\FinalStatisticsOutput.json", jsonTreeStructureMetrics);
        }
    }
}