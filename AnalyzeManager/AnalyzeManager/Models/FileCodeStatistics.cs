namespace AnalyzeManager.Models
{
    public class FileCodeStatistics
    {
        public string FileFullName { get; set; }
        public int Code { get; set; }
        public int Blank { get; set; }
        public int Comment { get; set; }
        public string Language { get; set; }
        public int AllCommitsNumber { get; set; }
        public int FixCommitsNumber { get; set; }
    }
}
