using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.Domain.Configurations
{
    [XmlRoot("request-configuration")]
    public class RequestConfigurationSection : IConfigurationSectionHandler
    {
        [XmlAttribute("successRequests")]
        public int SuccessRequests { get; set; }
        
        [XmlAttribute("maxRequests")]
        public int MaxRequests { get; set; }
        
        [XmlAttribute("delay")]
        public int Delay { get; set; }
        
        [XmlAttribute("timeout")]
        public int Timeout { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(RequestConfigurationSection));
            using (var sr = new StringReader(section.OuterXml))
                return (RequestConfigurationSection) ser.Deserialize(sr);
        }
    }
}