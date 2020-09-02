namespace ResiliencePatternsDotNet.Domain.Common
{
    public class MetricStatus
    {
        public int IterationCount { get; private set; }
        public int ErrorRequest { get; private set; }
        public int SuccessRequest { get; private set; }
        public int TotalRequest => ErrorRequest + SuccessRequest;

        protected MetricStatus()
        {
        }
        
        public static MetricStatus Create() 
            => new MetricStatus();

        public void IncrementIterationCount() 
            => IterationCount++;

        public void IncrementDelayRequest()
            => ErrorRequest++;

        public void IncrementSuccessRequest()
            => SuccessRequest++;
    }
}