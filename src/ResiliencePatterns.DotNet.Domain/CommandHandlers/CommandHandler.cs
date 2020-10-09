using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ResiliencePatterns.DotNet.Domain.Commands;

namespace ResiliencePatterns.DotNet.Domain.CommandHandlers
{
    public abstract class CommandHandler<TCommand, TResult> : IRequestHandler<TCommand, CommandResult<TResult>> where TCommand : ICommand<TResult>
    {
        public abstract Task<TResult> Handle(TCommand command);

        public async Task<CommandResult<TResult>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return ToResult(await Handle(request));
            }
            catch (Exception e)
            {
                return ToResult(e);
            }
        }
        
        private static CommandResult<TResult> ToResult(TResult result)
            => new CommandResult<TResult>(result);

        private static CommandResult<TResult> ToResult(Exception excecao)
            => new CommandResult<TResult>(excecao);
    }
}