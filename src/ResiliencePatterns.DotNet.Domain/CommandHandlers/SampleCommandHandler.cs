using System;
using System.Threading.Tasks;
using ResiliencePatterns.DotNet.Domain.Commands;

namespace ResiliencePatterns.DotNet.Domain.CommandHandlers
{
    public class SampleCommandHandler : CommandHandler<SampleCommand, int>
    {
        public override async Task<int> Handle(SampleCommand command) 
            => new Random().Next();
    }
}