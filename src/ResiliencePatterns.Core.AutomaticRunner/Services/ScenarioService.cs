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
using ResiliencePatternsDotNet.Commons;
using ResiliencePatternsDotNet.Commons.Configurations;

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

        private IEnumerable<ScenarioInput> LoadScenarios() 
            => ScenarioUtils.LoadScenarios(_automaticRunnerConfiguration.ScenariosPath);

        private void ProcessScenario(ScenarioInput scenario)
        {
            Console.WriteLine();
            Console.WriteLine($"Scenario: {scenario.Directory}\\{scenario.FileName}");
            Console.WriteLine(JsonConvert.SerializeObject(scenario));
            ConfigProxy(scenario);

            for (var count = 1; count <= scenario.Count; count++)
            {
                Console.WriteLine($"Scenario: {scenario.Directory}\\{scenario.FileName}");
                Console.WriteLine($"    Bateria de Teste: {count}/{scenario.Count}");

                foreach (var subScenario in scenario.Clients)
                {
                    Console.WriteLine($"Scenario: {scenario.Directory}\\{scenario.FileName}");
                    Console.WriteLine($"    Bateria de Teste: {count}/{scenario.Count}");
                    Console.WriteLine($"        SubScenario: {subScenario}");
                    Console.WriteLine("         Start sending");
                    
                    if (File.Exists(scenario.ResultPath(count, subScenario)))
                        File.Delete(scenario.ResultPath(count, subScenario));
                    
                    if (scenario.AsyncClients)
                    {
                        var tasks = new List<Task<MetricStatus>>();
                        for (var i = 1; i <= subScenario; i++)
                            tasks.Add(MakeRequestAsync(scenario, i, subScenario));

                        var results = Task.WhenAll(tasks).GetAwaiter().GetResult();
                        foreach (var result in results)
                            scenario.AddResult(count, subScenario, result);
                    }
                    else
                    {
                        for (var i = 1; i <= subScenario; i++)
                            scenario.AddResult(count, subScenario, MakeRequest(scenario, i, subScenario));
                    }
                }
            }
                
            _resultWriterService.Write(scenario);
        }

        private static void ConfigProxy(ScenarioInput scenario)
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

        private static async Task<MetricStatus> MakeRequestAsync(ScenarioInput scenario, int subScenarioStep, int subScenario)
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
                        Console.WriteLine($"[{subScenarioStep.ToString().PadLeft(3)}/{subScenario.ToString().PadLeft(3)}]: {httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
                        Console.ForegroundColor = ConsoleColor.White;
                        
                        return  JsonConvert.DeserializeObject<MetricStatus>(httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    })
                    .ConfigureAwait(false);
            }
        }

        private MetricStatus MakeRequest(ScenarioInput scenario, int subScenarioStep, int subScenario) 
            => MakeRequestAsync(scenario, subScenarioStep, subScenario).GetAwaiter().GetResult();
    }
}