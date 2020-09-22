using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ResiliencePatternsDotNet.AutomaticRunner.Configurations;

namespace ResiliencePatternsDotNet.AutomaticRunner.Services
{
    public class ScenarioService
    {
        private readonly AutomaticRunnerConfiguration _automaticRunnerConfiguration;
        private readonly ResultWriterService _resultWriterService;

        public ScenarioService(AutomaticRunnerConfiguration automaticRunnerConfiguration, ResultWriterService resultWriterService)
        {
            _automaticRunnerConfiguration = automaticRunnerConfiguration;
            _resultWriterService = resultWriterService;
        }

        public void ProcessScenarios()
        {
            var scenarios = LoadScenarios();
            foreach (var scenario in scenarios)
                ProcessScenario(scenario);
        }

        private IEnumerable<Scenario> LoadScenarios()
        {
            var scenariosPath = System.IO.Directory.GetFiles(_automaticRunnerConfiguration.ScenariosPath, "*.scenario", SearchOption.AllDirectories);;
            var scenarios = new List<Scenario>();
            foreach (var scenarioFile in scenariosPath)
            {
                using (var streamReader = new StreamReader(scenarioFile))
                {
                    var scenarioJson = streamReader.ReadToEnd();
                    var scenario = JsonConvert.DeserializeObject<Scenario>(scenarioJson);
                    scenario.Directory = Path.GetDirectoryName(scenarioFile);
                    scenario.FileName = Path.GetFileName(scenarioFile);
                    scenario.FileNameWithoutExtension = Path.GetFileNameWithoutExtension(scenarioFile);
                    scenarios.Add(scenario);
                }
            }

            return scenarios;
        }

        private void ProcessScenario(Scenario scenario)
        {
            for (int i = 0; i < scenario.Count; i++)
            {
                new Thread((x) =>
                {
                    Console.WriteLine(x);
                    var result = MakeRequest(scenario);
                    _resultWriterService.Write(scenario, ((int)x) + 1, result);
                }).Start(i);
            }
        }

        private HttpResponseMessage MakeRequest(Scenario scenario)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_automaticRunnerConfiguration.UrlFetch.BaseUrl);
                var methodEnum = new HttpMethod(_automaticRunnerConfiguration.UrlFetch.Method);

                var httpRequestMessage = new HttpRequestMessage(methodEnum, _automaticRunnerConfiguration.UrlFetch.ActionUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(scenario.Parameters), Encoding.UTF8, "application/json")
                };
                
                return httpClient.SendAsync(httpRequestMessage).GetAwaiter().GetResult();
            }
        }
    }
}