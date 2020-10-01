using System.Net.Http;
using ResiliencePatterns.DotNet.Domain.Configurations;

namespace ResiliencePatterns.DotNet.Domain.Services.RequestHandles
{
    public interface IRequestHandle
    {
        HttpResponseMessage HandleRequest(UrlConfigurationSection urlConfiguration);
    }
}