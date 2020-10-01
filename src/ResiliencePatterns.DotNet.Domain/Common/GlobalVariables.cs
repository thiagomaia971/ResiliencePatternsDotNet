using System;
using ResiliencePatterns.DotNet.Domain.Configurations;

namespace ResiliencePatterns.DotNet.Domain.Common
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