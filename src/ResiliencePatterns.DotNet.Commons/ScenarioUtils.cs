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
