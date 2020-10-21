using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Prometheus;
using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Extensions;
using ResiliencePatterns.DotNet.Domain.Services.RequestHandles;
using ResiliencePatterns.DotNet.Domain.Services.Resiliences;
using ResiliencePatternsDotNet.DotNet.Commons;
using ConfigurationSection = ResiliencePatterns.DotNet.Domain.Configurations.ConfigurationSection;

namespace ResiliencePatterns.DotNet.Domain.Services
{
    public class ExecuteService : IExecuteService
    {
        private readonly MetricService _metrics;
        private IResiliencePatterns ResiliencePatterns { get; set; }
        private IRequestHandle RequestHandle { get; set; }

        public ExecuteService(MetricService metricService) 
            => _metrics = metricService;

        public Task<MetricStatus> Execute(ConfigurationSection configurationSection)
        {
            Console.WriteLine($"EnvironmentValue: {GlobalVariables.EnvironmentValue}");
            Console.WriteLine($"Teste:     {configurationSection.ToJson()}");
            Console.WriteLine();
            
            ResiliencePatterns = new Resiliences.ResiliencePatterns(configurationSection, _metrics);
            RequestHandle = new RequestHandle(ResiliencePatterns, configurationSection, _metrics);
            // InitializePrometheusServer(configurationSection);

            return ProcessRequests(configurationSection);
        }
        
        private void InitializePrometheusServer(ConfigurationSection configurationSection)
        {
            if (configurationSection.PrometheusConfiguration == null || string.IsNullOrEmpty(configurationSection.PrometheusConfiguration.Hostname) ||
                !configurationSection.PrometheusConfiguration.Port.HasValue)
                return;
            
            var prometheusConfigurationHostname = configurationSection.PrometheusConfiguration.Hostname;
            var prometheusConfigurationPort = configurationSection.PrometheusConfiguration.Port;
            new MetricServer(
                    hostname: prometheusConfigurationHostname,
                    port: prometheusConfigurationPort.Value)
                .Start();
        }

        private async Task<MetricStatus> ProcessRequests(ConfigurationSection configurationSection)
        {
            _metrics.StartWatchTime();

            while (_metrics.Client.Success < configurationSection.RequestConfiguration.SuccessRequests && 
                   _metrics.Client.Total < configurationSection.RequestConfiguration.MaxRequests)
            {
                Console.WriteLine($"Client [{_metrics.Client.Total + 1}]");
                
                await ProcessRequest(configurationSection);
                
                Thread.Sleep(configurationSection.RequestConfiguration.Delay);
                
                Console.WriteLine(" -------- ");
                Console.WriteLine();
            }

            _metrics.ResetAll();
            
            _metrics.StopWatchTime();
            return _metrics.MetricStatus;
        }

        private Task<HttpResponseMessage> ProcessRequest(ConfigurationSection configurationSection) 
            => RequestHandle.HandleRequest(configurationSection.UrlConfiguration);
    }
}