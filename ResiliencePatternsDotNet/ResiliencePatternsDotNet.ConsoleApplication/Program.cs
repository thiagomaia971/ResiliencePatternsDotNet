using System;
using System.Collections.Generic;
using System.Threading;
using ResiliencePatternsDotNet.ConsoleApplication.Common;
using ResiliencePatternsDotNet.ConsoleApplication.Services;
using ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles;
using ResiliencePatternsDotNet.ConsoleApplication.Services.Resiliences;

namespace ResiliencePatternsDotNet.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"EnvironmentValue: {GlobalVariables.EnvironmentValue}");
            Console.WriteLine($"Teste: {GlobalVariables.ConfigurationSection.ToJson()}");
            
            ProcessRequests();
            
            Console.ReadKey();
        }

        private static void ProcessRequests()
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
                Console.WriteLine();
            }
        }

        private static void ProcessRequest()
        {
            IRequestHandle requestHandle;
            if (ErrorProbabilityService.IsErrorRequest(GlobalVariables.ConfigurationSection.RequestConfiguration
                .ProbabilityError))
                requestHandle = new RequestErrorHandle();
            else
                requestHandle = new RequestSuccessHandle();

            IList<IResiliencePattern> resiliencePatterns = new List<IResiliencePattern>();
            switch (GlobalVariables.ConfigurationSection.RunPolicy)
            {
                case RunPolicyEnum.RETRY:
                    resiliencePatterns.Add(new RetryResilience());
                    break;
                case RunPolicyEnum.CIRCUIT_BREAKER:
                    resiliencePatterns.Add(new CircuitBreakerResilience());
                    break;
                case RunPolicyEnum.ALL:
                    resiliencePatterns.Add(new RetryResilience());
                    resiliencePatterns.Add(new CircuitBreakerResilience());
                    break;
                case RunPolicyEnum.NONE:
                    resiliencePatterns.Add(new NoneResilience());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var resiliencePattern in resiliencePatterns)
                resiliencePattern.Execute(requestHandle);
        }
    }
}