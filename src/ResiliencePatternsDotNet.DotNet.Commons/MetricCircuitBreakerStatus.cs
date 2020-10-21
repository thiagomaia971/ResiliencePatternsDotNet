namespace ResiliencePatternsDotNet.DotNet.Commons
{
    public class MetricCircuitBreakerStatus : MetricResilicienceModuleStatus
    {
        public int BreakCount { get; set; }
        public int ResetStatCount { get; set; }
        public long TotalOfBreak { get; set; }

        public void IncrementBreakCount() => BreakCount++;

        public void IncrementBreakTime(long timeOfBreak) => TotalOfBreak += timeOfBreak;

        public void IncrementResetStat() => ResetStatCount++;
    }
}