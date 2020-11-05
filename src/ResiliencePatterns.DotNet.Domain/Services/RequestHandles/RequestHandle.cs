using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ResiliencePatterns.DotNet.Domain.Configurations;
using ResiliencePatterns.DotNet.Domain.Entities.Enums;
using ResiliencePatterns.DotNet.Domain.Exceptions;
using ResiliencePatterns.DotNet.Domain.Services.Resiliences;

namespace ResiliencePatterns.DotNet.Domain.Services.RequestHandles
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

        public async Task<HttpResponseMessage> HandleRequest(UrlConfigurationSection urlConfiguration)
        {
            switch (_configurationSection.RunPolicy)
            {
                case RunPolicyEnum.RETRY:
                    return await HandleClientResult(_resiliencePatterns.RetryPolicy
                        .ExecuteAndCapture(() => MakeRequest(urlConfiguration))
                        .Result);
                case RunPolicyEnum.CIRCUIT_BREAKER:
                    return await HandleClientResult(_resiliencePatterns.CircuitBreakerPolicy
                        .ExecuteAndCapture(() => MakeRequest(urlConfiguration))
                        .Result);
                case RunPolicyEnum.ALL:
                    return await HandleClientResult(_resiliencePatterns.RetryPolicy.ExecuteAndCapture(() =>
                            _resiliencePatterns.CircuitBreakerPolicy.Execute(() => MakeRequest(urlConfiguration)))
                        .Result);
                case RunPolicyEnum.NONE:
                    try
                    {
                        return await HandleClientResult(MakeRequest(urlConfiguration));
                    }
                    catch (RequestException e)
                    {
                        return await HandleClientResult(e.HttpResponseMessage);

                    }
                    catch (Exception e)
                    {
                        return await HandleClientResult(null);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private HttpResponseMessage MakeRequest(UrlConfigurationSection urlConfiguration)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    
                    httpClient.BaseAddress = new Uri(_configurationSection.UrlConfiguration.BaseUrl);
                    httpClient.Timeout =
                        TimeSpan.FromMilliseconds(_configurationSection.RequestConfiguration.Timeout);
                    var methodEnum = new HttpMethod(urlConfiguration.Method);

                    var result = httpClient.SendAsync(new HttpRequestMessage(methodEnum, urlConfiguration.Action)).GetAwaiter().GetResult();
                    Console.WriteLine($"Result: {result.StatusCode}");
                    
                    stopWatch.Stop();

                    if (result.IsSuccessStatusCode)
                    {
                        _metrics.IncrementeResilienceModuleSuccess();
                        _metrics.IncrementeResilienceModuleSuccessTime(stopWatch.ElapsedMilliseconds);
                    }else
                        _metrics.IncrementeResilienceModuleErrorTime(stopWatch.ElapsedMilliseconds);
                    

                    if (!result.IsSuccessStatusCode)
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

        private async Task<HttpResponseMessage> HandleClientResult(HttpResponseMessage result)
        {
            if (result?.IsSuccessStatusCode ?? false)
                _metrics.IncrementClientSuccess();
            else
                _metrics.IncrementClientError();
            
            return result;
        }
    }
}