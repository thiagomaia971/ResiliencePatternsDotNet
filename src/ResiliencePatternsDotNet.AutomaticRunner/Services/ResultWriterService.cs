using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using ResiliencePatternsDotNet.AutomaticRunner.Configurations;

namespace ResiliencePatternsDotNet.AutomaticRunner.Services
{
    public class ResultWriterService
    {
        private readonly AutomaticRunnerConfiguration _automaticRunnerConfiguration;
        private IDictionary<ResultType, Action<Scenario, int, HttpResponseMessage>> resultTypeHandler = new Dictionary<ResultType, Action<Scenario, int, HttpResponseMessage>>
        {
            { ResultType.TXT, WriteTxt },
            { ResultType.CSV, WriteCsv },
        };

        public ResultWriterService(AutomaticRunnerConfiguration automaticRunnerConfiguration) 
            => _automaticRunnerConfiguration = automaticRunnerConfiguration;

        public void Write(Scenario scenario, int count, HttpResponseMessage result) 
            => resultTypeHandler[_automaticRunnerConfiguration.ResultType](scenario, count, result);

        private static void WriteTxt(Scenario scenario, int count, HttpResponseMessage result)
        {
            var contentJsonUnPrettyfied = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var jsonElement = JsonConvert.DeserializeObject(contentJsonUnPrettyfied);

            using var streamWriter = new StreamWriter($"{scenario.Directory}\\{scenario.FileNameWithoutExtension}[{count}].result");
            streamWriter.Write(JsonConvert.SerializeObject(jsonElement, Formatting.Indented));
        }

        private static void WriteCsv(Scenario scenario, int count, HttpResponseMessage result)
        {
            scenario.Results.Add(result);

            if (scenario.Results.Count != scenario.Count) 
                return;
            

            using (var streamWriter =
                new StreamWriter($"{scenario.Directory}\\{scenario.FileNameWithoutExtension}-result.csv"))
            {
                WriteHeaderCsv(streamWriter);
                foreach (var httpResponseMessage in scenario.Results)
                {
                    var contentJsonUnPrettyfied = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var jsonElement = JsonConvert.DeserializeObject<dynamic>(contentJsonUnPrettyfied);
                    
                    streamWriter.WriteLine($"{jsonElement.TotalTime}; {jsonElement.Client.Success}; {jsonElement.Client.Error}; {jsonElement.ResilienceModule.Success}; {jsonElement.ResilienceModule.Error}");
                }
            }
        }

        private static void WriteHeaderCsv(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("Total Time; Client Success; Client Error; Resilience Module Success; Resilience Module Error;");
        }
    }
}