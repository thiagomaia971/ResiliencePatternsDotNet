namespace ResiliencePatternsDotNet.DotNet.Commons
{
    public class MetricCountStatus
    {
        public int Success { get; private set; }
        public int Error { get; private set; }
        public int Total => Success + Error;
        public long TotalSuccessTime { get; private set; }

        public void IncrementeSuccess() => Success++;
        public void IncrementeError() => Error++;

        public void IncrementeSuccessTime(long milliseconds) 
            => TotalSuccessTime += milliseconds;
    }
}