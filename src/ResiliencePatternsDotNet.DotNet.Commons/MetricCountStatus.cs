using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.DotNet.Commons
{
    public class MetricCountStatus
    {
        [JsonProperty]
        public int Success { get; private set; }

        [JsonProperty]
        public int Error { get; private set; }

        [JsonProperty]
        public int Total => Success + Error;
        
        [JsonProperty]
        public long TotalSuccessTime { get; private set; }

        [JsonProperty] public long TotalSuccessTimePerRequest => TotalSuccessTime / Success;

        public void IncrementeSuccess() => Success++;
        public void IncrementeError() => Error++;

        public void IncrementeSuccessTime(long milliseconds) 
            => TotalSuccessTime += milliseconds;
    }
}