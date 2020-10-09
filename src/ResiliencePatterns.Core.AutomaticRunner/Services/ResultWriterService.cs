using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using ResiliencePatterns.Core.AutomaticRunner.Configurations;

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
                        var jsonElement = JsonConvert.DeserializeObject<dynamic>(contentJsonUnPrettyfied);
                        
                        streamWriter.WriteLine($"{jsonElement.TotalTime}; {jsonElement.ClientToModule.Success}; {jsonElement.ClientToModule.Error}; {jsonElement.ResilienceModuleToExternalService.Success}; {jsonElement.ResilienceModuleToExternalService.Error}; {(jsonElement.RetryMetrics == null ? "" : jsonElement.RetryMetrics.RetryCount)}; {(jsonElement.RetryMetrics == null ? "" : jsonElement.RetryMetrics.TotalTimeout)}; {(jsonElement.CircuitBreakerMetrics == null ? "" : jsonElement.CircuitBreakerMetrics.BreakCount)}; {(jsonElement.CircuitBreakerMetrics == null ? "" : jsonElement.CircuitBreakerMetrics.ResetStatCount)}; {(jsonElement.CircuitBreakerMetrics == null ? "" : jsonElement.CircuitBreakerMetrics.TotalOfBreak)}");
                    }
                }
            }
        }

        private static void WriteCsv()
        {
            
        }

        private static void WriteHeaderCsv(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("Total Time; ClientToModule Success; ClientToModule Error; ResilienceModuleToExternalService Success; ResilienceModuleToExternalService Error; Retry Count; Retry TotalTimeout; CircuitBreaker Count; CircuitBreaker ResetCount; CircuitBreaker TotalOfBreaker");
        }
    }
}