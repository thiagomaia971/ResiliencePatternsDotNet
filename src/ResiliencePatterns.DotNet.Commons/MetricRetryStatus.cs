using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.Commons
{
    public class MetricRetryStatus : MetricResilicienceModuleStatus
    {
        [JsonProperty]
        public int RetryCount { get; private set; }
        
        [JsonProperty]
        public long TotalTimeout { get; set; }

        public void IncrementRetryCount() => RetryCount++;

        public void IncrementRetryTotalTimeout(long timeout) => TotalTimeout += timeout;
    }
}