using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ResiliencePatterns.DotNet.Domain.Configurations
{
    [XmlRoot("circuit-breaker-configuration")]
    public class CircuitBreakerConfiguration : IConfigurationSectionHandler
    {
        [XmlAttribute("is-simple-configuration")]
        public bool IsSimpleConfiguration { get; set; }
        
        [XmlAttribute("duration-of-breaking")]
        public int DurationOfBreaking { get; set; }
        
        [XmlAttribute("exceptions-allowed-before-breaking")]
        public int ExceptionsAllowedBeforeBreaking { get; set; }
        
        /// <summary>
        /// The proportion of failures at which to break. A double between 0 and 1. For example, 0.5 represents break on 50% or more of actions through the circuit resulting in a handled failure.
        /// </summary>
        [XmlAttribute("failure-threshold")]
        public double FailureThreshold { get; set; }
        
        /// <summary>
        /// The failure rate is considered for actions over this period. Successes/failures older than the period are discarded from metrics.
        /// </summary>
        [XmlAttribute("sampling-duration")]
        public double SamplingDuration { get; set; }
        
        /// <summary>
        /// This many calls must have passed through the circuit within the active samplingDuration for the circuit to consider breaking.
        /// </summary>
        [XmlAttribute("minimum-throughput")]
        public int MinimumThroughput { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var ser = new XmlSerializer(typeof(CircuitBreakerConfiguration));
            using (var sr = new StringReader(section.OuterXml))
                return (CircuitBreakerConfiguration) ser.Deserialize(sr);
        }
    }
}