using System;
using ResiliencePatternsDotNet.Domain.Configurations;

namespace ResiliencePatternsDotNet.Domain.Common
{
    public class GlobalVariables
    {
        private static ConfigurationSection _applicationConfiguration;
        
        public static string EnvironmentDescription => "ASPNETCORE_ENVIRONMENT";
        public static string EnvironmentValue => Environment.GetEnvironmentVariable(EnvironmentDescription);
        
        // public static ConfigurationSection ConfigurationSection 
        //     => _applicationConfiguration ??= ApplicationConfiguration.Create();
    }
}