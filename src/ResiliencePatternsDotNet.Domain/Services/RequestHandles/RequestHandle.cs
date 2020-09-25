using System;
using System.Net.Http;
using ResiliencePatternsDotNet.Domain.Common;
using ResiliencePatternsDotNet.Domain.Configurations;
using ResiliencePatternsDotNet.Domain.Entities.Enums;
using ResiliencePatternsDotNet.Domain.Exceptions;
using ResiliencePatternsDotNet.Domain.Services.Resiliences;

namespace ResiliencePatternsDotNet.Domain.Services.RequestHandles
{
    public class RequestHandle : IRequestHandle
    {
        private readonly IResiliencePatterns _resiliencePatterns;
        private readonly ConfigurationSection _configurationSection;
        private readonly MetricService _metrics;

        public RequestHandle(IResiliencePatterns resiliencePatterns, ConfigurationSection configurationSection,
            MetricService metrics)
        {
            _resiliencePatterns = resiliencePatterns;
            _configurationSection = configurationSection;
            _metrics = metrics;
            CreateCustomMetric();
        }

        private void CreateCustomMetric()
        {
            switch (_configurationSection.RunPolicy)
            {
                case RunPolicyEnum.RETRY:
                    _metrics.CreateRetryCustom();
                    break;
                case RunPolicyEnum.CIRCUIT_BREAKER:
                    _metrics.CreateCircuitBrekerCustom();
                    break;
                case RunPolicyEnum.ALL:
                    break;
                case RunPolicyEnum.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public HttpResponseMessage HandleRequest(UrlConfigurationSection urlConfiguration) 
            => _configurationSection.RunPolicy switch
                {
                    RunPolicyEnum.RETRY => HandleClientResult(_resiliencePatterns.RetryPolicy
                        .ExecuteAndCapture(() => MakeRequest(urlConfiguration))
                        .Result),
                    RunPolicyEnum.CIRCUIT_BREAKER => HandleClientResult(_resiliencePatterns.CircuitBreakerPolicy
                        .ExecuteAndCapture(() => MakeRequest(urlConfiguration))
                        .Result),
                    RunPolicyEnum.ALL => HandleClientResult(_resiliencePatterns.RetryPolicy.ExecuteAndCapture(() =>
                            _resiliencePatterns.CircuitBreakerPolicy.Execute(() =>
                                MakeRequest(urlConfiguration)))
                        .Result),
                    RunPolicyEnum.NONE => HandleClientResult(MakeRequest(urlConfiguration)),
                    _ => throw new ArgumentOutOfRangeException()
                };

        private HttpResponseMessage MakeRequest(UrlConfigurationSection urlConfiguration)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_configurationSection.UrlConfiguration.BaseUrl);
                    httpClient.Timeout =
                        TimeSpan.FromMilliseconds(_configurationSection.RequestConfiguration.Timeout);
                    var methodEnum = new HttpMethod(urlConfiguration.Method);

                    var result = httpClient.SendAsync(new HttpRequestMessage(methodEnum, urlConfiguration.Action)).GetAwaiter().GetResult();
                    Console.WriteLine($"Result: {result.StatusCode}");

                    if (result.IsSuccessStatusCode)
                        _metrics.IncrementeResilienceModuleSuccess();

                    if (_configurationSection.RunPolicy != RunPolicyEnum.NONE && !result.IsSuccessStatusCode)
                        throw new RequestException(result);

                    return result;
                }
            }
            catch (Exception e)
            {                        
                _metrics.IncrementeResilienceModuleError();
                throw;
            }
        }

        private HttpResponseMessage HandleClientResult(HttpResponseMessage result)
        {
            if (result?.IsSuccessStatusCode ?? false)
                _metrics.IncrementClientSuccess();
            else
                _metrics.IncrementClientError();
            
            return result;
        }
    }
}