using System;
using ResiliencePatternsDotNet.ConsoleApplication.Configurations;

namespace ResiliencePatternsDotNet.ConsoleApplication.Common
{
    public class GlobalVariables
    {
        private static ConfigurationSection _applicationConfiguration;
        
        public static string EnvironmentDescription => "ASPNETCORE_ENVIRONMENT";
        public static string EnvironmentValue => Environment.GetEnvironmentVariable(EnvironmentDescription);
        
        public static ConfigurationSection ConfigurationSection 
            => _applicationConfiguration ??= ResiliencePatternsDotNet.ConsoleApplication.Common.ApplicationConfiguration.Create();
    }
}