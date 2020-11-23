using Polly.CircuitBreaker;
using Polly.Retry;
using ResiliencePatterns.DotNet.Domain.Configurations;

namespace ResiliencePatterns.DotNet.Domain.Services.Resiliences
{
    public interface IResiliencePatterns
    {
        AsyncRetryPolicy RetryPolicy { get; }
        AsyncCircuitBreakerPolicy CircuitBreakerPolicy { get; }

        void Configure(ConfigurationSection configurationSection);
    }
}