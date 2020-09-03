using App.Metrics;
using ResiliencePatternsDotNet.Domain.Common;

namespace ResiliencePatternsDotNet.Domain.Services
{
    public class MetricService
    {
        private readonly IMetrics _metrics;
        public MetricStatus MetricStatus { get; private set; }

        public MetricService(IMetrics metrics)
        {
            _metrics = metrics;
            MetricStatus = MetricStatus.Create();
        }

        public void IncrementIterationCount()
        {
            _metrics.Measure.Counter.Increment(MetricsRegistry.IncrementIterationCount);
            MetricStatus.IncrementIterationCount();
        }

        public void IncrementErrorRequest()
        {
            _metrics.Measure.Counter.Increment(MetricsRegistry.IncrementErrorRequest);
            MetricStatus.IncrementDelayRequest();
        }

        public void IncrementSuccessRequest()
        {
            _metrics.Measure.Counter.Increment(MetricsRegistry.IncrementSuccessRequest);
            MetricStatus.IncrementSuccessRequest();
        }
    }
}