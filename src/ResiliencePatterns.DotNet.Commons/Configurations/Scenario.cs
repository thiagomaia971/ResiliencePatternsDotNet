﻿using System.Collections.Generic;
using System.Net.Http;

namespace ResiliencePatternsDotNet.Commons.Configurations
{
    public class Scenario
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string FileNameWithoutExtension { get; set; }
        public int SubScenario { get; set; }

        public bool Run { get; set; }
        public int[] Count { get; set; }
        public UrlFetchConfigurationSection UrlFetch { get; set; }
        public ProxyConfigurationSection ProxyConfiguration { get; set; }
        public ResultType ResultType { get; set; }
        public object Parameters { get; set; }

        public string ResultPath => $"{Directory}\\[{SubScenario}]{FileNameWithoutExtension}-result.{ResultType.ToString().ToLower()}";
        public List<MetricStatus> Results { get; set; }
        public bool AsyncClients { get; set; }

        public Scenario() => Results = new List<MetricStatus>();
    }
}