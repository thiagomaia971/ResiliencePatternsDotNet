using System;
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
            
            ResiliencePatterns = new ResiliencePatterns(configurationSection);
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
            var requestCount = configurationSection.RequestConfiguration.Count;
            
            while (requestCount > 0)
            {
                _metrics.IncrementIterationCount();
                
                ProcessRequest(configurationSection);
                
                Thread.Sleep(configurationSection.RequestConfiguration.Delay);
                requestCount--;
                Console.WriteLine($"Requests Remaining [{requestCount}]");
                Console.WriteLine();
            }

            _metrics.ResetAll();
            return _metrics.MetricStatus;
        }

        private void ProcessRequest(ResiliencePatternsDotNet.Domain.Configurations.ConfigurationSection configurationSection) 
            => RequestHandle.HandleRequest(
                configurationSection.RequestConfiguration.ProbabilityError, 
                configurationSection.UrlConfiguration.Success, 
                configurationSection.UrlConfiguration.Error);
    }
}