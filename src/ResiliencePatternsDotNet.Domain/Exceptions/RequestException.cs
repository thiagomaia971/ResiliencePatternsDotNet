using System.Net.Http;

namespace ResiliencePatternsDotNet.Domain.Exceptions
{
    public class RequestException : HttpRequestException
    {
        public HttpResponseMessage HttpResponseMessage { get; }

        public RequestException(HttpResponseMessage httpResponseMessage) 
            => HttpResponseMessage = httpResponseMessage;
    }
}