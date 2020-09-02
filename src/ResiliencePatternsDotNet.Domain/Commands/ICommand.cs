using MediatR;

namespace ResiliencePatternsDotNet.Domain.Commands
{
    public interface ICommand<TResult> : IRequest<CommandResult<TResult>>
    {
    }
}