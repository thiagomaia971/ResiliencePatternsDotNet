using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ResiliencePatternsDotNet.ConsoleApplication.Common;

namespace ResiliencePatternsDotNet.ConsoleApplication.Configurations
{
    [XmlRoot("retry-configuration")]
    public class RetryConfiguration : IConfigurationSectionHandler
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlAttribute("sleep-duration-type")]
        public SleepDurationType SleepDurationType { get; set; }

        [XmlAttribute("sleep-duration")]
        public int SleepDuration { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(RetryConfiguration));
            using (var sr = new StringReader(section.OuterXml))
                return (RetryConfiguration) ser.Deserialize(sr);
        }
    }
}