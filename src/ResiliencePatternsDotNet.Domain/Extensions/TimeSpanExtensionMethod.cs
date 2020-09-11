using System;

namespace ResiliencePatternsDotNet.Domain.Extensions
{
    public static class TimeSpanExtensionMethod
    {
        public static string ToExtensiveValue(this TimeSpan value)
            => $"{value.Hours:D2}h:{value.Minutes:D2}m:{value.Seconds:D2}s:{value.Milliseconds:D3}ms";
    }
}