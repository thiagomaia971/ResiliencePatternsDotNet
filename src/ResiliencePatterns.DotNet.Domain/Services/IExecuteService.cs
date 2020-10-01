using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Configurations;

namespace ResiliencePatterns.DotNet.Domain.Services
{
    public interface IExecuteService
    {
        MetricStatus Execute(ConfigurationSection configurationSection);
    }
}