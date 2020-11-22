using Polly.CircuitBreaker;
using Polly.Retry;
using ResiliencePatterns.DotNet.Domain.Configurations;

namespace ResiliencePatterns.DotNet.Domain.Services.Resiliences
{
    public interface IResiliencePatterns
    {
        RetryPolicy RetryPolicy { get; }
        CircuitBreakerPolicy CircuitBreakerPolicy { get; }

        void Configure(ConfigurationSection configurationSection);
    }
}