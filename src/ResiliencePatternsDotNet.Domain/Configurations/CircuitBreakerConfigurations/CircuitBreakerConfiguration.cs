using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.Domain.Configurations.CircuitBreakerConfigurations
{
    [XmlRoot("circuit-breaker-configuration")]
    public class CircuitBreakerConfiguration : IConfigurationSectionHandler
    {
        [XmlAttribute("is-simple-configuration")]
        public bool IsSimpleConfiguration { get; set; }
        
        [XmlAttribute("duration-of-breaking")]
        public int DurationOfBreaking { get; set; }
        
        [XmlAttribute("simple-configuration")]
        public CircuitBreakerSimpleConfiguration SimpleConfiguration { get; set; }

        [XmlAttribute("advanced-configuration")]
        public CircuitBreakerAdvancedConfiguration AdvancedConfiguration { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(CircuitBreakerConfiguration));
            using (var sr = new StringReader(section.OuterXml))
                return (CircuitBreakerConfiguration) ser.Deserialize(sr);
        }
    }
}