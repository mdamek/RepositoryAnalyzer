namespace AnalyzeManager.Models
{
    public class AllMetricsModel
    {
        public string FileFullName { get; set; }
        public int Code { get; set; }
        public int Blank { get; set; }
        public int Comment { get; set; }
        public string Language { get; set; }
        public int AllCommitsNumber { get; set; }
        public string TypeName { get; set; }
        public int MaintainabilityIndex { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int ClassCoupling { get; set; }
        public int DepthOfInheritance { get; set; }
        public int ExecutableLines { get; set; }
    }
}
