using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.Commons
{
    public class ClientToModuleMetric : MetricCountStatus 
    {
        [JsonProperty]
        public long TotalTime { get; private set; }

        [JsonProperty] public double AverageTimePerRequest => (double) TotalTime / (double) Total;

        public void IncrementTime(long milliseconds) 
            => TotalTime += milliseconds;
    }
}