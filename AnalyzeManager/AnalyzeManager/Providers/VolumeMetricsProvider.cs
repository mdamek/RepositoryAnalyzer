using System.Collections.Generic;
using System.IO;
using AnalyzeManager.Models;

namespace AnalyzeManager
{
    public class VolumeMetricsProvider
    {
        private List<MetricsModel> FileCodeStatistics { get; set; }
        private string PathToVolumeMetrics { get; }

        public VolumeMetricsProvider(string pathToVolumeMetrics)
        {
            PathToVolumeMetrics = pathToVolumeMetrics + "\\CodeStatisticsOutput.txt";
        }

        public List<MetricsModel> ProvideVolumeMetrics()
        {
            var jsonStatisticsRaw = File.ReadAllText(PathToVolumeMetrics);
            var jsonFormatter = new JsonFormatter();
            var allFilesStatistics = jsonFormatter.ConvertJsonToPlainObjectRepresentation(jsonStatisticsRaw);
            
            var allFilesData = new List<MetricsModel>();
            foreach (var (fullFileName, details) in allFilesStatistics)
            {
                var fileCodeStatistics = new MetricsModel
                {
                    Code = int.Parse(details["code"].ToString()),
                    Comment = int.Parse(details["comment"].ToString()),
                    FileFullName = fullFileName,
                };
                allFilesData.Add(fileCodeStatistics);
            }
            return allFilesData;
        }
    }
}
