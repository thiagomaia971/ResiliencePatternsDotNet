using System;
using System.Diagnostics;
using System.Threading;
using App.Metrics;
using ResiliencePatternsDotNet.Domain.Common;
using ResiliencePatternsDotNet.Domain.Extensions;

namespace ResiliencePatternsDotNet.Domain.Services
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

        public MetricCountStatus Client => _metricStatus.Client;
        public MetricCountStatus ResilienceModule => _metricStatus.ResilienceModule;
        
        
        public void CreateRetryCustom() => _metricStatus.CreateRetryCustom();
        public void CreateCircuitBrekerCustom() => _metricStatus.CreateCircuitBrekerCustom();
        public MetricRetryStatus RetryMetric => (MetricRetryStatus)_metricStatus.CustomResilience;
        public MetricCircuitBreakerStatus CircuitBreakerMetric => (MetricCircuitBreakerStatus)_metricStatus.CustomResilience;

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
            _metricStatus.AddTotalTime(_stopwatch?.Elapsed);
        }

        public void IncrementClientSuccess()
        {
            _metricStatus.Client.IncrementeSuccess();
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementClientSuccess);
        }

        public void IncrementClientError()
        {
            _metricStatus.Client.IncrementeError();
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementClientError);
        }

        public void IncrementeResilienceModuleError()
        {
            _metricStatus.ResilienceModule.IncrementeError();
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementeResilienceModuleError);
        }

        public void IncrementeResilienceModuleSuccess()
        {
            _metricStatus.ResilienceModule.IncrementeSuccess();
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementeResilienceModuleSucess);
        }
    }

    public abstract class MetricResilicienceModuleStatus
    {
    }

    public class MetricRetryStatus : MetricResilicienceModuleStatus
    {
        public int RetryCount { get; private set; }
        public string TotalTimeout => _totalTimeout.ToExtensiveValue();
        private TimeSpan _totalTimeout { get; set; }

        public MetricRetryStatus() 
            => _totalTimeout = new TimeSpan();

        public void IncrementRetryCount() => RetryCount++;

        public void IncrementRetryTotalTimeout(TimeSpan timeout) => _totalTimeout = _totalTimeout.Add(timeout);
    }

    public class MetricCircuitBreakerStatus : MetricResilicienceModuleStatus
    {
        public int BreakCount { get; set; }
        public int ResetStatCount { get; set; }
        public string TotalOfBreak => _timeOfBreak.ToExtensiveValue();
        
        private TimeSpan _timeOfBreak { get; set; }

        public MetricCircuitBreakerStatus() 
            => _timeOfBreak = new TimeSpan();
        
        public void IncrementBreakCount() => BreakCount++;

        public void IncrementBreakTime(TimeSpan timeOfBreak) => _timeOfBreak = _timeOfBreak.Add(timeOfBreak);

        public void IncrementResetStat() => ResetStatCount++;
    }
}