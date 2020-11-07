using System.Net.Http;
using System.Threading.Tasks;
using ResiliencePatterns.DotNet.Domain.Configurations;

namespace ResiliencePatterns.DotNet.Domain.Services.RequestHandles
{
    public interface IRequestHandle
    {
        HttpResponseMessage HandleRequest(UrlConfigurationSection urlConfiguration);
    }
}