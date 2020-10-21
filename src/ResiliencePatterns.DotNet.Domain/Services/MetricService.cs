using System;
using System.Diagnostics;
using App.Metrics;
using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Extensions;
using ResiliencePatternsDotNet.DotNet.Commons;

namespace ResiliencePatterns.DotNet.Domain.Services
{
    public class MetricService
    {
        private readonly MetricsRegistry _metricsRegistry;
        private readonly IMetrics _metrics;
        private Stopwatch _stopwatch;
        private readonly MetricStatus _metricStatus;

        public MetricService(IMetrics metrics, MetricsRegistry metricsRegistry)
        {
            _metrics = metrics;
            _metricsRegistry = metricsRegistry;
            _metricStatus = MetricStatus.Create();
        }

        public MetricStatus MetricStatus => _metricStatus;

        public MetricCountStatus Client => _metricStatus.ClientToModule;
        public MetricCountStatus ResilienceModule => _metricStatus.ResilienceModuleToExternalService;
        
        public void CreateRetryCustom() => _metricStatus.CreateRetryCustom();
        public void CreateCircuitBrekerCustom() => _metricStatus.CreateCircuitBrekerCustom();
        public MetricRetryStatus RetryMetric => _metricStatus.RetryMetrics;
        public MetricCircuitBreakerStatus CircuitBreakerMetric => _metricStatus.CircuitBreakerMetrics;

        public void ResetAll()
        {
            // _metrics.Measure.Counter.Decrement(_metricsRegistry.IncrementClientError, _metricStatus.ErrorRequest);
            // _metrics.Measure.Counter.Decrement(_metricsRegistry.IncrementIterationCount, _metricStatus.RequestCount);
            // _metrics.Measure.Counter.Decrement(_metricsRegistry.IncrementSuccessRequest, _metricStatus.SuccessRequest);
        }

        public void StartWatchTime()
        {
            _stopwatch = new System.Diagnostics.Stopwatch();
            _stopwatch.Start();
        }

        public void StopWatchTime()
        {
            _stopwatch?.Stop();
            _metricStatus.AddTotalTime(_stopwatch?.ElapsedMilliseconds);
        }

        public void IncrementClientSuccess()
        {
            _metricStatus.ClientToModule.IncrementeSuccess();
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementClientSuccess);
        }

        public void IncrementClientError()
        {
            _metricStatus.ClientToModule.IncrementeError();
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementClientError);
        }

        public void IncrementeResilienceModuleError()
        {
            _metricStatus.ResilienceModuleToExternalService.IncrementeError();
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementeResilienceModuleError);
        }

        public void IncrementeResilienceModuleSuccess()
        {
            _metricStatus.ResilienceModuleToExternalService.IncrementeSuccess();
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementeResilienceModuleSucess);
        }

        public void IncrementeResilienceModuleSuccessTime(long milliseconds) 
            => _metricStatus.ResilienceModuleToExternalService.IncrementeSuccessTime(milliseconds);
    }
}