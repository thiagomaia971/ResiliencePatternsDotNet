using System.Net.Http;
using System.Threading.Tasks;
using ResiliencePatterns.DotNet.Domain.Configurations;
using ResiliencePatterns.DotNet.Domain.Services.Resiliences;

namespace ResiliencePatterns.DotNet.Domain.Services.RequestHandles
{
    public interface IRequestHandle
    {
        void Configure(ConfigurationSection configurationSection);
        Task<HttpResponseMessage> HandleRequest(UrlConfigurationSection urlConfiguration);
    }
}