using System;
using ResiliencePatternsDotNet.Domain.Extensions;
using ResiliencePatternsDotNet.Domain.Services;

namespace ResiliencePatternsDotNet.Domain.Common
{
    public class MetricStatus
    {
        public string TotalTime 
            => _totalTime?.ToExtensiveValue();
        private TimeSpan? _totalTime { get; set; }
        public MetricCountStatus Client { get; private set; }
        public MetricCountStatus ResilienceModule { get; private set; }
        public MetricResilicienceModuleStatus CustomResilience { get; private set; }

        protected MetricStatus()
        {
            Client = new MetricCountStatus();
            ResilienceModule = new MetricCountStatus();
        }
        
        public static MetricStatus Create() 
            => new MetricStatus();

        public void CreateRetryCustom() => CustomResilience = new MetricRetryStatus();
        public void CreateCircuitBrekerCustom() => CustomResilience = new MetricCircuitBreakerStatus();
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