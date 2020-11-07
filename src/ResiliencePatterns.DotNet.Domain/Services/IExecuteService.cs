using System.Threading.Tasks;
using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Configurations;
using ResiliencePatternsDotNet.Commons;

namespace ResiliencePatterns.DotNet.Domain.Services
{
    public interface IExecuteService
    {
        MetricStatus Execute(ConfigurationSection configurationSection);
    }
}