using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://vaurien:9001");
                    // httpClient.Timeout =
                    //     TimeSpan.FromMilliseconds(_configurationSection.RequestConfiguration.Timeout);
                    var methodEnum = new HttpMethod("GET");

                    var result = httpClient.SendAsync(new HttpRequestMessage(methodEnum, "/get")).GetAwaiter().GetResult();
                    Console.WriteLine($"Result: {result.StatusCode}");
                    Console.WriteLine($"Result: {result.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
                }
                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}