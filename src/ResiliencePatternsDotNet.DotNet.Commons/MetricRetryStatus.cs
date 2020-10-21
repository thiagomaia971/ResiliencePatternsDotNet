namespace ResiliencePatternsDotNet.DotNet.Commons
{
    public class MetricRetryStatus : MetricResilicienceModuleStatus
    {
        public int RetryCount { get; private set; }
        public long TotalTimeout { get; set; }

        public void IncrementRetryCount() => RetryCount++;

        public void IncrementRetryTotalTimeout(long timeout) => TotalTimeout += timeout;
    }
}