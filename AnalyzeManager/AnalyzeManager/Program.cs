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
                        Blank = filesStatisticsWithCommits[elementIndex].Blank,
                        Code = filesStatisticsWithCommits[elementIndex].Code,
                        Comment = filesStatisticsWithCommits[elementIndex].Comment,
                        FileFullName = filesStatisticsWithCommits[elementIndex].FileFullName,
                        Language = filesStatisticsWithCommits[elementIndex].Language,
                        ClassCoupling = xmlMetricsModel.ClassCoupling,
                        CyclomaticComplexity = xmlMetricsModel.CyclomaticComplexity,
                        DepthOfInheritance = xmlMetricsModel.DepthOfInheritance,
                        ExecutableLines = xmlMetricsModel.ExecutableLines,
                        TypeName = xmlMetricsModel.Name,
                        MaintainabilityIndex = xmlMetricsModel.MaintainabilityIndex
                    });
                }
                else
                {
                    filesWithoutMetrics.Add(xmlMetricsModel.Name);
                }
            }

            var readyTreeObjectStructure = statisticsTransform.GenerateTreeObjectStructureFromPaths(finalEntities);
            var allStatistics = JsonConvert.SerializeObject(readyTreeObjectStructure);

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\OutputsFiles\\FinalStatisticsOutput.json", allStatistics);
        }




    }
}