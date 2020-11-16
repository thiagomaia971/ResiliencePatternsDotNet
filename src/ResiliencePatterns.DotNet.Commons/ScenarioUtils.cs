using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ResiliencePatternsDotNet.Commons.Configurations;

namespace ResiliencePatternsDotNet.Commons
{
    public static class ScenarioUtils
    {
        public static IEnumerable<ScenarioInput> LoadScenarios(string path)
        {
            var scenariosPath = System.IO.Directory.GetFiles(path, "*.scenario.json", SearchOption.AllDirectories);
            var scenarios = new List<ScenarioInput>();
            foreach (var scenarioFile in scenariosPath)
            {
                using (var streamReader = new StreamReader(scenarioFile))
                {
                    var scenarioJson = streamReader.ReadToEnd();
                    var scenario = JsonConvert.DeserializeObject<ScenarioInput>(scenarioJson);
                    if (!scenario.Run)
                        continue;
                    
                    scenario.Directory = Path.GetDirectoryName(scenarioFile);
                    scenario.FileName = Path.GetFileName(scenarioFile);
                    scenario.FileNameWithoutExtension = Path.GetFileNameWithoutExtension(scenarioFile);
                    scenarios.Add(scenario);
                }
            }

            return scenarios;
        }
        public static IEnumerable<ScenarioInput> LoadScenarioResults(string path, string system)
        {
            var scenariosPath = System.IO.Directory.GetFiles(path, "*.scenario-result.json", SearchOption.AllDirectories)
                .Where(x => x.Contains($"\\{system}"))
                .Where(x => !x.Contains("[25]Scenario01.scenario-result") && !x.Contains("[50]Scenario01.scenario-result") && !x.Contains("[100]Scenario01.scenario-result"))
                .ToList();
            var scenarioGrouped = scenariosPath.GroupBy(x => x.Split($"\\{system}").FirstOrDefault());
            var scenarios = new List<ScenarioInput>();
            foreach (var scenarioPath in scenarioGrouped)
            {
                var bateries = new List<BateriaResult>();
                var bateriesGrouped = scenarioPath.GroupBy(x => Path.GetDirectoryName(x)).OrderBy(x => x.Key);

                foreach (var bateryGrouped in bateriesGrouped)
                {
                    var clientsResult = new List<ClientResult>();
                    foreach (var scenarioFile in bateryGrouped)
                    {
                        using (var streamReader = new StreamReader(scenarioFile))
                        {
                            var scenarioJson = streamReader.ReadToEnd();
                            var metrics = JsonConvert.DeserializeObject<List<MetricStatus>>(scenarioJson);
                            clientsResult.Add(new ClientResult
                            {
                                Count = int.Parse(Path.GetFileName(scenarioFile).Split("[").LastOrDefault().Split("]").FirstOrDefault()),
                                Result = metrics
                            });
                        }    
                    }
                    bateries.Add(new BateriaResult
                    {
                        Count = int.Parse(bateryGrouped.Key.Split('\\').LastOrDefault()),
                        ClientResults = clientsResult
                    });
                }
                scenarios.Add(new ScenarioInput
                {
                    FileNameWithoutExtension = scenarioPath.FirstOrDefault().Split("]").LastOrDefault().Split("-").FirstOrDefault(),
                    Directory = scenarioPath.Key,
                    ResultType = ResultType.JSON,
                    Bateries = bateries,
                    UrlFetch = new UrlFetchConfigurationSection
                    {
                        BaseUrlName = new [] {"DotNet", "Java"}
                    }
                });
            }
            

            return scenarios;
        }
    }

    public class ScenarioChart
    {
        public string ScenarioFileName { get; set; }
        public List<MetricStatus> ScenarioResults { get; set; }
    }

    public class ScenarioChartCompiled
    {
        public string ScenarioFileName { get; set; }
        public List<MetricStatusCompiled> ScenarioResults { get; set; }
    }
}
