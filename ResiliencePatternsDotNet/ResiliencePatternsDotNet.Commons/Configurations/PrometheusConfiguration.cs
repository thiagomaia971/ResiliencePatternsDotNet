using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.Commons.Configurations
{
    [XmlRoot("prometheus-configuration")]
    public class PrometheusConfiguration : IConfigurationSectionHandler
    {
        [XmlAttribute("hostname")]
        public string Hostname { get; set; }
        
        [XmlAttribute("port")]
        public int Port { get; set; }
        
        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(PrometheusConfiguration));
            using (var sr = new StringReader(section.OuterXml))
                return (PrometheusConfiguration) ser.Deserialize(sr);
        }
    }
}