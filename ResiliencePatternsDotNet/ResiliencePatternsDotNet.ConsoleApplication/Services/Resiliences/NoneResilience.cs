using System;
using ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles;

namespace ResiliencePatternsDotNet.ConsoleApplication.Services.Resiliences
{
    public class NoneResilience : IResiliencePattern
    {
        public void Execute(IRequestHandle requestHandle)
        {
            Console.WriteLine("Without Resilience Pattern!");
            requestHandle.Handle();
        }
    }
}