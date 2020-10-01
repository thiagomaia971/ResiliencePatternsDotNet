using System;
using ResiliencePatterns.DotNet.Domain.Extensions;
using ResiliencePatterns.DotNet.Domain.Services;

namespace ResiliencePatterns.DotNet.Domain.Common
{
    public class MetricStatus
    {
        public string TotalTime 
            => _totalTime?.ToExtensiveValue();
        private TimeSpan? _totalTime { get; set; }
        public MetricCountStatus ClientToModule { get; private set; }
        public MetricCountStatus ResilienceModuleToExternalService { get; private set; }
        public MetricRetryStatus RetryMetrics { get; private set; }
        public MetricCircuitBreakerStatus CircuitBreakerMetrics { get; private set; }

        protected MetricStatus()
        {
            ClientToModule = new MetricCountStatus();
            ResilienceModuleToExternalService = new MetricCountStatus();
        }
        
        public static MetricStatus Create() 
            => new MetricStatus();
        
        public void CreateRetryCustom() => RetryMetrics = new MetricRetryStatus();
        public void CreateCircuitBrekerCustom() => CircuitBreakerMetrics = new MetricCircuitBreakerStatus();
        public void AddTotalTime(TimeSpan? totalTime) => _totalTime = totalTime;
    }

    public class MetricCountStatus
    {
        public int Success { get; private set; }
        public int Error { get; private set; }
        public int Total => Success + Error;

        public void IncrementeSuccess() => Success++;
        public void IncrementeError() => Error++;
    }
}