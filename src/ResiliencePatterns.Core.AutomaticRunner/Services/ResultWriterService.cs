using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using ResiliencePatternsDotNet.Commons;
using ResiliencePatternsDotNet.Commons.Configurations;

namespace ResiliencePatterns.Core.AutomaticRunner.Services
{
    public class ResultWriterService
    {
        private readonly AutomaticRunnerConfiguration _automaticRunnerConfiguration;
        private IDictionary<ResultType, Action<Scenario>> resultTypeHandler = new Dictionary<ResultType, Action<Scenario>>
        {
            { ResultType.TXT, WriteTxt },
            { ResultType.CSV, WriteCsv },
            { ResultType.JSON, WriteJson },
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
                using var streamWriter = new StreamWriter($"{scenario.Directory}\\{scenario.FileNameWithoutExtension}[{count}].result");
                streamWriter.Write(JsonConvert.SerializeObject(result, Formatting.Indented));
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
                        streamWriter.WriteLine(httpResponseMessage.GetCsvLine());
                }
            }
        }

        private static void WriteJson(Scenario scenario)
        {
            lock (scenario)
            {
                if (File.Exists(scenario.ResultPath)) 
                    return;
                
                using (var streamWriter =
                    new StreamWriter(scenario.ResultPath))
                {
                    var contentJsonUnPrettyfied = JsonConvert.SerializeObject(scenario.Results, Formatting.Indented);
                    streamWriter.WriteLine(contentJsonUnPrettyfied);
                }
            }
        }

        private static void WriteHeaderCsv(TextWriter streamWriter) 
            => streamWriter.WriteLine(MetricStatus.GetCsvHeader());
    }
}