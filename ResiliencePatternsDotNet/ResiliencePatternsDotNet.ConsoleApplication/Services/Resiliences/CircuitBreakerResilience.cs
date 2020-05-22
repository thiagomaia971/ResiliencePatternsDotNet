using System;
using Polly;
using Polly.CircuitBreaker;
using ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles;

namespace ResiliencePatternsDotNet.ConsoleApplication.Services.Resiliences
{
    public class CircuitBreakerResilience : IResiliencePattern
    {
        private CircuitBreakerPolicy CircuitBreakerPolicy { get; set; }
        
        public CircuitBreakerResilience() 
            => CircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 5, 
                    durationOfBreak: TimeSpan.FromSeconds(5), 
                    onBreak: (exception, span) => Console.WriteLine($"\tWait for [{span}]"),
                    onReset: () => Console.WriteLine($"\tReseted"));


        public void Execute(IRequestHandle requestHandle)
        {
            Console.WriteLine("CircuitBreaker Resilience Pattern!");
            CircuitBreakerPolicy.ExecuteAndCapture(requestHandle.Handle);
        }
    }
}