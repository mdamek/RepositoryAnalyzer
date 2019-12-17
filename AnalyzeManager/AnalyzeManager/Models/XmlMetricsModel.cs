using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyzeManager.Models
{
    public class XmlMetricsModel
    {
        public string Name { get; set; }
        public int MaintainabilityIndex { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int ClassCoupling { get; set; }
        public int DepthOfInheritance { get; set; }
        public int ExecutableLines { get; set; }
    }
}
