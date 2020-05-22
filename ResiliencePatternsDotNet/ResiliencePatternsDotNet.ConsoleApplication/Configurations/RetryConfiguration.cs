using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatternsDotNet.ConsoleApplication.Configurations
{
    [XmlRoot("retry-configuration")]
    public class RetryConfiguration : IConfigurationSectionHandler
    {

        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(RetryConfiguration));
            using (var sr = new StringReader(section.OuterXml))
                return (RetryConfiguration) ser.Deserialize(sr);
        }
    }
}