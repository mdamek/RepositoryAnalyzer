namespace AnalyzeManager.Models
{
    public class BasicMetricsModel
    {
        public string Name { get; set; }
        public int MaintainabilityIndex { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int ClassCoupling { get; set; }
        public int DepthOfInheritance { get; set; }
        public int ExecutableLines { get; set; }
    }
}
