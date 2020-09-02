using System;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using ResiliencePatternsDotNet.Commons.Common;

namespace ResiliencePatternsDotNet.Commons.Services.Resiliences
{
    public class ResiliencePatterns : IResiliencePatterns
    {
        public RetryPolicy RetryPolicy { get; private set; }
        public CircuitBreakerPolicy CircuitBreakerPolicy { get; private set; }

        public ResiliencePatterns()
        {
            CreateRetryPolicy();
            CreateCircuitBreakerPolicy();
        }

        private void CreateRetryPolicy()
        {
            switch (GlobalVariables.ConfigurationSection.RetryConfiguration.SleepDurationType)
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
                    retryCount: GlobalVariables.ConfigurationSection.RetryConfiguration.Count,
                    sleepDurationProvider: (i) =>
                        TimeSpan.FromMilliseconds(GlobalVariables.ConfigurationSection.RetryConfiguration.SleepDuration),
                    onRetry: (exception, i) => Console.WriteLine($"\tTry number [{i}]"));

        private void CreateRetryExponencialBackoffSleepDurationPolicy()
            => RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    retryCount: GlobalVariables.ConfigurationSection.RetryConfiguration.Count,
                    sleepDurationProvider: (i) =>
                        TimeSpan.FromMilliseconds(Math.Pow(2, i) * GlobalVariables.ConfigurationSection.RetryConfiguration.SleepDuration),
                    onRetry: (exception, i) => Console.WriteLine($"\tTry number [{i}]"));

        private void CreateCircuitBreakerPolicy()
        {
            if (GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.IsSimpleConfiguration)
                CreateCircuitBreakerSimplePolicy();
            else
                CreateCircuitBreakerAdvancedPolicy();
        }

        private void CreateCircuitBreakerSimplePolicy() 
            => CircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.SimpleConfiguration.ExceptionsAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromMilliseconds(GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.DurationOfBreaking),
                    onBreak: (exception, span) => Console.WriteLine($"\tWait for [{span}]"),
                    onReset: () => Console.WriteLine($"\tReseted"));

        private void CreateCircuitBreakerAdvancedPolicy() 
            => CircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .AdvancedCircuitBreaker(
                    failureThreshold: GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.AdvancedConfiguration.FailureThreshold,
                    samplingDuration: TimeSpan.FromMilliseconds(GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.AdvancedConfiguration.SamplingDuration),
                    minimumThroughput: GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.AdvancedConfiguration.MinimumThroughput,
                    durationOfBreak: TimeSpan.FromMilliseconds(GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.DurationOfBreaking),
                    onBreak: (exception, span) => Console.WriteLine($"\tWait for [{span}]"),
                    onReset: () => Console.WriteLine($"\tReseted"));
    }
}