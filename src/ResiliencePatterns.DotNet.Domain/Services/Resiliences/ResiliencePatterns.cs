using System;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using ResiliencePatterns.DotNet.Domain.Configurations;
using ResiliencePatterns.DotNet.Domain.Entities.Enums;

namespace ResiliencePatterns.DotNet.Domain.Services.Resiliences
{
    public class ResiliencePatterns : IResiliencePatterns
    {
        private readonly MetricService _metricService;
        public ConfigurationSection ConfigurationSection { get; private set; }
        public AsyncRetryPolicy RetryPolicy { get; private set; }
        public AsyncCircuitBreakerPolicy CircuitBreakerPolicy { get; private set; }

        public ResiliencePatterns(MetricService metricService)
        {
            _metricService = metricService;
        }
        
        public void Configure(ConfigurationSection configurationSection)
        {
            Console.WriteLine("------------------- Created ResiliencePatterns ------------------- ");
            ConfigurationSection = configurationSection;
            CreateRetryPolicy();
            CreateCircuitBreakerPolicy();
        }

        private void CreateRetryPolicy()
        {
            switch (ConfigurationSection.RetryConfiguration.SleepDurationType)
            {
                case SleepDurationType.FIXED:
                    CreateRetryFixedSleepDurationPolicy();
                    break;
                case SleepDurationType.EXPONENTIAL_BACKOFF:
                    CreateRetryExponencialBackoffSleepDurationPolicy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CreateRetryFixedSleepDurationPolicy() 
            => RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: ConfigurationSection.RetryConfiguration.Count,
                    sleepDurationProvider: (i) =>
                        TimeSpan.FromMilliseconds(ConfigurationSection.RetryConfiguration.SleepDuration),
                    onRetry: (exception, timeout, context) =>
                    {
                        _metricService.RetryMetric.IncrementRetryCount();
                        _metricService.RetryMetric.IncrementRetryTotalTimeout((long) timeout.TotalMilliseconds);
                        Console.WriteLine($"\tNew timeout of [{timeout}]");
                    });

        private void CreateRetryExponencialBackoffSleepDurationPolicy()
            => RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: ConfigurationSection.RetryConfiguration.Count,
                    sleepDurationProvider: (i) =>
                        TimeSpan.FromMilliseconds(Math.Pow(ConfigurationSection.RetryConfiguration.ExponentialBackoffPow, i) * ConfigurationSection.RetryConfiguration.SleepDuration),
                    onRetry: (exception, timeout, context) =>
                    {
                        _metricService.RetryMetric.IncrementRetryCount();
                        _metricService.RetryMetric.IncrementRetryTotalTimeout((long) timeout.TotalMilliseconds);
                        Console.WriteLine($"\tNew timeout of [{timeout}]");
                    });

        private void CreateCircuitBreakerPolicy()
        {
            if (ConfigurationSection.CircuitBreakerConfiguration.IsSimpleConfiguration)
                CreateCircuitBreakerSimplePolicy();
            else
                CreateCircuitBreakerAdvancedPolicy();
        }

        private void CreateCircuitBreakerSimplePolicy() 
            => CircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: ConfigurationSection.CircuitBreakerConfiguration.ExceptionsAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromMilliseconds(ConfigurationSection.CircuitBreakerConfiguration.DurationOfBreaking),
                    onBreak: (exception, timeOfBreak) =>
                    {
                        _metricService.CircuitBreakerMetric.IncrementBreakCount();
                        _metricService.CircuitBreakerMetric.IncrementBreakTime((long) timeOfBreak.TotalMilliseconds);
                        Console.WriteLine($"\t[{DateTime.Now}] Break for [{timeOfBreak}]");
                    },
                    onReset: () => Console.WriteLine($"\tReseted"),
                    () => Console.WriteLine($"\t[{DateTime.Now}] HalfOpen"));

        private void CreateCircuitBreakerAdvancedPolicy() 
            => CircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: ConfigurationSection.CircuitBreakerConfiguration.FailureThreshold,
                    samplingDuration: TimeSpan.FromMilliseconds(ConfigurationSection.CircuitBreakerConfiguration.SamplingDuration),
                    minimumThroughput: ConfigurationSection.CircuitBreakerConfiguration.MinimumThroughput,
                    durationOfBreak: TimeSpan.FromMilliseconds(ConfigurationSection.CircuitBreakerConfiguration.DurationOfBreaking),
                    onBreak: (exception, timeOfBreak) =>
                    {
                        _metricService.CircuitBreakerMetric.IncrementBreakCount();
                        _metricService.CircuitBreakerMetric.IncrementBreakTime((long) timeOfBreak.TotalMilliseconds);
                        Console.WriteLine($"\tBreak for [{timeOfBreak}]");
                    },
                    onReset: () =>
                    {
                        _metricService.CircuitBreakerMetric.IncrementResetStat();
                        Console.WriteLine($"\tReseted");
                    });
    }
}