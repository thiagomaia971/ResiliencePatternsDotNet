using System;
using Polly;
using ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles;

namespace ResiliencePatternsDotNet.ConsoleApplication.Services.Resiliences
{
    public class RetryResilience : IResiliencePattern
    {
        public void Execute(IRequestHandle requestHandle)
        {
            Console.WriteLine("Retry Resilience Pattern!");
            Policy
                .Handle<Exception>()
                .Retry(5, (exception, i) => Console.WriteLine($"\tTry number [{i}]"))
                .ExecuteAndCapture(requestHandle.Handle);
        }
    }
}