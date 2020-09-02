using Polly.CircuitBreaker;
using Polly.Retry;

namespace ResiliencePatternsDotNet.Domain.Services.Resiliences
{
    public interface IResiliencePatterns
    {
        RetryPolicy RetryPolicy { get; }
        CircuitBreakerPolicy CircuitBreakerPolicy { get; }
    }
}