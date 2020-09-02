using System;
using System.Threading;
using Prometheus;
using ResiliencePatternsDotNet.Commons.Common;
using ResiliencePatternsDotNet.Commons.Services;
using ResiliencePatternsDotNet.Commons.Services.RequestHandles;
using ResiliencePatternsDotNet.Commons.Services.Resiliences;

namespace ResiliencePatternsDotNet.ConsoleApplication
{
    internal class Program
    {
        

        static void Main(string[] args)
        {
            // Console.WriteLine($"EnvironmentValue: {GlobalVariables.EnvironmentValue}");
            // Console.WriteLine($"Teste: {GlobalVariables.ConfigurationSection.ToJson()}");
            // Console.WriteLine();
            //
            // ResiliencePatterns = new ResiliencePatterns();
            // RequestHandle = new RequestHandle(ResiliencePatterns);
            // InitializePrometheusServer();
            //
            // ProcessRequests();
            
            Console.ReadKey();
        }

        
    }
}