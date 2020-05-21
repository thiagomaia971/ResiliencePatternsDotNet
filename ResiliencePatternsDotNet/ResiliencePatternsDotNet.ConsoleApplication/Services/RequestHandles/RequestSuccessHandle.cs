using System;
using System.Net.Http;
using ResiliencePatternsDotNet.ConsoleApplication.Common;

namespace ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles
{
    public class RequestSuccessHandle : IRequestHandle
    {
        public HttpResponseMessage Handle()
        {
            Console.WriteLine("Request Normal!");
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(GlobalVariables.ConfigurationSection.UrlConfiguration.BaseUrl);
                var method = GlobalVariables.ConfigurationSection.UrlConfiguration.Success.Method;
                var methodEnum = new HttpMethod(method);
                var uri = GlobalVariables.ConfigurationSection.UrlConfiguration.Success.Url;
                return httpClient.SendAsync(new HttpRequestMessage(methodEnum, uri)).GetAwaiter().GetResult();
            }
        }
    }
}