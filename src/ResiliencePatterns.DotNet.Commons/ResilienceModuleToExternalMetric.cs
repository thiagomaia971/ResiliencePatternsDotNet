using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.Commons
{
    public class ResilienceModuleToExternalMetric : MetricCountStatus
    {
        [JsonProperty]
        public long TotalSuccessTime { get; private set; }

        [JsonProperty] public double AverageSuccessTimePerRequest => (double) TotalSuccessTime / (double) Success;

        public void IncrementeSuccessTime(long milliseconds) 
            => TotalSuccessTime += milliseconds;
    }
}