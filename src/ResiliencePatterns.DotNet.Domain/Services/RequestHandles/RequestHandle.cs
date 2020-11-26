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
        private readonly MetricService _metrics;
        private readonly HttpClient _httpClient;
        public ConfigurationSection ConfigurationSection { get; private set; }

        public RequestHandle(IResiliencePatterns resiliencePatterns, MetricService metrics, HttpClient httpClient)
        {
            _resiliencePatterns = resiliencePatterns;
            _metrics = metrics;
            _httpClient = httpClient;
        }

        public void Configure(ConfigurationSection configurationSection)
        {
            ConfigurationSection = configurationSection;
            CreateCustomMetric();
        }

        private void CreateCustomMetric()
        {
            switch (ConfigurationSection.RunPolicy)
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
            var httpResponseMessage = await Get(urlConfiguration);
            return HandleClientResult(httpResponseMessage);
        }

        private async Task<HttpResponseMessage> Get(UrlConfigurationSection urlConfiguration)
        {
            switch (ConfigurationSection.RunPolicy)
            {
                case RunPolicyEnum.RETRY:
                    return (await _resiliencePatterns.RetryPolicy
                        .ExecuteAndCaptureAsync(async () => await MakeRequest(urlConfiguration)))
                        .Result;
                case RunPolicyEnum.CIRCUIT_BREAKER:
                    return (await _resiliencePatterns.CircuitBreakerPolicy
                        .ExecuteAndCaptureAsync(() => MakeRequest(urlConfiguration)))
                        .Result;
                case RunPolicyEnum.ALL:
                    return (await _resiliencePatterns.RetryPolicy
                            .ExecuteAndCaptureAsync(() =>
                                _resiliencePatterns.CircuitBreakerPolicy.ExecuteAsync(() => MakeRequest(urlConfiguration))))
                        .Result;
                case RunPolicyEnum.NONE:
                    try
                    {
                        return await MakeRequest(urlConfiguration);
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

        private async Task<HttpResponseMessage> MakeRequest(UrlConfigurationSection urlConfiguration)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                HttpResponseMessage result = null;
                // result = await ResponseViaTCP(urlConfiguration);
                result = await ResponseViaHttpClient(urlConfiguration);

                stopWatch.Stop();

                if (result.IsSuccessStatusCode)
                {
                    _metrics.IncrementeResilienceModuleSuccess();
                    _metrics.IncrementeResilienceModuleSuccessTime(stopWatch.ElapsedMilliseconds);
                }
                else
                    _metrics.IncrementeResilienceModuleErrorTime(stopWatch.ElapsedMilliseconds);

                if (!result.IsSuccessStatusCode)
                    throw new RequestException(result);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _metrics.IncrementeResilienceModuleError();
                throw;
            }
        }

        private Task<HttpResponseMessage> ResponseViaHttpClient(UrlConfigurationSection urlConfiguration)
        {
            lock (_httpClient)
            {
                if (_httpClient.BaseAddress == null)
                    _httpClient.BaseAddress = new Uri(urlConfiguration.BaseUrl);
                var httpMethod = new HttpMethod(urlConfiguration.Method);
                return _httpClient.SendAsync(new HttpRequestMessage(httpMethod, urlConfiguration.Action));    
            }
        }

        private async Task<HttpResponseMessage> ResponseViaTCP(UrlConfigurationSection urlConfiguration)
        {
            HttpResponseMessage result;
            var server = ConfigurationSection.UrlConfiguration.BaseUrl.Split("//").LastOrDefault().Split(':').FirstOrDefault();
            var port = int.Parse(ConfigurationSection.UrlConfiguration.BaseUrl.Split(':').LastOrDefault());
            // var server = ConfigurationSection.UrlConfiguration.BaseUrl.Split("//").LastOrDefault();
            // var port = 80;
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(server, port);
                using NetworkStream networkStream = tcpClient.GetStream();
                networkStream.ReadTimeout = ConfigurationSection.RequestConfiguration.Timeout;
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

                result = new HttpResponseMessage
                {
                    StatusCode = successCodes.Any(x => readToEnd.Contains(x)) ? HttpStatusCode.OK : HttpStatusCode.BadRequest
                };
            }

            return result;
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