using System.Text.Json;

namespace ResiliencePatternsDotNet.Commons.Common
{
    public static class ObjectExtensionMethod
    {
        public static string ToJson(this object value) 
            => JsonSerializer.Serialize(value);
    }
}