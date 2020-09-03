using App.Metrics;
using App.Metrics.Counter;

namespace ResiliencePatternsDotNet.Domain.Common
{
    public class MetricsRegistry
    {
        public static CounterOptions IncrementIterationCount => new CounterOptions
        {
            Name = "Incremented Iteration Count",
            Context = "IncrementIterationCount",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions IncrementErrorRequest => new CounterOptions
        {
            Name = "Incremented Error Request Count",
            Context = "IncrementErrorRequest",
            MeasurementUnit = Unit.Calls
        };
        public static CounterOptions IncrementSuccessRequest => new CounterOptions
        {
            Name = "Incremented Success Request Count",
            Context = "IncrementSuccessRequest",
            MeasurementUnit = Unit.Calls
        };
    }
}