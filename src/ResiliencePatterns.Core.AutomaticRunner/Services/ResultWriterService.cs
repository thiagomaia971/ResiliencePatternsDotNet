using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using ResiliencePatterns.Core.AutomaticRunner.Configurations;
using ResiliencePatternsDotNet.DotNet.Commons;

namespace ResiliencePatterns.Core.AutomaticRunner.Services
{
    public class ResultWriterService
    {
        private readonly AutomaticRunnerConfiguration _automaticRunnerConfiguration;
        private IDictionary<ResultType, Action<Scenario>> resultTypeHandler = new Dictionary<ResultType, Action<Scenario>>
        {
            { ResultType.TXT, WriteTxt },
            { ResultType.CSV, WriteCsv },
        };

        public ResultWriterService(AutomaticRunnerConfiguration automaticRunnerConfiguration) 
            => _automaticRunnerConfiguration = automaticRunnerConfiguration;

        public void Write(Scenario scenario) 
            => resultTypeHandler[scenario.ResultType](scenario);

        private static void WriteTxt(Scenario scenario)
        {
            var count = 1;
            foreach (var result in scenario.Results)
            {
                var contentJsonUnPrettyfied = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var jsonElement = JsonConvert.DeserializeObject(contentJsonUnPrettyfied);

                using var streamWriter = new StreamWriter($"{scenario.Directory}\\{scenario.FileNameWithoutExtension}[{count}].result");
                streamWriter.Write(JsonConvert.SerializeObject(jsonElement, Formatting.Indented));
                count++;
            }
        }

        private static void WriteCsv(Scenario scenario)
        {
            lock (scenario)
            {
                if (File.Exists(scenario.ResultPath)) 
                    return;
                
                using (var streamWriter =
                    new StreamWriter(scenario.ResultPath))
                {
                    WriteHeaderCsv(streamWriter);
                    foreach (var httpResponseMessage in scenario.Results)
                    {
                        var contentJsonUnPrettyfied = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var jsonElement = JsonConvert.DeserializeObject<MetricStatus>(contentJsonUnPrettyfied);
                        
                        streamWriter.WriteLine(jsonElement.GetCsvLine());
                    }
                }
            }
        }

        private static void WriteHeaderCsv(TextWriter streamWriter) 
            => streamWriter.WriteLine(MetricStatus.GetCsvHeader());
    }
}