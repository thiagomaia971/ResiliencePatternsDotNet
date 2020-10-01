using System.Collections.Generic;
using System.Net.Http;

namespace ResiliencePatterns.Core.AutomaticRunner.Configurations
{
    public class Scenario
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string FileNameWithoutExtension { get; set; }
        
        public int Count { get; set; }
        public UrlFetchConfigurationSection UrlFetch { get; set; }
        public ResultType ResultType { get; set; }
        public object Parameters { get; set; }

        public string ResultPath => $"{Directory}\\{FileNameWithoutExtension}-result.csv";
        public List<HttpResponseMessage> Results { get; set; }
        public Scenario() => Results = new List<HttpResponseMessage>();
    }
}