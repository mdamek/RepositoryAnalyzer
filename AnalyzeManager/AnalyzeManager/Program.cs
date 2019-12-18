using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using AnalyzeManager.Models;
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

            var doc = new XmlDocument();
            var BasicMetricsPath = args[0].Split("\\");
            BasicMetricsPath[BasicMetricsPath.Length - 1] = "BasicMetrics.xml";
            doc.Load(string.Join("\\", BasicMetricsPath));
            var basicMetrics = new List<XmlMetricsModel>();
            var elemList = doc.GetElementsByTagName("NamedType");
            for (int i = 0; i < elemList.Count; i++)
            {
                var actualNode = elemList[i];
                var childsMetrics = actualNode["Metrics"].ChildNodes.OfType<XmlElement>();
                basicMetrics.Add(new XmlMetricsModel
                {
                    Name = actualNode.Attributes["Name"].Value,
                    MaintainabilityIndex = int.Parse(childsMetrics.First(e => e.Attributes["Name"].Value == "MaintainabilityIndex").Attributes["Value"].Value),
                    ClassCoupling = int.Parse(childsMetrics.First(e => e.Attributes["Name"].Value == "ClassCoupling").Attributes["Value"].Value),
                    CyclomaticComplexity = int.Parse(childsMetrics.First(e => e.Attributes["Name"].Value == "CyclomaticComplexity").Attributes["Value"].Value),
                    DepthOfInheritance = int.Parse(childsMetrics.First(e => e.Attributes["Name"].Value == "DepthOfInheritance").Attributes["Value"].Value),
                    ExecutableLines = int.Parse(childsMetrics.First(e => e.Attributes["Name"].Value == "ExecutableLines").Attributes["Value"].Value),
                });
            }

            var filesWithoutMetrics = new List<string>();
            var finalEntities = new List<AllMetricsModel>();

            foreach (var xmlMetricsModel in basicMetrics)
            {
                if (filesStatisticsWithCommits.Any(e => e.FileFullName.Split("\\").Last().Contains(xmlMetricsModel.Name)))
                {
                    var elementIndex = filesStatisticsWithCommits.FindIndex(e => e.FileFullName.Contains(xmlMetricsModel.Name));
                    finalEntities.Add(new AllMetricsModel
                    {
                        AllCommitsNumber = filesStatisticsWithCommits[elementIndex].AllCommitsNumber,
                        Code = filesStatisticsWithCommits[elementIndex].Code,
                        Comment = filesStatisticsWithCommits[elementIndex].Comment,
                        FileFullName = filesStatisticsWithCommits[elementIndex].FileFullName,
                        ClassCoupling = xmlMetricsModel.ClassCoupling,
                        CyclomaticComplexity = xmlMetricsModel.CyclomaticComplexity,
                        DepthOfInheritance = xmlMetricsModel.DepthOfInheritance,
                        TypeName = xmlMetricsModel.Name,
                        MaintainabilityIndex = xmlMetricsModel.MaintainabilityIndex,
                        BadQualityMetricsNumber = 0
                    });
                }
                else
                {
                    filesWithoutMetrics.Add(xmlMetricsModel.Name);
                }
            }

            CalculateNumberBadQualityMetrics(finalEntities);

            var additionalInformationsCreator = new AdditionalInformationsCreator();
            additionalInformationsCreator.SaveToFileAllCompartments(finalEntities);
            additionalInformationsCreator.SaveToFileRawDataJson(finalEntities);

            var readyTreeObjectStructure = statisticsTransform.GenerateTreeObjectStructureFromPaths(finalEntities);
            var allStatistics = JsonConvert.SerializeObject(readyTreeObjectStructure);

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\OutputsFiles\\FinalStatisticsOutput.json", allStatistics);
        }

        private static void CalculateNumberBadQualityMetrics(List<AllMetricsModel> finalEntities)
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
        }
    }
}