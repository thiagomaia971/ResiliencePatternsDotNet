using System;
using System.Threading;
using ResiliencePatternsDotNet.ConsoleApplication.Common;
using ResiliencePatternsDotNet.ConsoleApplication.Services;
using ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles;
using ResiliencePatternsDotNet.ConsoleApplication.Services.Resiliences;

namespace ResiliencePatternsDotNet.ConsoleApplication
{
    internal class Program
    {
        private static IResiliencePatterns ResiliencePatterns { get; set; }
        private static IRequestHandle RequestHandle { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine($"EnvironmentValue: {GlobalVariables.EnvironmentValue}");
            Console.WriteLine($"Teste: {GlobalVariables.ConfigurationSection.ToJson()}");
            Console.WriteLine();
            
            ResiliencePatterns = new ResiliencePatterns();
            RequestHandle = new RequestHandle(ResiliencePatterns);

            ProcessRequests();
            
            Console.ReadKey();
        }

        private static  void ProcessRequests()
        {
            var requestCount = GlobalVariables.ConfigurationSection.RequestConfiguration.Count;
            var count = 1;
            while (requestCount < 0)
            {
                Console.WriteLine($"ProcessRequest [{count}]");
                
                ProcessRequest();
                
                Thread.Sleep(GlobalVariables.ConfigurationSection.RequestConfiguration.Delay);
                requestCount--;
                count++;
                Console.WriteLine($"Requests Remaining [{requestCount}]");
                Console.WriteLine();
            }
        }

        private static void ProcessRequest()
        {
            if (ErrorProbabilityService.IsErrorRequest(GlobalVariables.ConfigurationSection.RequestConfiguration
                .ProbabilityError))
                RequestHandle.HandleErrorRequest(GlobalVariables.ConfigurationSection.UrlConfiguration.Error.Method, GlobalVariables.ConfigurationSection.UrlConfiguration.Error.Url);
            else
                RequestHandle.HandleSuccessRequest(GlobalVariables.ConfigurationSection.UrlConfiguration.Success.Method, GlobalVariables.ConfigurationSection.UrlConfiguration.Success.Url);
        }
    }
}