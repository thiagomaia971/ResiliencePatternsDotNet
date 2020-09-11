using System.Collections;

namespace ResiliencePatternsDotNet.AutomaticRunner.Configurations
{
    public class Scenario
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string FileNameWithoutExtension { get; set; }
        public object Parameters { get; set; }
        public int Count { get; set; }
    }
}