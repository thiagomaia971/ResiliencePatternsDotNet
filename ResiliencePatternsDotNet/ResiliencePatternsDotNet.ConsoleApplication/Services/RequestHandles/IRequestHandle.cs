using System.Net.Http;

namespace ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles
{
    public interface IRequestHandle
    {
        HttpResponseMessage Handle();
    }
}