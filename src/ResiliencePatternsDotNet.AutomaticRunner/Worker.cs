using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResiliencePatternsDotNet.AutomaticRunner.Configurations;

namespace ResiliencePatternsDotNet.AutomaticRunner
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AutomaticRunnerConfiguration _automaticRunnerConfiguration;

        public Worker(ILogger<Worker> logger, AutomaticRunnerConfiguration automaticRunnerConfiguration)
        {
            _logger = logger;
            _automaticRunnerConfiguration = automaticRunnerConfiguration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ProcessScenarios(LoadScenarios());
                
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
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

        private void ProcessScenarios(IEnumerable<Scenario> scenarios)
        {
            foreach (var scenario in scenarios)
                ProcessScenario(scenario);
        }

        private void ProcessScenario(Scenario scenario)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_automaticRunnerConfiguration.UrlFetch.BaseUrl);
                var methodEnum = new HttpMethod(_automaticRunnerConfiguration.UrlFetch.Method);

                var httpRequestMessage = new HttpRequestMessage(methodEnum, _automaticRunnerConfiguration.UrlFetch.ActionUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(scenario.Parameters), Encoding.UTF8, "application/json")
                };
                var result = httpClient.SendAsync(httpRequestMessage).GetAwaiter().GetResult();

                var xx =  result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                using (var streamWriter = new StreamWriter($"{scenario.Directory}\\{scenario.FileNameWithoutExtension}.result"))
                {
                    streamWriter.Write(xx);
                }
            }
        }
    }

    public class Scenario
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string FileNameWithoutExtension { get; set; }
        public object Parameters { get; set; }
    }
}