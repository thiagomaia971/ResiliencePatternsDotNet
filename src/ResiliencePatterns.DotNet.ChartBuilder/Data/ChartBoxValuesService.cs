using System.IO;
using System.Threading.Tasks;

namespace ResiliencePatterns.DotNet.ChartBuilder.Data
{
    public class ChartBoxValuesService
    {
        public async Task<string[]> GetByPath(string path)
        {
            var xx = Directory.GetFiles(path, "*.scenario-result-compiled.json", SearchOption.AllDirectories);
            return xx;
        }        
    }
}