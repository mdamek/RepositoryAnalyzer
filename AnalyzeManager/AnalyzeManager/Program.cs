using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AnalyzeManager.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnalyzeManager
{
    public class Program
    {
        private static List<FileCodeStatistics> FileCodeStatistics { get; set; }
        static void Main(string[] args)
        {
            var jsonStatisticsRaw = File.ReadAllText(args[0]);
            var jsonStatisticsReadable = Regex.Replace(jsonStatisticsRaw, @"[^,\\]\\[^,\\]",
                e => Regex.Replace(e.Value, @"\\", "\\\\"));
            var allFilesStatisticsRaw = JObject.Parse(jsonStatisticsReadable);
            allFilesStatisticsRaw.First.Remove();
            allFilesStatisticsRaw.Last.Remove();
            var allFilesData = new List<FileCodeStatistics>();
            FileCodeStatistics = allFilesData;
            foreach (var (fullFileName, details) in allFilesStatisticsRaw)
            {
                var fileCodeStatistics = new FileCodeStatistics()
                {
                    Code = int.Parse(details["code"].ToString()),
                    Blank = int.Parse(details["blank"].ToString()),
                    Comment = int.Parse(details["comment"].ToString()),
                    Language = details["language"].ToString(),
                    FileFullName = fullFileName
                };
                allFilesData.Add(fileCodeStatistics);
            }

            var largestAmountOfFilesLanguage = allFilesData
                .GroupBy(e => e.Language)
                .First(c => c.Count() == allFilesData.GroupBy(e => e.Language)
                                .Select(e => e.Count()).Max())
                .First()
                .Language;

            var coreProjectFiles = allFilesData.Where(e => e.Language == largestAmountOfFilesLanguage).ToList();

            void GoInsideTree(Node node, IEnumerable<string> pathParts)
            {
                var paths = pathParts.ToList();
                if (!paths.Any()) return;
                var name = paths.First();
                var child = node.Children.SingleOrDefault(x => x.Name == name);
                if (child == null)
                {
                    child = new Node
                    {
                        Name = name,
                        Children = new List<Node>(),
                    };
                    node.Children.Add(child);
                }

                GoInsideTree(child, paths.Skip(1));
            }

            var rootNode = new Node { Name = "/", Children = new List<Node>() };

            foreach (var actualFile in allFilesData)
            {
                var path = actualFile.FileFullName;
                var separatedValuesOfPath = path.Replace("\\\\", "\\").Split('\\');
                GoInsideTree(rootNode, separatedValuesOfPath);
            }

            var rawJson = JsonConvert.SerializeObject(rootNode);

            var objectJson = JToken.Parse(rawJson);
            var objectJsonWithoutArtificialRoot = objectJson["children"][0];
            GoRecursiveCreateLeafs(objectJsonWithoutArtificialRoot);
            var readyJson = JsonConvert.SerializeObject(objectJsonWithoutArtificialRoot);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\..\\OutputsFiles\\FinalStatisticsOutput.json", readyJson);
        }


        private static void GoRecursiveCreateLeafs(JToken jToken)
        {
            if (!jToken["children"].Children().Any())
            {
                var name = jToken["name"];
                var dataToAppend = FileCodeStatistics.First(e => e.FileFullName.Contains(name.ToString()));

                jToken["children"].Parent.Remove();
                foreach (var property in dataToAppend.GetType().GetProperties())
                {
                    var isInt = int.TryParse(property.GetValue(dataToAppend).ToString(), out var intNumber);
                    var isDouble = double.TryParse(property.GetValue(dataToAppend).ToString(), out var doubleNumber);
                    if (isInt)
                    {
                        jToken[property.Name.ToLower()] = intNumber;
                    }
                    else if (isDouble)
                    {
                        jToken[property.Name.ToLower()] = doubleNumber;
                    }
                    else
                    {
                        jToken[property.Name.ToLower()] = property.GetValue(dataToAppend).ToString();
                    }
                }
                return;
            }
            foreach (var nestedJToken in jToken["children"].Children())
            {
                GoRecursiveCreateLeafs(nestedJToken);
            }
        }
    }
}