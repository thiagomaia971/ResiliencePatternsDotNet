using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatterns.Core.AutomaticRunner.Configurations
{
    [XmlRoot("url-fetch")]
    public class UrlFetchConfigurationSection : IConfigurationSectionHandler
    {
        [XmlAttribute("base-url")]
        public string BaseUrl { get; set; }
        
        [XmlAttribute("action-url")]
        public string ActionUrl { get; set; }
        
        [XmlAttribute("method")]
        public string Method { get; set; }
        
        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(UrlFetchConfigurationSection));
            using (var sr = new StringReader(section.OuterXml))
                return (UrlFetchConfigurationSection) ser.Deserialize(sr);
        }
    }
}