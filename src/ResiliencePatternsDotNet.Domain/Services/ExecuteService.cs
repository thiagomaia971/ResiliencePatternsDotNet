using System;
using System.Threading;
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
        private IResiliencePatterns ResiliencePatterns { get; set; }
        private IRequestHandle RequestHandle { get; set; }
        
        public MetricStatus Execute(ResiliencePatternsDotNet.Domain.Configurations.ConfigurationSection configurationSection)
        {
            Console.WriteLine($"EnvironmentValue: {GlobalVariables.EnvironmentValue}");
            Console.WriteLine($"Teste:     {configurationSection.ToJson()}");
            Console.WriteLine();
            
            ResiliencePatterns = new ResiliencePatterns(configurationSection);
            RequestHandle = new RequestHandle(ResiliencePatterns, configurationSection);
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
            var metric = MetricStatus.Create();
            var requestCount = configurationSection.RequestConfiguration.Count;
            
            while (requestCount > 0)
            {
                metric.IncrementIterationCount();
                Console.WriteLine($"ProcessRequest [{metric.IterationCount}]");
                
                ProcessRequest(configurationSection, metric);
                
                Thread.Sleep(configurationSection.RequestConfiguration.Delay);
                requestCount--;
                Console.WriteLine($"Requests Remaining [{requestCount}]");
                Console.WriteLine();
            }

            return metric;
        }

        private void ProcessRequest(ResiliencePatternsDotNet.Domain.Configurations.ConfigurationSection configurationSection, MetricStatus metric) 
            => RequestHandle.HandleRequest(
                metric, 
                configurationSection.RequestConfiguration.ProbabilityError, 
                configurationSection.UrlConfiguration.Success, 
                configurationSection.UrlConfiguration.Error);
    }
}