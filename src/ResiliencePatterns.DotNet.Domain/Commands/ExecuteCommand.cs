﻿using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Configurations;
using ResiliencePatternsDotNet.Commons;

namespace ResiliencePatterns.DotNet.Domain.Commands
{
    public class ExecuteCommand: ConfigurationSection, ICommand<MetricStatus>
    {
    }
}