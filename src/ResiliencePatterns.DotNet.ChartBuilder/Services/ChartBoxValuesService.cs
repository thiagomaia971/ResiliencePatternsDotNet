﻿using System;
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

        public async Task<Dictionary<string, List<BoxandWhiskerData>>> GetByPath(IFileListEntry[] files, string system)
        {
            var results = files
                .Where(x => x.Name.Contains("scenario-result-compiled") && x.RelativePath.Contains(system))
                .Select(x => new FileList((FileListEntryImpl)x))
                .OrderBy(x => x.ClientGroup)
                .ToList();
            var scenarios = results.OrderBy(x => x.ScenarioGroup).GroupBy(x => x.ScenarioGroup).ToList();
            var datas = new Dictionary<string, List<BoxandWhiskerData>>();
            foreach (var scenario in scenarios)
            {
                var _datas = new List<BoxandWhiskerData>();
                foreach (var fileList in scenario)
                {
                    var data = new BoxandWhiskerData
                    {
                        x = $"{fileList.ClientGroup} clients"
                    };

                    var stream = files.First(xx => xx.RelativePath == fileList.RelativePath && xx.Name == fileList.Name).Data;
                    using (var streamReader = new StreamReader(stream))
                    {
                        var resultJson = await streamReader.ReadToEndAsync();
                        var result = JsonConvert
                            .DeserializeObject<MetricStatusCompiled[]>(resultJson).ToList();
                        data.y = result.Where(z => z != null).Select(y => y.ClientToModuleTotalTime).ToArray();
                    }
                    _datas.Add(data);
                }
                datas.Add(scenario.Key, _datas);
            }
            return datas;
            //foreach (var fileList in results)
            //{
            //    var data = new BoxandWhiskerData
            //    {
            //        x = fileList.ClientGroup.ToString()
            //    };

            //    var stream = files.First(xx => xx.RelativePath == fileList.RelativePath && xx.Name == fileList.Name).Data;
            //    using (var streamReader = new StreamReader(stream))
            //    {
            //        var resultJson= await streamReader.ReadToEndAsync();
            //        var result = JsonConvert
            //            .DeserializeObject<MetricStatusCompiled[]>(resultJson).ToList();
            //        data.y = result.Where(z => z != null).Select(y => y.ClientToModuleTotalTime).ToArray();
            //    }
            //    datas.Add(data);
            //}

            //return datas;
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

        public FileList(FileListEntryImpl file)
        {
            Id = file.Id;
            Name = file.Name;
            RelativePath = file.RelativePath;
        }
    }
}