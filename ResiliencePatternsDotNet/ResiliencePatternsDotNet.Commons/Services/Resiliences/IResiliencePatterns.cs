using Polly.CircuitBreaker;
using Polly.Retry;

namespace ResiliencePatternsDotNet.Commons.Services.Resiliences
{
    public interface IResiliencePatterns
    {
        RetryPolicy RetryPolicy { get; }
        CircuitBreakerPolicy CircuitBreakerPolicy { get; }
    }
}