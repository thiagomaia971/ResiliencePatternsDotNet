using System.Net.Http;

namespace ResiliencePatternsDotNet.Commons.Services.RequestHandles
{
    public interface IRequestHandle
    {
        HttpResponseMessage HandleSuccessRequest(string actionMethod, string actionUrl);
        HttpResponseMessage HandleErrorRequest(string actionMethod, string actionUrl);
    }
}