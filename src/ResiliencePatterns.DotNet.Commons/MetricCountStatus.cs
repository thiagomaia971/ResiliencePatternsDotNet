using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.Commons
{
    public class MetricCountStatus
    {
        [JsonProperty]
        public int Success { get; private set; }

        [JsonProperty]
        public int Error { get; private set; }

        [JsonProperty]
        public int Total => Success + Error;

        public void IncrementeSuccess() => Success++;
        public void IncrementeError() => Error++;
    }
}