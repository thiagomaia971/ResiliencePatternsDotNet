using System.Text;
using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.Commons
{
    public class MetricStatus
    {
        [JsonProperty]
        public long? TotalTime { get; private set; }
        
        [JsonProperty]
        public MetricCountStatus ClientToModule { get; private set; }
        
        [JsonProperty]
        public MetricCountStatus ResilienceModuleToExternalService { get; private set; }
        
        [JsonProperty]
        public MetricRetryStatus RetryMetrics { get; private set; }
        
        [JsonProperty]
        public MetricCircuitBreakerStatus CircuitBreakerMetrics { get; private set; }

        protected MetricStatus()
        {
            ClientToModule = new MetricCountStatus();
            ResilienceModuleToExternalService = new MetricCountStatus();
        }
        
        public static MetricStatus Create() 
            => new MetricStatus();
        
        public void CreateRetryCustom() => RetryMetrics = new MetricRetryStatus();
        public void CreateCircuitBrekerCustom() => CircuitBreakerMetrics = new MetricCircuitBreakerStatus();
        public void AddTotalTime(long? totalTime) => TotalTime = totalTime;

        public static string GetCsvHeader()
        {
            var valueString = new StringBuilder();
            valueString.Append("Total Time; ");
            valueString.Append("ClientToModule Success; ");
            valueString.Append("ClientToModule Error; ");
            valueString.Append("ResilienceModuleToExternalService Success; ");
            valueString.Append("ResilienceModuleToExternalService Error; ");
            valueString.Append("ResilienceModuleToExternalService TotalSuccessTime; ");
            valueString.Append("ResilienceModuleToExternalService TotalSuccessTimePerRequest; ");
            valueString.Append("Retry Count; ");
            valueString.Append("Retry TotalTimeout; ");
            valueString.Append("CircuitBreaker Count; ");
            valueString.Append("CircuitBreaker ResetCount; ");
            valueString.Append("CircuitBreaker TotalOfBreaker; ");
            
            return valueString.ToString();
        }

        public string GetCsvLine()
        {
            var valueString = new StringBuilder();

            valueString.Append($"{TotalTime}; ");
            valueString.Append($"{ClientToModule.Success}; ");
            valueString.Append($"{ClientToModule.Error}; ");
            valueString.Append($"{ResilienceModuleToExternalService.Success}; ");
            valueString.Append($"{ResilienceModuleToExternalService.Error}; ");
            valueString.Append($"{ResilienceModuleToExternalService.TotalSuccessTime}; ");
            valueString.Append($"{ResilienceModuleToExternalService.TotalSuccessTimePerRequest}; ");
            valueString.Append($"{RetryMetrics?.RetryCount}; ");
            valueString.Append($"{RetryMetrics?.TotalTimeout}; ");
            valueString.Append($"{CircuitBreakerMetrics?.BreakCount}; ");
            valueString.Append($"{CircuitBreakerMetrics?.ResetStatCount}; ");
            valueString.Append($"{CircuitBreakerMetrics?.TotalOfBreak}; ");

            return valueString.ToString();
        }
    }
}