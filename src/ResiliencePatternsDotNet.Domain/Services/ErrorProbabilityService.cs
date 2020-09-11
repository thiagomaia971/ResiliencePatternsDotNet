using System;

namespace ResiliencePatternsDotNet.Domain.Services
{
    public class ErrorProbabilityService
    {
        public static bool IsDalayRequest(int percent)
        {
            var r = new Random();
            var rInt = r.Next(0, 100);
            return rInt < percent;
        }
    }
}