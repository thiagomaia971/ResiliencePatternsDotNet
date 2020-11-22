using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Prometheus;
using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Extensions;
using ResiliencePatterns.DotNet.Domain.Services.RequestHandles;
using ResiliencePatterns.DotNet.Domain.Services.Resiliences;
using ResiliencePatternsDotNet.Commons;
using ConfigurationSection = ResiliencePatterns.DotNet.Domain.Configurations.ConfigurationSection;

namespace ResiliencePatterns.DotNet.Domain.Services
{
    public class ExecuteService : IExecuteService
    {
        private readonly MetricService _metrics;
        private readonly IResiliencePatterns _resiliencePatterns;
        private readonly IRequestHandle _requestHandle;

        public ExecuteService(MetricService metricService, IResiliencePatterns resiliencePatterns, IRequestHandle requestHandle)
        {
            _metrics = metricService;
            _resiliencePatterns = resiliencePatterns;
            _requestHandle = requestHandle;
        }

        public Task<MetricStatus> Execute(ConfigurationSection configurationSection)
        {
            // Console.WriteLine($"Teste:     {configurationSection.ToJson()}");
            // Console.WriteLine();
            
             _resiliencePatterns.Configure(configurationSection);
            _requestHandle.Configure(configurationSection);
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
            var watch = new Stopwatch();
            watch.Start();
            
            while (_metrics.Client.Success < configurationSection.RequestConfiguration.SuccessRequests &&
                   (configurationSection.RequestConfiguration.MaxRequests.HasValue ? _metrics.Client.Total < configurationSection.RequestConfiguration.MaxRequests : true))
            {
                await ProcessRequest(configurationSection);
            }
                
            watch.Stop();
            _metrics.IncrementClientTotalTime(watch.ElapsedMilliseconds);

            return _metrics.MetricStatus;
        }

        private Task<HttpResponseMessage> ProcessRequest(ConfigurationSection configurationSection) 
            => _requestHandle.HandleRequest(configurationSection.UrlConfiguration);
    }
}