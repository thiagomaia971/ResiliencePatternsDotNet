using System.Net.Http;
using ResiliencePatternsDotNet.Domain.Configurations;

namespace ResiliencePatternsDotNet.Domain.Services.RequestHandles
{
    public interface IRequestHandle
    {
        HttpResponseMessage HandleRequest(UrlConfigurationSection urlConfiguration);
    }
}