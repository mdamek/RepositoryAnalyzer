using System;
using AnalyzeManager.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace AnalyzeManager
{
    public class AdditionalInformationCreator
    {
        public void SaveToFileLimitValues(List<MetricsModel> allMetricsModels)
        {
            var compartmentValues = new
            {
                code = new
                {
                    range = new[] {0, allMetricsModels.Max(e => e.Code)},
                    colors = new[] { "#ffffff", "#ff0000" }
                },
                comment = new
                {
                    range = new[] { 0, allMetricsModels.Max(e => e.Comment) },
                    colors = new[] { "#ffffff", "#ff0000" }
                },
                allcommitsnumber = new
                {
                    range = new[] {0, allMetricsModels.Max(e => e.AllCommitsNumber)},
                    colors = new[] { "#ffffff", "#ff0000" }
                },
                badqualitymetricsnumber = new
                {
                    range = new[] { 0, allMetricsModels.Max(e => e.BadQualityMetricsNumber) },
                    colors = new[] { "#ffffff", "#ff0000" }
                },
                maintainabilityindex = new
                {
                    range = new[] { 0, 100 },
                    colors = new[] { "#ff0000", "#ffffff" }
                },
                cyclomaticcomplexity = new
                {
                    range = new[] { 0, 10 },
                    colors = new[] { "#ffffff", "#ff0000" }
                },
                classcoupling = new
                {
                    range = new[] { 0, 80 },
                    colors = new[] { "#ffffff", "#ff0000" }
                },
                depthofinheritance = new
                {
                    range = new[] { 0, 6 },
                    colors = new[] { "#ffffff", "#ff0000" }
                }
            };
            var jsonValues = JsonConvert.SerializeObject(compartmentValues);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\AnalyzeReport\\CompartmentValues.json", jsonValues);

            var globalValues = new dynamic[]
            {
                new
                {
                    name = "Min",
                    LOC = allMetricsModels.Min(e => e.Code),
                    Comments = allMetricsModels.Min(e => e.Comment),
                    Commits = allMetricsModels.Min(e => e.AllCommitsNumber),          
                    MI = allMetricsModels.Min(e => e.MaintainabilityIndex),
                    Cyclo = allMetricsModels.Min(e => e.CyclomaticComplexity),
                    CC = allMetricsModels.Min(e => e.ClassCoupling),
                    DiT = allMetricsModels.Min(e => e.DepthOfInheritance),
                    All = allMetricsModels.Min(e => e.BadQualityMetricsNumber)
                },
                new
                {
                    name = "Max",
                    LOC = allMetricsModels.Max(e => e.Code),
                    Comments = allMetricsModels.Max(e => e.Comment),
                    Commits = allMetricsModels.Max(e => e.AllCommitsNumber),
                    MI = allMetricsModels.Max(e => e.MaintainabilityIndex),
                    Cyclo = allMetricsModels.Max(e => e.CyclomaticComplexity),
                    CC = allMetricsModels.Max(e => e.ClassCoupling),
                    DiT = allMetricsModels.Max(e => e.DepthOfInheritance),
                    All = allMetricsModels.Max(e => e.BadQualityMetricsNumber),
                },
                new
                {
                    name = "Average",
                    LOC = Math.Round(allMetricsModels.Average(e => e.Code), 2),
                    Comments = Math.Round(allMetricsModels.Average(e => e.Comment), 2),
                    Commits = Math.Round(allMetricsModels.Average(e => e.AllCommitsNumber), 2),
                    MI = Math.Round(allMetricsModels.Average(e => e.MaintainabilityIndex), 2),
                    Cyclo = Math.Round(allMetricsModels.Average(e => e.CyclomaticComplexity), 2),
                    CC = Math.Round(allMetricsModels.Average(e => e.ClassCoupling), 2),
                    DiT = Math.Round(allMetricsModels.Average(e => e.DepthOfInheritance), 2),
                    All = Math.Round(allMetricsModels.Average(e => e.BadQualityMetricsNumber), 2)
                }
            };

                var jsonGlobalValues = JsonConvert.SerializeObject(globalValues);
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\AnalyzeReport\\GlobalValues.json", jsonGlobalValues);
        }

        public void SaveToFileAsJson(List<MetricsModel> allMetricsModels)
        {
            var jsonList = JsonConvert.SerializeObject(allMetricsModels);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\AnalyzeReport\\MetricsRawToArray.json", jsonList);
        }

        public List<MetricsModel> CalculateBadQualityMetrics(List<MetricsModel> finalEntities)
        {
            foreach (var finalEntity in finalEntities)
            {
                if (finalEntity.ClassCoupling > 50) finalEntity.BadQualityMetricsNumber++;
                if (finalEntity.DepthOfInheritance >= 6) finalEntity.BadQualityMetricsNumber++;
                if (finalEntity.CyclomaticComplexity >= 10) finalEntity.BadQualityMetricsNumber++;
                if (finalEntity.MaintainabilityIndex <= 10) finalEntity.BadQualityMetricsNumber++;
                if (finalEntity.AllCommitsNumber > finalEntities.Select(e => e.AllCommitsNumber).Average() * 3 / 4)
                    finalEntity.BadQualityMetricsNumber++;
                if (finalEntity.Comment > 0) finalEntity.BadQualityMetricsNumber++;
                if (finalEntity.Code > finalEntities.Select(e => e.Code).Average() * 3 / 4)
                    finalEntity.BadQualityMetricsNumber++;
            }

            return finalEntities;
        }
    }
}
