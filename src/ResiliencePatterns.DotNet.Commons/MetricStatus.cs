using System.Text;
using Newtonsoft.Json;

namespace ResiliencePatternsDotNet.Commons
{
    public class MetricStatusCompiled
    {
        public double ClientToModuleTotalTime { get; set; }
        public double ClientToModulePercentualError { get; set; }
        public double ResilienceModuleToExternalTotalSuccessTime { get; set; }
        public double ResilienceModuleToExternalTotalErrorTime { get; set; }
        public double ResilienceModuleToExternalTotalTime { get; set; }
        public double ResilienceModuleToExternalAverageTimePerRequest { get; set; }
    }
    
    public class MetricStatus
    {
        [JsonProperty]
        public ClientToModuleMetric ClientToModule { get; private set; }
        
        [JsonProperty]
        public ResilienceModuleToExternalMetric ResilienceModuleToExternalService { get; private set; }
        
        [JsonProperty]
        public MetricRetryStatus RetryMetrics { get; private set; }
        
        [JsonProperty]
        public MetricCircuitBreakerStatus CircuitBreakerMetrics { get; private set; }

        protected MetricStatus()
        {
            ClientToModule = new ClientToModuleMetric();
            ResilienceModuleToExternalService = new ResilienceModuleToExternalMetric();
        }
        
        public static MetricStatus Create() 
            => new MetricStatus();
        
        public void CreateRetryCustom() => RetryMetrics = new MetricRetryStatus();
        public void CreateCircuitBrekerCustom() => CircuitBreakerMetrics = new MetricCircuitBreakerStatus();

        public static string GetCsvHeader()
        {
            var valueString = new StringBuilder();
            valueString.Append("ClientToModule TotalTime; ");
            valueString.Append("ClientToModule AverageTime; ");
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

            valueString.Append($"{ClientToModule.TotalTime}; ");
            valueString.Append($"{ClientToModule.AverageTimePerRequest}; ");
            valueString.Append($"{ClientToModule.Success}; ");
            valueString.Append($"{ClientToModule.Error}; ");
            valueString.Append($"{ResilienceModuleToExternalService.Success}; ");
            valueString.Append($"{ResilienceModuleToExternalService.Error}; ");
            valueString.Append($"{ResilienceModuleToExternalService.TotalSuccessTime}; ");
            valueString.Append($"{ResilienceModuleToExternalService.AverageSuccessTimePerRequest}; ");
            valueString.Append($"{RetryMetrics?.RetryCount}; ");
            valueString.Append($"{RetryMetrics?.TotalTimeout}; ");
            valueString.Append($"{CircuitBreakerMetrics?.BreakCount}; ");
            valueString.Append($"{CircuitBreakerMetrics?.ResetStatCount}; ");
            valueString.Append($"{CircuitBreakerMetrics?.TotalOfBreak}; ");

            return valueString.ToString();
        }
    }
}