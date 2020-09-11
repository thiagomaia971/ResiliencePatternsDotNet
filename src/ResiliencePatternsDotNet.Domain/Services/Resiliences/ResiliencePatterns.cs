using System;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using ResiliencePatternsDotNet.Domain.Common;
using ResiliencePatternsDotNet.Domain.Configurations;
using ResiliencePatternsDotNet.Domain.Entities.Enums;

namespace ResiliencePatternsDotNet.Domain.Services.Resiliences
{
    public class ResiliencePatterns : IResiliencePatterns
    {
        private readonly ConfigurationSection _configurationSection;
        private readonly MetricService _metricService;
        public RetryPolicy RetryPolicy { get; private set; }
        public CircuitBreakerPolicy CircuitBreakerPolicy { get; private set; }

        public ResiliencePatterns(ConfigurationSection configurationSection, MetricService metricService)
        {
            _configurationSection = configurationSection;
            _metricService = metricService;
            CreateRetryPolicy();
            CreateCircuitBreakerPolicy();
        }

        private void CreateRetryPolicy()
        {
            switch (_configurationSection.RetryConfiguration.SleepDurationType)
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
                .WaitAndRetry(
                    retryCount: _configurationSection.RetryConfiguration.Count,
                    sleepDurationProvider: (i) =>
                        TimeSpan.FromMilliseconds(_configurationSection.RetryConfiguration.SleepDuration),
                    onRetry: (exception, timeout) =>
                    {
                        _metricService.RetryMetric.IncrementRetryCount();
                        _metricService.RetryMetric.IncrementRetryTotalTimeout(timeout);
                        Console.WriteLine($"\tNew timeout of [{timeout}]");
                    });

        private void CreateRetryExponencialBackoffSleepDurationPolicy()
            => RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    retryCount: _configurationSection.RetryConfiguration.Count,
                    sleepDurationProvider: (i) =>
                        TimeSpan.FromMilliseconds(Math.Pow(2, i) * _configurationSection.RetryConfiguration.SleepDuration),
                    onRetry: (exception, timeout) =>
                    {
                        _metricService.RetryMetric.IncrementRetryCount();
                        _metricService.RetryMetric.IncrementRetryTotalTimeout(timeout);
                        Console.WriteLine($"\tNew timeout of [{timeout}]");
                    });

        private void CreateCircuitBreakerPolicy()
        {
            if (_configurationSection.CircuitBreakerConfiguration.IsSimpleConfiguration)
                CreateCircuitBreakerSimplePolicy();
            else
                CreateCircuitBreakerAdvancedPolicy();
        }

        private void CreateCircuitBreakerSimplePolicy() 
            => CircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: _configurationSection.CircuitBreakerConfiguration.SimpleConfiguration.ExceptionsAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromMilliseconds(_configurationSection.CircuitBreakerConfiguration.DurationOfBreaking),
                    onBreak: (exception, timeOfBreak) =>
                    {
                        _metricService.CircuitBreakerMetric.IncrementBreakCount();
                        _metricService.CircuitBreakerMetric.IncrementBreakTime(timeOfBreak);
                        Console.WriteLine($"\tBreak for [{timeOfBreak}]");
                    },
                    onReset: () => Console.WriteLine($"\tReseted"));

        private void CreateCircuitBreakerAdvancedPolicy() 
            => CircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .AdvancedCircuitBreaker(
                    failureThreshold: _configurationSection.CircuitBreakerConfiguration.AdvancedConfiguration.FailureThreshold,
                    samplingDuration: TimeSpan.FromMilliseconds(_configurationSection.CircuitBreakerConfiguration.AdvancedConfiguration.SamplingDuration),
                    minimumThroughput: _configurationSection.CircuitBreakerConfiguration.AdvancedConfiguration.MinimumThroughput,
                    durationOfBreak: TimeSpan.FromMilliseconds(_configurationSection.CircuitBreakerConfiguration.DurationOfBreaking),
                    onBreak: (exception, timeOfBreak) =>
                    {
                        _metricService.CircuitBreakerMetric.IncrementBreakCount();
                        _metricService.CircuitBreakerMetric.IncrementBreakTime(timeOfBreak);
                        Console.WriteLine($"\tBreak for [{timeOfBreak}]");
                    },
                    onReset: () =>
                    {
                        _metricService.CircuitBreakerMetric.IncrementResetStat();
                        Console.WriteLine($"\tReseted");
                    });
    }
}