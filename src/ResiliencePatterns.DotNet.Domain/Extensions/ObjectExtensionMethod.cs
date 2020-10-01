using System.Text.Json;

namespace ResiliencePatterns.DotNet.Domain.Extensions
{
    public static class ObjectExtensionMethod
    {
        public static string ToJson(this object value) 
            => JsonSerializer.Serialize(value);
    }
}