using System;
using App.Metrics;
using App.Metrics.Counter;

namespace ResiliencePatternsDotNet.Domain.Common
{
    public class MetricsRegistry
    {
        public CounterOptions IncrementIterationCount { get; private set; }
        public CounterOptions IncrementErrorRequest { get; private set; }
        public CounterOptions IncrementSuccessRequest { get; private set; }

        public MetricsRegistry()
        {
            var unique = DateTime.Now.ToString("g");
            var tag = new MetricTags("run", unique);
            
            IncrementIterationCount = new CounterOptions
            {
                Name = "Incremented Iteration Count",
                Context = "IncrementIterationCount",
                Tags = tag,
                // ResetOnReporting = true,
                MeasurementUnit = Unit.Calls
            };
            
            IncrementErrorRequest = new CounterOptions
            {
                Name = "Incremented Error Request Count",
                Context = "IncrementErrorRequest",
                Tags = tag,
                // ResetOnReporting = true,
                MeasurementUnit = Unit.Calls
            };
            
            IncrementSuccessRequest = new CounterOptions
            {
                Name = "Incremented Success Request Count",
                Context = "IncrementSuccessRequest",
                Tags = tag,
                // ResetOnReporting = true,
                MeasurementUnit = Unit.Calls
            };
        }
    }
}