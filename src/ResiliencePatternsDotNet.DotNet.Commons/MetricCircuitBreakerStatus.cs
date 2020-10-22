using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.DotNet.Commons
{
    public class MetricCircuitBreakerStatus : MetricResilicienceModuleStatus
    {
        [JsonProperty]
        public int BreakCount { get; set; }
        
        [JsonProperty]
        public int ResetStatCount { get; set; }
        
        [JsonProperty]
        public long TotalOfBreak { get; set; }

        public void IncrementBreakCount() => BreakCount++;

        public void IncrementBreakTime(long timeOfBreak) => TotalOfBreak += timeOfBreak;

        public void IncrementResetStat() => ResetStatCount++;
    }
}