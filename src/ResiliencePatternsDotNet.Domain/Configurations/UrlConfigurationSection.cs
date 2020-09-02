using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.Domain.Configurations
{
    [XmlRoot("url-configuration")]
    public class UrlConfigurationSection : IConfigurationSectionHandler
    {
        [XmlAttribute("base-url")]
        public string BaseUrl { get; set; }
        
        [XmlAttribute("succes")]
        public UrlFetchConfigurationSection Success { get; set; }
        
        [XmlAttribute("error")]
        public UrlFetchConfigurationSection Error { get; set; }
        
        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(UrlConfigurationSection));
            using (var sr = new StringReader(section.OuterXml))
                return (UrlConfigurationSection) ser.Deserialize(sr);
        }
    }
}