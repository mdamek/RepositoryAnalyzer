using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyzeManager.Models
{
    public class FileCodeStatistics
    {
        public string FileFullName { get; set; }
        public int Code { get; set; }
        public int Blank { get; set; }
        public int Comment { get; set; }
        public string Language { get; set; }
    }
}
