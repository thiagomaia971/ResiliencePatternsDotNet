using System.Threading;
using App.Metrics;
using ResiliencePatternsDotNet.Domain.Common;

namespace ResiliencePatternsDotNet.Domain.Services
{
    public class MetricService
    {
        private readonly MetricsRegistry _metricsRegistry;
        private readonly IMetrics _metrics;
        public MetricStatus MetricStatus { get; private set; }

        public MetricService(IMetrics metrics, MetricsRegistry metricsRegistry)
        {
            _metrics = metrics;
            _metricsRegistry = metricsRegistry;
            MetricStatus = MetricStatus.Create();
        }

        public void IncrementIterationCount()
        {
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementIterationCount);
            MetricStatus.IncrementIterationCount();
        }

        public void IncrementErrorRequest()
        {
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementErrorRequest);
            MetricStatus.IncrementDelayRequest();
        }

        public void IncrementSuccessRequest()
        {
            _metrics.Measure.Counter.Increment(_metricsRegistry.IncrementSuccessRequest);
            MetricStatus.IncrementSuccessRequest();
        }

        public void ResetAll()
        {
            Thread.Sleep(2000);
            _metrics.Measure.Counter.Decrement(_metricsRegistry.IncrementErrorRequest, MetricStatus.ErrorRequest);
            _metrics.Measure.Counter.Decrement(_metricsRegistry.IncrementIterationCount, MetricStatus.IterationCount);
            _metrics.Measure.Counter.Decrement(_metricsRegistry.IncrementSuccessRequest, MetricStatus.SuccessRequest);
        }
    }
}