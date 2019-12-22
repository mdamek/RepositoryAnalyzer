using System.Collections.Generic;
using System.Linq;
using AnalyzeManager.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnalyzeManager.Tools
{
    public class TreeStructureConverter
    {
        private List<MetricsModel> Metrics { get; set; }
        public JToken GenerateTreeStructureFromPaths(List<MetricsModel> allFilesData)
        {
            Metrics = allFilesData;
            void GoInsideTree(TreeNode node, IEnumerable<string> pathParts)
            {
                var paths = pathParts.ToList();
                if (!paths.Any()) return;
                var name = paths.First();
                var child = node.Children.SingleOrDefault(x => x.Name == name);
                if (child == null)
                {
                    child = new TreeNode
                    {
                        Name = name,
                        Children = new List<TreeNode>(),
                    };
                    node.Children.Add(child);
                }

                GoInsideTree(child, paths.Skip(1));
            }

            var rootNode = new TreeNode { Name = "/", Children = new List<TreeNode>() };

            foreach (var actualFile in allFilesData)
            {
                var path = actualFile.FileFullName;
                var separatedValuesOfPath = path.Replace("\\\\", "\\").Split('\\');
                GoInsideTree(rootNode, separatedValuesOfPath);
            }

            var rawJson = JsonConvert.SerializeObject(rootNode);

            var objectJson = JToken.Parse(rawJson);
            var ready = objectJson["children"][0];
            CreateLeaf(ready);
            return ready;
        }

        private void CreateLeaf(JToken jToken)
        {
            if (!jToken["children"].Children().Any())
            {
                var name = jToken["name"];
                var dataToAppend = Metrics.First(e => e.FileFullName.Contains(name.ToString()));

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
                CreateLeaf(nestedJToken);
            }
        }
    }
}
