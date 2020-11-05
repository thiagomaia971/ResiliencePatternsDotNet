using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.Commons.Configurations
{
    public class Scenario
    {
        public bool Run { get; set; }
        public int Count { get; set; }
        public int[] Clients { get; set; }
        public UrlFetchConfigurationSection UrlFetch { get; set; }
        public ProxyConfigurationSection ProxyConfiguration { get; set; }
        public ResultType ResultType { get; set; }
        public object Parameters { get; set; }
        public bool AsyncClients { get; set; }
    }
    
    public class ScenarioInput
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string FileNameWithoutExtension { get; set; }
        // public int SubScenario { get; set; }

        public bool Run { get; set; }
        public int Count { get; set; }
        public int[] Clients { get; set; }
        public UrlFetchConfigurationSection UrlFetch { get; set; }
        public ProxyConfigurationSection ProxyConfiguration { get; set; }
        public ResultType ResultType { get; set; }
        public object Parameters { get; set; }

        public string ResultPath(int bateria, int subScenario) => $"{Directory}\\{bateria}\\[{subScenario}]{FileNameWithoutExtension}-result.{ResultType.ToString().ToLower()}";
        public string ResultCompiledPath(int subScenario) => $"{Directory}\\[{subScenario}]{FileNameWithoutExtension}-result-compiled.{ResultType.ToString().ToLower()}";
        public List<BateriaResult> Bateries { get; set; }
        public bool AsyncClients { get; set; }

        public ScenarioInput() => Bateries = new List<BateriaResult>();

        public void AddResult(int bateria, int client, MetricStatus result)
        {
            var bateriaResult = Bateries.FirstOrDefault(x => x.Count == bateria);
            if (bateriaResult == null)
            {
                bateriaResult = new BateriaResult
                {
                    Count = bateria
                };
                Bateries.Add(bateriaResult);
            }

            var clientResult = bateriaResult.ClientResults.FirstOrDefault(x => x.Count == client);
            if (clientResult == null)
            {
                clientResult = new ClientResult
                {
                    Count = client
                };
                bateriaResult.ClientResults.Add(clientResult);
            }
            
            clientResult.Result.Add(result);
        }

        public Scenario ToScenario()
        {
            var scenario = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Scenario>(scenario);
        }
    }

    public class BateriaResult
    {
        public int Count { get; set; }
        public List<ClientResult> ClientResults { get; set; }

        public BateriaResult() => ClientResults = new List<ClientResult>();
    }

    public class ClientResult
    {
        public int Count { get; set; }
        public List<MetricStatus> Result { get; set; }

        public ClientResult() => Result = new List<MetricStatus>();
    }
}