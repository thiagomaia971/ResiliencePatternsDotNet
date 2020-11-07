using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
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
        private readonly IHttpClientFactory _httpClient;

        public RequestHandle(IResiliencePatterns resiliencePatterns, ConfigurationSection configurationSection,
            MetricService metrics, IHttpClientFactory httpClient)
        {
            _resiliencePatterns = resiliencePatterns;
            _configurationSection = configurationSection;
            _metrics = metrics;
            _httpClient = httpClient;
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
        {
            switch (_configurationSection.RunPolicy)
            {
                case RunPolicyEnum.RETRY:
                    return HandleClientResult(_resiliencePatterns.RetryPolicy
                        .ExecuteAndCapture(() => MakeRequest(urlConfiguration))
                        .Result);
                case RunPolicyEnum.CIRCUIT_BREAKER:
                    return HandleClientResult(_resiliencePatterns.CircuitBreakerPolicy
                        .ExecuteAndCapture(() => MakeRequest(urlConfiguration))
                        .Result);
                case RunPolicyEnum.ALL:
                    return HandleClientResult(_resiliencePatterns.RetryPolicy.ExecuteAndCapture(() =>
                            _resiliencePatterns.CircuitBreakerPolicy.Execute(() => MakeRequest(urlConfiguration)))
                        .Result);
                case RunPolicyEnum.NONE:
                    try
                    {
                        return HandleClientResult(MakeRequest(urlConfiguration));
                    }
                    catch (RequestException e)
                    {
                        return HandleClientResult(e.HttpResponseMessage);

                    }
                    catch (Exception e)
                    {
                        return HandleClientResult(null);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private HttpResponseMessage MakeRequest(UrlConfigurationSection urlConfiguration)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var server = _configurationSection.UrlConfiguration.BaseUrl.Split("//").LastOrDefault().Split(':').FirstOrDefault();
                var port = int.Parse(_configurationSection.UrlConfiguration.BaseUrl.Split(':').LastOrDefault());
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(server, port);
                    using NetworkStream networkStream = tcpClient.GetStream();
                    networkStream.ReadTimeout = _configurationSection.RequestConfiguration.Timeout;
                    using var writer = new StreamWriter(networkStream);
                    var message = $@"{urlConfiguration.Method} / HTTP/1.1
Host: {server}" + "\r\n\r\n";

                    using var reader = new StreamReader(networkStream, Encoding.UTF8);
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    networkStream.Write(bytes, 0, bytes.Length);
                    var readToEnd = reader.ReadToEnd();
                    //Console.WriteLine(readToEnd);
                    var successCodes = new string[]
                        {"HTTP/1.1 200", "HTTP/1.1 201", "HTTP/1.1 202", "HTTP/1.1 203", "HTTP/1.1 204"};
                    
                    var result = new HttpResponseMessage
                    {
                        StatusCode =  successCodes.Any(x => readToEnd.Contains(x)) ? HttpStatusCode.OK : HttpStatusCode.BadRequest
                    };
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
                Console.WriteLine(e);
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