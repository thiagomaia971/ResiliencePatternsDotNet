﻿using System.Threading.Tasks;
using ResiliencePatterns.DotNet.Domain.Commands;
using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Services;
using ResiliencePatternsDotNet.Commons;

namespace ResiliencePatterns.DotNet.Domain.CommandHandlers
{
    public class ExecuteCommandHandler : CommandHandler<ExecuteCommand, MetricStatus>
    {
        private readonly IExecuteService _executeService;

        public ExecuteCommandHandler(IExecuteService executeService) 
            => _executeService = executeService;

        public override Task<MetricStatus> Handle(ExecuteCommand command) 
            => Task.Run(() => _executeService.Execute(command));
    }
}