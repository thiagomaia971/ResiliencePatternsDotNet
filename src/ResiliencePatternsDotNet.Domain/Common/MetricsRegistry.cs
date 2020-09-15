using System;
using App.Metrics;
using App.Metrics.Counter;

namespace ResiliencePatternsDotNet.Domain.Common
{
    public class MetricsRegistry
    {
        public CounterOptions IncrementClientSuccess { get; private set; }
        public CounterOptions IncrementClientError { get; private set; }
        public CounterOptions IncrementeResilienceModuleError { get; private set; }
        public CounterOptions IncrementeResilienceModuleSucess { get; private set; }

        public MetricsRegistry()
        {
            var unique = DateTime.Now.ToString("g");
            var tag = new MetricTags("run", unique);
            
            IncrementClientSuccess = new CounterOptions
            {
                Name = "Increment Client Success",
                Context = "IncrementClientSuccess",
                Tags = tag,
                MeasurementUnit = Unit.Calls
            };
            
            IncrementClientError = new CounterOptions
            {
                Name = "Increment Client Error",
                Context = "IncrementClientError",
                Tags = tag,
                MeasurementUnit = Unit.Calls
            };
            
            IncrementeResilienceModuleError = new CounterOptions
            {
                Name = "Incremente Resilience Module Error",
                Context = "IncrementeResilienceModuleError",
                Tags = tag,
                MeasurementUnit = Unit.Calls
            };
            
            IncrementeResilienceModuleSucess = new CounterOptions
            {
                Name = "Incremente Resilience Module Sucess",
                Context = "IncrementeResilienceModuleSucess",
                Tags = tag,
                MeasurementUnit = Unit.Calls
            };
        }
    }
}