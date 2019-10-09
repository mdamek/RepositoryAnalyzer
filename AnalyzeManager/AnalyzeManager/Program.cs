using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using AnalyzeManager.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnalyzeManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonStatisticsRaw = File.ReadAllText(args[0]);
            var jsonStatisticsReadable = Regex.Replace(jsonStatisticsRaw, @"[^,\\]\\[^,\\]" , e => Regex.Replace(e.Value, @"\\", "\\\\") );
            var allFilesStatisticsRaw = JObject.Parse(jsonStatisticsReadable);
            allFilesStatisticsRaw.First.Remove();
            allFilesStatisticsRaw.Last.Remove();
            var allFilesData = new List<FileCodeStatistics>();
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
        }
    }
}
