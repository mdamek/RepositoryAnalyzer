using AnalyzeManager.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace AnalyzeManager
{
    public class AdditionalInformationsCreator
    {
        public void SaveToFileAllCompartments(List<AllMetricsModel> allMetricsModels)
        {
            var compartmentValues = new
            {
                code = new
                {
                    range = new int[] {0, allMetricsModels.Max(e => e.Code)},
                    colors = new string[] { "#00ff2b", "#ff0000" }
                },
                comment = new
                {
                    range = new int[] { 0, allMetricsModels.Max(e => e.Comment) },
                    colors = new string[] { "#00ff2b", "#ff0000" }
                },
                allcommitsnumber = new
                {
                    range = new int[] {0, allMetricsModels.Max(e => e.AllCommitsNumber)},
                    colors = new string[] { "#00ff2b", "#ff0000" }
                },
                maintainabilityindex = new
                {
                    range = new int[] { 0, 100 },
                    colors = new string[] { "#ff0000", "#00ff2b" }
                },
                cyclomaticcomplexity = new
                {
                    range = new int[] { 0, 10 },
                    colors = new string[] { "#00ff2b", "#ff0000" }
                },
                classcoupling = new
                {
                    range = new int[] { 0, 80 },
                    colors = new string[] { "#ff0000", "#fff400", "#00ff2b" }
                },
                depthofinheritance = new
                {
                    range = new int[] { 0, 6 },
                    colors = new string[] { "#00ff2b", "#ff0000" }
                },
                badqualitymetricsnumber = new
                {
                    range = new int[] { 0, allMetricsModels.Max(e => e.BadQualityMetricsNumber) },
                    colors = new string[] { "#00ff2b", "#ff0000" }
                }
            };
            var jsoncompartmentValues = JsonConvert.SerializeObject(compartmentValues);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\AnalyzeReport\\CompartmentValues.json", jsoncompartmentValues);
        }

        public void SaveToFileRawDataJson(List<AllMetricsModel> allMetricsModels)
        {
            var jsonList = JsonConvert.SerializeObject(allMetricsModels);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\OutputsFiles\\MetricsRaw.json", jsonList);
        }
    }
}
