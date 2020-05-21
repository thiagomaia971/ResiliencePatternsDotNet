using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.ConsoleApplication.Configurations
{
    [XmlRoot("url-fetch")]
    public class UrlFetchConfigurationSection : IConfigurationSectionHandler
    {
        [XmlAttribute("url")]
        public string Url { get; set; }
        
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