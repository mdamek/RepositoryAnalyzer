using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace AnalyzeManager
{
    public class JsonFormatter
    {
        public string FormatJsonOutput(string json)
        {
            return Regex.Replace(json, @"[^,\\]\\[^,\\]",
                e => Regex.Replace(e.Value, @"\\", "\\\\"));
        }

        public JObject ConvertJsonToPlainObjectRepresentation(string json)
        {
            var allFilesStatisticsRaw = JObject.Parse(json);
            allFilesStatisticsRaw.First.Remove();
            allFilesStatisticsRaw.Last.Remove();
            return allFilesStatisticsRaw;
        }
    }
}
