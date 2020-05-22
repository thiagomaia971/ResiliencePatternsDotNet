using System;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using ResiliencePatternsDotNet.ConsoleApplication.Common;
using ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles;

namespace ResiliencePatternsDotNet.ConsoleApplication.Services.Resiliences
{
    public class ResiliencePatterns : IResiliencePatterns
    {
        public RetryPolicy RetryPolicy { get; private set; }
        public CircuitBreakerPolicy CircuitBreakerPolicy { get; private set; }

        public ResiliencePatterns()
        {
            RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    retryCount: GlobalVariables.ConfigurationSection.RetryConfiguration.Count, 
                    sleepDurationProvider: (i) => TimeSpan.FromMilliseconds(GlobalVariables.ConfigurationSection.RetryConfiguration.SleepDuration), 
                    onRetry: (exception, i) => Console.WriteLine($"\tTry number [{i}]"));
            
            CircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.ExceptionsAllowedBeforeBreaking, 
                    durationOfBreak: TimeSpan.FromMilliseconds(GlobalVariables.ConfigurationSection.CircuitBreakerConfiguration.DurationOfBreaking), 
                    onBreak: (exception, span) => Console.WriteLine($"\tWait for [{span}]"),
                    onReset: () => Console.WriteLine($"\tReseted"));
        }
    }
}