﻿using System.Collections.Generic;
using System.Linq;
using AnalyzeManager.Models;

namespace AnalyzeManager.Tools
{
    public class MetricsAggregator
    {
        public List<MetricsModel> AggregateMetrics(List<BasicMetricsModel> basicMetrics, List<MetricsModel> volumeMetricsWithCommits)
        {
            var aggregatedMetrics = new List<MetricsModel>();
            var ids = 0;
            foreach (var xmlMetricsModel in basicMetrics)
            {
                if (volumeMetricsWithCommits.Any(e => e.FileFullName.Split("\\").Last().Split(".")[0].ToLower() == xmlMetricsModel.Name.ToLower()))
                {
                    var elementIndex = volumeMetricsWithCommits.FindIndex(e => e.FileFullName.Split("\\").Last().Split(".")[0].ToLower() == xmlMetricsModel.Name.ToLower());
                    aggregatedMetrics.Add(new MetricsModel
                    {
                        Id = ++ids,
                        AllCommitsNumber = volumeMetricsWithCommits[elementIndex].AllCommitsNumber,
                        Code = volumeMetricsWithCommits[elementIndex].Code,
                        Comment = volumeMetricsWithCommits[elementIndex].Comment,
                        FileFullName = volumeMetricsWithCommits[elementIndex].FileFullName,
                        ClassCoupling = xmlMetricsModel.ClassCoupling,
                        CyclomaticComplexity = xmlMetricsModel.CyclomaticComplexity,
                        DepthOfInheritance = xmlMetricsModel.DepthOfInheritance,
                        TypeName = xmlMetricsModel.Name,
                        MaintainabilityIndex = xmlMetricsModel.MaintainabilityIndex,
                        BadQualityMetricsNumber = 0,
                        NameWithType = volumeMetricsWithCommits[elementIndex].FileFullName.Split("\\").Last()
                    });
                }
            }
            var additionalInformationCreator = new AdditionalInformationCreator();
            aggregatedMetrics = additionalInformationCreator.CalculateBadQualityMetrics(aggregatedMetrics);
            return aggregatedMetrics;
        }
    }
}
