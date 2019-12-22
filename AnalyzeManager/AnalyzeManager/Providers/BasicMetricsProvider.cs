using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AnalyzeManager.Models;

namespace AnalyzeManager.Providers
{
    public class BasicMetricsProvider
    {
        private string BasicMetricsPath { get; set; }
        public BasicMetricsProvider(string pathToFolderWithBasicMetrics)
        {
            BasicMetricsPath = pathToFolderWithBasicMetrics + "\\BasicMetrics.xml";
        }

        public List<BasicMetricsModel> ProvideBasicMetrics()
        {
            var doc = new XmlDocument();
            doc.Load(BasicMetricsPath);
            var basicMetrics = new List<BasicMetricsModel>();
            var elemList = doc.GetElementsByTagName("NamedType");
            for (var i = 0; i < elemList.Count; i++)
            {
                var actualNode = elemList[i];
                var childNode = actualNode["Metrics"].ChildNodes.OfType<XmlElement>();
                basicMetrics.Add(new BasicMetricsModel
                {
                    Name = actualNode.Attributes["Name"].Value,
                    MaintainabilityIndex = int.Parse(
                        childNode.First(e => e.Attributes["Name"].Value == "MaintainabilityIndex").Attributes["Value"].Value),
                    ClassCoupling = int.Parse(
                        childNode.First(e => e.Attributes["Name"].Value == "ClassCoupling").Attributes["Value"].Value),
                    CyclomaticComplexity = int.Parse(
                        childNode.First(e => e.Attributes["Name"].Value == "CyclomaticComplexity").Attributes["Value"].Value),
                    DepthOfInheritance = int.Parse(
                        childNode.First(e => e.Attributes["Name"].Value == "DepthOfInheritance").Attributes["Value"].Value),
                });
            }


            var singleValues = basicMetrics.GroupBy(x => x.Name)
                .Select(group => group.First())
                .ToList();


            return singleValues;
        }
    }
}
