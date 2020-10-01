﻿using System;
using System.Diagnostics;
using App.Metrics;
using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Extensions;

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
            _metricStatus.AddTotalTime(_stopwatch?.Elapsed);
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