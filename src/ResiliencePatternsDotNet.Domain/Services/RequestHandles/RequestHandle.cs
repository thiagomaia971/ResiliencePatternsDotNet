using System;
using System.Net.Http;
using ResiliencePatternsDotNet.Domain.Common;
using ResiliencePatternsDotNet.Domain.Configurations;
using ResiliencePatternsDotNet.Domain.Entities.Enums;
using ResiliencePatternsDotNet.Domain.Services.Resiliences;

namespace ResiliencePatternsDotNet.Domain.Services.RequestHandles
{
    public class RequestHandle : IRequestHandle
    {
        private readonly IResiliencePatterns _resiliencePatterns;
        private readonly ConfigurationSection _configurationSection;

        public RequestHandle(IResiliencePatterns resiliencePatterns, ConfigurationSection configurationSection)
        {
            _resiliencePatterns = resiliencePatterns;
            _configurationSection = configurationSection;
        }

        public HttpResponseMessage HandleRequest(MetricStatus metric, int probabilityErrorPercent, UrlFetchConfigurationSection success, UrlFetchConfigurationSection error)
            => _configurationSection.RunPolicy switch
            {
                RunPolicyEnum.RETRY => _resiliencePatterns.RetryPolicy
                    .ExecuteAndCapture(() => MakeRequest(metric, probabilityErrorPercent, success, error))
                    .Result,
                RunPolicyEnum.CIRCUIT_BREAKER => _resiliencePatterns.CircuitBreakerPolicy
                    .ExecuteAndCapture(() => MakeRequest(metric, probabilityErrorPercent, success, error))
                    .Result,
                RunPolicyEnum.ALL => _resiliencePatterns
                    .RetryPolicy
                    .ExecuteAndCapture(() =>
                        _resiliencePatterns.CircuitBreakerPolicy.Execute(() =>
                            MakeRequest(metric, probabilityErrorPercent, success, error)))
                    .Result,
                RunPolicyEnum.NONE => MakeRequest(metric, probabilityErrorPercent, success, error),
                _ => throw new ArgumentOutOfRangeException()
            };

        private HttpResponseMessage MakeRequest(MetricStatus metric, int probabilityErrorPercent, UrlFetchConfigurationSection success, UrlFetchConfigurationSection error)
        {
            try
            {
                var actionMethod = String.Empty;
                var actionUrl = String.Empty;
                
                if (ErrorProbabilityService.IsDalayRequest(probabilityErrorPercent))
                {
                    metric.IncrementDelayRequest();
                    actionMethod = error.Method;
                    actionUrl = error.Url;
                    Console.WriteLine($"Request With Delay!");
                }
                else
                {
                    metric.IncrementSuccessRequest();
                    actionMethod = success.Method;
                    actionUrl = success.Url;
                    Console.WriteLine($"Request Normal!");
                }
                
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_configurationSection.UrlConfiguration.BaseUrl);
                    httpClient.Timeout =
                        TimeSpan.FromMilliseconds(_configurationSection.RequestConfiguration.Timeout);
                    var methodEnum = new HttpMethod(actionMethod);

                    var result = httpClient.SendAsync(new HttpRequestMessage(methodEnum, actionUrl)).GetAwaiter().GetResult();
                    Console.WriteLine($"Result: {result.StatusCode}");

                    if (_configurationSection.RunPolicy != RunPolicyEnum.NONE && !result.IsSuccessStatusCode)
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