using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatterns.Core.AutomaticRunner.Configurations
{
    [XmlRoot("proxy-configuration")]
    public class ProxyConfigurationSection : IConfigurationSectionHandler
    {
        [XmlAttribute("dockerComposePath")]
        public string DockerComposePath { get; set; }
        
        [XmlAttribute("vaurienConfigPath")]
        public string VaurienConfigPath { get; set; }
        
        [XmlAttribute("RestartVaurienContainerCommand")]
        public string RestartVaurienContainerCommand { get; set; }
        
        [XmlAttribute("behavior")]
        public string Behavior { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(ProxyConfigurationSection));
            using (var sr = new StringReader(section.OuterXml))
                return (ProxyConfigurationSection) ser.Deserialize(sr);
        }
    }
}