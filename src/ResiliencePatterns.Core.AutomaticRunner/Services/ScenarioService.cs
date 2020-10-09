using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ResiliencePatterns.Core.AutomaticRunner.Configurations;

namespace ResiliencePatterns.Core.AutomaticRunner.Services
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
            var scenariosPath = System.IO.Directory.GetFiles(_automaticRunnerConfiguration.ScenariosPath, "*.scenario.json", SearchOption.AllDirectories);;
            var scenarios = new List<Scenario>();
            foreach (var scenarioFile in scenariosPath)
            {
                using (var streamReader = new StreamReader(scenarioFile))
                {
                    var scenarioJson = streamReader.ReadToEnd();
                    var scenario = JsonConvert.DeserializeObject<Scenario>(scenarioJson);
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

        private void ProcessScenario(Scenario scenario)
        {
            Console.WriteLine();
            Console.WriteLine($"Scenario: {scenario.FileName}");
            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(scenario));
            
            ConfigProxy(scenario);
            
            Console.WriteLine("Start sending");
            if (File.Exists(scenario.ResultPath))
                File.Delete(scenario.ResultPath);

            if (scenario.AsyncClients)
            {
                var tasks = new List<Task<HttpResponseMessage>>();
                for (var i = 0; i < scenario.Count; i++)
                    tasks.Add(MakeRequestAsync(scenario));

                var results = Task.WhenAll(tasks).GetAwaiter().GetResult();
                scenario.Results.AddRange(results);
            }
            else
            {
                for (var i = 0; i < scenario.Count; i++)
                    scenario.Results.Add(MakeRequest(scenario));
            }
            
            _resultWriterService.Write(scenario);
            Thread.Sleep(5000);
        }

        private void ConfigProxy(Scenario scenario)
        {
            Console.WriteLine($"Config Proxy to {scenario.ProxyConfiguration.Behavior}");
            
            var vaurienConfigLines = File.ReadAllLines(scenario.ProxyConfiguration.VaurienConfigPath);
            vaurienConfigLines[4] = $"behavior = {scenario.ProxyConfiguration.Behavior}";
            
            using (var streamWriter = new StreamWriter(scenario.ProxyConfiguration.VaurienConfigPath))
            {
                foreach (var vaurienConfigLine in vaurienConfigLines)
                    streamWriter.WriteLine(vaurienConfigLine);
            }

            var p = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = scenario.ProxyConfiguration.DockerComposePath,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                }
            };

            p.Start();
            p.StandardInput.WriteLine(scenario.ProxyConfiguration.RestartVaurienContainerCommand);
            Console.ForegroundColor = ConsoleColor.White;
            
            Thread.Sleep(10000);
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(Scenario scenario)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(scenario.UrlFetch.BaseUrl);
                var methodEnum = new HttpMethod(scenario.UrlFetch.Method);
                httpClient.Timeout = TimeSpan.FromHours(10d);

                var httpRequestMessage = new HttpRequestMessage(methodEnum, scenario.UrlFetch.ActionUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(scenario.Parameters), Encoding.UTF8, "application/json")
                };
                
                return await httpClient.SendAsync(httpRequestMessage)
                    .ContinueWith(x =>
                    {
                        var httpResponseMessage = x.GetAwaiter().GetResult();
                        if (httpResponseMessage.IsSuccessStatusCode)
                            Console.ForegroundColor = ConsoleColor.Green;
                        else
                            Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                        Console.ForegroundColor = ConsoleColor.White;
                        
                        return  httpResponseMessage;
                    })
                    .ConfigureAwait(false);
            }
        }

        private HttpResponseMessage MakeRequest(Scenario scenario) 
            => MakeRequestAsync(scenario).GetAwaiter().GetResult();
    }
}