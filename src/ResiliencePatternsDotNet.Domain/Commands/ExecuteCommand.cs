
using ResiliencePatternsDotNet.Domain.Common;
using ResiliencePatternsDotNet.Domain.Configurations;

namespace ResiliencePatternsDotNet.Domain.Commands
{
    public class ExecuteCommand: ConfigurationSection, ICommand<MetricStatus>
    {
    }
}