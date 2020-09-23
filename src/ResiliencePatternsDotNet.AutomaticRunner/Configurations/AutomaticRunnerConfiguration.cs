using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.AutomaticRunner.Configurations
{
    [XmlRoot("automatic-runner")]
    public class AutomaticRunnerConfiguration : IConfigurationSectionHandler
    {
        
        [XmlAttribute("scenarios-path")]
        public string ScenariosPath { get; set; }
        
        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(AutomaticRunnerConfiguration));
            using (var sr = new StringReader(section.OuterXml))
                return (AutomaticRunnerConfiguration) ser.Deserialize(sr);
        }
    }
}