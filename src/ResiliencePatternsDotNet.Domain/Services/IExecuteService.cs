using ResiliencePatternsDotNet.Domain.Common;

namespace ResiliencePatternsDotNet.Domain.Services
{
    public interface IExecuteService
    {
        MetricStatus Execute(ResiliencePatternsDotNet.Domain.Configurations.ConfigurationSection configurationSection);
    }
}