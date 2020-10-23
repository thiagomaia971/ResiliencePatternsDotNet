using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResiliencePatterns.Core.AutomaticRunner.Services;
using ResiliencePatternsDotNet.Commons.Configurations;

namespace ResiliencePatterns.Core.AutomaticRunner
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
                    services.AddSingleton<ScenarioService>();
                    services.AddSingleton<ResultWriterService>();
                    services.AddHostedService<Worker>();
                });
    }
}