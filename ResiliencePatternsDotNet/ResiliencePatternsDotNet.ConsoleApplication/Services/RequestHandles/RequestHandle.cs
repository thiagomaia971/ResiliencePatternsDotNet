using System;
using System.Net.Http;
using ResiliencePatternsDotNet.ConsoleApplication.Common;
using ResiliencePatternsDotNet.ConsoleApplication.Services.Resiliences;

namespace ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles
{
    public class RequestHandle : IRequestHandle
    {
        private readonly IResiliencePatterns _resiliencePatterns;

        public RequestHandle(IResiliencePatterns resiliencePatterns) 
            => _resiliencePatterns = resiliencePatterns;

        public HttpResponseMessage HandleErrorRequest(string actionMethod, string actionUrl) 
            => HandleRequest(actionMethod, actionUrl, true);

        public HttpResponseMessage HandleSuccessRequest(string actionMethod, string actionUrl) 
            => HandleRequest(actionMethod, actionUrl, false);

        private HttpResponseMessage HandleRequest(string actionMethod, string actionUrl, bool isErrorRequest) 
            => GlobalVariables.ConfigurationSection.RunPolicy switch
            {
                RunPolicyEnum.RETRY => _resiliencePatterns.RetryPolicy
                                            .ExecuteAndCapture(() => MakeRequest(actionMethod, actionUrl, isErrorRequest))
                                            .Result,
                RunPolicyEnum.CIRCUIT_BREAKER => _resiliencePatterns.CircuitBreakerPolicy
                                                    .ExecuteAndCapture(() => MakeRequest(actionMethod, actionUrl, isErrorRequest))
                                                    .Result,
                RunPolicyEnum.ALL => _resiliencePatterns
                                        .RetryPolicy
                                        .ExecuteAndCapture(() =>
                                            _resiliencePatterns.CircuitBreakerPolicy.Execute(() =>
                                                MakeRequest(actionMethod, actionUrl, isErrorRequest)))
                                        .Result,
                RunPolicyEnum.NONE => MakeRequest(actionMethod, actionUrl, isErrorRequest),
                _ => throw new ArgumentOutOfRangeException()
            };

        private static HttpResponseMessage MakeRequest(string actionMethod, string actionUrl, bool isErrorRequest)
        {
            try
            {
                Console.WriteLine($"Request {(isErrorRequest ? "With Delay" : "Normal")}!");
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(GlobalVariables.ConfigurationSection.UrlConfiguration.BaseUrl);
                    httpClient.Timeout =
                        TimeSpan.FromMilliseconds(GlobalVariables.ConfigurationSection.RequestConfiguration.Timeout);
                    var methodEnum = new HttpMethod(actionMethod);

                    var result = httpClient.SendAsync(new HttpRequestMessage(methodEnum, actionUrl)).GetAwaiter().GetResult();
                    Console.WriteLine($"Result: {result.StatusCode}");

                    if (GlobalVariables.ConfigurationSection.RunPolicy != RunPolicyEnum.NONE && !result.IsSuccessStatusCode)
                        throw new HttpRequestException();

                    return result;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Result: Timeout");
                throw new HttpRequestException();
            }
        }
    }
}