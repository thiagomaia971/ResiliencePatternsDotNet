using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatterns.DotNet.Domain.Configurations
{
    [XmlRoot("url-configuration")]
    public class UrlConfigurationSection : IConfigurationSectionHandler
    {
        [XmlAttribute("base-url")]
        public string BaseUrl { get; set; }
        
        [XmlAttribute("action")]
        public string Action { get; set; }
        
        [XmlAttribute("method")]
        public string Method { get; set; }
        
        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(UrlConfigurationSection));
            using (var sr = new StringReader(section.OuterXml))
                return (UrlConfigurationSection) ser.Deserialize(sr);
        }
    }
}