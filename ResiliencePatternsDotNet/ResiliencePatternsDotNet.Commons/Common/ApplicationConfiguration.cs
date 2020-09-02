using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ConfigurationSection = ResiliencePatternsDotNet.Commons.Configurations.ConfigurationSection;

namespace ResiliencePatternsDotNet.Commons.Common
{
    public class ApplicationConfiguration
    {
        private static IConfigurationBuilder ConfigurationBuilder { get; set; }

        public ConfigurationSection Configuration { get; set; }

        public static IConfigurationRoot ConfigurationRoot()
            => Builder().Build();

        public static ConfigurationSection Create()
        {
            try
            {
                var appConfig = new ApplicationConfiguration();
                ConfigurationRoot().Bind(appConfig);
                return appConfig.Configuration;
            }
            catch (Exception)
            {
                return ConfigurationSection.Instance;
            }
        }

        private static IConfigurationBuilder Builder()
        {
            if (ConfigurationBuilder != null)
                return ConfigurationBuilder;

            ConfigurationBuilder = new ConfigurationBuilder();
            SetElasticBeanstalkEnvironment();

            return ConfigurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{GlobalVariables.EnvironmentValue}.json", optional: true,
                    reloadOnChange: true);
        }

        private static void SetElasticBeanstalkEnvironment()
        {
            ConfigurationBuilder.AddJsonFile(
                @"C:\Program Files\Amazon\ElasticBeanstalk\config\containerconfiguration",
                optional: true,
                reloadOnChange: true
            );
            var configuration = ConfigurationBuilder.Build();

            var ebEnv =
                configuration.GetSection("iis:env")
                    .GetChildren()
                    .Select(pair => pair.Value.Split(new[] {'='}, 2))
                    .ToDictionary(keypair => keypair[0], keypair => keypair[1]);

            foreach (var keyVal in ebEnv)
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(keyVal.Key)))
                    Environment.SetEnvironmentVariable(keyVal.Key, keyVal.Value);
            }
        }
    }
}