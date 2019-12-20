using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace AnalyzeManager
{
    public class JsonFormatter
    {
        public JObject ConvertJsonToPlainObjectRepresentation(string json)
        {
            var validJson = Regex.Replace(json, @"[^,\\]\\[^,\\]",
                e => Regex.Replace(e.Value, @"\\", "\\\\"));
            var allFilesStatisticsRaw = JObject.Parse(validJson);
            allFilesStatisticsRaw.First.Remove();
            allFilesStatisticsRaw.Last.Remove();
            return allFilesStatisticsRaw;
        }
    }
}
