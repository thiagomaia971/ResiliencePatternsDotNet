using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResiliencePatternsDotNet.ConsoleApplication.Common
{
    public static class ObjectExtensionMethod
    {
        public static string ToJson(this object value) 
            => JsonSerializer.Serialize(value);
    }
}