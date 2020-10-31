using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            // lock (scenario)
            // {
            //     foreach (var scenarioResult in scenario.Results)
            //     {
            //         if (File.Exists(scenario.ResultPath(scenarioResult.Key))) 
            //             return;
            //     
            //         using (var streamWriter =
            //             new StreamWriter(scenario.ResultPath(scenarioResult.Key)))
            //         {
            //             WriteHeaderCsv(streamWriter);
            //             foreach (var httpResponseMessage in scenarioResult.Value)
            //                 streamWriter.WriteLine(httpResponseMessage.GetCsvLine());
            //         }
            //     }
            // }
        }

        private static void WriteJson(Scenario scenario)
        {
            lock (scenario)
            {
                foreach (var scenarioResult in scenario.Results)
                {
                    if (!Directory.Exists($"{scenario.Directory}\\{scenarioResult.Count}"))
                        Directory.CreateDirectory($"{scenario.Directory}\\{scenarioResult.Count}");

                    foreach (var scenarioResultClientResult in scenarioResult.ClientResults)
                    {
                        using (var streamWriter =
                            new StreamWriter(scenario.ResultPath(scenarioResult.Count, scenarioResultClientResult.Count)))
                        {
                            var contentJsonUnPrettyfied = JsonConvert.SerializeObject(scenarioResultClientResult.Result, Formatting.Indented);
                            streamWriter.WriteLine(contentJsonUnPrettyfied);
                        }
                    }
                }

                var baterias = scenario.Results.SelectMany(x => x.ClientResults).GroupBy(x => x.Count).ToList();
                foreach (var bateriaGrouped in baterias)
                {
                    var results = bateriaGrouped.SelectMany(x => x.Result).ToList();
                    var compiledValues = new MetricStatusCompiled
                    {
                        ClientToModuleTotalTime = GetMedian(results.Select(x => (double) x.ClientToModule.TotalTime).ToArray()),
                        ClientToModulePercentualError = GetMedian(results.Select(x => ((double) x.ClientToModule.Error) / ((double) x.ClientToModule.Success)).ToArray()),
                        ResilienceModuleToExternalTotalSuccessTime = GetMedian(results.Select(x => (double) x.ResilienceModuleToExternalService.TotalSuccessTime).ToArray()),
                        ResilienceModuleToExternalTotalErrorTime = GetMedian(results.Select(x => (double) x.ResilienceModuleToExternalService.TotalErrorTime).ToArray()),
                        ResilienceModuleToExternalTotalTime = GetMedian(results.Select(x => (double) x.ResilienceModuleToExternalService.Total).ToArray()),
                        ResilienceModuleToExternalAverageTimePerRequest = GetMedian(results.Select(x => x.ResilienceModuleToExternalService.AverageSuccessTimePerRequest).ToArray()),
                    };
                    
                    using (var streamWriter =
                        new StreamWriter(scenario.ResultPath(bateriaGrouped.Key)))
                    {
                        var contentJsonUnPrettyfied = JsonConvert.SerializeObject(compiledValues, Formatting.Indented);
                        streamWriter.WriteLine(contentJsonUnPrettyfied);
                    }
                }
            }
        }
        
        public static double GetMedian(double[] sourceNumbers) {
            //Framework 2.0 version of this method. there is an easier way in F4        
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                throw new System.Exception("Median of empty array not defined.");

            //make sure the list is sorted, but use a new array
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        private static void WriteHeaderCsv(TextWriter streamWriter) 
            => streamWriter.WriteLine(MetricStatus.GetCsvHeader());
    }
}