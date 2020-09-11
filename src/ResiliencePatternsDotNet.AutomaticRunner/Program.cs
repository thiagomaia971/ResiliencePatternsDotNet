using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResiliencePatternsDotNet.AutomaticRunner.Configurations;

namespace ResiliencePatternsDotNet.AutomaticRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    AutomaticRunnerConfiguration options = configuration.GetSection("AutomaticRunnerConfiguration").Get<AutomaticRunnerConfiguration>();
                    services.AddSingleton(options);
                    services.AddHostedService<Worker>();
                });
    }
}