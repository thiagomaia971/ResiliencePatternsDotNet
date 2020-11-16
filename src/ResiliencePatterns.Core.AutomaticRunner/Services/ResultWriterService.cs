using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using ResiliencePatterns.Core.AutomaticRunner.Extensions;
using ResiliencePatternsDotNet.Commons;
using ResiliencePatternsDotNet.Commons.Configurations;

namespace ResiliencePatterns.Core.AutomaticRunner.Services
{
    public class ResultWriterService
    {
        private IDictionary<ResultType, Action<ScenarioInput>> resultTypeHandler = new Dictionary<ResultType, Action<ScenarioInput>>
        {
            { ResultType.TXT, WriteTxt },
            { ResultType.CSV, WriteCsv },
            { ResultType.JSON, WriteJson },
        };

        public void Write(ScenarioInput scenario) 
            => resultTypeHandler[scenario.ResultType](scenario);

        private static void WriteTxt(ScenarioInput scenario)
        {
            var count = 1;
            foreach (var result in scenario.Bateries)
            {
                using var streamWriter = new StreamWriter($"{scenario.Directory}\\{scenario.FileNameWithoutExtension}[{count}].result");
                streamWriter.Write(JsonConvert.SerializeObject(result, Formatting.Indented));
                count++;
            }
        }

        private static void WriteCsv(ScenarioInput scenario)
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

        private static void WriteJson(ScenarioInput scenario)
        {
            lock (scenario)
            {
                WriteEachClientResultJson(scenario);
                WriteCompiledResultJson(scenario);
                WriteScenario(scenario);
            }
        }

        private static void WriteEachClientResultJson(ScenarioInput scenario)
        {
            foreach (var scenarioResult in scenario.Bateries)
            {
                if (!Directory.Exists($"{scenario.Directory}\\{scenario.CurrentSystemName}"))
                    Directory.CreateDirectory($"{scenario.Directory}\\{scenario.CurrentSystemName}");

                if (!Directory.Exists($"{scenario.Directory}\\{scenario.CurrentSystemName}\\{scenarioResult.Count}"))
                    Directory.CreateDirectory($"{scenario.Directory}\\{scenario.CurrentSystemName}\\{scenarioResult.Count}");
                
                foreach (var scenarioResultClientResult in scenarioResult.ClientResults)
                {
                    using (var streamWriter =
                        new StreamWriter(scenario.ResultPath(scenarioResult.Count, scenarioResultClientResult.Count)))
                    {
                        var contentJsonUnPrettyfied =
                            JsonConvert.SerializeObject(scenarioResultClientResult.Result, Formatting.Indented);
                        streamWriter.WriteLine(contentJsonUnPrettyfied);
                    }
                }
            }
        }

        public static void WriteCompiledResultJson(ScenarioInput scenario)
        {
            var baterias = scenario.Bateries.SelectMany(x => x.ClientResults).GroupBy(x => x.Count).ToList();
            foreach (var bateriaGrouped in baterias)
            {
                var results = bateriaGrouped.Select(y => new MetricStatusCompiled
                {
                    ClientToModuleTotalTime = y.Result.Select(x => (double) x.ClientToModule.TotalTime).GetMedian(),
                    ClientToModulePercentualError = y.Result
                        .Select(x => ((double) x.ClientToModule.Error) / ((double) x.ClientToModule.Total)).GetMedian(),
                    ResilienceModuleToExternalTotalSuccessTime = y.Result
                        .Select(x => (double) x.ResilienceModuleToExternalService.TotalSuccessTime).GetMedian(),
                    ResilienceModuleToExternalTotalErrorTime = y.Result
                        .Select(x => (double) x.ResilienceModuleToExternalService.TotalErrorTime).GetMedian(),
                    ResilienceModuleToExternalTotalTime =
                        y.Result.Select(x => (double) x.ResilienceModuleToExternalService.TotalSuccessTime + (double) x.ResilienceModuleToExternalService.TotalErrorTime).GetMedian(),
                    ResilienceModuleToExternalAverageTimePerRequest = y.Result
                        .Select(x => x.ResilienceModuleToExternalService.AverageSuccessTimePerRequest).GetMedian(),
                }).ToList();

                if (!Directory.Exists($"{scenario.Directory}\\{scenario.CurrentSystemName}"))
                    Directory.CreateDirectory($"{scenario.Directory}\\{scenario.CurrentSystemName}");

                var resultCompiledPath = scenario.ResultCompiledPath(bateriaGrouped.Key);
                using (var streamWriter =
                    new StreamWriter(resultCompiledPath))
                {
                    var contentJsonUnPrettyfied = JsonConvert.SerializeObject(results, Formatting.Indented);
                    streamWriter.WriteLine(contentJsonUnPrettyfied);
                }
            }
        }

        private static void WriteScenario(ScenarioInput scenario)
        {
            scenario.Run = false;
            try
            {

                using (var streamWriter =
                    new StreamWriter($"{scenario.Directory}\\{scenario.FileName}"))
                {
                    var contentJsonUnPrettyfied =
                        JsonConvert.SerializeObject(scenario.ToScenario(), Formatting.Indented);
                    streamWriter.WriteLine(contentJsonUnPrettyfied);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void WriteHeaderCsv(TextWriter streamWriter) 
            => streamWriter.WriteLine(MetricStatus.GetCsvHeader());
    }
}