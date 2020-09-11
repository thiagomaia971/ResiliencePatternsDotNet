using System;
using System.Net.Http;
using System.Threading;
using App.Metrics;
using App.Metrics.Counter;
using Microsoft.Extensions.Configuration;
using Prometheus;
using ResiliencePatternsDotNet.Domain.Common;
using ResiliencePatternsDotNet.Domain.Extensions;
using ResiliencePatternsDotNet.Domain.Services.RequestHandles;
using ResiliencePatternsDotNet.Domain.Services.Resiliences;

namespace ResiliencePatternsDotNet.Domain.Services
{
    public class ExecuteService : IExecuteService
    {
        private readonly MetricService _metrics;
        private IResiliencePatterns ResiliencePatterns { get; set; }
        private IRequestHandle RequestHandle { get; set; }

        public ExecuteService(MetricService metricService) 
            => _metrics = metricService;

        public MetricStatus Execute(ResiliencePatternsDotNet.Domain.Configurations.ConfigurationSection configurationSection)
        {
            Console.WriteLine($"EnvironmentValue: {GlobalVariables.EnvironmentValue}");
            Console.WriteLine($"Teste:     {configurationSection.ToJson()}");
            Console.WriteLine();
            
            ResiliencePatterns = new ResiliencePatterns(configurationSection, _metrics);
            RequestHandle = new RequestHandle(ResiliencePatterns, configurationSection, _metrics);
            // InitializePrometheusServer();

            return ProcessRequests(configurationSection);
        }
        
        private void InitializePrometheusServer(ResiliencePatternsDotNet.Domain.Configurations.ConfigurationSection configurationSection)
        {
            var prometheusConfigurationHostname = configurationSection.PrometheusConfiguration.Hostname;
            var prometheusConfigurationPort = configurationSection.PrometheusConfiguration.Port;
            new MetricServer(
                    hostname: prometheusConfigurationHostname,
                    port: prometheusConfigurationPort)
                .Start();
        }

        private MetricStatus ProcessRequests(ResiliencePatternsDotNet.Domain.Configurations.ConfigurationSection configurationSection)
        {
            _metrics.StartWatchTime();
            
            while (_metrics.Client.Success < configurationSection.RequestConfiguration.SuccessRequests && 
                   _metrics.Client.Total < configurationSection.RequestConfiguration.MaxRequests)
            {
                Console.WriteLine($"Client [{_metrics.Client.Total + 1}]");
                
                ProcessRequest(configurationSection);
                
                Thread.Sleep(configurationSection.RequestConfiguration.Delay);
                
                Console.WriteLine(" -------- ");
                Console.WriteLine();
            }

            _metrics.ResetAll();
            
            _metrics.StopWatchTime();
            return _metrics.MetricStatus;
        }

        private HttpResponseMessage ProcessRequest(ResiliencePatternsDotNet.Domain.Configurations.ConfigurationSection configurationSection) 
            => RequestHandle.HandleRequest(
                configurationSection.RequestConfiguration.ProbabilityError, 
                configurationSection.UrlConfiguration.Success, 
                configurationSection.UrlConfiguration.Error);
    }
}