using Polly.CircuitBreaker;
using Polly.Retry;

namespace ResiliencePatterns.DotNet.Domain.Services.Resiliences
{
    public interface IResiliencePatterns
    {
        RetryPolicy RetryPolicy { get; }
        CircuitBreakerPolicy CircuitBreakerPolicy { get; }
    }
}