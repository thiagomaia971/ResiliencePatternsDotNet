using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorInputFile;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using ResiliencePatterns.DotNet.ChartBuilder.Entities;
using ResiliencePatternsDotNet.Commons;

namespace ResiliencePatterns.DotNet.ChartBuilder.Services
{
    public class ChartBoxValuesService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChartBoxValuesService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Dictionary<string, List<BoxandWhiskerData>>> GetAllDatas(
            IFileListEntry[] files, 
            string system, 
            Func<FileList, string> groupBy,
            Func<FileList, string> groupByName,
            Func<MetricStatusCompiled, double> selector)
        {
            try
            {
                var results = files
                    .Where(x => x.Name.Contains("scenario-result-compiled") && x.RelativePath.Contains(system))
                    .Select(x => new FileList((FileListEntryImpl)x))
                    .OrderBy(x => x.OrderByResults)
                    .ToList();

                var batches = new Dictionary<string, List<BoxandWhiskerData>>();
                foreach (var scenario in results.GroupBy(groupBy).ToList())
                {
                    var group = batches.FirstOrDefault(x => x.Key == scenario.Key);
                    if (group.Key == null)
                        group = new KeyValuePair<string, List<BoxandWhiskerData>>(scenario.Key, new List<BoxandWhiskerData>());

                    foreach (var fileList in scenario.OrderBy(x => x.OrderByBatchs))
                    {
                        var byName = groupByName(fileList);
                        var data = group.Value.FirstOrDefault(x => x.x == byName) ?? new BoxandWhiskerData { x = byName, y = new double[0] };
                        
                        var stream = files.First(xx => xx.RelativePath == fileList.RelativePath && xx.Name == fileList.Name).Data;
                        using (var streamReader = new StreamReader(stream))
                        {
                            var resultJson = await streamReader.ReadToEndAsync();
                            var result = JsonConvert
                                .DeserializeObject<MetricStatusCompiled[]>(resultJson).ToList();
                            var enumerable = result.Where(z => z != null).Select(selector).ToList();
                            enumerable.AddRange(data.y);

                            data.y = enumerable.ToArray();
                        }

                        if (!group.Value.Exists(x => x.x == data.x))
                            group.Value.Add(data);
                    }
                    batches.Add(scenario.Key, group.Value);
                }

                return batches;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string[]> GetByPath(string path)
        {
            var xx = Directory.GetFiles(path, "*.scenario-result-compiled.json", SearchOption.AllDirectories);

            return xx;
        }
    }

    public class FileList : FileListEntryImpl
    {
        public int ClientGroup => int.Parse(Name.Split("[").LastOrDefault().Split("]").FirstOrDefault());
        public string ScenarioGroup => Name.Split("]").LastOrDefault().Split(".").FirstOrDefault();
        public string ScenarioSucessTax => RelativePath.Split("_SUCESSO").FirstOrDefault().Split("/").LastOrDefault();
        public int Ms => Name.Contains("ms") ? (int.Parse(Name.Split("_").LastOrDefault().Split("ms").FirstOrDefault())) : 0;
        private int NameWidth => Name.Contains("BaseLine") ? 0 : (Name.Contains("Retry") ? 1000 : 2000);
        public int OrderByBatchs => int.Parse(ScenarioSucessTax);
        public int OrderByResults => ClientGroup + Ms + + NameWidth;

        public FileList(FileListEntryImpl file)
        {
            Id = file.Id;
            Name = file.Name;
            RelativePath = file.RelativePath;
        }
    }
}