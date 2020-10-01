using MediatR;

namespace ResiliencePatterns.DotNet.Domain.Commands
{
    public interface ICommand<TResult> : IRequest<CommandResult<TResult>>
    {
    }
}