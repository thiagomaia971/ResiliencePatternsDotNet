using ResiliencePatternsDotNet.Domain.Commands;
using ResiliencePatternsDotNet.Domain.Common;
using ResiliencePatternsDotNet.Domain.Services;

namespace ResiliencePatternsDotNet.Domain.CommandHandlers
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