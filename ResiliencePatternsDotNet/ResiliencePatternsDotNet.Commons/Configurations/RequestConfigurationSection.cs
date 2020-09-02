using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.Commons.Configurations
{
    [XmlRoot("request-configuration")]
    public class RequestConfigurationSection : IConfigurationSectionHandler
    {
        [XmlAttribute("count")]
        public int Count { get; set; }
        
        [XmlAttribute("delay")]
        public int Delay { get; set; }
        
        [XmlAttribute("probability-error")]
        public int ProbabilityError { get; set; }
        
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