using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.Domain.Configurations.CircuitBreakerConfigurations
{

    [XmlRoot("circuit-breaker-sample-configuration")]
    public class CircuitBreakerSimpleConfiguration : IConfigurationSectionHandler
    {
        [XmlAttribute("exceptions-allowed-before-breaking")]
        public int ExceptionsAllowedBeforeBreaking { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(CircuitBreakerSimpleConfiguration));
            using (var sr = new StringReader(section.OuterXml))
                return (CircuitBreakerSimpleConfiguration) ser.Deserialize(sr);
        }
    }
}