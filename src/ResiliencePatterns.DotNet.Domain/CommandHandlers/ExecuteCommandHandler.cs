using ResiliencePatterns.DotNet.Domain.Commands;
using ResiliencePatterns.DotNet.Domain.Common;
using ResiliencePatterns.DotNet.Domain.Services;

namespace ResiliencePatterns.DotNet.Domain.CommandHandlers
{
    public class ExecuteCommandHandler : CommandHandler<ExecuteCommand, MetricStatus>
    {
        private readonly IExecuteService _executeService;

        public ExecuteCommandHandler(IExecuteService executeService) 
            => _executeService = executeService;

        public override MetricStatus Handle(ExecuteCommand command) 
            => _executeService.Execute(command);
    }
}