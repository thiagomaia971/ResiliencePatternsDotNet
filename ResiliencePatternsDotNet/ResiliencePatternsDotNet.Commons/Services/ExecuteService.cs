using System;
using System.Threading;
using Prometheus;
using ResiliencePatternsDotNet.Commons.Common;
using ResiliencePatternsDotNet.Commons.Services.RequestHandles;
using ResiliencePatternsDotNet.Commons.Services.Resiliences;

namespace ResiliencePatternsDotNet.Commons.Services
{
    public class ExecuteService : IExecuteService
    {
        private IResiliencePatterns ResiliencePatterns { get; set; }
        private IRequestHandle RequestHandle { get; set; }
        
        public void Execute()
        {
            Console.WriteLine($"EnvironmentValue: {GlobalVariables.EnvironmentValue}");
            Console.WriteLine($"Teste:     {GlobalVariables.ConfigurationSection.ToJson()}");
            Console.WriteLine();
            
            ResiliencePatterns = new ResiliencePatterns();
            RequestHandle = new RequestHandle(ResiliencePatterns);
            // InitializePrometheusServer();

            ProcessRequests();
        }
        
        private void InitializePrometheusServer()
        {
            var prometheusConfigurationHostname = GlobalVariables.ConfigurationSection.PrometheusConfiguration.Hostname;
            var prometheusConfigurationPort = GlobalVariables.ConfigurationSection.PrometheusConfiguration.Port;
            new MetricServer(
                    hostname: prometheusConfigurationHostname,
                    port: prometheusConfigurationPort)
                .Start();
        }

        private void ProcessRequests()
        {
            var requestCount = GlobalVariables.ConfigurationSection.RequestConfiguration.Count;
            var count = 1;
            var counter = Metrics.CreateCounter("teste_requests", "Quantidade de requisições!");
            counter.Inc(2);
            counter.Inc(2);
            counter.Inc(2);
            counter.Inc(2);
            counter.Publish();
            while (requestCount < 0)
            {
                counter.Inc();
                Console.WriteLine($"ProcessRequest [{count}]");
                
                ProcessRequest();
                
                Thread.Sleep(GlobalVariables.ConfigurationSection.RequestConfiguration.Delay);
                requestCount--;
                count++;
                Console.WriteLine($"Requests Remaining [{requestCount}]");
                Console.WriteLine();
            }
            counter.Publish();
        }

        private void ProcessRequest()
        {
            if (ErrorProbabilityService.IsErrorRequest(GlobalVariables.ConfigurationSection.RequestConfiguration
                .ProbabilityError))
                RequestHandle.HandleErrorRequest(GlobalVariables.ConfigurationSection.UrlConfiguration.Error.Method, GlobalVariables.ConfigurationSection.UrlConfiguration.Error.Url);
            else
                RequestHandle.HandleSuccessRequest(GlobalVariables.ConfigurationSection.UrlConfiguration.Success.Method, GlobalVariables.ConfigurationSection.UrlConfiguration.Success.Url);
        }
    }
}